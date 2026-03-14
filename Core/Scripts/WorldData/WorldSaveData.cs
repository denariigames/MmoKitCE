using Insthync.SerializationSurrogates;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class WorldSaveData
    {
        public List<BuildingSaveData> buildings = new List<BuildingSaveData>();

        public void SavePersistentData(string id, string map)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddAllUnitySurrogate();
            BuildingSaveDataSerializationSurrogate buildingSaveDataSS = new BuildingSaveDataSerializationSurrogate();
            WorldSaveDataSerializationSurrogate worldSaveDataSS = new WorldSaveDataSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(BuildingSaveData), new StreamingContext(StreamingContextStates.All), buildingSaveDataSS);
            surrogateSelector.AddSurrogate(typeof(WorldSaveData), new StreamingContext(StreamingContextStates.All), worldSaveDataSS);
            binaryFormatter.SurrogateSelector = surrogateSelector;
            binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
            string path = Application.persistentDataPath + "/" + id + "_world_" + map + ".sav";
            FileStream file = File.Open(path, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(file, this);
            file.Close();
        }

        public void LoadPersistentData(string id, string map)
        {
            string path = Application.persistentDataPath + "/" + id + "_world_" + map + ".sav";
            buildings.Clear();
            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                surrogateSelector.AddAllUnitySurrogate();
                BuildingSaveDataSerializationSurrogate buildingSaveDataSS = new BuildingSaveDataSerializationSurrogate();
                WorldSaveDataSerializationSurrogate worldSaveDataSS = new WorldSaveDataSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(BuildingSaveData), new StreamingContext(StreamingContextStates.All), buildingSaveDataSS);
                surrogateSelector.AddSurrogate(typeof(WorldSaveData), new StreamingContext(StreamingContextStates.All), worldSaveDataSS);
                binaryFormatter.SurrogateSelector = surrogateSelector;
                binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
                FileStream file = File.Open(path, FileMode.Open);
                WorldSaveData result = (WorldSaveData)binaryFormatter.Deserialize(file);
                buildings = result.buildings;
                file.Close();
            }
        }
    }
}
