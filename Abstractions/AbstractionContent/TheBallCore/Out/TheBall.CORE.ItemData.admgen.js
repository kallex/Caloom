 

var ItemData {
	DataName: string;
	ItemTextData: string;

    constructor() {
					this.DataName = ko.observable(this.DataName);
			this.ItemTextData = ko.observable(this.ItemTextData);
    }
}

