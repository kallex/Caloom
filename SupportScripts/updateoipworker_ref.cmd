rem Replace <accountname>, <accountkey>, <storagename>, <root>
%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
SET DestUri=https://<accountname>.blob.core.windows.net/
<root>\Tools\Ext\CloudCopy.exe "%~dp0<root>\bin\CaloomWorkerRole\*.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"
<root>\Tools\Ext\CloudCopy.exe "%~dp0<root>\bin\CaloomWorkerRole\CaloomWorkerRole.dll" "%DestUri%worker-role-accelerator" "%DestStorage%"

