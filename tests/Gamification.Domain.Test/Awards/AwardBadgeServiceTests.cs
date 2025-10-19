// AwardBadgeServiceTests.cs
using Xunit;
using System;
using Gamification.Domain.Model;
using Gamification.Domain.Ports; 
using Gamification.Domain.Tests.Awards; 
using Gamification.Domain.Services; 

namespace Gamification.Domain.Tests.Awards;

public class AwardBadgeServiceTests
{

    private readonly Guid _studentId = Guid.NewGuid();
    private readonly Guid _missionId = Guid.NewGuid();
    private readonly Badge _defaultBadge = new Badge("first-mission-badge", "Primeira Missão");
    private readonly XpAmount _xpBase = 500;
    
    private readonly DateTimeOffset _bonusFullWeightEnd = DateTimeOffset.MinValue; 
    private readonly DateTimeOffset _bonusFinalDate = DateTimeOffset.MinValue; 
    private readonly int _xpFullWeight = 100;
    private readonly int _xpReducedWeight = 50;

    private void CallAwardBadge(IAwardsReadStore readStore, IAwardsWriteStore writeStore, Guid? requestId = null)
    {
        var service = new AwardBadgeService(readStore, writeStore);
        service.AwardBadge(
            _studentId, _missionId, DateTimeOffset.UtcNow, _defaultBadge, _xpBase,
            _bonusFullWeightEnd, _bonusFinalDate, _xpFullWeight, _xpReducedWeight,
            requestId
        );
    }

    [Fact(DisplayName = "ConcederBadge_quando_missao_concluida_concede_uma_unica_vez")]
    [Trait("Categoria", "Concessão Básica")]
    public void T1_ConcederBadge_quando_missao_concluida_concede_uma_unica_vez()
    {

        var readStoreFake = new FakeAwardsReadStore(isMissionCompleted: true, isBadgeAlreadyAwarded: false);
        var writeStoreFake = new FakeAwardsWriteStore();
        
        CallAwardBadge(readStoreFake, writeStoreFake);

        Assert.Equal(1, writeStoreFake.CallsCount); 
        Assert.Equal(500, writeStoreFake.LastSavedXp.Value); 
        Assert.Contains("sem bônus", writeStoreFake.LastAuditReason);
    }

    [Fact(DisplayName = "ConcederBadge_sem_concluir_missao_deve_falhar_com_excecao")]
    [Trait("Categoria", "Elegibilidade")]
    public void T2_ConcederBadge_sem_concluir_missao_deve_falhar_com_excecao()
    {

        var readStoreFake = new FakeAwardsReadStore(isMissionCompleted: false);
        var writeStoreFake = new FakeAwardsWriteStore();
        
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            CallAwardBadge(readStoreFake, writeStoreFake);
        });

        Assert.Equal(0, writeStoreFake.CallsCount);
        Assert.Contains("não é elegível", ex.Message);
    }
    
    [Fact(DisplayName = "AwardBadge_com_requestId_repetido_deve_garantir_idempotencia")]
    [Trait("Categoria", "Idempotência")]
    public void T6_AwardBadge_com_requestId_repetido_deve_garantir_idempotencia()
    {
        var requestId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        
        var readStoreFakePrimeiraChamada = new FakeAwardsReadStore(isRequestProcessed: false);
        var writeStoreFake = new FakeAwardsWriteStore(); 

        CallAwardBadge(readStoreFakePrimeiraChamada, writeStoreFake, requestId);

        var readStoreFakeSegundaChamada = new FakeAwardsReadStore(isMissionCompleted: true, isBadgeAlreadyAwarded: false, isRequestProcessed: true);
        var serviceRepetido = new AwardBadgeService(readStoreFakeSegundaChamada, writeStoreFake);
        
        serviceRepetido.AwardBadge(
            _studentId, _missionId, now, _defaultBadge, _xpBase, 
            _bonusFullWeightEnd, _bonusFinalDate, _xpFullWeight, _xpReducedWeight,
            requestId 
        );

        Assert.Equal(1, writeStoreFake.CallsCount); 
    }
}