using FlowWise.Services.Consolidacao.Application.QueryHandlers;
using FlowWise.Services.Consolidacao.Application.Queries;
using FlowWise.Services.Consolidacao.Application.Dtos;
using FlowWise.Services.Consolidacao.Domain.Entities;
using FlowWise.Services.Consolidacao.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using NSubstitute.ExceptionExtensions;

namespace FlowWise.Services.Consolidacao.Tests.UnitTests.Application.QueryHandlers
{
    /// <summary>
    /// Contém testes unitários para o <see cref="GetSaldoDiarioQueryHandler"/>.
    /// </summary>
    /// <remarks>
    /// Estes testes verificam se o handler de consulta para o saldo diário
    /// recupera e mapeia corretamente os dados do repositório.
    /// REQ-FLW-CON-001: Exibir saldo diário consolidado.
    /// HU-006: Como um usuário, eu quero ver o saldo total de um dia específico.
    /// NFR-PERF-002: O serviço de consolidação deve ter alta performance na leitura de dados.
    /// TDD: Desenvolvido seguindo os princípios de Test-Driven Development.
    /// </remarks>
    public class GetSaldoDiarioQueryHandlerTests
    {
        private readonly ISaldoDiarioRepository _mockSaldoDiarioRepository;
        private readonly ILogger<GetSaldoDiarioQueryHandler> _mockLogger;
        private readonly GetSaldoDiarioQueryHandler _handler;

        public GetSaldoDiarioQueryHandlerTests()
        {
            _mockSaldoDiarioRepository = Substitute.For<ISaldoDiarioRepository>();
            _mockLogger = Substitute.For<ILogger<GetSaldoDiarioQueryHandler>>();
            _handler = new GetSaldoDiarioQueryHandler(
                _mockSaldoDiarioRepository,
                _mockLogger
            );
        }

        /// <summary>
        /// Testa se o handler retorna um <see cref="SaldoDiarioDto"/> correto quando um saldo é encontrado.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Garante a recuperação e mapeamento correto do saldo diário.
        /// HU-006: Retorno esperado para a exibição do saldo.
        /// TDD: Cenário de sucesso na recuperação do saldo.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001, HU-006")]
        public async Task Handle_SaldoDiarioFound_ReturnsCorrectDto()
        {
            // Arrange
            var dataConsulta = (DateTime.Today);
            var query = new GetSaldoDiarioQuery(dataConsulta);

            var saldoDiarioEntity = SaldoDiario.Create(dataConsulta);
            saldoDiarioEntity.AplicarLancamento(TipoLancamento.Credito.ToString(), 200.00m);
            saldoDiarioEntity.AplicarLancamento(TipoLancamento.Debito.ToString(), 50.00m);

            _mockSaldoDiarioRepository.GetByDateAsync(dataConsulta).Returns(saldoDiarioEntity);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().Be(dataConsulta);
            result.TotalCreditos.Should().Be(saldoDiarioEntity.TotalCreditos);
            result.TotalDebitos.Should().Be(saldoDiarioEntity.TotalDebitos);
            result.SaldoTotal.Should().Be(saldoDiarioEntity.SaldoTotal);
            _mockSaldoDiarioRepository.Received(1).GetByDateAsync(dataConsulta);
        }

        /// <summary>
        /// Testa se o handler retorna <c>null</c> quando nenhum <see cref="SaldoDiario"/> é encontrado para a data.
        /// </summary>
        /// <remarks>
        /// REQ-FLW-CON-001: Tratamento de cenário onde o saldo não existe.
        /// TDD: Cenário de falha (não encontrado) na recuperação do saldo.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, REQ-FLW-CON-001")]
        public async Task Handle_SaldoDiarioNotFound_ReturnsNull()
        {
            // Arrange
            var dataConsulta = (DateTime.Today.AddDays(-1));
            var query = new GetSaldoDiarioQuery(dataConsulta);

            _mockSaldoDiarioRepository.GetByDateAsync(dataConsulta).Returns((SaldoDiario)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
            _mockSaldoDiarioRepository.Received(1).GetByDateAsync(dataConsulta);
        }

        /// <summary>
        /// Testa se o handler lida com exceções durante a busca do saldo.
        /// </summary>
        /// <remarks>
        /// NFR-RES-001: Teste de resiliência e tratamento de erros.
        /// TDD: Cenário de falha - tratamento de exceção.
        /// </remarks>
        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, NFR-RES-001")]
        public async Task Handle_RepositoryThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var dataConsulta = (DateTime.Today);
            var query = new GetSaldoDiarioQuery(dataConsulta);

            _mockSaldoDiarioRepository
                .GetByDateAsync(dataConsulta)
                .Throws(new InvalidOperationException("Erro de banco de dados simulado."));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));

            _mockSaldoDiarioRepository.Received(1).GetByDateAsync(dataConsulta);
        }
    }
}