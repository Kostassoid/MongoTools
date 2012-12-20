using System;
using System.Linq;
using MongoDB.Driver;

namespace Kostassoid.Mongo.Tester
{
    internal class MongoConnectionTester
    {
        private readonly bool _isReplicaSet;
        private readonly string[] _hosts;

        public MongoConnectionTester(bool isReplicaSet, params string[] hosts)
        {
            _isReplicaSet = isReplicaSet;
            _hosts = hosts;
        }

        public MongoServerAddress GetServerAddress(string host)
        {
            var parts = host.Split(':');
            return parts.Length == 1
                       ? new MongoServerAddress(host)
                       : new MongoServerAddress(parts[0], Convert.ToInt32(parts[1]));
        }

        public bool Perform()
        {
            var hostsAsString = string.Join(", ", _hosts);

            Console.WriteLine("Trying {0} connection to {1}", _isReplicaSet ? "replica set" : "direct", hostsAsString);

            var connection = new MongoClientSettings
                {
                    ConnectionMode = _isReplicaSet ? ConnectionMode.ReplicaSet : ConnectionMode.Direct,
                    Servers = _hosts.Select(GetServerAddress),
                    ReadPreference = new ReadPreference(ReadPreferenceMode.PrimaryPreferred)
                };

            var server = new MongoClient(connection).GetServer();
            try
            {
                server.Connect(TimeSpan.FromSeconds(10));
                server.Ping();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Successfully connected to {0} (actual servers: {1})", hostsAsString, string.Join(", ", server.Instances.Select(i => i.Address.Host + ":" + i.Address.Port)));
                Console.ResetColor();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed to connect to {0} due to {1}", hostsAsString, ex);
                Console.ResetColor();
                return false;
            }
        }
    }
}