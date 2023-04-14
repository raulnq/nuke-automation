namespace Core.Migrator
{
    public class DbMigrationException : Exception
    {
        public DbMigrationException() : base()
        {
        }

        public DbMigrationException(string message) : base(message)
        {
        }

        public DbMigrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public DbMigrationException(Exception innerException) : base("Migration failed", innerException)
        {
        }
    }
}