rem replace <webprojectsroot>, <relativepath-to-ball-root>, <container-name>, <admin-group-id>, <accountname>, <accountkey>
@echo off
pushd <webprojectsroot>\OIPTemplates\UI\admin
<relativepath-to-ball-root>\Tools\WebTemplateManager\bin\Debug\WebTemplateManager.exe <container-name> -pri admin grp<admin-group-id> DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>
popd
