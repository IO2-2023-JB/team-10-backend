﻿namespace Entities.DatabaseUtils
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string UsersCollectionName { get; set; }
    }
}