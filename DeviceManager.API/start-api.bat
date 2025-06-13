@echo off
setlocal

REM === CONFIGURAÇOES ===
set PROJECT_PATH="D:\PESSOAL\LUCAS\VSoft\Projetos\DeviceManager.API\DeviceManager.API.csproj"
set PORT=5000

REM === LIBERAR FIREWALL ===
echo Verificando/liberando porta %PORT% no firewall...
netsh advfirewall firewall add rule name="DeviceManager API" dir=in action=allow protocol=TCP localport=%PORT%

REM === INICIAR API ===
echo Iniciando API em http://0.0.0.0:%PORT% ...
start cmd /k "dotnet run --project %PROJECT_PATH% --urls=http://0.0.0.0:%PORT%"

REM === ABRIR NO NAVEGADOR ===
timeout /t 3 >nul
start http://localhost:%PORT%/api/dispositivos

echo API em execução. Pressione qualquer tecla para sair...
pause >nul
endlocal
