 

var QueryToken {
	QueryRequestObjectDomainName: string;
	QueryRequestObjectName: string;
	QueryRequestObjectID: string;

    constructor() {
					this.QueryRequestObjectDomainName = ko.observable(this.QueryRequestObjectDomainName);
			this.QueryRequestObjectName = ko.observable(this.QueryRequestObjectName);
			this.QueryRequestObjectID = ko.observable(this.QueryRequestObjectID);
    }
}

