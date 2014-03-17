using System;
using System.IO;
using AzureSupport;

namespace TheBall.CORE
{
    public class RemoteDeviceCoreOperationImplementation
    {
        public static DeviceOperationData GetTarget_DeviceOperationData(Stream inputStream)
        {
            return JSONSupport.GetObjectFromStream<DeviceOperationData>(inputStream);
        }

        public static DeviceMembership GetTarget_CurrentDevice()
        {
            return InformationContext.CurrentExecutingForDevice;
        }

        public static void ExecuteMethod_PerformOperation(DeviceMembership currentDevice, DeviceOperationData deviceOperationData)
        {
            switch (deviceOperationData.OperationRequestString)
            {
                case "DELETEREMOTEDEVICE":
                    currentDevice.DeleteInformationObject();
                    break;
                default:
                    throw new NotImplementedException("Not implemented RemoteDevice operation for request string: " + deviceOperationData.OperationRequestString);
            }
            deviceOperationData.OperationResult = true;
        }

        public static void ExecuteMethod_SerializeDeviceOperationDataToOutput(Stream outputStream, DeviceOperationData deviceOperationData)
        {
            JSONSupport.SerializeToJSONStream(deviceOperationData, outputStream);
        }
    }
}