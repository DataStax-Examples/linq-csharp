using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Cassandra.Data.Linq;

namespace linq_csharp
{
    class Program
    {
        public ICluster Cluster { get; set; }
        public ISession Session { get; set; }
        public static Guid User0Guid { get; set; }

        private static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            User0Guid = Guid.NewGuid();
            // build cluster connection
            Cluster =
                Cassandra.Cluster.Builder()
                    .AddContactPoint("127.0.0.1")
                    .Build();

            //Set the Mapping Configuration
            MappingConfiguration.Global.Define(
               new Map<User>()
                  .TableName("users")
                  .PartitionKey(u => u.UserId)
                  .Column(u => u.UserId, cm => cm.WithName("id")));

            // create session
            Session = await Cluster.ConnectAsync().ConfigureAwait(false);

            // prepare schema
            await Session.ExecuteAsync(new SimpleStatement("CREATE KEYSPACE IF NOT EXISTS examples WITH replication = { 'class': 'SimpleStrategy', 'replication_factor': '1' }")).ConfigureAwait(false);
            await Session.ExecuteAsync(new SimpleStatement("USE examples")).ConfigureAwait(false);
            await Session.ExecuteAsync(new SimpleStatement("CREATE TABLE IF NOT EXISTS users(id uuid, name text, age int, PRIMARY KEY(id))")).ConfigureAwait(false);

            try
            {
                //Create an instance of a Mapper from the session
                var users = new Table<User>(Session);
                await InsertOperations(users);
                await QueryOperations(users);
                await UpdateOperations(users);
                await DeleteOperations(users);
            }
            finally
            {
                await Cluster.ShutdownAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Demonstrates how to delete via Linq
        /// </summary>
        /// <returns></returns>
        private static async Task DeleteOperations(Table<User> users)
        {
            // Delete
            await users.Where(u => u.UserId == User0Guid)
                  .Delete()
                  .ExecuteAsync();

            // Delete If 
            await users.Where(u => u.UserId == User0Guid)
                  .DeleteIf(u => u.Name == "User 0")
                  .ExecuteAsync();
        }

        /// <summary>
        /// Demonstrates the ability to update via Linq
        /// </summary>
        /// <returns></returns>
        private static async Task UpdateOperations(Table<User> users)
        {
            await users.Where(u => u.UserId == User0Guid)
                .Select(u => new User { Name = "Update Linq" })
                .Update()
                .ExecuteAsync();
            var user = await users.Where(u => u.UserId == User0Guid).ExecuteAsync();
        }

        /// <summary>
        /// Demonstrates the different query methods allowed via Linq. 
        /// </summary>
        /// <returns></returns>
        private static async Task QueryOperations(Table<User> users)
        {
            IEnumerable<User> usrs = await users.Select(a => a).ExecuteAsync();
            Console.WriteLine($"Retrieved {usrs.Count()} users");
            usrs = await users.Where(u => u.UserId == User0Guid).ExecuteAsync();
            Console.WriteLine($"Retrieved {usrs.Count()} users");
            var usr = await users.Where(u => u.UserId == User0Guid).First().ExecuteAsync();
            Console.WriteLine($"Retrieved {usr.ToString()}");
            usr = await users.Where(u => u.UserId == User0Guid).FirstOrDefault().ExecuteAsync();
            Console.WriteLine($"Retrieved {usr.ToString()}");
        }

        /// <summary>
        /// Demonstrates how to perform singular insert operations using Linq
        /// </summary>
        /// <param name="users">The User table</param>
        /// <returns></returns>
        private static async Task InsertOperations(Table<User> users)
        {
            //Insert a single record using a POCO
            await users.Insert(new User() { UserId = User0Guid, Name = "User 0", Age = 0 }).ExecuteAsync();
        }
    }
}