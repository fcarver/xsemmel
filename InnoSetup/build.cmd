del /Q output\*.exe
cd..
del /Q Xsemmel\bin\Release\*
msbuild Xsemmel.sln /p:Configuration=Release
cd InnoSetup
"C:\Program Files (x86)\Inno Setup 5\ISCC.exe" setup.iss