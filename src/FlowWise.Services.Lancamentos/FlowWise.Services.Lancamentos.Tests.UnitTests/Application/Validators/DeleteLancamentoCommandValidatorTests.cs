using FluentValidation.TestHelper;
using FlowWise.Services.Lancamentos.Application.Commands;
using FlowWise.Services.Lancamentos.Application.Validators;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Application.Validators
{
    public class DeleteLancamentoCommandValidatorTests
    {
        private readonly DeleteLancamentoCommandValidator _validator;

        public DeleteLancamentoCommandValidatorTests()
        {
            _validator = new DeleteLancamentoCommandValidator();
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        public void Should_HaveError_WhenIdIsEmpty()
        {
            // Arrange
            var command = new DeleteLancamentoCommand(Guid.NewGuid().ToString());
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage("O ID do lançamento é obrigatório para exclusão.");
        }

        [Fact]
        [Trait("Category", "ApplicationUnitTests")]
        public void Should_NotHaveError_WhenCommandIsValid()
        {
            // Arrange
            var command = new DeleteLancamentoCommand(Guid.NewGuid().ToString()) { Id = Guid.NewGuid() };
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}