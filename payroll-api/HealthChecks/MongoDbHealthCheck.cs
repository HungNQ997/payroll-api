using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace payroll_api.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public MongoDbHealthCheck(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create MongoClient instance
                var mongoClient = new MongoClient(_connectionString);

                // Access a MongoDB database (you can replace "admin" with any existing database name)
                var adminDatabase = mongoClient.GetDatabase("admin");

                // Run a sample command to check connectivity (e.g., listing collections in the admin database)
                var collectionNames = await adminDatabase.ListCollectionNamesAsync(cancellationToken: cancellationToken);

                // If no exception is thrown and collections are retrieved, MongoDB connection is successful
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                // If any exception occurs, MongoDB connection is unhealthy
                return HealthCheckResult.Unhealthy($"MongoDB connection check failed: {ex.Message}", ex); ;
            }
        }
    }
}