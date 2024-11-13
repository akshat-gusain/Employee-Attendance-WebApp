using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.Core.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; } 
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public string PAN { get; set; }
        public string Aadhaar { get; set; }
    }

    public class EmployeeAttendanceRecord
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } 
    }
}

