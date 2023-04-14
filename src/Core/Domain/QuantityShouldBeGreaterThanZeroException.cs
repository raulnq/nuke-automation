namespace Core.Domain;

public class QuantityShouldBeGreaterThanZeroException : DomainException
{
    public QuantityShouldBeGreaterThanZeroException() : base(typeof(QuantityShouldBeGreaterThanZeroException))
    {

    }
}