using System.Text.Json;

namespace eLib.Common;

public class SerializedObject
{
    public SerializedObject()
    {
    }

    public SerializedObject(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        Type = obj.GetType().Name;
        Data = JsonSerializer.Serialize(obj);
    }

    public string Type { get; set; }
    public string Data { get; set; }

    public T Deserialize<T>()
    {
        return JsonSerializer.Deserialize<T>(Data);
    }
}