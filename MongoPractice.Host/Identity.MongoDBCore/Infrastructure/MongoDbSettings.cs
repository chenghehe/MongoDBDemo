namespace Identity.MongoDBCore.Infrastructure
{
    /// <summary>
    /// A class representing the settings for the MongoDb server.
    /// </summary>
    public class MongoDbSettings
    {
        /// <summary>
        /// The connection string for the MongoDb server.
        /// </summary>
        public string MongoConnection { get; set; }
        /// <summary>
        /// The name of the MongoDb database where the identity data will be stored.
        /// </summary>
        public string DataBase { get; set; }
    }
}
