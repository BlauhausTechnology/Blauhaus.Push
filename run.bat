dotnet publish ./src/Blauhaus.Push.Abstractions/Blauhaus.Push.Abstractions.csproj -f netstandard2.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client/Blauhaus.Push.Client.csproj -f netstandard2.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client/Blauhaus.Push.Client.csproj -f Xamarin.iOS10 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client/Blauhaus.Push.Client.csproj -f MonoAndroid10.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client/Blauhaus.Push.Client.csproj -f uap10.0.19041 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client.Maui/Blauhaus.Push.Client.Maui.csproj -f net7.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client.Maui/Blauhaus.Push.Client.Maui.csproj -f net7.0-ios -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client.Maui/Blauhaus.Push.Client.Maui.csproj -f net7.0-android -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Server/Blauhaus.Push.Server.csproj -f net7.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.TestHelpers/Blauhaus.Push.TestHelpers.csproj -f net7.0 -c Release -o $(build.sourcesdirectory)
dotnet publish ./src/Blauhaus.Push.Client.Maui/Blauhaus.Push.Client.Maui.csproj -f net7.0-windows10.0.19041 -c Release -p:PublishReadyToRun=false -o $(build.sourcesdirectory)
 