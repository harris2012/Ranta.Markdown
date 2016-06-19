msbuild Ranta.Markdown.Net40\Ranta.Markdown.Net40.csproj /t:rebuild /p:configuration=release;DocumentationFile=bin\Release\Ranta.Markdown.xml;DebugType=none

msbuild Ranta.Markdown.Net45\Ranta.Markdown.Net45.csproj /t:rebuild /p:configuration=release;DocumentationFile=bin\Release\Ranta.Markdown.xml;DebugType=none

nuget pack Ranta.Markdown.nuspec

pause