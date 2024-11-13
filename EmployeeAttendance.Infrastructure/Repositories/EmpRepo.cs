using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAttendance.Core.Entities;
using EmployeeAttendance.Core.Repositories;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EmployeeAttendance.Infrastructure.Repositories
{
    public class EmpRepo : MyEmpRepo
    {
        private readonly string _connectionString;

        public EmpRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AttCon");
        }

        public async Task<Employee> GetEmployeeById(int employeeId)
        {
            Employee employee = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetEmployeeById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        employee = new Employee
                        {
                            EmployeeId = reader.GetInt32(0),
                            EmployeeCode = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            DOB = reader.GetDateTime(4),
                            PAN = reader.GetString(5),
                            Aadhaar = reader.GetString(6)
                        };
                    }
                }
            }
            return employee;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetAllEmployees", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            EmployeeId = reader.GetInt32(0),
                            EmployeeCode = reader.GetString(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3),
                            DOB = reader.GetDateTime(4),
                            PAN = reader.GetString(5),
                            Aadhaar = reader.GetString(6)
                        });
                    }
                }
            }
            return employees;
        }

        public async Task AddEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_AddEmployee", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@DOB", employee.DOB);
                command.Parameters.AddWithValue("@PAN", employee.PAN);
                command.Parameters.AddWithValue("@Adhaar", employee.Aadhaar);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_UpdateEmployee", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                command.Parameters.AddWithValue("@EmployeeCode", employee.EmployeeCode);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                command.Parameters.AddWithValue("@DOB", employee.DOB);
                command.Parameters.AddWithValue("@PAN", employee.PAN);
                command.Parameters.AddWithValue("@Adhaar", employee.Aadhaar);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteEmployee(int employeeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_DeleteEmployee", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
