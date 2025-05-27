using FlowWise.Services.Lancamentos.Domain.Exceptions;
using FlowWise.Services.Lancamentos.Domain.ValueObjects;
using FluentAssertions;

namespace FlowWise.Services.Lancamentos.Tests.UnitTests.Domain.ValueObjects
{
    /// <summary>
    /// Contém testes unitários para o Value Object CategoriaLancamento.
    /// </summary>
    public class CategoriaLancamentoTests
    {
        [Fact(DisplayName = "CategoriaLancamento_From_DeveRetornarInstanciaValida_ParaCategoriasConhecidas")]
        [Trait("Category", "DomainUnitTests")]
        public void From_ShouldReturnValidInstance_ForKnownCategories()
        {
            foreach (var categoriaString in CategoriaLancamento.CategoriasValidas.Select(c => c.Valor))
            {
                var categoria = CategoriaLancamento.From(categoriaString);
                categoria.Should().NotBeNull();
                categoria.Valor.Should().Be(categoriaString);
                CategoriaLancamento.CategoriasValidas.Should().Contain(c => c.Valor == categoriaString);
            }
        }

        [Theory(DisplayName = "CategoriaLancamento_From_DeveSerCaseInsensitive")]
        [InlineData("salario")]
        [InlineData("ALIMENTACAO")]
        [Trait("Category", "DomainUnitTests")]
        public void From_ShouldBeCaseInsensitive(string categoriaString)
        {
            var categoria = CategoriaLancamento.From(categoriaString);
            categoria.Should().NotBeNull();
            categoria.Valor.Should().Be(categoriaString.Substring(0, 1).ToUpper() + categoriaString.Substring(1).ToLower()); // Assumindo capitalização para o valor final
        }

        [Theory(DisplayName = "CategoriaLancamento_From_DeveLancarExcecao_QuandoStringForInvalida")]
        [InlineData("CategoriaInvalida")]
        [InlineData("Outros Gastos")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [Trait("Category", "DomainUnitTests")]
        public void From_ShouldThrowException_WhenStringIsInvalid(string invalidCategoriaString)
        {
            Action act = () => CategoriaLancamento.From(invalidCategoriaString);

            act.Should().Throw<LancamentoDomainException>()
               .WithMessage("Categoria de lançamento inválida.");
        }

        [Fact(DisplayName = "CategoriaLancamento_IsValid_DeveRetornarTrue_ParaCategoriasValidas")]
        [Trait("Category", "DomainUnitTests")]
        public void IsValid_ShouldReturnTrue_ForValidCategories()
        {
            CategoriaLancamento.IsValid("Salario").Should().BeTrue();
            CategoriaLancamento.IsValid("Alimentacao").Should().BeTrue();
            CategoriaLancamento.IsValid("saude").Should().BeTrue(); // Case-insensitive
        }

        [Fact(DisplayName = "CategoriaLancamento_IsValid_DeveRetornarFalse_ParaCategoriasInvalidas")]
        [Trait("Category", "DomainUnitTests")]
        public void IsValid_ShouldReturnFalse_ForInvalidCategories()
        {
            CategoriaLancamento.IsValid(null).Should().BeFalse();
            CategoriaLancamento.IsValid("").Should().BeFalse();
            CategoriaLancamento.IsValid("Nova Categoria").Should().BeFalse();
        }

        [Fact(DisplayName = "CategoriaLancamento_Equality_DeveSerBaseadoEmValor")]
        [Trait("Category", "DomainUnitTests")]
        public void Equality_ShouldBeValueBased()
        {
            var alimentacao1 = CategoriaLancamento.From("Alimentacao");
            var alimentacao2 = CategoriaLancamento.From("alimentacao");
            var transporte = CategoriaLancamento.From("Transporte");

            alimentacao1.Should().Be(alimentacao2);
            (alimentacao1 == alimentacao2).Should().BeTrue();
            (alimentacao1 != transporte).Should().BeTrue();
            alimentacao1.Equals(alimentacao2).Should().BeTrue();
            alimentacao1.GetHashCode().Should().Be(alimentacao2.GetHashCode());

            alimentacao1.Should().NotBe(transporte);
            (alimentacao1 == transporte).Should().BeFalse();
        }

        [Fact(DisplayName = "CategoriaLancamento_ImplicitConversion_DeveFuncionar")]
        [Trait("Category", "DomainUnitTests")]
        public void ImplicitConversion_ShouldWork()
        {
            string categoriaStringSalario = CategoriaLancamento.Salario.ToString();
            string categoriaStringLazer = CategoriaLancamento.Lazer.ToString();

            categoriaStringSalario.Should().Be("Salario");
            categoriaStringLazer.Should().Be("Lazer");
        }
    }
}