// IAwardsWriteStore.cs
using Gamification.Domain.Model;
using System;

namespace Gamification.Domain.Ports;

public interface IAwardsWriteStore
{
    void SalvarConcessaoAtomicamente(
        Guid studentId, 
        Guid missionId, 
        Badge awardedBadge, 
        XpAmount xpBonus, 
        string auditReason,
        Guid? requestId = null
    );
}