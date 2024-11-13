using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.Application.DTOs
{
    public class EmpAttendanceDTO
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Attendance Date is required.")]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; } 

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }
    }

}
