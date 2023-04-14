namespace Core.Domain;

public record Money
{
    public decimal Value { get; private set; }
    public static Money From(Money money)
    {
        return new Money(money.Value);
    }

    public Money(decimal value)
    {
        if (value < 0)
        {
            throw new MoneyShouldBeGreaterOrEqualThanZeroException();
        }

        Value = value;
    }

    public Money Round(int decimals = 2)
    {
        var value = Math.Round(Value, decimals, MidpointRounding.AwayFromZero);

        return new Money(value);
    }

    public Money Add(Money money)
    {
        return new Money(Value + money.Value);
    }

    public Money Subtract(Money money)
    {
        return new Money(Value - money.Value);
    }

    public static Money From(decimal value)
    {
        return new Money(value);
    }

    public Money Multiply(decimal value)
    {
        return new Money(Value * value);
    }

    public static implicit operator decimal(Money money) => money.Value;

    public static Money operator +(Money a, Money b) => a.Add(b);

    public static Money operator *(Money a, decimal b) => a.Multiply(b);

    public static Money operator -(Money a, Money b) => a.Subtract(b);
}

