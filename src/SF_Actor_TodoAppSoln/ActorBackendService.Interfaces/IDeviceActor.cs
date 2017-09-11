// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ActorBackendService.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    using System.Runtime.Serialization;
    using System;
    using System.Linq;

    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDeviceActor : IActor
    {
        Task<GetDeviceInfo> GetAsync();
        Task<DeviceAddedInfo> AddNewAsync(string deviceId);
        Task<DeviceRenamedInfo> RenameAsync(string deviceId, string newDeviceName);
        Task<DeviceRemovedInfo> RemoveAsync(string deviceId);
        Task<IDeviceToggledActivationStatusInfo> ToggleActivationStatusAsync(string deviceId);
        Task<string> StubActionAsync();
        Task StartProcessingAsync(CancellationToken none);
    }

    public class DeviceInfoResult: IDeviceInfoResult
    {
        public DeviceErrorInfo ErrorInfo { get; set; }

        public DeviceInfoResult()
        {
        }

        public DeviceInfoResult(DeviceErrorInfo error)
        {
            ErrorInfo = error;
        }
    }

    public interface IDeviceInfoResult
    {
    }

    [DataContract]
    public class DeviceErrorInfo
    {
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public ErrorInfo ErrorInfo { get; set; }

        public DeviceErrorInfo()
        {
        }

        public DeviceErrorInfo(bool isSuccess, ErrorInfo errorInfo)
        {
            IsSuccess = isSuccess;
            ErrorInfo = errorInfo;
        }
    }

    [DataContract]
    public class GetDeviceInfo : IGetDeviceInfo
    {
        [DataMember]
        public Device Device { get; set; }
        [DataMember]
        public DeviceErrorInfo DevErrinfo { get; set; }

        // HMTODO: remove all usused ctors.
        public GetDeviceInfo()
        {
        }
        public GetDeviceInfo(Device addedDevice, DeviceErrorInfo devErrInfo)
        {
            Device = addedDevice;
            DevErrinfo = devErrInfo;
        }
    }

    public interface IGetDeviceInfo: IDeviceInfoResult
    {
        Device Device { get; set; }
        DeviceErrorInfo DevErrinfo { get; set; }
    }

    public interface IDeviceAddedInfo : IDeviceInfoResult
    {
        Device Device { get; set; }
        DeviceErrorInfo DevErrInfo { get; set; }
    }

    public interface IDeviceRemovedInfo : IDeviceInfoResult
    {
        bool IsSuccess { get; set; }
        DeviceErrorInfo DevErrInfo { get; set; }
    }

    public interface IDeviceRenamedInfo : IDeviceInfoResult
    {
        // HMTODO: Return only the success state, NOT the whole device.
        Device Device { get; set; }

        // HMTODO: move DevErrInfo to IDeviceInfoResult
        DeviceErrorInfo DevErrInfo { get; set; }
    }

    public interface IDeviceToggledActivationStatusInfo : IDeviceInfoResult
    {
    }

    [DataContract]
    [KnownType(typeof(DeviceAddedInfo))]
    public class DeviceAddedInfo: IDeviceAddedInfo
    {
        [DataMember]
        public Device Device { get; set; }
        [DataMember]
        public DeviceErrorInfo DevErrInfo { get; set; }

        public DeviceAddedInfo()
        {
        }

        public DeviceAddedInfo(Device device, DeviceErrorInfo devErrInfo)
        {
            Device = device;
            DevErrInfo = devErrInfo;
        }
    }

    [DataContract]
    public class DeviceRemovedInfo: IDeviceRemovedInfo
    {
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public DeviceErrorInfo DevErrInfo { get; set; }

        public DeviceRemovedInfo()
        {
        }

        public DeviceRemovedInfo(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }

    [DataContract]
    public class DeviceRenamedInfo: IDeviceRenamedInfo
    {
        [DataMember]
        public Device Device { get; set; }
        [DataMember]
        public DeviceErrorInfo DevErrInfo { get; set; }

        public DeviceRenamedInfo()
        {
        }

        public DeviceRenamedInfo(Device device, DeviceErrorInfo devErrInfo)
        {
            Device = device;
            DevErrInfo = devErrInfo;
        }
    }

    public class DeviceToggledActivationStatusInfo: IDeviceToggledActivationStatusInfo
    {
        public string Id { get; set; }
        public bool ActivationStatus { get; set; }

        public DeviceToggledActivationStatusInfo()
        {
        }

        public DeviceToggledActivationStatusInfo(string id, bool activationStatus)
        {
            Id = id;
            ActivationStatus = activationStatus;
        }
    }

    [DataContract]
    public class ErrorInfo
    {
        [DataMember]
        public string Msg { get; set; }

        public ErrorInfo()
        {
        }

        public ErrorInfo(string msg)
        {
            Msg = msg;
        }
    }

    [DataContract]
    public class Device : IDevice
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool ActivationStatus { get; set; }

        public Device()
        {
        }

        public Device(string id, string name, bool activationStatus)
        {
            Id = id;
            Name = name;
            ActivationStatus = activationStatus;
        }
    }

    public interface IDevice
    {
        // HMTODO: Check get; set.
        string Id { get; set; }
        string Name { get; set; }
        bool ActivationStatus { get; set; }
    }
}