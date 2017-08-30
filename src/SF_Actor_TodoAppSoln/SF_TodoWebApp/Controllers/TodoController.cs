using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SF_TodoWebApp.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : ApiController
    {
        private readonly FabricClient fabricClient;
        private readonly ConfigSettings configSettings;
        private readonly StatelessServiceContext serviceContext;

        public TodoController(StatelessServiceContext serviceContext, ConfigSettings settings, FabricClient fabricClient)
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

            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(new Uri(serviceUri));

            long count = 0;
            foreach (Partition partition in partitions)
            {
                long partitionKey = ((Int64RangePartitionInformation)partition.PartitionInformation).LowKey;
                IActorService actorServiceProxy = ActorServiceProxy.Create(new Uri(serviceUri), partitionKey);

                ContinuationToken continuationToken = null;

                do
                {
                    PagedResult<ActorInformation> page = await actorServiceProxy.GetActorsAsync(continuationToken, CancellationToken.None);

                    count += page.Items.Where(x => x.IsActive).LongCount();

                    continuationToken = page.ContinuationToken;
                }
                while (continuationToken != null);
            }

            return this.Json(new CountViewModel() { Count = count });
        }

        // POST api/actorbackendservice
        [HttpPost]
        public async Task<IActionResult> PostAsync()
        {

            string serviceUri = this.serviceContext.CodePackageActivationContext.ApplicationName + "/" + this.configSettings.ActorBackendServiceName;

            IMyActor proxy = ActorProxy.Create<IMyActor>(ActorId.CreateRandom(), new Uri(serviceUri));

            await proxy.StartProcessingAsync(CancellationToken.None);

            return this.Json(true);

        }
    }
}
