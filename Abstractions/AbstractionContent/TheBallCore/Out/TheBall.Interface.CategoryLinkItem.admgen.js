 

var CategoryLinkItem {
	SourceCategoryID: string;
	TargetCategoryID: string;
	LinkingType: string;

    constructor() {
					this.SourceCategoryID = ko.observable(this.SourceCategoryID);
			this.TargetCategoryID = ko.observable(this.TargetCategoryID);
			this.LinkingType = ko.observable(this.LinkingType);
    }
}

