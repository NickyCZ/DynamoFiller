using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using ConsoleTables;
using Filler;

var credentials = new BasicAWSCredentials("fakeMyAccessKeyId", "fakeSecretAccessKey");
var config = new AmazonDynamoDBConfig
{
    ServiceURL = "http://localhost:8000/",
    AuthenticationRegion = "eu-central-1"
};
var client = new AmazonDynamoDBClient(credentials, config);
var context = new DynamoDBContext(client);

await CreateTable(client);
await CreateNewData(context);

var data = await context.ScanAsync<WeatherForecast>(null).GetRemainingAsync();
ConsoleTable.From(data).Write();
Console.Read();

async Task CreateTable(AmazonDynamoDBClient client)
{
    var request = new CreateTableRequest
    {
        TableName = "WeatherForecast",
        AttributeDefinitions = new List<AttributeDefinition>
     {
         new AttributeDefinition
         {
             AttributeName = "City",  AttributeType = "s"
         },
         new AttributeDefinition
         {
             AttributeName = "Date",  AttributeType = "s"
         }
     },
        KeySchema = new List<KeySchemaElement>
     {
         new KeySchemaElement
         {
             AttributeName = "City",
             KeyType = "HASH"
         },
                  new KeySchemaElement
         {
             AttributeName = "Date",
             KeyType = "RANGE"
         }
     },
        ProvisionedThroughput = new ProvisionedThroughput
        {
            ReadCapacityUnits = 5,
            WriteCapacityUnits = 5
        }
    };
    var response = await client.CreateTableAsync(request);
}
async Task CreateNewData(DynamoDBContext context)
{
    var prague = new WeatherForecast { City = "Prague", Date = DateTime.Now.ToString() };
    var bruntal = new WeatherForecast { City = "Bruntal", Date = DateTime.Now.ToString() };
    await context.SaveAsync(prague);
    await context.SaveAsync(bruntal);
}