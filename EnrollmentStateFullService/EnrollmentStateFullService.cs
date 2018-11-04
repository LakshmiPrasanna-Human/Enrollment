using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Data;

namespace EnrollmentStateFullService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class EnrollmentStateFullService : StatefulService, IStudentEnrollmentService
    {
        //public EnrollmentStateFullService(StatefulServiceContext context)
        //    : base(context)
        //{ }

        public EnrollmentStateFullService(StatefulServiceContext context, InitializationCallbackAdapter adapter)
            : base(context, new ReliableStateManager(context, new ReliableStateManagerConfiguration(onInitializeStateSerializersEvent: adapter.OnInitialize)))
        {
            adapter.StateManager = this.StateManager;
        }


        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
           // return new ServiceReplicaListener[0];
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(serviceContext => new OwinCommunicationListener(
        serviceContext, this, ServiceEventSource.Current,   "ServiceEndpoint"))
            };

        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            //var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
            //        // discarded, and nothing is saved to the secondary replicas.
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

        }


        /// <summary>
        /// Gets the count dictionary. If it doesn't already exist it is created.
        /// </summary>
        /// <remarks>This is a good example of how to initialize reliable collections. 
        /// Rather than initializing and caching a value, this approach will work on 
        /// both primary and secondary replicas (if secondary reads are enabled.</remarks>
        private async Task<IReliableDictionary<string, EnrollmentData>> GetEnrollmentDictionaryAsync()
        {
            return await StateManager.GetOrAddAsync<IReliableDictionary<string, EnrollmentData>>("EnrollmentDictionary").ConfigureAwait(false);
        }

        // Track the number of requests to the controller.
        private long _requestCount = 0;

        
        /// <summary>
        /// Gets the list of Enrollment items.
        /// </summary>
        public async Task<IReadOnlyList<EnrollmentData>> GetEnrollmentDataAsync(CancellationToken token)
        {
            List<EnrollmentData> items = new List<EnrollmentData>();
            ServiceEventSource.Current.ServiceRequestStart("EnrollmentState.GetEnrollmentDataAsync", "" );

            // Get the dictionary.
            var dictionary = await GetEnrollmentDictionaryAsync();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                // Create the enumerable and get the enumerator.
                var enumItems = (await dictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumItems.MoveNextAsync(token))
                {
                    items.Add(enumItems.Current.Value);
                    if (items.Count > 1000)
                        break;
                }
            }

            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task AddEnrollmentAsync(string Grade, string SchoolYear, string FirstName, string LastName, CancellationToken token)
        {
            ServiceEventSource.Current.ServiceRequestStart("EnrollmentState.AddEnrollmentAsync", "");

            // Get the dictionary.
            var dictionary = await GetEnrollmentDictionaryAsync();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                // Try to get the existing value
                ConditionalValue<EnrollmentData> result = await dictionary.TryGetValueAsync(tx, SchoolYear, LockMode.Update);
                if (result.HasValue)
                {
                     EnrollmentData newData = new EnrollmentData
                    {
                        Grade = Grade,
                        SchoolYear = SchoolYear,
                        FirstName = FirstName,
                        LastName = LastName,
                    };

                    await dictionary.TryUpdateAsync(tx, SchoolYear, newData, result.Value);
                }
                else
                {
                   
                    await dictionary.AddAsync(tx, SchoolYear, new EnrollmentData(Grade, SchoolYear, FirstName, LastName));
                }

                // Commit the transaction.
                await tx.CommitAsync();
            }
        }

    }
}
