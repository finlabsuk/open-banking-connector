param(
    [string]$config = "Debug"
)

$ErrorActionPreference = "Stop"

$solution = "FinnovationLabs.OpenBanking.Library.Connector.sln"
$mainproj = "src\OpenBanking.Library.Connector\OpenBanking.Library.Connector.csproj"
$testprojs = "test\unit-tests\OpenBanking.Library.Connector.UnitTests\OpenBanking.Library.Connector.UnitTests.csproj"
$inttestprojs = "test\integration-tests\OpenBanking.Library.Connector.IntegrationTests\OpenBanking.Library.Connector.IntegrationTests.csproj"

dotnet clean $solution -c $config

dotnet restore $solution

dotnet build $solution -c $config

dotnet test 

dotnet publish $mainproj --no-build --no-restore -c $config

dotnet publish $inttestprojs --no-build --no-restore -c $config
