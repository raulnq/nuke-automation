namespace Core.Infrastructure;

public class DbSchema
{
    public string Name { get; set; }

    public DbSchema(string name)
    {
        Name = name;
    }
}
