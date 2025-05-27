using FlowWise.Services.Lancamentos.Domain.Entities;
using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FlowWise.Services.Lancamentos.Domain.Events;
using FluentAssertions;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Domain.Entities
{
    /// <summary>
    /// Contém testes unitários para a entidade de domínio Lancamento.
    /// </summary>
    public class LancamentoTests
    {
        #region Testes de Criação (Lancamento.Create)

        [Fact(DisplayName = "Lancamento_Create_DeveCriarLancamentoValido")]
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldCreateValidLancamento()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Compra no mercado";
            string tipo = TipoLancamento.Debito.Valor;
            string categoria = CategoriaLancamento.Alimentacao.Valor;
            string observacoes = "Compras do mês";
            string correlationId = Guid.NewGuid().ToString();

            // Act
            var lancamento = Lancamento.Create(valor, data, descricao, tipo, categoria, observacoes, correlationId);

            // Assert
            lancamento.Should().NotBeNull();
            lancamento.Valor.Should().Be(valor);
            lancamento.Data.Should().Be(data);
            lancamento.Descricao.Should().Be(descricao);
            lancamento.Tipo.Valor.Should().Be(tipo);
            lancamento.Categoria.Valor.Should().Be(categoria);
            lancamento.Observacoes.Should().Be(observacoes);
            lancamento.IsDeleted.Should().BeFalse();
            lancamento.DomainEvents.Should().ContainSingle(e => e is LancamentoRegistradoEvent);
            var domainEvent = lancamento.DomainEvents.OfType<LancamentoRegistradoEvent>().Single();
            domainEvent.LancamentoId.Should().Be(lancamento.Id);
            domainEvent.CorrelationId.Should().Be(correlationId);
        }

        [Fact(DisplayName = "Lancamento_Create_DeveCriarLancamentoComObservacoesNulas")]
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldCreateValidLancamento_WhenObservacoesAreNull()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Compra no mercado";
            string tipo = TipoLancamento.Debito.Valor;
            string categoria = CategoriaLancamento.Alimentacao.Valor;
            string? observacoes = null;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            var lancamento = Lancamento.Create(valor, data, descricao, tipo, categoria, observacoes, correlationId);

            // Assert
            lancamento.Should().NotBeNull();
            lancamento.Observacoes.Should().BeNull();
            lancamento.DomainEvents.Should().ContainSingle(e => e is LancamentoRegistradoEvent);
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoValorForZero")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-006")]
        public void Create_ShouldThrowException_WhenValorIsZero()
        {
            // Arrange
            decimal invalidValor = 0m;
            DateTime data = DateTime.Today;
            string descricao = "Compra de teste";
            string tipo = TipoLancamento.Debito.Valor;
            string categoria = CategoriaLancamento.Alimentacao.Valor;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(invalidValor, data, descricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoValorForNegativo")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-006")]
        public void Create_ShouldThrowException_WhenValorIsNegative()
        {
            // Arrange
            decimal invalidValor = -10m;
            DateTime data = DateTime.Today;
            string descricao = "Compra de teste";
            string tipo = TipoLancamento.Debito.Valor;
            string categoria = CategoriaLancamento.Alimentacao.Valor;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(invalidValor, data, descricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoDataForFutura")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-002")]
        public void Create_ShouldThrowException_WhenDataIsFuture()
        {
            // Arrange
            decimal valor = 100m;
            DateTime futureData = DateTime.Today.AddDays(1);
            string descricao = "Teste de data futura";
            string tipo = TipoLancamento.Credito.Valor;
            string categoria = CategoriaLancamento.Salario.Valor;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(valor, futureData, descricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("A data do lançamento não pode ser futura. [RN-002]");
        }

        [Theory(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoDescricaoForInvalida")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Esta é uma descrição muito longa para um lançamento, excedendo o limite permitido pelo sistema e demonstrando a validação de domínio para comprimento de texto. Esta é uma descrição muito longa para um lançamento, excedendo o limite permitido pelo sistema e demonstrando a validação de domínio para comprimento de texto.")] // Mais de 250 caracteres
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldThrowException_WhenDescricaoIsInvalid(string invalidDescricao)
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string tipo = TipoLancamento.Credito.Valor;
            string categoria = CategoriaLancamento.Salario.Valor;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(valor, data, invalidDescricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>();
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoTipoForInvalido")]
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldThrowException_WhenTipoIsInvalid()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Descricao valida";
            string tipo = "TipoInvalido"; // Tipo que não existe
            string categoria = CategoriaLancamento.Salario.Valor;
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(valor, data, descricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>();
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoCategoriaForInvalida")]
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldThrowException_WhenCategoriaIsInvalid()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Descricao valida";
            string tipo = TipoLancamento.Credito.Valor;
            string categoria = "CategoriaInvalida"; // Categoria que não existe
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(valor, data, descricao, tipo, categoria, null, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>();
        }

        [Fact(DisplayName = "Lancamento_Create_DeveLancarExcecao_QuandoObservacoesForemMuitoLongas")]
        [Trait("Category", "DomainUnitTests")]
        public void Create_ShouldThrowException_WhenObservacoesAreTooLong()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Descricao valida";
            string tipo = TipoLancamento.Credito.Valor;
            string categoria = CategoriaLancamento.Salario.Valor;
            string longObservacoes = new string('A', 501); // Excedendo o limite de 500 caracteres
            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => Lancamento.Create(valor, data, descricao, tipo, categoria, longObservacoes, correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>();
        }

        #endregion

        #region Testes de Atualização (Update)

        [Fact(DisplayName = "Lancamento_Update_DeveAtualizarPropriedadesEAdicionarEventoDeDominioAtualizado")]
        [Trait("Category", "DomainUnitTests")]
        public void Update_ShouldUpdatePropertiesAndAddLancamentoAtualizadoDomainEvent()
        {
            // Arrange
            var lancamentoOriginal = Lancamento.Create(
                100m,
                DateTime.Today,
                "Descricao Original",
                TipoLancamento.Credito.Valor,
                CategoriaLancamento.Salario.Valor,
                "Observacoes originais",
                Guid.NewGuid().ToString()
            );
            lancamentoOriginal.ClearDomainEvents(); // Limpa eventos de criação

            decimal novoValor = 150m;
            DateTime novaData = DateTime.Today.AddDays(-1);
            string novaDescricao = "Nova Descricao";
            string novaCategoria = CategoriaLancamento.Investimentos.Valor;
            string novasObservacoes = "Novas Observacoes";
            string novoCorrelationId = Guid.NewGuid().ToString();

            // Act
            lancamentoOriginal.Update(novoValor, novaData, novaDescricao, TipoLancamento.Credito.Valor, novaCategoria, novasObservacoes, novoCorrelationId);

            // Assert
            lancamentoOriginal.Valor.Should().Be(novoValor);
            lancamentoOriginal.Data.Should().Be(novaData);
            lancamentoOriginal.Descricao.Should().Be(novaDescricao);
            lancamentoOriginal.Categoria.Valor.Should().Be(novaCategoria);
            lancamentoOriginal.Observacoes.Should().Be(novasObservacoes);

            lancamentoOriginal.DomainEvents.Should().ContainSingle(e => e is LancamentoAtualizadoEvent);
            var domainEvent = lancamentoOriginal.DomainEvents.OfType<LancamentoAtualizadoEvent>().Single();
            domainEvent.Should().NotBeNull();
            domainEvent.LancamentoId.Should().Be(lancamentoOriginal.Id);
            domainEvent.CorrelationId.Should().Be(novoCorrelationId);
            domainEvent.LancamentoAntigo.Should().NotBeNull();
            domainEvent.LancamentoAntigo.Valor.Should().Be(100m);
            domainEvent.LancamentoAntigo.Data.Should().Be(DateTime.Today);
            domainEvent.LancamentoAntigo.Descricao.Should().Be("Descricao Original");
            domainEvent.LancamentoAntigo.Tipo.Should().Be(TipoLancamento.Credito.Valor);
            domainEvent.LancamentoAntigo.Categoria.Should().Be(CategoriaLancamento.Salario.Valor);
            domainEvent.LancamentoAntigo.Observacoes.Should().Be("Observacoes originais");

            domainEvent.LancamentoNovo.Should().NotBeNull();
            domainEvent.LancamentoNovo.Valor.Should().Be(novoValor);
            domainEvent.LancamentoNovo.Data.Should().Be(novaData);
            domainEvent.LancamentoNovo.Descricao.Should().Be(novaDescricao);
            domainEvent.LancamentoNovo.Tipo.Should().Be(TipoLancamento.Credito.Valor);
            domainEvent.LancamentoNovo.Categoria.Should().Be(novaCategoria);
            domainEvent.LancamentoNovo.Observacoes.Should().Be(novasObservacoes);
        }

        [Fact(DisplayName = "Lancamento_Update_NaoDeveAdicionarEvento_QuandoNenhumaPropriedadeForAlterada")]
        [Trait("Category", "DomainUnitTests")]
        public void Update_ShouldNotAddEvent_WhenNoPropertiesAreChanged()
        {
            // Arrange
            decimal valor = 100m;
            DateTime data = DateTime.Today;
            string descricao = "Descricao Original";
            string tipo = TipoLancamento.Credito.Valor;
            string categoria = CategoriaLancamento.Salario.Valor;
            string observacoes = "Observacoes originais";
            string correlationId = Guid.NewGuid().ToString();

            var lancamentoOriginal = Lancamento.Create(valor, data, descricao, tipo, categoria, observacoes, correlationId);
            lancamentoOriginal.ClearDomainEvents();

            // Act
            lancamentoOriginal.Update(valor, data, descricao, tipo, categoria, observacoes, correlationId);

            // Assert
            lancamentoOriginal.DomainEvents.Should().BeEmpty();
        }

        [Fact(DisplayName = "Lancamento_Update_DeveLancarExcecao_QuandoTipoForAlterado")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-005")]
        public void Update_ShouldThrowException_WhenTipoIsChanged()
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today,
                "Original",
                TipoLancamento.Credito.Valor,
                CategoriaLancamento.Salario.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.ClearDomainEvents();

            // Act
            Action act = () => lancamento.Update(
                150m,
                DateTime.Today,
                "Atualizado",
                TipoLancamento.Debito.Valor, // Tentando mudar o tipo
                CategoriaLancamento.Investimentos.Valor,
                "Obs",
                Guid.NewGuid().ToString()
            );

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("O tipo de um lançamento não pode ser alterado após o registro. [RN-005]");
        }

        [Theory(DisplayName = "Lancamento_Update_DeveLancarExcecao_QuandoValorAtualizadoForInvalido")]
        [InlineData(0)]
        [InlineData(-50)]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-006")]
        public void Update_ShouldThrowException_WhenUpdatedValorIsInvalid(decimal invalidValor)
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today,
                "Descricao",
                TipoLancamento.Debito.Valor,
                CategoriaLancamento.Alimentacao.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.ClearDomainEvents();

            // Act
            Action act = () => lancamento.Update(
                invalidValor,
                DateTime.Today,
                lancamento.Descricao,
                lancamento.Tipo.Valor,
                lancamento.Categoria.Valor,
                lancamento.Observacoes,
                Guid.NewGuid().ToString()
            );

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "Lancamento_Update_DeveLancarExcecao_QuandoDataAtualizadaForFutura")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-002")]
        public void Update_ShouldThrowException_WhenUpdatedDataIsFuture()
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today,
                "Descricao",
                TipoLancamento.Debito.Valor,
                CategoriaLancamento.Alimentacao.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.ClearDomainEvents();

            // Act
            Action act = () => lancamento.Update(
                lancamento.Valor,
                DateTime.Today.AddDays(1), // Data futura
                lancamento.Descricao,
                lancamento.Tipo.Valor,
                lancamento.Categoria.Valor,
                lancamento.Observacoes,
                Guid.NewGuid().ToString()
            );

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("A data do lançamento não pode ser futura. [RN-002]");
        }

        #endregion

        #region Testes de Exclusão (Delete)

        [Fact(DisplayName = "Lancamento_Delete_DeveMarcarComoExcluidoEAdicionarEventoDeDominioExcluido")]
        [Trait("Category", "DomainUnitTests")]
        public void Delete_ShouldMarkAsDeletedAndAddLancamentoExcluidoDomainEvent()
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today,
                "Descricao para exclusão",
                TipoLancamento.Debito.Valor,
                CategoriaLancamento.Alimentacao.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.ClearDomainEvents();

            string correlationId = Guid.NewGuid().ToString();

            // Act
            lancamento.Delete(correlationId);

            // Assert
            lancamento.IsDeleted.Should().BeTrue();
            lancamento.DomainEvents.Should().ContainSingle(e => e is LancamentoExcluidoEvent);
            var domainEvent = lancamento.DomainEvents.OfType<LancamentoExcluidoEvent>().Single();
            domainEvent.Should().NotBeNull();
            domainEvent.LancamentoId.Should().Be(lancamento.Id);
            domainEvent.CorrelationId.Should().Be(correlationId);
        }

        [Fact(DisplayName = "Lancamento_Delete_DeveLancarExcecao_QuandoDataForAnteriorAoDiaAtual")]
        [Trait("Category", "DomainUnitTests")]
        [Trait("Requirement", "RN-001")]
        public void Delete_ShouldThrowException_WhenDataIsBeforeToday()
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today.AddDays(-1), // Lançamento de dia anterior
                "Descricao antiga",
                TipoLancamento.Debito.Valor,
                CategoriaLancamento.Alimentacao.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.ClearDomainEvents();

            string correlationId = Guid.NewGuid().ToString();

            // Act
            Action act = () => lancamento.Delete(correlationId);

            // Assert
            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("Lançamento só pode ser excluído se a data for igual ao dia atual.");
        }

        #endregion

        #region Testes de Eventos de Domínio

        [Fact(DisplayName = "Lancamento_ClearDomainEvents_DeveLimparEventos")]
        [Trait("Category", "DomainUnitTests")]
        public void ClearDomainEvents_ShouldClearEvents()
        {
            // Arrange
            var lancamento = Lancamento.Create(
                100m,
                DateTime.Today,
                "Descricao",
                TipoLancamento.Debito.Valor,
                CategoriaLancamento.Alimentacao.Valor,
                null,
                Guid.NewGuid().ToString()
            );
            lancamento.DomainEvents.Should().NotBeEmpty(); // Deve ter o evento de criação

            // Act
            lancamento.ClearDomainEvents();

            // Assert
            lancamento.DomainEvents.Should().BeEmpty();
        }

        #endregion
    }
}