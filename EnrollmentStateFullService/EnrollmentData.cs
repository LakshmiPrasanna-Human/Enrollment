using Bond;
using System;


namespace EnrollmentStateFullService
{// Defined as a Bond schema to assist with data versioning and serialization.
    // Using a structure and read only properties to make this an immutable entity.
    // This helps to ensure Reliable Collections are used properly.
    // See https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-      
    // work-with-reliable-collections
    [Bond.Schema]
    public struct EnrollmentData
    {
        // Each field is attributed with an id that is unique. The field definition is never changed.
        // This is the name of the vote, which will also be the key of the reliable dictionary.
        
        [Bond.Id(0)]
        public string Grade { get; set; }

        [Bond.Id(10)]
        public string SchoolYear { get; set; }
        [Bond.Id(20)]
        public string FirstName { get; set; }
        [Bond.Id(30)]
        public string LastName { get;  set; }


        // Enrollment constructor.
        public EnrollmentData(string Grade, string SchoolYear, string FirstName, string LastName)
        {
            this.Grade = Grade;
            this.SchoolYear = SchoolYear;
            this.FirstName = FirstName;
            this.LastName = LastName;
        }

        // Updates the Grade of a EnrollmentData structure returning a new instance.
        public EnrollmentData UpdateWith(int Grade)
        {
          
            return new EnrollmentData(this.Grade, SchoolYear, FirstName, LastName);
        }
    }

}
