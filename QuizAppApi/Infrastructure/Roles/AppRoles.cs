using System.Text.Json.Serialization;

namespace QuizAppApi.Infrastructure.Roles
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AppRoles
    {
        Admin = 1,
        Participant = 2
    }
}
