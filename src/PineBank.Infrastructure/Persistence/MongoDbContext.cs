// ./src/PineBank.Infrastructure/Persistence/MongoDbContext.cs
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PineBank.Domain.Entities;
using System;

namespace PineBank.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        private readonly ILogger<MongoDbContext> _logger;
        public IMongoDatabase Database { get; }
        private readonly MongoClient _client;

        public MongoDbContext(IConfiguration configuration, ILogger<MongoDbContext> logger)
        {
            _logger = logger;

            try
            {
                _logger.LogInformation("Attempting to connect to MongoDB...");

                // Use simple connection string without authentication
                var settings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
                settings.ConnectTimeout = TimeSpan.FromSeconds(30);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);

                _client = new MongoClient(settings);
                Database = _client.GetDatabase("pinebankdb");

                // Test connection
                _logger.LogInformation("Testing MongoDB connection...");
                Database.RunCommand<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument("ping", 1));
                _logger.LogInformation("Successfully connected to MongoDB");

                // Ensure indexes
                EnsureIndexes();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB connection");
                throw;
            }
        }

        private void EnsureIndexes()
        {
            try
            {
                _logger.LogInformation("Creating indexes...");
                var conversationsCollection = Conversations;
                var indexKeysDefinition = Builders<Conversation>.IndexKeys.Ascending(c => c.UserId);
                var createIndexOptions = new CreateIndexOptions { Background = true };
                var indexModel = new CreateIndexModel<Conversation>(indexKeysDefinition, createIndexOptions);
                conversationsCollection.Indexes.CreateOne(indexModel);
                _logger.LogInformation("Indexes created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Warning: Error creating indexes");
            }
        }

        public IMongoCollection<Conversation> Conversations => Database.GetCollection<Conversation>("Conversations");
    }
}