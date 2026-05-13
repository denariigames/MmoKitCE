using System.Threading;

namespace MultiplayerARPG
{
    public class ObjectWithCancellationTokenSource<T>
    {
        public T Object { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public bool IsCancellationRequested => CancellationTokenSource.IsCancellationRequested;
        public CancellationToken Token => CancellationTokenSource.Token;

        public ObjectWithCancellationTokenSource(T obj)
        {
            Object = obj;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void Cancel() => CancellationTokenSource.Cancel();

        public void Dispose() => CancellationTokenSource.Dispose();
    }
}