using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Insthync.DevExtension;

namespace MultiplayerARPG.MMO
{
    public sealed class DatabaseCharacterSaveScheduler
    {
        private sealed class PendingCharacterSave
        {
            public string CharacterId;
            public UpdateCharacterReq Request;
            public int LaneIndex;
            public int Version;
            public bool IsQueued;
            public bool IsSaving;
            public double LastSavedAt;
            public double NextEligibleSaveAt;
        }

        private readonly object _lock = new object();
        private readonly DatabaseNetworkManager _owner;
        private readonly Queue<string>[] _lanes;
        private readonly Dictionary<string, PendingCharacterSave> _pending = new Dictionary<string, PendingCharacterSave>();

        private readonly int _tickMilliseconds;
        private readonly int _maxSavesPerTick;
        private readonly float _minSaveIntervalSeconds;
        private readonly float _retryDelaySeconds;

        private bool _running;
        private int _nextLaneIndex;

        public DatabaseCharacterSaveScheduler(
            DatabaseNetworkManager owner,
            int laneCount,
            int tickMilliseconds,
            int maxSavesPerTick,
            float minSaveIntervalSeconds,
            float retryDelaySeconds)
        {
            _owner = owner;
            _tickMilliseconds = Math.Max(50, tickMilliseconds);
            _maxSavesPerTick = Math.Max(1, maxSavesPerTick);
            _minSaveIntervalSeconds = Math.Max(1f, minSaveIntervalSeconds);
            _retryDelaySeconds = Math.Max(1f, retryDelaySeconds);

            laneCount = Math.Max(1, laneCount);
            _lanes = new Queue<string>[laneCount];
            for (int i = 0; i < laneCount; ++i)
                _lanes[i] = new Queue<string>();
        }

        public int PendingCount
        {
            get
            {
                lock (_lock)
                {
                    return _pending.Count;
                }
            }
        }

        public void Start()
        {
            if (_running)
                return;

            _running = true;
            RunLoop().Forget();
        }

        public void Stop()
        {
            _running = false;
        }

        public void Enqueue(UpdateCharacterReq request, bool forceImmediate = false)
        {
            string characterId = request.CharacterData.Id;
            if (string.IsNullOrEmpty(characterId))
                return;

            double now = GetUtcSeconds();

            lock (_lock)
            {
                if (!_pending.TryGetValue(characterId, out PendingCharacterSave pending))
                {
                    pending = new PendingCharacterSave()
                    {
                        CharacterId = characterId,
                        LaneIndex = GetLaneIndex(characterId),
                        LastSavedAt = 0d,
                        NextEligibleSaveAt = now,
                        Version = 0,
                        IsQueued = false,
                        IsSaving = false,
                    };
                    _pending.Add(characterId, pending);
                }

                pending.Request = request;
                pending.Version++;

                if (forceImmediate)
                {
                    pending.NextEligibleSaveAt = now;
                }
                else
                {
                    double nextAllowed = pending.LastSavedAt + _minSaveIntervalSeconds;
                    pending.NextEligibleSaveAt = nextAllowed > now ? nextAllowed : now;
                }

                if (!pending.IsQueued && !pending.IsSaving)
                {
                    pending.IsQueued = true;
                    _lanes[pending.LaneIndex].Enqueue(characterId);
                }
            }
        }

        public async UniTask FlushAllAsync()
        {
            while (true)
            {
                string[] ids;

                lock (_lock)
                {
                    if (_pending.Count == 0)
                        break;

                    ids = new string[_pending.Count];
                    int index = 0;
                    foreach (string id in _pending.Keys)
                        ids[index++] = id;
                }

                bool savedAny = false;
                for (int i = 0; i < ids.Length; ++i)
                {
                    if (await SaveCharacterNow(ids[i], true))
                        savedAny = true;
                }

                if (!savedAny)
                    break;
            }
        }

        private async UniTaskVoid RunLoop()
        {
            while (_running)
            {
                try
                {
                    await UniTask.Delay(_tickMilliseconds);
                    await ProcessNextLane();
                }
                catch (Exception ex)
                {
                     System.Console.WriteLine($"[{nameof(DatabaseCharacterSaveScheduler)}] {ex}");
                }
            }
        }

        private async UniTask ProcessNextLane()
        {
            Queue<string> lane;

            lock (_lock)
            {
                lane = _lanes[_nextLaneIndex];
                _nextLaneIndex = (_nextLaneIndex + 1) % _lanes.Length;
            }

            int processed = 0;
            while (processed < _maxSavesPerTick)
            {
                string characterId;

                lock (_lock)
                {
                    if (lane.Count == 0)
                        break;

                    characterId = lane.Dequeue();
                }

                await SaveCharacterNow(characterId, false);
                processed++;
            }
        }

        private async UniTask<bool> SaveCharacterNow(string characterId, bool ignoreInterval)
        {
            PendingCharacterSave pending;
            UpdateCharacterReq request;
            int version;
            double now = GetUtcSeconds();

            lock (_lock)
            {
                if (!_pending.TryGetValue(characterId, out pending))
                    return false;

                if (pending.IsSaving)
                    return false;

                if (!ignoreInterval && now < pending.NextEligibleSaveAt)
                {
                    if (!pending.IsQueued)
                    {
                        pending.IsQueued = true;
                        _lanes[pending.LaneIndex].Enqueue(characterId);
                    }
                    return false;
                }

                pending.IsSaving = true;
                pending.IsQueued = false;
                request = pending.Request;
                version = pending.Version;
            }

            bool success = await _owner.InternalPersistCharacterUpdate(request);

            lock (_lock)
            {
                if (!_pending.TryGetValue(characterId, out pending))
                    return success;

                pending.IsSaving = false;

                if (!success)
                {
                    pending.NextEligibleSaveAt = GetUtcSeconds() + _retryDelaySeconds;
                    if (!pending.IsQueued)
                    {
                        pending.IsQueued = true;
                        _lanes[pending.LaneIndex].Enqueue(characterId);
                    }
                    return false;
                }

                pending.LastSavedAt = GetUtcSeconds();

                if (pending.Version != version)
                {
                    pending.NextEligibleSaveAt = ignoreInterval
                        ? pending.LastSavedAt
                        : pending.LastSavedAt + _minSaveIntervalSeconds;

                    if (!pending.IsQueued)
                    {
                        pending.IsQueued = true;
                        _lanes[pending.LaneIndex].Enqueue(characterId);
                    }
                }
                else
                {
                    _pending.Remove(characterId);
                }
            }

            return true;
        }

        private int GetLaneIndex(string characterId)
        {
            unchecked
            {
                int hash = 17;
                for (int i = 0; i < characterId.Length; ++i)
                    hash = (hash * 31) + characterId[i];

                if (hash < 0)
                    hash = -hash;

                return hash % _lanes.Length;
            }
        }

        private static double GetUtcSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d;
        }
    }
}
