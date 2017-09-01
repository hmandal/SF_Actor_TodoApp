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

    [Route("api/[controller]")]
    public class ActorBackendServiceController : Controller
    {
        private readonly FabricClient fabricClient;
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;
        private ActorId actorId = new ActorId(111);

        public ActorBackendServiceController(StatelessServiceContext serviceContext, ConfigSettings settings, FabricClient fabricClient)
        {
            this.serviceContext = serviceContext;
            this.configSettings = settings;
            this.fabricClient = fabricClient;
        }

        // GET: api/actorbackendservice
        [HttpGet]
        public async Task<IActionResult> GetAsync()
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
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {
            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            IDeviceActor proxy = ActorProxy.Create<IDeviceActor>(actorId, new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            return this.Json(true);
        }
    }
}