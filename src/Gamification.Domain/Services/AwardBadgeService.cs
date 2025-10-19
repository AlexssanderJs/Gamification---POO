using System;
using Gamification.Domain.Model;
using Gamification.Domain.Ports;

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
        Guid? requestId = null) 
    {
        if (!_readStore.MissaoConcluida(studentId, missionId))
        {
            throw new InvalidOperationException("Estudante não concluiu a missão e não é elegível para a badge.");
        }

        if (_readStore.BadgeJaConcedida(studentId, missionId, badge))
        {
            return;
        }
        
        _writeStore.SalvarConcessaoAtomicamente(
            studentId, 
            missionId, 
            badge, 
            xpBase, 
            "mission_completion_no_bonus", 
            requestId
        );
    }
}