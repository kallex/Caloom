rem Replace <container-name>, <accountname>, <accountkey>
@echo off
pushd WebContainerInitialContent\sys\AAA\account\webui
..\..\..\..\..\..\Tools\WebTemplateManager\bin\Debug\WebTemplateManager.exe <container-name> -sys webui sysaccount DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
popd
