﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ActorBackendService.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;

    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDeviceActor : IActor
    {
        Task<IGetDeviceInfo> GetAsync(string deviceId);
        Task<IDeviceAddedInfo> AddNewAsync();
        Task<IDeviceRemovedInfo> RemoveAsync(string deviceId);
        Task<IDeviceRenamedInfo> RenameAsync(string deviceId, string newName);
        Task<IDeviceToggledActivationStatusInfo> ToggleActivationStatusAsync(string deviceId);
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

    public class DeviceErrorInfo
    {
        public bool IsSuccess { get; set; }
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

    public class GetDeviceInfo : IGetDeviceInfo
    {
        public Device AddedDevice { get; set; }
        public DeviceErrorInfo DevErrinfo { get; set; }

        // HMTODO: remove all usused ctors.
        public GetDeviceInfo()
        {
        }
        public GetDeviceInfo(Device addedDevice, DeviceErrorInfo devErrinfo)
        {
            AddedDevice = addedDevice;
            DevErrinfo = devErrinfo;
        }
    }

    public interface IGetDeviceInfo: IDeviceInfoResult
    {
        Device AddedDevice { get; set; }
        DeviceErrorInfo DevErrinfo { get; set; }
    }

    public interface IDeviceAddedInfo : IDeviceInfoResult
    {
    }

    public interface IDeviceRemovedInfo : IDeviceInfoResult
    {
    }

    public interface IDeviceRenamedInfo : IDeviceInfoResult
    {
    }

    public interface IDeviceToggledActivationStatusInfo : IDeviceInfoResult
    {
    }

    public class DeviceAddedInfo: IDeviceAddedInfo
    {
        public string Id { get; set; }

        public DeviceAddedInfo()
        {
        }

        public DeviceAddedInfo(string id)
        {
            Id = id;
        }
    }

    public class DeviceRemovedInfo: IDeviceRemovedInfo
    {
        public string Id { get; set; }

        public DeviceRemovedInfo()
        {
        }

        public DeviceRemovedInfo(string id)
        {
            Id = id;
        }
    }

    public class DeviceRenamedInfo: IDeviceRenamedInfo
    {
        public string Id { get; set; }

        public DeviceRenamedInfo()
        {
        }

        public DeviceRenamedInfo(string id)
        {
            Id = id;
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

    public class ErrorInfo
    {
        public string Msg { get; set; }

        public ErrorInfo()
        {
        }

        public ErrorInfo(string msg)
        {
            Msg = msg;
        }
    }

    public class Device : IDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }
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