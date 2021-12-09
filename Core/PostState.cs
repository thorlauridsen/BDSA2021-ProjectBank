using System.Text.Json.Serialization;

namespace ProjectBank.Core
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PostState
    {
        Draft,
        Active,
        Closed,
        Archived,
        Deleted
    }
}
