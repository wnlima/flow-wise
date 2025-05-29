#!/bin/bash
CRITICAL_MODULES=("FlowWise.Services.Lancamentos.Domain" "FlowWise.Services.Lancamentos.Application" "FlowWise.Services.Consolidacao.Domain" "FlowWise.Services.Consolidacao.Application")

echo "Install tools if not present"
find . -type d -name "bin" -prune -o -name "*.xml" -exec rm -f {} +
find . -type d -name "bin" -prune -o -name "*.trx" -exec rm -f {} +

dotnet tool install --global coverlet.console
dotnet tool install --global dotnet-reportgenerator-globaltool

echo "Clean and build solution"
dotnet clean
dotnet build --configuration Release

echo "Run tests with coverage"
dotnet test ./src --no-restore --verbosity normal \
/p:CollectCoverage=true \
/p:CoverletOutputFormat=cobertura \
/p:CoverletOutput=./CoverageResults/coverage.xml

echo "Generate coverage report"
reportgenerator \
-reports:"./src/**/CoverageResults/coverage.xml" \
-targetdir:"./CoverageReport" \
-reporttypes:Html

echo "Removing temporary files"
rm -rf bin obj

echo ""
echo "Coverage report generated at CoverageReport/index.html"

echo "Checking minimum coverage for critical DLLs"

get_coverage() {
    local module="$1"
    local xml_file="$2"

    coverage=$(xmllint --xpath "string(//coverage/packages/package[contains(@name,'$module')]/@line-rate)" "$xml_file" 2>/dev/null)
    if [ -z "$coverage" ]; then
        echo "Erro: Não foi possível encontrar cobertura para $module"
        return 1
    fi

    percent=$(awk "BEGIN { printf \"%.0f\", $coverage * 100 }")
    echo "$percent"
    return 0
}

COVERAGE_XML=$(find ./src -name "coverage.xml" | head -n 1)

FAILED=0

get_coverage_from_all_reports() {
    local module="$1"
    local total_coverage=0
    local count=0

    while IFS= read -r xml_file; do
        coverage=$(xmllint --xpath "string(//coverage/packages/package[contains(@name,'$module')]/@line-rate)" "$xml_file" 2>/dev/null)
        if [ -n "$coverage" ]; then
            percent=$(awk "BEGIN { printf \"%.0f\", $coverage * 100 }")
            total_coverage=$((total_coverage + percent))
            count=$((count + 1))
        fi
    done < <(find ./src -name "coverage.xml")

    if [ "$count" -eq 0 ]; then
        echo "0"
    else
        echo $((total_coverage / count))
    fi
}

for module in "${CRITICAL_MODULES[@]}"; do
    echo "Verificando cobertura para $module"
    percent=$(get_coverage_from_all_reports "$module")

    echo "$module coverage: $percent%"
    if [ "$percent" -lt 25 ]; then
        echo "❌ Cobertura insuficiente para $module: ${percent}% (mínimo: 25%)"
        FAILED=1
    else
        echo "✅ Cobertura adequada para $module"
    fi
done

if [ $FAILED -ne 0 ]; then
    echo "❌ Failing pipeline: cobertura mínima não atingida."
    exit 1
else
    echo "✅ Todas as coberturas mínimas atingidas."
fi
