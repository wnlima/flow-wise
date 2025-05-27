using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FluentAssertions;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Contém testes unitários para o Value Object TipoLancamento.
    /// </summary>
    public class TipoLancamentoTests
    {
        [Theory(DisplayName = "TipoLancamento_From_DeveRetornarInstanciaValida")]
        [InlineData("credito")]
        [InlineData("DEBITO")]
        [InlineData("debito")]
        [InlineData("Debito")]
        [Trait("Category", "DomainUnitTests")]
        public void From_ShouldReturnValidInstance(string tipoString)
        {
            var tipo = TipoLancamento.From(tipoString);
            if (tipoString.ToLower() == "credito")
            {
                tipo.Should().Be(TipoLancamento.Credito);
            }
            else
            {
                tipo.Should().Be(TipoLancamento.Debito);
            }
        }

        [Theory(DisplayName = "TipoLancamento_From_DeveLancarExcecao_QuandoStringForNulaOuVaziaOuInvalida")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Invalido")]
        [InlineData("Transferencia")]
        [Trait("Category", "DomainUnitTests")]
        public void From_ShouldThrowException_WhenStringIsInvalid(string invalidTipoString)
        {
            Action act = () => TipoLancamento.From(invalidTipoString);

            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("O tipo de lançamento deve ser 'Credito' ou 'Debito'.");
        }

        [Fact(DisplayName = "TipoLancamento_IsValid_DeveRetornarTrue_ParaTiposValidos")]
        [Trait("Category", "DomainUnitTests")]
        public void IsValid_ShouldReturnTrue_ForValidTypes()
        {
            TipoLancamento.IsValid("Credito").Should().BeTrue();
            TipoLancamento.IsValid("debito").Should().BeTrue();
            TipoLancamento.IsValid("Debito").Should().BeTrue();
            TipoLancamento.IsValid("credito").Should().BeTrue();
        }

        [Fact(DisplayName = "TipoLancamento_IsValid_DeveRetornarFalse_ParaTiposInvalidos")]
        [Trait("Category", "DomainUnitTests")]
        public void IsValid_ShouldReturnFalse_ForInvalidTypes()
        {
            TipoLancamento.IsValid(null).Should().BeFalse();
            TipoLancamento.IsValid("").Should().BeFalse();
            TipoLancamento.IsValid("Invalido").Should().BeFalse();
        }

        [Fact(DisplayName = "TipoLancamento_Equality_DeveSerBaseadoEmValor")]
        [Trait("Category", "DomainUnitTests")]
        public void Equality_ShouldBeValueBased()
        {
            var credito1 = TipoLancamento.From("Credito");
            var credito2 = TipoLancamento.From("credito");
            var debito = TipoLancamento.From("Debito");

            credito1.Should().Be(credito2);
            (credito1 == credito2).Should().BeTrue();
            (credito1 != debito).Should().BeTrue();
            credito1.Equals(credito2).Should().BeTrue();
            credito1.GetHashCode().Should().Be(credito2.GetHashCode());

            credito1.Should().NotBe(debito);
            (credito1 == debito).Should().BeFalse();
        }

        [Fact(DisplayName = "TipoLancamento_ImplicitConversion_DeveFuncionar")]
        [Trait("Category", "DomainUnitTests")]
        public void ImplicitConversion_ShouldWork()
        {
            string tipoStringCredito = TipoLancamento.Credito.ToString();
            string tipoStringDebito = TipoLancamento.Debito.ToString();

            tipoStringCredito.Should().Be("Credito");
            tipoStringDebito.Should().Be("Debito");
        }
    }
}