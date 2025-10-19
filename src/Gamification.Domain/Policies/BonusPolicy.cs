using Gamification.Domain.Model;
using System;

namespace Gamification.Domain.Policies;

public record BonusPolicyResult(XpAmount TotalXp, string AuditReason);

public static class BonusPolicy
{
    public static BonusPolicyResult CalculateBonus(
        DateTimeOffset now,
        DateTimeOffset bonusFullWeightEndDate,
        DateTimeOffset bonusFinalDate,
        XpAmount xpBase,
        int xpFullWeight,
        int xpReducedWeight)
    {
        XpAmount bonus;
        string reason;

        if (now <= bonusFullWeightEndDate)
        {
            bonus = xpFullWeight;
            reason = "janela integral";
        }
      
        else if (now <= bonusFinalDate)
        {
            bonus = xpReducedWeight;
            reason = "janela reduzida";
        }
       
        else
        {
            bonus = 0;
            reason = "sem bônus (data final expirou)";
        }

        return new BonusPolicyResult(xpBase.Value + bonus.Value, reason);
    }
}