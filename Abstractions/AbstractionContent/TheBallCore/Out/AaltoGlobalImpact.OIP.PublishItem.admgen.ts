 

declare var ko: any;

class PublishItem {
	Name: string;

    constructor() {
					this.Name = ko.observable(this.Name);
    }
}

