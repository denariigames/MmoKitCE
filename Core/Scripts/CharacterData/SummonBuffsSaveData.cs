using Insthync.SerializationSurrogates;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class SummonBuffsSaveData
    {
        public List<CharacterBuff> summonBuffs = new List<CharacterBuff>();

        public void SavePersistentData(string id)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddAllUnitySurrogate();
            CharacterBuffSerializationSurrogate buildingSaveDataSS = new CharacterBuffSerializationSurrogate();
            SummonBuffsSaveDataSerializationSurrogate summonBuffsSaveDataSS = new SummonBuffsSaveDataSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(CharacterBuff), new StreamingContext(StreamingContextStates.All), buildingSaveDataSS);
            surrogateSelector.AddSurrogate(typeof(SummonBuffsSaveData), new StreamingContext(StreamingContextStates.All), summonBuffsSaveDataSS);
            binaryFormatter.SurrogateSelector = surrogateSelector;
            binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
            string path = Application.persistentDataPath + "/" + id + "_summon_buffs.sav";
            FileStream file = File.Open(path, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(file, this);
            file.Close();
        }

        public void LoadPersistentData(string id)
        {
            string path = Application.persistentDataPath + "/" + id + "_summon_buffs.sav";
            summonBuffs.Clear();
            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                surrogateSelector.AddAllUnitySurrogate();
                CharacterBuffSerializationSurrogate buildingSaveDataSS = new CharacterBuffSerializationSurrogate();
                SummonBuffsSaveDataSerializationSurrogate summonBuffsSaveDataSS = new SummonBuffsSaveDataSerializationSurrogate();
                surrogateSelector.AddSurrogate(typeof(CharacterBuff), new StreamingContext(StreamingContextStates.All), buildingSaveDataSS);
                surrogateSelector.AddSurrogate(typeof(SummonBuffsSaveData), new StreamingContext(StreamingContextStates.All), summonBuffsSaveDataSS);
                binaryFormatter.SurrogateSelector = surrogateSelector;
                binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
                FileStream file = File.Open(path, FileMode.Open);
                SummonBuffsSaveData result = (SummonBuffsSaveData)binaryFormatter.Deserialize(file);
                summonBuffs = result.summonBuffs;
                file.Close();
            }
        }
    }
}
