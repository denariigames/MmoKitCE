using Insthync.SerializationSurrogates;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class StorageSaveData
    {
        public List<StorageCharacterItem> storageItems = new List<StorageCharacterItem>();

        public void SavePersistentData(string id)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddAllUnitySurrogate();
            StorageCharacterItemSerializationSurrogate storageCharacterItemSS = new StorageCharacterItemSerializationSurrogate();
            StorageSaveDataSerializationSurrogate storageSaveDataSS = new StorageSaveDataSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(StorageCharacterItem), new StreamingContext(StreamingContextStates.All), storageCharacterItemSS);
            surrogateSelector.AddSurrogate(typeof(StorageSaveData), new StreamingContext(StreamingContextStates.All), storageSaveDataSS);
            binaryFormatter.SurrogateSelector = surrogateSelector;
            binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
            string path = Application.persistentDataPath + "/" + id + "_storage.sav";
            FileStream file = File.Open(path, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(file, this);
            file.Close();
        }

        public void LoadPersistentData(string id)
        {
            string path = Application.persistentDataPath + "/" + id + "_storage.sav";
            storageItems.Clear();
            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                surrogateSelector.AddAllUnitySurrogate();
                StorageCharacterItemSerializationSurrogate storageCharacterItemSS = new StorageCharacterItemSerializationSurrogate();
                StorageSaveDataSerializationSurrogate storageSaveDataSS = new StorageSaveDataSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(StorageCharacterItem), new StreamingContext(StreamingContextStates.All), storageCharacterItemSS);
                surrogateSelector.AddSurrogate(typeof(StorageSaveData), new StreamingContext(StreamingContextStates.All), storageSaveDataSS);
                binaryFormatter.SurrogateSelector = surrogateSelector;
                binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
                FileStream file = File.Open(path, FileMode.Open);
                StorageSaveData result = (StorageSaveData)binaryFormatter.Deserialize(file);
                storageItems = result.storageItems;
                file.Close();
            }
        }
    }
}
