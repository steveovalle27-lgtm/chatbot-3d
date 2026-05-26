@echo off
title Chatbot Avatar 3D - Backend + Frontend
echo.
echo ============================================
echo   CHATBOT AVATAR 3D - Iniciando servidores
echo ============================================
echo.
echo   Limpiando puertos 3000 y 5173...
powershell -Command "Get-NetTCPConnection -LocalPort 3000,5173 -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue }"
timeout /t 2 /nobreak >nul
echo   Puertos libres.
echo.
echo   Backend:  http://localhost:3000
echo   Frontend: http://localhost:5173
echo.
echo   Ctrl+C para detener ambos servidores
echo ============================================
echo.
cd /d "%~dp0"
npm run dev
pause
