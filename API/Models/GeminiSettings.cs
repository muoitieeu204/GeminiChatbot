namespace API.Models
{
    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-3.1-flash-lite-preview";
        public double Temperature { get; set; } = 0.7;
     public int MaxTokens { get; set; } = 8192;
    }
}
