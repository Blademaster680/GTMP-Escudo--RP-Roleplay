G:
echo Y|del "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\*"
rmdir /s /q "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\clientside"
rmdir /s /q "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Data"
rmdir /s /q "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Database"
rmdir /s /q "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Global"


xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Account.cs" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP"
echo Y xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Commands.cs" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP" 
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Main.cs" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP" 
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\meta.xml" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP" 
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Player.cs" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP" 
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\VehicleController.cs" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP"
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\clientside" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\clientside\"
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Data" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Data\"
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Database" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Database\"
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\Global" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP\Global\"

xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\bin\Debug\EGRPProject.dll" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP"
xcopy /s "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRPProject\EGRPProject\bin\Debug\MySql.Data.dll" "G:\Users\Dylan\Dropbox\Shared\Escudo\GTA 5 Server\resources\EGRP"

winscp.com /script=SyncEGRPScript.txt
pause

