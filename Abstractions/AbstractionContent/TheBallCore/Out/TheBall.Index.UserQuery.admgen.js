 

var UserQuery {
	QueryString: string;
	DefaultFieldName: string;

    constructor() {
					this.QueryString = ko.observable(this.QueryString);
			this.DefaultFieldName = ko.observable(this.DefaultFieldName);
    }
}

