namespace Core.Infrastructure;

public class Table
{
    private readonly string _fullName;
    private readonly string? _alias;

    public Table(string schema, string name, string? alias = null)
    {
        _fullName = string.IsNullOrEmpty(alias) ? $"{schema}.{name}" : $"{schema}.{name} AS {alias}";
        _alias = alias;
    }

    public string Field(string field) => string.IsNullOrEmpty(_alias) ? $"{_fullName}.{field}" : $"{_alias}.{field}";

    public string Field(string field, string alias) => $"{Field(field)} AS {alias}";

    public string AllFields => $"{_fullName}.*";

    public static implicit operator string(Table table) => table.ToString();

    public override string ToString() => _fullName;
}

