 

var CategoryLinkParameters {
	ConnectionID: string;
	LinkItems: CategoryLinkItem[];

    constructor() {
					this.ConnectionID = ko.observable(this.ConnectionID);
			this.LinkItems = ko.observableArray(this.LinkItems);
    }
}

