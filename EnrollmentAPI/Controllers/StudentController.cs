using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnrollmentAPI.Model;
using System.Threading;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EnrollmentUI.Controllers
{
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        // Used for health checks.
        public static long _requestCount = 0L;


        //schoolid, grade, School year
        // GET: api/values
        [HttpPost]
        [Route("api/[controller]/Enroll")]
        public void Enroll([FromBody] StudentEnroll enrollObj)
        {
            Interlocked.Increment(ref _requestCount);
            string activityId = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("StudentController.Enroll", activityId);

            ServiceEventSource.Current.ServiceRequestStop("StudentController.Enroll", activityId);
        }
        
        // GET: api/values
        [HttpGet]
        public IEnumerable<StudentEnroll> Get()
        {
            Interlocked.Increment(ref _requestCount);
            string activityId = Guid.NewGuid().ToString();
            ServiceEventSource.Current.ServiceRequestStart("StudentController.Get", activityId);

            List<StudentEnroll> _list = new List<StudentEnroll>();
            _list.Add(new StudentEnroll {
                Schoolid= "1000",
                Grade = "1",
                Schoolyear = "2018",
                Firstname="Lakshmi",
                Lastname = "Prasanna",
                Middlename = "MN"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "2",
                Schoolyear = "2018",
                Firstname = "Test1",
                Lastname = "Test1",
                Middlename = "Test1"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "3",
                Schoolyear = "2018",
                Firstname = "Test2",
                Lastname = "Test2",
                Middlename = "Test2"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "4",
                Schoolyear = "2018",
                Firstname = "Test4",
                Lastname = "Test4",
                Middlename = "Test4"
            });

            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "5",
                Schoolyear = "2018",
                Firstname = "Test5",
                Lastname = "Test5",
                Middlename = "Test5"
            });
             _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "6",
                Schoolyear = "2018",
                Firstname = "Test6",
                Lastname = "Test6",
                Middlename = "Test6"
            });

            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "7",
                Schoolyear = "2018",
                Firstname = "Test7",
                Lastname = "Test7",
                Middlename = "Test7"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "8",
                Schoolyear = "2018",
                Firstname = "Test8",
                Lastname = "Test8",
                Middlename = "Test8"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "9",
                Schoolyear = "2018",
                Firstname = "Test9",
                Lastname = "Test9",
                Middlename = "Test9"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "10",
                Schoolyear = "2018",
                Firstname = "Test10",
                Lastname = "Test10",
                Middlename = "Test10"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "11",
                Schoolyear = "2018",
                Firstname = "Test10",
                Lastname = "Test10",
                Middlename = "Test10"
            });
            _list.Add(new StudentEnroll
            {
                Schoolid = "1000",
                Grade = "12",
                Schoolyear = "2018",
                Firstname = "Test10",
                Lastname = "Test10",
                Middlename = "Test10"
            });
            _list.Add(new StudentEnroll
            {   
                Schoolid = "1000",
                Grade = "11111",
                Schoolyear = "2018",
                Firstname = "Test10",
                Lastname = "Test10",
                Middlename = "Test10"
            });

           

            ServiceEventSource.Current.ServiceRequestStop("StudentController.Get", activityId);
            return _list;
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
