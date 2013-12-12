"use strict"
var oip = {
    model: {},
    view: {}
};

oip.view.ContentView = Backbone.View.extend({
    el: $("#main"),
    initialize: function () {
        this.render();
    },
    events: {

    },
    render: function () {

        $.when(this.model.fetch()).then(function(response) {

            var data = {
                "activities": response.data
            }
            $("#main").html(Mustache.render($("#main-content-template").html(), data));
        });

        //console.log("Model after render: " + data.bloglist);
    }
})

oip.model.Activity = Backbone.Model.extend({
    initialize: function (title) {
        this.set({
            author: "",
            title: title,
            location: "",
            group: ""
        });
        //this.save();
    }
})

oip.model.ActivityList = Backbone.Collection.extend({
    url: "datatable.json",
    model: oip.model.Activity,
    collection: {},
//    fetch: function (options) {
//        options = options || {};
//        options.dataType = "xml";
//        Backbone.Collection.prototype.fetch.call(this, options);
//    },
    parse: function(response, xhr) {
        return response.data;
    }
//    parse: function (data) {
//        var parsed = [];
//        var collection = this.collection;
//        $(data).find('Blog').each(function (index) {
//            var blogTitle = $(this).find('Title').text();
//            var id = $(this).find('ID').text();
//            //var blog = new oip.model.Blog(id, blogTitle, id);
//            //parsed.push(blog);
//            parsed.push({id: id, title: blogTitle, name: id});
//            console.log(blogTitle);
//            collection.add({id: id, title: blogTitle, name: id});
//        });
//
//        return parsed;
//    }

})

$(document).ready(function () {
    var activities = new oip.model.ActivityList();
//    var activity = new oip.model.Activity({
//    });
//    //activity.set("id", 0);
//    //blog.set("text", "Lorem ipsum jne")
//
//    activities.add(activity);

    activities.fetch({
        add: true
    })
    new oip.view.ContentView({
        model: activities
    })


});

