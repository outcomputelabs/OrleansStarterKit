@"%HOMEDRIVE%%HOMEPATH%\.nuget\packages\opencover\4.7.922\tools\OpenCover.Console.exe"^
 -target:"%ProgramFiles%\dotnet\dotnet.exe"^
 -targetargs:"test"^
 -output:coverage.xml^
 -oldStyle^
 -register:user^
 -filter:"+[Grains*]* -[*.Tests]*"^
 -excludebyattribute:"*.GeneratedCode*"

@reportgenerator -reports:coverage.xml -targetdir:.coverage
