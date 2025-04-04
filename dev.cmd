CALL dotnet build || PAUSE

CD bin\Debug\net10.0-windows

CALL ScreenMonitor.exe capture || PAUSE

PAUSE

CD ..\..\..