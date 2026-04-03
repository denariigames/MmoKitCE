# unity3d-serialization-surrogates
Serialization surrogates for Unity's structs

**Can make formatter to serialize and deserialize:**
* Color
* Quaternion
* Vector2Int
* Vector2
* Vector3Int
* Vector3
* Vector4

## Usage Example
**Serialize**
```csharp
using (var memoryStream = new MemoryStream())
{
    var binaryFormatter = new BinaryFormatter();
    // Setup Unity's structs serialization surrogates
    var surrogateSelector = new SurrogateSelector();
    surrogateSelector.AddAllUnitySurrogate();
    binaryFormatter.SurrogateSelector = surrogateSelector;
    // Serialize and put to packet
    binaryFormatter.Serialize(memoryStream, data);
    memoryStream.Flush();
    memoryStream.Seek(0, SeekOrigin.Begin);
    var bytes = memoryStream.ToArray();
}
```

**Deserialize**
```csharp
using (MemoryStream memoryStream = new MemoryStream(bytes))
{
    BinaryFormatter binaryFormatter = new BinaryFormatter();
    // Setup Unity's structs serialization surrogates
    var surrogateSelector = new SurrogateSelector();
    surrogateSelector.AddAllUnitySurrogate();
    binaryFormatter.SurrogateSelector = surrogateSelector;
    // Deserialize
    var data = binaryFormatter.Deserialize(memoryStream);
}
```
