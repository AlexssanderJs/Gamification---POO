// AwardBadgeServiceTests.cs
using Xunit;
using System;
using Gamification.Domain.Model;
using Gamification.Domain.Tests.Awards;
using Gamification.Domain.Services;

namespace Gamification.Domain.Tests.Awards;

public class AwardBadgeServiceTests
{

    private readonly Guid _studentId = Guid.NewGuid();
    private readonly Guid _missionId = Guid.NewGuid();
    private readonly Badge _defaultBadge = new Badge("first-mission-badge", "Primeira Missão");
    private readonly XpAmount _xpBase = 500; 

    [Fact(DisplayName = "ConcederBadge_quando_missao_concluida_concede_uma_unica_vez")]
    [Trait("Categoria", "Concessão Básica")]
    public void T1_ConcederBadge_quando_missao_concluida_concede_uma_unica_vez()
    {
      
        var readStoreFake = new FakeAwardsReadStore(isMissionCompleted: true, isBadgeAlreadyAwarded: false);
        var writeStoreFake = new FakeAwardsWriteStore();
        
        var service = new AwardBadgeService(readStoreFake, writeStoreFake); 
        
        service.AwardBadge(_studentId, _missionId, DateTimeOffset.UtcNow, _defaultBadge, _xpBase);

        Assert.Equal(1, writeStoreFake.CallsCount); 
        
        Assert.Equal(500, writeStoreFake.LastSavedXp.Value); 
    }
}