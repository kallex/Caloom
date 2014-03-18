 

var CategoryInfo {
	CategoryID: string;
	NativeCategoryID: string;
	NativeCategoryDomainName: string;
	NativeCategoryObjectName: string;
	NativeCategoryTitle: string;
	IdentifyingCategoryName: string;
	ParentCategoryID: string;

    constructor() {
					this.CategoryID = ko.observable(this.CategoryID);
			this.NativeCategoryID = ko.observable(this.NativeCategoryID);
			this.NativeCategoryDomainName = ko.observable(this.NativeCategoryDomainName);
			this.NativeCategoryObjectName = ko.observable(this.NativeCategoryObjectName);
			this.NativeCategoryTitle = ko.observable(this.NativeCategoryTitle);
			this.IdentifyingCategoryName = ko.observable(this.IdentifyingCategoryName);
			this.ParentCategoryID = ko.observable(this.ParentCategoryID);
    }
}

