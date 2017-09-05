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
        private List<ActorId> actorIds = new List<ActorId>();

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
            
            // This only creates a proxy object, it does not activate an actor or invoke any methods yet.
            IDeviceActor deviceActor = ActorProxy.Create<IDeviceActor>(actorId, new Uri(serviceUri));

            // HMTODO: use interfaces instead of concrete classes.
            // HMTODO: use real deviceId.
            IGetDeviceInfo device = await deviceActor.GetAsync("stubDeviceId");

            return this.Json(new DeviceViewModel() { Device = device.AddedDevice });
        }

        // POST api/actorbackendservice
        [Route("api/[controller]/[action]/[id]")]
        [HttpPost]
        public async Task<IActionResult> AddNewDeviceAsync(string deviceActorId)
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            ActorId devActorId = new ActorId(deviceActorId);

            // HMTODO: add proper comments.
            actorIds.Add(devActorId);

            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(devActorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            IDeviceAddedInfo deviceAddedInfo = await proxy.AddNewAsync();

            return this.Json(deviceAddedInfo);
        }

        [HttpPost]
        public async Task<IActionResult> StubEndpointAsync()
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(actorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            string stubTxt = await proxy.StubActionAsync();
            
            return await Task.Run(() => { return this.Json(stubTxt); });
        }
    }
}