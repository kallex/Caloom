 

var DeviceOperationData {
	OperationRequestString: string;
	OperationResult: boolean;

    constructor() {
					this.OperationRequestString = ko.observable(this.OperationRequestString);
			this.OperationResult = ko.observable(this.OperationResult);
    }
}

