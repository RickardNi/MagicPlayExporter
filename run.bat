@echo off
netstat -ano | findstr :5028 > nul
if %errorlevel% equ 0 (
    echo Port 5028 in use, killing process...
    for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5028') do taskkill /F /PID %%a 2>nul
)

dotnet watch run --non-interactive
