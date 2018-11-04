using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Threading.Tasks;
using EnrollmentStateFullService;
using EnrollmentStateFullService.Models;

namespace EnrollmentStateFullService.Controllers
{
    public sealed class StatefulEnrollmentController : ApiController
    {
        const string activityHeader = "activity-id";

        // Keep an instance of the service.
        private IStudentEnrollmentService _service = null;

        // Controller constructor taking a IEnrollmentService instance.
        // This is cheap dependency injection done in the listener.
        // You can also use your favorite DI framework.
        public StatefulEnrollmentController(IStudentEnrollmentService vs)
        {
            _service = vs;
        }

        // GET api/votes 
        [HttpGet]
        [Route("api/Enrollments")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            string activityId = GetHeaderValueOrDefault(Request, activityHeader, () => { return Guid.NewGuid().ToString(); });
            ServiceEventSource.Current.ServiceRequestStart("EnrollmentController.Get", activityId);
         //   await _service.AddEnrollmentAsync("11", "2018", "FN", "LN", CancellationToken.None);

            IReadOnlyList<EnrollmentData> Enrollments = await _service.GetEnrollmentDataAsync( CancellationToken.None);

            //List<KeyValuePair<string, EnrollmentData>> Enrollmentslist = new List<KeyValuePair<string, int>>(Enrollments.Count);
            //foreach (EnrollmentData data in Enrollmentslist)
            //{
            //    Enrollments.Add(new KeyValuePair<string, int>(data.Grade, data.SchoolYear, data.FirstName, data.LastName));
            //}

            var response = Request.CreateResponse(HttpStatusCode.OK, Enrollments);
            response.Headers.CacheControl = new CacheControlHeaderValue() { NoCache = true, MustRevalidate = true };

            ServiceEventSource.Current.ServiceRequestStop("EnrollmentController.Get", activityId);
            //_service.RequestCount = 1;
            return response;
        }

        [HttpPost]
        [Route("api/Enrollmemt")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] StudentEnroll enrollObj)
        {
            string activityId = GetHeaderValueOrDefault(Request, activityHeader, () => { return Guid.NewGuid().ToString(); });
            ServiceEventSource.Current.ServiceRequestStart("EnrollmentController.Post", activityId);

            // Update or add the item.
      
            await _service.AddEnrollmentAsync(enrollObj.Grade, enrollObj.Schoolyear, enrollObj.Firstname, enrollObj.Lastname, CancellationToken.None);

            ServiceEventSource.Current.ServiceRequestStop("EnrollmentController.Post", activityId);
            //_service.RequestCount = 1;
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Gets a value from a header collection or returns the default value from the function.
        /// </summary>
        public static string GetHeaderValueOrDefault(HttpRequestMessage request, string headerName, Func<string> getDefault)
        {
            // If headers are not specified, return the default string.
            if ((null == request) || (null == request.Headers))
                return getDefault();

            // Search for the header name in the list of headers.
            IEnumerable<string> values;
            if (true == request.Headers.TryGetValues(headerName, out values))
            {
                // Return the first value from the list.
                foreach (string value in values)
                    return value;
            }

            // return an empty string as default.
            return getDefault();
        }
    }

}
