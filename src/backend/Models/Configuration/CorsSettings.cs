namespace GameOfLife.Models.Configuration;

public class CorsSettings
{
    public string PolicyName { get; set; } = "DefaultCorsPolicy";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}