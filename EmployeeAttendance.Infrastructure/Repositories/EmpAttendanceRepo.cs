using EmployeeAttendance.Core.Entities;
using EmployeeAttendance.Core.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeeAttendance.Application.DTOs;

namespace EmployeeAttendance.Infrastructure.Repositories
{
    public class EmpAttendanceRepo : IEmployeeAttendanceRepository
    {
        private readonly string _connectionString;

        public EmpAttendanceRepo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AttCon");
        }

        public interface IEmployeeAttendanceRepository
        {
            Task<IEnumerable<EmployeeAttendanceRecord>> GetAllAttendanceRecords();
        }

        public async Task<IEnumerable<EmployeeAttendanceRecord>> GetAttendanceByEmployeeId(int employeeId)
        {
            var attendanceRecords = new List<EmployeeAttendanceRecord>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("sp_GetAttendanceByEmployeeId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        attendanceRecords.Add(new EmployeeAttendanceRecord
                        {
                            EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")), 
                            AttendanceDate = reader.GetDateTime(reader.GetOrdinal("AttendanceDate")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        });
                    }
                }
            }
            return attendanceRecords;
        }



        public async Task<IEnumerable<EmployeeAttendanceRecord>> GetAllAttendanceRecords()
        {
            var attendanceRecords = new List<EmployeeAttendanceRecord>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_GetAllEmployeeAttendanceWithNames", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                          
                            var attendanceRecord = new EmployeeAttendanceRecord
                            {
                                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                AttendanceDate = reader.GetDateTime(reader.GetOrdinal("AttendanceDate")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };

                            attendanceRecords.Add(attendanceRecord);
                        }
                    }
                }
            }

            return attendanceRecords;
        }


        public async Task<EmployeeAttendanceRecord> GetAttendanceByEmployeeAndDate(int employeeId, DateTime attendanceDate)
        {
            EmployeeAttendanceRecord attendanceRecord = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("sp_GetAttendanceByEmployeeAndDate", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new SqlParameter("@EmployeeId", employeeId));
                    command.Parameters.Add(new SqlParameter("@AttendanceDate", attendanceDate));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            attendanceRecord = new EmployeeAttendanceRecord
                            {
                                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                AttendanceDate = reader.GetDateTime(reader.GetOrdinal("AttendanceDate")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                        }
                    }
                }
            }

            return attendanceRecord;
        }
        public async Task AddAttendance(EmployeeAttendanceRecord attendance)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand("sp_AddAttendance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@EmployeeId", attendance.EmployeeId);
                    command.Parameters.AddWithValue("@AttendanceDate", attendance.AttendanceDate);
                    command.Parameters.AddWithValue("@Status", attendance.Status);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAttendance(EmployeeAttendanceRecord attendanceRecord)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("sp_UpdateAttendanceRecord", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@EmployeeId", attendanceRecord.EmployeeId);
                command.Parameters.AddWithValue("@AttendanceDate", attendanceRecord.AttendanceDate);
                command.Parameters.AddWithValue("@Status", attendanceRecord.Status);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAttendance(int employeeId, DateTime attendanceDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("sp_DeleteAttendance", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);
                    command.Parameters.AddWithValue("@AttendanceDate", attendanceDate);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


    }
}
