dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Exclude="[xunit]*"
rem @reportgenerator -reports:*/*/coverage.opencover.xml -targetdir:.coverage