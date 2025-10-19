using Gamification.Domain.Model;
using System;

namespace Gamification.Domain.Ports;

public interface IAwardsReadStore
{
    bool MissaoConcluida(Guid studentId, Guid missionId);
    
    bool BadgeJaConcedida(Guid studentId, Guid missionId, Badge badge);

    bool RequisicaoJaProcessada(Guid requestId);
}