﻿namespace Entities.DatabaseUtils
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }


        #region Collections
        
        string UsersCollectionName { get; set; }
        
        #endregion
    }
}