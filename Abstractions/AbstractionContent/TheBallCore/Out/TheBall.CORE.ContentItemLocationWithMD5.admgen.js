 

var ContentItemLocationWithMD5 {
	ContentLocation: string;
	ContentMD5: string;
	ItemDatas: ItemData[];

    constructor() {
					this.ContentLocation = ko.observable(this.ContentLocation);
			this.ContentMD5 = ko.observable(this.ContentMD5);
			this.ItemDatas = ko.observableArray(this.ItemDatas);
    }
}

