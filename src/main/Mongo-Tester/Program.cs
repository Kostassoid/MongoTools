using System;
using System.Linq;

namespace Kostassoid.Mongo.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var isReplicaSet = args.Any(a => a.ToLower() == "-replicaset");
            var hosts = args.Where(a => !a.StartsWith("-")).ToList();

            if (hosts.Count < 1)
            {
                ShowUsage();
                return;
            }

            foreach (var host in hosts)
            {
                new MongoConnectionTester(isReplicaSet, host).Perform();
            }

            if (isReplicaSet)
            {
                new MongoConnectionTester(true, hosts.ToArray()).Perform();
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("MongoDb Connection Tester\n");
            Console.WriteLine("Usage: Mongo-Tester [-direct|-replicaset] host[:port] [host[:port]...]");
        }
    }
}
