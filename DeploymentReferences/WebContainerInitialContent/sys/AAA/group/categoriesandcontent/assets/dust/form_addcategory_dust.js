(function(){dust.register("form_addcategory.dust",body_0);function body_0(chk,ctx){return chk.partial("modal_addobject_begin.dust",ctx,{"form_name":"category","header_title":"Add Category","object_domain":"AaltoGlobalImpact.OIP","object_type":"Category"}).partial("fileupload.dust",ctx,{"initial_class_mode":"fileupload-new","field_name":"ImageData"}).partial("textinput_singleline.dust",ctx,{"field_id":"AddCategory_Title","field_name":"Title","field_label":"Title"}).partial("textinput_multiline_markdown.dust",ctx,{"field_id":"AddCategory_Excerpt","field_name":"Excerpt","field_label":"Excerpt"}).partial("modal_addobject_end.dust",ctx,null).write("<script type=\"text/javascript\">$(function() {$(\".open-addcategory\").on(\"click\", function() {InitializeModalClasses();$('#add-category').modal('show');});});</script>");}return body_0;})();