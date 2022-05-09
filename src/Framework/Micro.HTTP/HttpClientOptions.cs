namespace Micro.HTTP;

public sealed class HttpClientOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ResiliencyOptions Resiliency { get; set; } = new();
    public Dictionary<string, string> Services { get; set; } = new();

    public sealed class ResiliencyOptions
    {
        public int Retries { get; set; } = 3;
        public TimeSpan? RetryInterval { get; set; }
        public bool Exponential { get; set; }
    }
}