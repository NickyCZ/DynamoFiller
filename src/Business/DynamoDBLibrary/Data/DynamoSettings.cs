namespace DynamoDBLibrary.Data;

public class DynamoSettings
{
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string ServiceURL { get; set; } = string.Empty;
    public string AuthenticationRegion { get; set; } = string.Empty;
}
