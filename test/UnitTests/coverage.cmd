"%HOMEDRIVE%%HOMEPATH%\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe"^
 -target:"%ProgramFiles%\dotnet\dotnet.exe"^
 -targetargs:"test"^
 -output:coverage.xml^
 -oldStyle^
 -register:user^
 -filter:"+[Client]* +[Grains]* +[Grains.Abstractions]* +[Silo]* -[UnitTests]*"^
 -excludebyattribute:"*.GeneratedCode*"

dotnet "%HOMEDRIVE%%HOMEPATH%\.nuget\packages\reportgenerator\4.0.12\tools\netcoreapp2.0\ReportGenerator.dll" -reports:coverage.xml -targetdir:coverage