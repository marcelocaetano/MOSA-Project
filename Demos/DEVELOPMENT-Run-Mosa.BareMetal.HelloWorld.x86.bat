cd %~dp0
cd ..\bin
start Mosa.Tool.Launcher.exe --q --autostart --qemu --output-map --output-asm --output-debug Mosa.BareMetal.HelloWorld.x86.exe
