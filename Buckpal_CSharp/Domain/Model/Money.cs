namespace Buckpal_CSharp.Domain.Model;

public record Money(decimal Amount)
{
    public static Money Zero() => new Money(0m);
    public static Money Of(decimal value) => new Money(value);
    public static Money Add(Money a, Money b) => new Money(a.Amount + b.Amount);
    public static Money Subtract(Money a, Money b) => new Money(a.Amount - b.Amount);
    
    public Money Plus(Money money) => new Money(this.Amount + money.Amount);
    public Money Minus(Money money) => new Money(this.Amount - money.Amount);
    public Money Negate() => new Money(-this.Amount);
    
    public bool IsPositiveOrZero() => this.Amount >= 0;
    public bool IsNegative() => this.Amount < 0;
    public bool IsPositive() => this.Amount > 0;
    public bool IsGreaterThanOrEqualTo(Money money) => this.Amount >= money.Amount;
}
