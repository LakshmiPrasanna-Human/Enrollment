using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnrollmentUI.Model;
using System.Net.Http;
using System.Fabric.Query;
using System.Fabric;
using Newtonsoft.Json;
using Microsoft.ServiceFabric.Services.Client;
using System.Threading;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EnrollmentUI.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly FabricClient fabricClient;
        private readonly StatelessServiceContext serviceContext;

        public StudentController(HttpClient httpClient, StatelessServiceContext context, FabricClient fabricClient)
        {
            this.fabricClient = fabricClient;
            this.httpClient = httpClient;

            this.serviceContext = context;
        }

        // GET: /<controller>/
        public IActionResult Enroll()
        {
            return View();
        }

        public async Task<IActionResult> StudentEnrollDetails()
        {


            //var resolver = ServicePartitionResolver.GetDefault();
            //var fabricClient = new FabricClient();
            //var apps = fabricClient.QueryManager.GetApplicationListAsync().Result;
            //foreach (var app in apps)
            //{
            //    Console.WriteLine($"Discovered application:'{app.ApplicationName}");

            //    var services = fabricClient.QueryManager.GetServiceListAsync(app.ApplicationName).Result;
            //    foreach (var service in services)
            //    {
            //        Console.WriteLine($"Discovered Service:'{service.ServiceName}");

            //        var partitions = fabricClient.QueryManager.GetPartitionListAsync(service.ServiceName).Result;
            //        foreach (var partition in partitions)
            //        {
            //            Console.WriteLine($"Discovered Service Partition:'{partition.PartitionInformation.Kind} {partition.PartitionInformation.Id}");


            //            ServicePartitionKey key;
            //            switch (partition.PartitionInformation.Kind)
            //            {
            //                case ServicePartitionKind.Singleton:
            //                    key = ServicePartitionKey.Singleton;
            //                    break;
            //                case ServicePartitionKind.Int64Range:
            //                    var longKey = (Int64RangePartitionInformation)partition.PartitionInformation;
            //                    key = new ServicePartitionKey(longKey.LowKey);
            //                    break;
            //                case ServicePartitionKind.Named:
            //                    var namedKey = (NamedPartitionInformation)partition.PartitionInformation;
            //                    key = new ServicePartitionKey(namedKey.Name);
            //                    break;
            //                default:
            //                    throw new ArgumentOutOfRangeException("partition.PartitionInformation.Kind");
            //            }
            //            var resolved = resolver.ResolveAsync(service.ServiceName, key, CancellationToken.None).Result;
            //            foreach (var endpoint in resolved.Endpoints)
            //            {
            //                Console.WriteLine($"Discovered Service Endpoint:'{endpoint.Address}");
            //            }
            //        }
            //    }
            //}
            //List<StudentEnroll> _list = new List<StudentEnroll>();
            //_list.Add(new StudentEnroll
            //{
            //    Schoolid = "1000",
            //    Grade = "1",
            //    Schoolyear = "2018",
            //    Firstname = "Lakshmi",
            //    Lastname = "Prasanna",
            //    Middlename = "MN"
            //});
            //_list.Add(new StudentEnroll
            //{
            //    Schoolid = "1000",
            //    Grade = "1",
            //    Schoolyear = "2018",
            //    Firstname = "Test1",
            //    Lastname = "Test1",
            //    Middlename = "Test1"
            //});
            //_list.Add(new StudentEnroll
            //{
            //    Schoolid = "1000",
            //    Grade = "1",
            //    Schoolyear = "2018",
            //    Firstname = "Test2",
            //    Lastname = "Test2",
            //    Middlename = "Test2"
            //});

            //ViewBag.StudentEnrollList = _list;

            Uri serviceName = EnrollmentUI.GetEnrollmentServiceName(this.serviceContext);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            ServicePartitionList partitions = await this.fabricClient.QueryManager.GetPartitionListAsync(serviceName);
            List<StudentEnroll> result = new List<StudentEnroll>();
            ServicePartitionKey key;

            foreach (Partition partition in partitions)
            {
                //string proxyUrl =
                //    $"{proxyAddress}/api/Student/?PartitionKey={((Int64RangePartitionInformation)partition.PartitionInformation).LowKey}&PartitionKind=Int64Range";
                //System.Fabric.Int64RangePartitionInformation

                string proxyUrl =
                   $"{proxyAddress}/api/Student/";


                using (HttpResponseMessage response = await this.httpClient.GetAsync(proxyUrl))
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        continue;
                    }

                    result.AddRange(JsonConvert.DeserializeObject<List<StudentEnroll>>(await response.Content.ReadAsStringAsync()));
                }
            }

            ViewBag.StudentEnrollList = result;

            return View();
        }

        [HttpPost]
        public ActionResult LoadData()
        {
            try
            {
                var draw = 1;
                var start = 1;
                var length = 4;
                List<StudentEnroll> _list = new List<StudentEnroll>();
                _list.Add(new StudentEnroll
                {
                    Schoolid = "1000",
                    Grade = "1",
                    Schoolyear = "2018",
                    Firstname = "Lakshmi",
                    Lastname = "Prasanna",
                    Middlename = "MN"
                });
                _list.Add(new StudentEnroll
                {
                    Schoolid = "1000",
                    Grade = "1",
                    Schoolyear = "2018",
                    Firstname = "Test1",
                    Lastname = "Test1",
                    Middlename = "Test1"
                });
                _list.Add(new StudentEnroll
                {
                    Schoolid = "1000",
                    Grade = "1",
                    Schoolyear = "2018",
                    Firstname = "Test2",
                    Lastname = "Test2",
                    Middlename = "Test2"
                });


                var data = _list;
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = 3, recordsTotal = 3, data = data });
                
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Constructs a reverse proxy URL for a given service.
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private Uri GetProxyAddress(Uri serviceName)
        {
            ////return new Uri($"http://localhost:19081{serviceName.AbsolutePath}");
           //  return new Uri($"http://localhost:19087{serviceName.AbsolutePath}");
            return new Uri($"http://mysfcluster123.southcentralus.cloudapp.azure.com:19081{serviceName.AbsolutePath}");
            
        }

        /// <summary>
        /// Creates a partition key from the given name.
        /// Uses the zero-based numeric position in the alphabet of the first letter of the name (0-25).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private long GetPartitionKey(string name)
        {
            return Char.ToUpper(name.First()) - 'A';
        }
    }
}
