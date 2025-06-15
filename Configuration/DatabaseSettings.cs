namespace FluentNHibernate.DAL.Configuration;

public class DatabaseSettings
{
    public DBType DBType { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public string Database { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }    

    public int BatchSize { get; set; } = 1000;        

    public bool UseProfiler { get; set; } = false;

    public bool ShowSql { get; set; } = false;

    /// <summary>
    /// File path for database (SQLite etc..)
    /// </summary>
    public string DBFilePath { get; set; }

    public bool UpdateSchema { get; set; } = false;

    public bool DropAndCreateSchema { get; set; } = false;

    public bool ValidateSchema { get; set; } = false;
}