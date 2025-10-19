using System;
using Gamification.Domain.Model;
using Gamification.Domain.Ports;

namespace Gamification.Domain.Tests.Awards;

public class FakeAwardsReadStore : IAwardsReadStore
{
    private readonly bool _isMissionCompleted;
    private readonly bool _isBadgeAlreadyAwarded;
    private readonly bool _isRequestProcessed;

    public FakeAwardsReadStore(bool isMissionCompleted = true, 
                               bool isBadgeAlreadyAwarded = false, 
                               bool isRequestProcessed = false)
    {
        _isMissionCompleted = isMissionCompleted;
        _isBadgeAlreadyAwarded = isBadgeAlreadyAwarded;
        _isRequestProcessed = isRequestProcessed;
    }

    public bool MissaoConcluida(Guid studentId, Guid missionId) => _isMissionCompleted;
    public bool BadgeJaConcedida(Guid studentId, Guid missionId, Badge badge) => _isBadgeAlreadyAwarded;
    public bool RequisicaoJaProcessada(Guid requestId) => _isRequestProcessed;
}


public class FakeAwardsWriteStore : IAwardsWriteStore
{
    public int CallsCount { get; private set; } = 0;
    public XpAmount LastSavedXp { get; private set; } = new XpAmount(0);
    public string LastAuditReason { get; private set; } = string.Empty;

    public void SalvarConcessaoAtomicamente(
        Guid studentId, 
        Guid missionId, 
        Badge awardedBadge, 
        XpAmount xpBonus, 
        string auditReason, 
        Guid? requestId = null)
    {
        CallsCount++; 
        LastSavedXp = xpBonus; 
        LastAuditReason = auditReason;
    }
}