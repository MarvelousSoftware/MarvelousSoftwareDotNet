cd MarvelousSoftware.Core
nuget pack MarvelousSoftware.Core.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..

cd MarvelousSoftware.Core.Host.Owin
nuget pack MarvelousSoftware.Core.Host.Owin.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..

cd MarvelousSoftware.Core.Host.SystemWeb
nuget pack MarvelousSoftware.Core.Host.SystemWeb.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..

cd MarvelousSoftware.Grid.DataSource
nuget pack MarvelousSoftware.Grid.DataSource.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..

cd MarvelousSoftware.Grid.QueryLanguage
nuget pack MarvelousSoftware.Grid.QueryLanguage.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..

cd MarvelousSoftware.QueryLanguage
nuget pack MarvelousSoftware.QueryLanguage.csproj -IncludeReferencedProjects -Prop Configuration=Release -Build -OutputDirectory ../.releases
cd ..