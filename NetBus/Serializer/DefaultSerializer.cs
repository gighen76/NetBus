using Newtonsoft.Json;
using System.Text;

namespace NetBus.Serializer
{
    public class DefaultSerializer : ISerializer
    {

        public byte[] Serialize<T>(T message)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
        }

        public T Deserialize<T>(byte[] message)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message), new JsonSerializerSettings
            {
                Error = (sender, e) =>
                {
                    e.ErrorContext.Handled = true;
                }
            });
        }


    }
}