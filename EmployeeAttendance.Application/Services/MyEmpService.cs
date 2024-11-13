using EmployeeAttendance.Application.DTOs;
using EmployeeAttendance.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.Application.Services
{
    public interface MyEmpService
    {
        Task<EmpDTO> GetEmployeeById(int employeeId);
        Task<IEnumerable<EmpDTO>> GetAllEmployees();
        Task AddEmployee(EmpDTO employeeDto);
        Task UpdateEmployee(EmpDTO employeeDto);
        Task DeleteEmployee(int employeeId);
    }
    public interface IEmployeeAttendanceService
    {
        Task<IEnumerable<EmpAttendanceDTO>> GetAttendanceByEmployeeId(int employeeId);
        
       
        Task AddAttendance(EmpAttendanceDTO attendanceDto);
        Task<IEnumerable<EmpAttendanceDTO>> GetAllAttendanceRecords();
        Task DeleteAttendance(int employeeId, DateTime attendanceDate);
        Task UpdateAttendance(EmpAttendanceDTO attendanceDto);
    }
}
