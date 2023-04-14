namespace Core.Domain;

public record class Quantity
{
    public decimal Value { get; private set; }

    public static Quantity From(Quantity quantity)
    {
        return new Quantity(quantity.Value);
    }

    public static Quantity From(decimal quantity)
    {
        return new Quantity(quantity);
    }

    public Quantity(decimal value)
    {
        if (value < 0)
        {
            throw new QuantityShouldBeGreaterThanZeroException();
        }

        Value = value;
    }

    public static implicit operator decimal(Quantity quantity) => quantity.Value;

    public Quantity Multiply(decimal value)
    {
        return new Quantity(Value * value);
    }

    public static Quantity operator *(Quantity a, decimal b) => a.Multiply(b);
}

