using System.Text.Json.Serialization;

namespace MISBackend.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // Anotasi untuk menggunakan JsonStringEnumConverter pada enum
    public class DataEnum
    {
        public enum Gender
        {
            Male,
            Female
        }
    }
}
