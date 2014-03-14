 

var ConnectionCommunicationData {
	ActiveSideConnectionID: string;
	ReceivingSideConnectionID: string;
	ProcessRequest: string;

    constructor() {
					this.ActiveSideConnectionID = ko.observable(this.ActiveSideConnectionID);
			this.ReceivingSideConnectionID = ko.observable(this.ReceivingSideConnectionID);
			this.ProcessRequest = ko.observable(this.ProcessRequest);
    }
}

