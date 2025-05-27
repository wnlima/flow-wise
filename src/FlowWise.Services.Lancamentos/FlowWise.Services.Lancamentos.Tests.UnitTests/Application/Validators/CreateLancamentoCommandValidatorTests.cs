using FluentValidation.TestHelper;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.Validators;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FluentAssertions;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.Validators
{
    public class CreateLancamentoCommandValidatorTests
    {
        private readonly CreateLancamentoCommandValidator _validator;

        public CreateLancamentoCommandValidatorTests()
        {
            _validator = new CreateLancamentoCommandValidator();
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Valor_Is_Zero")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void CreateCommand_Should_Have_Error_When_Valor_Is_Zero()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 0m, // Valor inválido: zero
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Valor)
                  .WithErrorMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Valor_Is_Negative")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void CreateCommand_Should_Have_Error_When_Valor_Is_Negative()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = -10m, // Valor inválido: negativo
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Valor)
                  .WithErrorMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Valor_Is_Positive")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Valor_Is_Positive()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 100.50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Valor);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Data_Is_Future")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-002, TDD")]
        public void CreateCommand_Should_Have_Error_When_Data_Is_Future()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today.AddDays(1),
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data)
                  .WithErrorMessage("A data do lançamento não pode ser futura. [RN-002]");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Data_Is_Today_Or_Past")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-002, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Data_Is_Today_Or_Past()
        {
            // Arrange
            var commandToday = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today, // Data válida: hoje
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };
            var commandPast = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today.AddDays(-1), // Data válida: passada
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var resultToday = _validator.TestValidate(commandToday);
            var resultPast = _validator.TestValidate(commandPast);

            // Assert
            resultToday.ShouldNotHaveValidationErrorFor(x => x.Data);
            resultPast.ShouldNotHaveValidationErrorFor(x => x.Data);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Descricao_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Have_Error_When_Descricao_Is_Empty()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "", // Descrição inválida: vazia
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Descricao)
                  .WithErrorMessage("A descrição do lançamento é obrigatória.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Descricao_Is_Too_Long")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Have_Error_When_Descricao_Is_Too_Long()
        {
            // Arrange
            var longDescription = new string('A', 256); // Mais de 255 caracteres
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = longDescription, // Descrição inválida: muito longa
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Descricao)
                  .WithErrorMessage("A descrição do lançamento não pode exceder 255 caracteres.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Descricao_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Descricao_Is_Valid()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição de teste válida", // Descrição válida
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Descricao);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Tipo_Is_Invalid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-001, TDD")]
        public void CreateCommand_Should_Have_Error_When_Tipo_Is_Invalid()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = "TipoInvalido", // Tipo inválido
                Categoria = CategoriaLancamento.Lazer.Valor,
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tipo)
                  .WithErrorMessage("O tipo de lançamento deve ser 'Credito' ou 'Debito'.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Tipo_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-001, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Tipo_Is_Valid()
        {
            // Arrange
            var commandCredito = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor, // Tipo válido
                Categoria = CategoriaLancamento.Lazer.Valor,
                Observacoes = null,
            };
            var commandDebito = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor, // Tipo válido
                Categoria = CategoriaLancamento.Lazer.Valor,
                Observacoes = null,
            };

            // Act
            var resultCredito = _validator.TestValidate(commandCredito);
            var resultDebito = _validator.TestValidate(commandDebito);

            // Assert
            resultCredito.ShouldNotHaveValidationErrorFor(x => x.Tipo);
            resultDebito.ShouldNotHaveValidationErrorFor(x => x.Tipo);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Categoria_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Have_Error_When_Categoria_Is_Empty()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = "", // Categoria inválida: vazia
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Categoria)
                  .WithErrorMessage("A categoria do lançamento é obrigatória.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Categoria_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Categoria_Is_Valid()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Transporte.Valor, // Categoria válida
                Observacoes = null,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Categoria);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_Observacoes_Is_Too_Long")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Have_Error_When_Observacoes_Is_Too_Long()
        {
            // Arrange
            var longObservacoes = new string('B', 501); // Mais de 500 caracteres
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = longObservacoes, // Observações inválidas: muito longas
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Observacoes)
                  .WithErrorMessage("As observações não podem exceder 500 caracteres.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_Observacoes_Is_Valid_Or_Null")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-002, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_Observacoes_Is_Valid_Or_Null()
        {
            // Arrange
            var commandValid = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Compras.Valor,
                Observacoes = "Observações válidas", // Observações válidas
            };
            var commandNull = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Compras.Valor,
                Observacoes = null, // Observações nulas (válidas)
            };

            // Act
            var resultValid = _validator.TestValidate(commandValid);
            var resultNull = _validator.TestValidate(commandNull);

            // Assert
            resultValid.ShouldNotHaveValidationErrorFor(x => x.Observacoes);
            resultNull.ShouldNotHaveValidationErrorFor(x => x.Observacoes);
        }

        [Fact(DisplayName = "CreateCommand_Should_Have_Error_When_CorrelationId_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "NFR-OBS-001, TDD")]
        public void CreateCommand_Should_Have_Error_When_CorrelationId_Is_Empty()
        {
            // Arrange
            var command = new CreateLancamentoCommand(string.Empty)
            {
                Valor = 100m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
                Observacoes = null
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CorrelationId)
                  .WithErrorMessage("O Correlation ID é obrigatório para rastreabilidade.");
        }

        [Fact(DisplayName = "CreateCommand_Should_Not_Have_Error_When_CorrelationId_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "NFR-OBS-001, TDD")]
        public void CreateCommand_Should_Not_Have_Error_When_CorrelationId_Is_Valid()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 100m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
                Observacoes = null,
                CorrelationId = Guid.NewGuid().ToString() // CorrelationId válido
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CorrelationId);
        }

        [Fact(DisplayName = "CreateCommand_Should_Pass_All_Validations_When_Command_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-001, HU-002, RN-002, RN-006, NFR-OBS-001, TDD")]
        public void CreateCommand_Should_Pass_All_Validations_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 250.75m,
                Data = DateTime.Today.AddDays(-10), // Data válida (passada)
                Descricao = "Pagamento de conta de água",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = "Referente ao consumo do mês atual.",
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, RN-001")]
        public void Validate_ValorZeroOrNegative_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 0m, // ou -10m
                Data = DateTime.Today,
                Descricao = "Teste",
                Tipo = "Credito",
                Categoria = "Teste",
            };
            var validator = new CreateLancamentoCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Valor" && e.ErrorMessage.Contains("maior que zero"));
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        [Trait("Requirement", "TDD, RN-002")]
        public void Validate_FutureDate_ShouldHaveValidationError()
        {
            // Arrange
            var command = new CreateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Valor = 100m,
                Data = DateTime.Today.AddDays(1), // Data futura
                Descricao = "Teste",
                Tipo = "Credito",
                Categoria = "Teste",
            };
            var validator = new CreateLancamentoCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Data" && e.ErrorMessage.Contains("não pode ser futura"));
        }
    }
}