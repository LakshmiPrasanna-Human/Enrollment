using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnrollmentStateFullService.Models
{
    public class StudentEnroll
    {
        private string schoolid;
        private string grade;
        private string schoolyear;
        private string firstname;
        private string lastname;
        private string middlename;
        public string Schoolid
        {
            get
            {
                return schoolid;
            }

            set
            {
                schoolid = value;
            }
        }

        public string Grade
        {
            get
            {
                return grade;
            }

            set
            {
                grade = value;
            }
        }

        public string Schoolyear
        {
            get
            {
                return schoolyear;
            }

            set
            {
                schoolyear = value;
            }
        }

        public string Firstname
        {
            get
            {
                return firstname;
            }

            set
            {
                firstname = value;
            }
        }

        public string Lastname
        {
            get
            {
                return lastname;
            }

            set
            {
                lastname = value;
            }
        }

        public string Middlename
        {
            get
            {
                return middlename;
            }

            set
            {
                middlename = value;
            }
        }
    }
}
