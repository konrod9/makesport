using System.Text.Json.Serialization;

namespace AuthService.Domain.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Moderator,
    Admin
}