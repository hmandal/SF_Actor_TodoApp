// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace ActorBackendService
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using ActorBackendService.Interfaces;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Runtime;

    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [System.Runtime.Serialization.KnownType(typeof(DeviceAddedInfo))]
    public class DeviceActor : Actor, IDeviceActor
    {
        private string _stateName = "DeviceList"; // HMTODO: Store list in WebApi, and Device here.
        private bool _isReplay = false; // HMTODO: Store list in WebApi, and Device here.

        /// <summary>
        /// Initializes a new instance of ActorBackendService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public DeviceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Device stubDevice = new Device("stubDeviceId", "stubDeviceName", true);

                //bool added = await this.StateManager.TryAddStateAsync<Device>(_stateName, stubDevice);

                //if (!added)
                //{
                //    _isReplay = true;
                //    // value already exists, which means processing has already started.
                //    //throw new InvalidOperationException($"Processing for this actor (ActorId: { this.ActorService.ActorTypeInformation.ImplementationType.Name }) has already started.");
                //}
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, $"{ this.ActorService.ActorTypeInformation.ImplementationType.Name } Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization
            await base.OnActivateAsync();
        }

        #region DeviceActor

        async Task<GetDeviceInfo> IDeviceActor.GetAsync()
        {
            DeviceErrorInfo devErrinfo = null;
            ErrorInfo errInfo = null;
            GetDeviceInfo retVal = null;

            try
            {
                // HMTODO: Get a real Device here.
                // HMTODO: use cancellation token.
                Device addedDevice = await this.StateManager.GetStateAsync<Device>(_stateName, CancellationToken.None);

                devErrinfo = new DeviceErrorInfo(true, errInfo);
                retVal = new GetDeviceInfo(addedDevice, devErrinfo);

                return retVal;
            }
            catch (Exception)
            {
                errInfo = new ErrorInfo("GetAsync Failed.");
                devErrinfo = new DeviceErrorInfo(false, errInfo);
                retVal = new GetDeviceInfo(null, devErrinfo);

                return retVal;
            }
        }

        async Task<DeviceAddedInfo> IDeviceActor.AddNewAsync()
        {
            DeviceErrorInfo devErrinfo = new DeviceErrorInfo(true, null);
            ErrorInfo errInfo = null;
            DeviceAddedInfo retVal = null;

            try
            {
                Device deviceToAdd = new Device($"{{DeviceId-{Guid.NewGuid()}}}", $"{{DeviceName-{Guid.NewGuid()}}}", true);

                // HMTODO: Add a real Device here.
                // HMTODO: use cancellation token.
                bool added = await this.StateManager.TryAddStateAsync<Device>(_stateName, deviceToAdd, CancellationToken.None);

                if (!added)
                {
                    _isReplay = true;
                    errInfo = new ErrorInfo($"Replay! Device: AddNewAsync");
                    // value already exists, which means processing has already started.
                    //throw new InvalidOperationException($"Processing for this actor (ActorId: { this.ActorService.ActorTypeInformation.ImplementationType.Name }) has already started.");
                }

                devErrinfo = new DeviceErrorInfo(true, errInfo);

                retVal = new DeviceAddedInfo(deviceToAdd.Id, devErrinfo);

                return retVal;
            }
            catch (Exception)
            {
                errInfo = new ErrorInfo($"AddNewAsync failed!");
                devErrinfo = new DeviceErrorInfo(false, errInfo);

                // HMTODO: Replace this with a Representation of no device.
                retVal = new DeviceAddedInfo(string.Empty, devErrinfo);

                return retVal;
            }
        }

        async Task<IDeviceRemovedInfo> IDeviceActor.RemoveAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        async Task<IDeviceRenamedInfo> IDeviceActor.RenameAsync(string deviceId, string newName)
        {
            throw new NotImplementedException();
        }

        async Task<IDeviceToggledActivationStatusInfo> IDeviceActor.ToggleActivationStatusAsync(string deviceId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}