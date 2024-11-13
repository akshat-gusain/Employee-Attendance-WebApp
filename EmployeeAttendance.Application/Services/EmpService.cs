using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAttendance.Application.DTOs;
using EmployeeAttendance.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Data.SqlClient;
using EmployeeAttendance.Core.Entities;


namespace EmployeeAttendance.Application.Services
{
    public class EmpService : MyEmpService
    {
        private readonly MyEmpRepo _employeeRepository;
        private readonly string _connectionString;
        public EmpService(MyEmpRepo employeeRepository, string connectionString)
        {
            _employeeRepository = employeeRepository;
            _connectionString = connectionString; 
        }

        public async Task<EmpDTO> GetEmployeeById(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);

            
            if (employee == null)
            {
                
                return null;
            }

            return new EmpDTO
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                DOB = employee.DOB,
                PAN = employee.PAN,
                Adhaar = employee.Aadhaar
            };
        }


        public async Task<IEnumerable<EmpDTO>> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetAllEmployees();
            var employeeDtos = new List<EmpDTO>();

            foreach (var employee in employees)
            {
                employeeDtos.Add(new EmpDTO
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeCode = employee.EmployeeCode,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    DOB = employee.DOB,
                    PAN = employee.PAN,
                    Adhaar = employee.Aadhaar
                });
            }

            return employeeDtos;
        }

        public async Task AddEmployee(EmpDTO employeeDto)
        {
            var employee = new EmployeeAttendance.Core.Entities.Employee
            {
                EmployeeId = employeeDto.EmployeeId,
                EmployeeCode = employeeDto.EmployeeCode,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                DOB = employeeDto.DOB,
                PAN = employeeDto.PAN,
                Aadhaar = employeeDto.Adhaar
            };

            await _employeeRepository.AddEmployee(employee);
        }

        public async Task UpdateEmployee(EmpDTO employeeDto)
        {
            
            var employee = await _employeeRepository.GetEmployeeById(employeeDto.EmployeeId);
            if (employee == null)
            {
                throw new Exception($"Employee with ID {employeeDto.EmployeeId} not found.");
            }

            employee.EmployeeCode = employeeDto.EmployeeCode;
            employee.FirstName = employeeDto.FirstName;
            employee.LastName = employeeDto.LastName;
            employee.DOB = employeeDto.DOB;
            employee.PAN = employeeDto.PAN;
            employee.Aadhaar = employeeDto.Adhaar;

            await _employeeRepository.UpdateEmployee(employee);
        }

        public async Task DeleteEmployee(int employeeId)
        {
            if (employeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID");
            }

            await _employeeRepository.DeleteEmployee(employeeId);
        }
    }
}
