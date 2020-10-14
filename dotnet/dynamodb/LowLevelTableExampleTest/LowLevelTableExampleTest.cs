using System;
using System.Net;
using System.Net.NetworkInformation;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Xunit;
using Xunit.Abstractions;

namespace DynamoDBCRUD
{
    public class LowLevelTableExampleTest
    {
        private readonly ITestOutputHelper output;

        public LowLevelTableExampleTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static string ip = "localhost";
        private static int port = 8000;
        private readonly string _endpointURL = "http://" + ip + ":" + port.ToString();

        private IDynamoDBContext CreateMockDynamoDBContext(AmazonDynamoDBClient client)
        {
            var mockDynamoDBContext = new DynamoDBContext(client);

            return mockDynamoDBContext;
        }

        private bool IsPortInUse(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        [Fact]
        public void CheckLowLevelTableExample()
        {
            var portUsed = IsPortInUse(port);
            if (portUsed)
            {
                throw new Exception("You must run local DynamoDB on port 8000");
            }

            var clientConfig = new AmazonDynamoDBConfig();
            clientConfig.ServiceURL = _endpointURL;
            var client = new AmazonDynamoDBClient(clientConfig);

            output.WriteLine("Creating example table.");
            var result = LowLevelTableExample.CreateExampleTable(client);

            if (!result.Result)
            {
                output.WriteLine("Could not create example table.");
                return;
            }

            output.WriteLine("Listing tables.");
            result = LowLevelTableExample.ListMyTables(client);

            if (!result.Result)
            {
                output.WriteLine("Could not create example table.");
                return;
            }

            output.WriteLine("Getting example table information.");
            result = LowLevelTableExample.GetTableInformation(client);

            if (!result.Result)
            {
                output.WriteLine("Could not get example table information.");
                return;
            }

            output.WriteLine("Updating example table.");
            result = LowLevelTableExample.UpdateExampleTable(client);

            if (!result.Result)
            {
                output.WriteLine("Could not update example table");
                return;
            }

            output.WriteLine("Deleting example table.");
            result = LowLevelTableExample.DeleteExampleTable(client);

            if (!result.Result)
            {
                output.WriteLine("Could not delete example table");
                return;
            }

            output.WriteLine("Done");




        }
    }
}