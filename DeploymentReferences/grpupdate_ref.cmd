rem Replace <container-name>, <accountname>, <accountkey>
@echo off
pushd WebContainerInitialContent\sys\AAA\group\categoriesandcontent
..\..\..\..\..\..\Tools\WebTemplateManager\bin\Debug\WebTemplateManager.exe <container-name> -sys categoriesandcontent sysgroup DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
popd
