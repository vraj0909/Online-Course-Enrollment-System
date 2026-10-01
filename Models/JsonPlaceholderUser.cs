using System.Text.Json.Serialization;

namespace CourseEnrollmentSystem.Models;

public class JsonPlaceholderUser
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public JsonPlaceholderCompany? Company { get; set; }
}

public class JsonPlaceholderCompany
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}