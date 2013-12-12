rem Replace <accountname>, <accountkey>, <storagename>, <root>, <containername>
rem <root> = TheBallPlatform
rem <containername> = website-container, such as weball-cloudapp-net
%~d0
CD "%~dp0"

SET DestStorage=DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
SET DestUri=https://<storagename>.blob.core.windows.net/
<root>\Tools\Ext\CloudCopy.exe "%~dp0<root>\DeploymentReferences\WebContainerInitialContent\*" "%DestUri%<containername>" "%DestStorage%"


