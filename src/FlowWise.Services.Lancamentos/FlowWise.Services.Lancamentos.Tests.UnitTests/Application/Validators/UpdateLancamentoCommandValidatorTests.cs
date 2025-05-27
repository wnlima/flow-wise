using FluentValidation.TestHelper;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.Validators;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.Validators
{
    public class UpdateLancamentoCommandValidatorTests
    {
        private readonly UpdateLancamentoCommandValidator _validator;

        public UpdateLancamentoCommandValidatorTests()
        {
            _validator = new UpdateLancamentoCommandValidator();
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Id_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.Empty, // ID inválido
                Valor = 100.00m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("O ID do lançamento é obrigatório.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Id_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Id_Is_Valid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(), // ID válido
                Valor = 100.00m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Valor_Is_Zero")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Valor_Is_Zero()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 0m, // Valor inválido: zero
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Valor)
                  .WithErrorMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Valor_Is_Negative")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Valor_Is_Negative()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = -10m, // Valor inválido: negativo
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Valor)
                  .WithErrorMessage("O valor do lançamento deve ser maior que zero. [RN-006]");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Valor_Is_Positive")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-006, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Valor_Is_Positive()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 100.50m, // Valor válido: positivo
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Valor);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Data_Is_Future")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-002, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Data_Is_Future()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today.AddDays(1), // Data inválida: futura
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Data)
                  .WithErrorMessage("A data do lançamento não pode ser futura. [RN-002]");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Data_Is_Today_Or_Past")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "RN-002, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Data_Is_Today_Or_Past()
        {
            // Arrange
            var commandToday = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today, // Data válida: hoje
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
            };
            var commandPast = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today.AddDays(-1), // Data válida: passada
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
            };

            // Act
            var resultToday = _validator.TestValidate(commandToday);
            var resultPast = _validator.TestValidate(commandPast);

            // Assert
            resultToday.ShouldNotHaveValidationErrorFor(x => x.Data);
            resultPast.ShouldNotHaveValidationErrorFor(x => x.Data);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Descricao_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Descricao_Is_Empty()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "", // Descrição inválida: vazia
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Descricao)
                  .WithErrorMessage("A descrição do lançamento é obrigatória.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Descricao_Is_Too_Long")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Descricao_Is_Too_Long()
        {
            // Arrange
            var longDescription = new string('A', 256); // Mais de 255 caracteres
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = longDescription, // Descrição inválida: muito longa
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Descricao)
                  .WithErrorMessage("A descrição do lançamento não pode exceder 255 caracteres.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Descricao_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Descricao_Is_Valid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição de teste válida", // Descrição válida
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Descricao);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Tipo_Is_Invalid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Tipo_Is_Invalid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = "TipoInvalido", // Tipo inválido
                Categoria = CategoriaLancamento.Lazer.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Tipo)
                  .WithErrorMessage("O tipo de lançamento deve ser 'Credito' ou 'Debito'.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Tipo_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Tipo_Is_Valid()
        {
            // Arrange
            var commandCredito = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor, // Tipo válido
                Categoria = CategoriaLancamento.Lazer.Valor,
            };
            var commandDebito = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor, // Tipo válido
                Categoria = CategoriaLancamento.Lazer.Valor,
            };

            // Act
            var resultCredito = _validator.TestValidate(commandCredito);
            var resultDebito = _validator.TestValidate(commandDebito);

            // Assert
            resultCredito.ShouldNotHaveValidationErrorFor(x => x.Tipo);
            resultDebito.ShouldNotHaveValidationErrorFor(x => x.Tipo);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Categoria_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Categoria_Is_Empty()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = "", // Categoria inválida: vazia
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Categoria)
                  .WithErrorMessage("A categoria do lançamento é obrigatória.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Categoria_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Categoria_Is_Valid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Transporte.Valor, // Categoria válida
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Categoria);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_Observacoes_Is_Too_Long")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Have_Error_When_Observacoes_Is_Too_Long()
        {
            // Arrange
            var longObservacoes = new string('B', 501); // Mais de 500 caracteres
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Educacao.Valor,
                Observacoes = longObservacoes, // Observações inválidas: muito longas
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Observacoes)
                  .WithErrorMessage("As observações não podem exceder 500 caracteres.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_Observacoes_Is_Valid_Or_Null")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_Observacoes_Is_Valid_Or_Null()
        {
            // Arrange
            var commandValid = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Educacao.Valor,
                Observacoes = "Observações válidas", // Observações válidas
            };
            var commandNull = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 50m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Educacao.Valor,
                Observacoes = null, // Observações nulas (válidas)
            };

            // Act
            var resultValid = _validator.TestValidate(commandValid);
            var resultNull = _validator.TestValidate(commandNull);

            // Assert
            resultValid.ShouldNotHaveValidationErrorFor(x => x.Observacoes);
            resultNull.ShouldNotHaveValidationErrorFor(x => x.Observacoes);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Have_Error_When_CorrelationId_Is_Empty")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "NFR-OBS-001, TDD")]
        public void UpdateCommand_Should_Have_Error_When_CorrelationId_Is_Empty()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(string.Empty)
            {
                Id = Guid.NewGuid(),
                Valor = 100m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CorrelationId)
                  .WithErrorMessage("O Correlation ID é obrigatório para rastreabilidade.");
        }

        [Fact(DisplayName = "UpdateCommand_Should_Not_Have_Error_When_CorrelationId_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "NFR-OBS-001, TDD")]
        public void UpdateCommand_Should_Not_Have_Error_When_CorrelationId_Is_Valid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 100m,
                Data = DateTime.Today,
                Descricao = "Descrição válida",
                Tipo = TipoLancamento.Credito.Valor,
                Categoria = CategoriaLancamento.Salario.Valor,
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CorrelationId);
        }

        [Fact(DisplayName = "UpdateCommand_Should_Pass_All_Validations_When_Command_Is_Valid")]
        [Trait("Category", "ValidationUnitTests")]
        [Trait("Requirement", "HU-004, RN-002, RN-006, NFR-OBS-001, TDD")]
        public void UpdateCommand_Should_Pass_All_Validations_When_Command_Is_Valid()
        {
            // Arrange
            var command = new UpdateLancamentoCommand(Guid.NewGuid().ToString())
            {
                Id = Guid.NewGuid(),
                Valor = 250.75m,
                Data = DateTime.Today.AddDays(-10), // Data válida (passada)
                Descricao = "Pagamento de conta de luz",
                Tipo = TipoLancamento.Debito.Valor,
                Categoria = CategoriaLancamento.Alimentacao.Valor,
                Observacoes = "Referente ao mês passado."
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}