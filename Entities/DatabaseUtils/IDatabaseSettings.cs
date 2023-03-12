namespace Entities.DatabaseUtils
{
    public interface IDatabaseSettings
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string TestCollectionName { get; set; }
    }
}
