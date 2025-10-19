# 🏅 Projeto Gamification.Domain: Concessão Atômica de Badges

## 🎯 Objetivo

Este projeto implementa o serviço de domínio (`AwardBadgeService`) para a concessão de Badges (Insígnias) e XP (Pontos de Experiência) em um contexto de gamificação acadêmica (ClassHero).

O foco principal foi o desenvolvimento guiado por testes (TDD - Test Driven Development) e a aplicação dos princípios da Arquitetura Limpa (Ports and Adapters), garantindo que todas as regras de negócio fossem cobertas por testes unitários e o domínio fosse totalmente isolado da infraestrutura.

## 🌟 Requisitos de Negócio Cobertos

O serviço de domínio garante as seguintes regras críticas, testadas e implementadas no ciclo TDD:

| Regra de Domínio | Testes Cobertos | Descrição |
| :--- | :--- | :--- |
| **Unicidade** | `T1` | Uma mesma badge não pode ser concedida mais de uma vez para o mesmo estudante na mesma missão. |
| **Elegibilidade** | `T2` | A concessão só ocorre se a missão pré-requisito for comprovadamente concluída. |
| **Janelas de Bônus** | `T3`, `T4`, `T5` | O cálculo do XP bônus é determinado pela data/hora da concessão (`now`) em relação às janelas de prazo (Integral, Reduzido, Zero). |
| **Idempotência** | `T6` | Requisições repetidas (com o mesmo `requestId`) são processadas uma única vez, garantindo a atomicidade e consistência. |
| **Invariante de Valor** | N/A | O `XpAmount` (Value Object) garante que o XP nunca pode ser negativo. |

## 📐 Arquitetura: Ports and Adapters (Arquitetura Limpa)

O design da solução está estritamente separado em camadas para isolar a lógica de negócio (Domínio) da tecnologia (Infraestrutura):

| Camada | Componentes | Função |
| :--- | :--- | :--- |
| **Domínio** | `AwardBadgeService` | Contém toda a lógica de negócio e orquestração. |
| **Domínio (VOs)** | `XpAmount`, `Badge` | Objetos imutáveis que garantem a validade dos valores. |
| **Domínio (Policies)** | `BonusPolicy` | Lógica pura e estática de cálculo de bônus, isolada do serviço. |
| **Ports** | `IAwardsReadStore`, `IAwardsWriteStore` | Interfaces que definem **o que** o Domínio precisa ler/escrever, sem saber **onde** (Banco de Dados, Fila, etc.). |
| **Adapters/Testes** | `FakeStores.cs` | Implementações de teste dos Ports, permitindo o isolamento do Domínio. |

## 🚀 Como Rodar e Validar os Testes

O projeto utiliza **.NET 8** e o framework de testes **xUnit**.

Para validar a implementação de todas as regras de negócio, execute os testes unitários na raiz da solução:

### Pré-requisitos

* [.NET 9.0 SDK]

### Comandos

1.  **Clone o repositório:**
    ```bash
    git clone [LINK DO SEU REPOSITÓRIO]
    cd Gamification
    ```

2.  **Rode todos os testes unitários:**
    ```bash
    dotnet test
    ```

**Saída Esperada:** Todos os 6 testes unitários em `Gamification.Domain.Tests` devem passar (`[Passed]`), confirmando a cobertura total das regras de domínio.

## 📝 Histórico de Desenvolvimento (TDD)

O projeto foi desenvolvido seguindo o ciclo **RED, GREEN, REFACTOR**, com *commits* pequenos e atômicos.

O histórico do Git demonstra a progressão da cobertura de testes, onde a regra de negócio só foi implementada após a falha do teste correspondente. A sequência de desenvolvimento seguiu:

1.  **T1/T2:** Implementação e testes de Unicidade e Elegibilidade.
2.  **T3/T4/T5:** Implementação e testes da `BonusPolicy` e integração com o serviço.
3.  **T6:** Implementação e teste da Idempotência (regra final).