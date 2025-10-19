using Gamification.Domain.Model;
using Gamification.Domain.Policies;

namespace Gamification.Domain.Tests.Policies;

public class BonusPolicyTests
{
   
    private readonly XpAmount _xpBase = 500;
    private readonly int _xpFullWeight = 100;
    private readonly int _xpReducedWeight = 50; 
    
    private readonly DateTimeOffset _bonusFullWeightEnd = DateTimeOffset.UtcNow.AddHours(5); 
    private readonly DateTimeOffset _bonusFinalDate = DateTimeOffset.UtcNow.AddDays(1);

    [Fact(DisplayName = "ConcederBadge_dentro_FullWeight_concede_bonus_integral")]
    [Trait("Categoria", "Janelas de Bônus")]
    public void T3_ConcederBadge_dentro_FullWeight_concede_bonus_integral()
    {
        var now = _bonusFullWeightEnd.AddMilliseconds(-1);

        var result = BonusPolicy.CalculateBonus(
            now,
            _bonusFullWeightEnd,
            _bonusFinalDate,
            _xpBase,
            _xpFullWeight,
            _xpReducedWeight
        );

        Assert.Equal(600, result.TotalXp.Value);
        Assert.Contains("janela integral", result.AuditReason);
    }

    [Fact(DisplayName = "ConcederBadge_apos_FullWeight_e_ate_FinalDate_concede_bonus_reduzido")]
    [Trait("Categoria", "Janelas de Bônus")]
    public void T4_ConcederBadge_apos_FullWeight_e_ate_FinalDate_concede_bonus_reduzido()
    {

        var now = _bonusFullWeightEnd.AddMilliseconds(1);

        // ACT (WHEN)
        var result = BonusPolicy.CalculateBonus(
            now,
            _bonusFullWeightEnd,
            _bonusFinalDate,
            _xpBase,
            _xpFullWeight,
            _xpReducedWeight
        );

        Assert.Equal(550, result.TotalXp.Value);
        Assert.Contains("janela reduzida", result.AuditReason);
    }

    [Fact(DisplayName = "ConcederBadge_apos_FinalDate_nao_concede_bonus")]
    [Trait("Categoria", "Janelas de Bônus")]
    public void T5_ConcederBadge_apos_FinalDate_nao_concede_bonus()
    {
        
        var now = _bonusFinalDate.AddMilliseconds(1); 

        var result = BonusPolicy.CalculateBonus(
            now, 
            _bonusFullWeightEnd, 
            _bonusFinalDate, 
            _xpBase, 
            _xpFullWeight, 
            _xpReducedWeight
        );

        Assert.Equal(500, result.TotalXp.Value); 
        Assert.Contains("sem bônus", result.AuditReason);
    }
}