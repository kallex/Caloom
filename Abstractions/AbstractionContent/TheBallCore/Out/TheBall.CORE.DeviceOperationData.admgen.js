 

var DeviceOperationData {
	OperationRequestString: string;
	OperationParameters: string[];
	OperationReturnValues: string[];
	OperationResult: boolean;
	OperationSpecificContentData: ContentItemLocationWithMD5[];

    constructor() {
					this.OperationRequestString = ko.observable(this.OperationRequestString);
			this.OperationParameters = ko.observableArray(this.OperationParameters);
			this.OperationReturnValues = ko.observableArray(this.OperationReturnValues);
			this.OperationResult = ko.observable(this.OperationResult);
			this.OperationSpecificContentData = ko.observableArray(this.OperationSpecificContentData);
    }
}

