@echo off
:: 检查是否以管理员身份运行
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo 正在尝试以管理员权限重新启动...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

REM 删除所有 bin 文件夹
for /d /r %%i in (bin) do (
    if exist "%%i" (
        echo 正在删除文件夹：%%i
        rd /s /q "%%i"
    )
)
REM 删除所有 obj 文件夹
for /d /r %%i in (obj) do (
    if exist "%%i" (
        echo 正在删除文件夹：%%i
        rd /s /q "%%i"
    )
)
echo 删除完成
pause