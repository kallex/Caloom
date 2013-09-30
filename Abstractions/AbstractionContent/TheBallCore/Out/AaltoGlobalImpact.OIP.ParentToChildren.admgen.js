 

var ParentToChildren {
	id: string;
	children: ParentToChildren[];

    constructor() {
					this.id = ko.observable(this.id);
			this.children = ko.observableArray(this.children);
    }
}

