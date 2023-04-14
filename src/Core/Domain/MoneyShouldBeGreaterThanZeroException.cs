namespace Core.Domain;
public class MoneyShouldBeGreaterOrEqualThanZeroException : DomainException
{
    public MoneyShouldBeGreaterOrEqualThanZeroException() : base(typeof(MoneyShouldBeGreaterOrEqualThanZeroException))
    {

    }
}
