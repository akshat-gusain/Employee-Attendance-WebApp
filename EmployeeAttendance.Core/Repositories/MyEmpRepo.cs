using EmployeeAttendance.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace EmployeeAttendance.Core.Repositories
{
    public interface MyEmpRepo
    {
        Task<Employee> GetEmployeeById(int employeeId);
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task AddEmployee(Employee employee);
        Task UpdateEmployee(Employee employee);
        Task DeleteEmployee(int employeeId);
    }

    public interface IEmployeeAttendanceRepository
    {
        Task<IEnumerable<EmployeeAttendanceRecord>> GetAttendanceByEmployeeId(int employeeId);
        Task AddAttendance(EmployeeAttendanceRecord attendance);
        Task<EmployeeAttendanceRecord> GetAttendanceByEmployeeAndDate(int employeeId, DateTime attendanceDate); 
        Task<IEnumerable<EmployeeAttendanceRecord>> GetAllAttendanceRecords();
        Task UpdateAttendance(EmployeeAttendanceRecord attendanceRecord); 
        Task DeleteAttendance(int employeeId, DateTime attendanceDate);
        
    }
}