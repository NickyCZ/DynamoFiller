using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace DynamoDBLibrary.Data;

public class DynamoContext : IDatabaseContext
{
    protected AmazonDynamoDBClient client;
    protected DynamoDBContext context;

    public DynamoContext(IOptions<DynamoSettings> settings)
    {
        var credentials = new BasicAWSCredentials(settings.Value.AccessKeyId, settings.Value.SecretAccessKey);
        var config = new AmazonDynamoDBConfig()
        {
            ServiceURL = settings.Value.ServiceURL,
            AuthenticationRegion = settings.Value.AuthenticationRegion
        };

        client = new AmazonDynamoDBClient(credentials, config);
        context = new DynamoDBContext(client);
    }

    public AmazonDynamoDBClient Client()
    {
        return client;
    }
}
