// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebService.Controllers
{
    using ActorBackendService.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Actors.Query;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [Route("api/[controller]/[action]")]
    public class ActorBackendServiceController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;
        private static Dictionary<string, ActorId> deviceActorIdMap = new Dictionary<string, ActorId>();

        public ActorBackendServiceController(StatelessServiceContext serviceContext, ConfigSettings settings, FabricClient fabricClient)
        {
            this.serviceContext = serviceContext;
            this.configSettings = settings;
            this.fabricClient = fabricClient;
        }

        // GET: api/actorbackendservice
        [HttpGet]
        public async Task<IActionResult> GetAllDevicesAsync()
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            List<IGetDeviceInfo> devices = new List<IGetDeviceInfo>();

            // HMFLOW: Iterate through all the saved dict. of <ActorID, DeviceID>
            foreach (var deviceActorIdPair in deviceActorIdMap)
            {
                // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
                IDeviceActor deviceActor = ActorProxy.Create<IDeviceActor>(deviceActorIdPair.Value, new Uri(serviceUri));

                // HMTODO: use interfaces instead of concrete classes.
                // HMTODO: use real deviceId.
                // HMTODO: [Improvement] Can't we query here GetAsync(<deviceId>) and then we can store several devices with one Actor.
                // HMFLOW: The device is retrieved here.
                IGetDeviceInfo device = await deviceActor.GetAsync();

                devices.Add(device);
            }

            // HMTODO: Implement similar wrappers for other endpoints too.
            // HMFLOW: This is a wrapper to wrap all the returned devices.
            return this.Json(new DevicesViewModel() { DevicesInfo = devices });
        }

        // POST api/actorbackendservice
        [HttpPost("{id}")]
        public async Task<IActionResult> AddNewDeviceAsync(string id)
        {
            string deviceId = id;
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            ActorId devActorId = ActorId.CreateRandom();

            // HMTODO: add proper comments.
            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(devActorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            // HMFLOW: Device gets added here, returns the Device details.
            IDeviceAddedInfo deviceAddedInfo = await proxy.AddNewAsync(deviceId);

            // save the DeviceId
            // HMFLOW: save the DeviceId
            deviceActorIdMap.Add(deviceAddedInfo.Device.Id, devActorId);

            return this.Json(deviceAddedInfo);
        }

        // POST api/actorbackendservice
        [HttpPost]
        public async Task<IActionResult> RenameDeviceAsync([FromBody] RenameDeviceModel renameDeviceModel)
        {
            string deviceId = renameDeviceModel.DeviceId;

            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            ActorId devActorId = deviceActorIdMap[deviceId];

            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(devActorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            IDeviceRenamedInfo deviceRenamedInfo = await proxy.RenameDeviceAsync(deviceId, renameDeviceModel.NewDeviceName);

            return this.Json(deviceRenamedInfo);
        }

        [Serializable]
        public class RenameDeviceModel
        {
            public string DeviceId { get; set; }
            public string NewDeviceName { get; set; }

            public RenameDeviceModel()
            {

            }
        }

        [HttpPost]
        public async Task<IActionResult> StubEndpointAsync(string deviceActorId)
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            ActorId devActorId = new ActorId(deviceActorId);

            // HMTODO: add proper comments.
            //actorIds.Add(devActorId);

            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(devActorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            string stubTxt = await proxy.StubActionAsync();
            
            return await Task.Run(() => { return this.Json(stubTxt); });
        }
    }
}