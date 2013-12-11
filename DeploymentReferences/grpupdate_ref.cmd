rem Replace <webprojectsroot>, <relativepath-to-ball-root>, <container-name>, <admin-group-id>, <accountname>, <accountkey>
@echo off
pushd <webprojectsroot>\OIPTemplates\UI\categoriesandcontent
<relativepath-to-ball-root>\Tools\WebTemplateManager\bin\Debug\WebTemplateManager.exe <container-name> -sys categoriesandcontent sysgroup DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
popd
