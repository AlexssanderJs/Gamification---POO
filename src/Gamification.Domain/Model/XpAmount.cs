namespace Gamification.Domain.Model;

public record XpAmount
{
    public int Value { get; }
    public XpAmount(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("A quantidade de XP não pode ser negativa.", nameof(value));
        }

        Value = value;

    }
    
    public static implicit operator XpAmount(int value) => new XpAmount(value);
    public static implicit operator int(XpAmount xp) => xp.Value;
}
