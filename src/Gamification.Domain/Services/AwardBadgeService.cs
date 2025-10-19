// AwardBadgeService.cs
using System;
using Gamification.Domain.Model;
using Gamification.Domain.Ports; // <--- OBRIGATÓRIO para as interfaces
using Gamification.Domain.Policies; // <--- OBRIGATÓRIO para a BonusPolicy e BonusPolicyResult

namespace Gamification.Domain.Services; 

public class AwardBadgeService
{
    private readonly IAwardsReadStore _readStore;
    private readonly IAwardsWriteStore _writeStore;

    public AwardBadgeService(IAwardsReadStore readStore, IAwardsWriteStore writeStore)
    {
        _readStore = readStore;
        _writeStore = writeStore;
    }

    public void AwardBadge(
        Guid studentId, 
        Guid missionId, 
        DateTimeOffset now,
        Badge badge, 
        XpAmount xpBase, 
        DateTimeOffset bonusFullWeightEnd,
        DateTimeOffset bonusFinalDate,
        int xpFullWeight,
        int xpReducedWeight,
        Guid? requestId = null) 
    {
        // 0. CHECAGEM DE IDEMPOTÊNCIA (T6)
        if (requestId.HasValue && _readStore.RequisicaoJaProcessada(requestId.Value))
        {
            return; 
        }

        // 1. CHECAGEM DE ELEGIBILIDADE (T2)
        if (!_readStore.MissaoConcluida(studentId, missionId))
        {
            throw new InvalidOperationException("Estudante não concluiu a missão e não é elegível para a badge.");
        }

        // 2. CHECAGEM DE UNICIDADE (T1)
        if (_readStore.BadgeJaConcedida(studentId, missionId, badge))
        {
            return; 
        }

        // 3. CÁLCULO DE BÔNUS (T3, T4, T5)
        var bonusResult = BonusPolicy.CalculateBonus(
            now, 
            bonusFullWeightEnd, 
            bonusFinalDate, 
            xpBase, 
            xpFullWeight, 
            xpReducedWeight
        );

        // 4. GRAVAR ATOMICAMENTE
        _writeStore.SalvarConcessaoAtomicamente(
            studentId, 
            missionId, 
            badge, 
            bonusResult.TotalXp, 
            bonusResult.AuditReason, 
            requestId
        );
    }
}