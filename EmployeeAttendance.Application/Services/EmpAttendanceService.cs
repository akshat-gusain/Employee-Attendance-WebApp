using EmployeeAttendance.Application.DTOs;
using EmployeeAttendance.Core.Entities;
using EmployeeAttendance.Core.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAttendance.Application.Services
{
    public class EmpAttendanceService : IEmployeeAttendanceService
    {
        private readonly IEmployeeAttendanceRepository _attendanceRepository;

        private readonly string _connectionString;

        public EmpAttendanceService(IEmployeeAttendanceRepository attendanceRepository, string connectionString)
        {
            _attendanceRepository = attendanceRepository;
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EmpAttendanceDTO>> GetAllAttendanceRecords()
        {
            
            var attendanceRecords = await _attendanceRepository.GetAllAttendanceRecords();

            var attendanceDTOs = attendanceRecords.Select(record => new EmpAttendanceDTO
            {
                EmployeeId = record.EmployeeId,
                FirstName=record.FirstName,
                AttendanceDate = record.AttendanceDate,
                Status = record.Status
            });

           
            return attendanceDTOs.OrderByDescending(record => record.AttendanceDate);
        }




        public async Task<IEnumerable<EmpAttendanceDTO>> GetAttendanceByEmployeeId(int employeeId)
        {
            
            var attendanceEntities = await _attendanceRepository.GetAttendanceByEmployeeId(employeeId);

            
            var attendanceRecords = attendanceEntities.Select(record => new EmpAttendanceDTO
            {
                EmployeeId = record.EmployeeId,
                FirstName = record.FirstName, 
                AttendanceDate = record.AttendanceDate,
                Status = record.Status
            });

            return attendanceRecords;
        }




        public async Task<EmployeeAttendanceRecord> GetAttendanceByEmployeeAndDate(int employeeId, DateTime attendanceDate)
        {
            EmployeeAttendanceRecord attendanceRecord = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                if (attendanceDate < new DateTime(1753, 1, 1) || attendanceDate > new DateTime(9999, 12, 31))
                {
                    throw new ArgumentOutOfRangeException(nameof(attendanceDate), "Date is out of SQL Server's supported range.");
                }


                var command = new SqlCommand("sp_GetAttendanceByEmployeeAndDate", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                command.Parameters.AddWithValue("@AttendanceDate", attendanceDate);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        
                        attendanceRecord = new EmployeeAttendanceRecord
                        {
                            EmployeeId = reader.GetInt32(0),
                            AttendanceDate = reader.GetDateTime(1),
                            Status = reader.GetString(2)
                        };
                    }
                }
            }

            return attendanceRecord;
        }


        public async Task AddAttendance(EmpAttendanceDTO attendanceDto)
        {
           
            var attendanceRecord = new EmployeeAttendanceRecord
            {
                EmployeeId = attendanceDto.EmployeeId,
                AttendanceDate = attendanceDto.AttendanceDate,
                Status = attendanceDto.Status
            };

            try
            {
                await _attendanceRepository.AddAttendance(attendanceRecord);
            }
            catch (Exception ex)
            {
               
                throw new Exception("An attendance record for this date already exists for the employee.");
            }
        }



       


        public async Task UpdateAttendance(EmpAttendanceDTO attendanceDto)
        {
            
            var existingRecord = await _attendanceRepository.GetAttendanceByEmployeeAndDate(attendanceDto.EmployeeId, attendanceDto.AttendanceDate);

            if (existingRecord == null)
            {
                throw new InvalidOperationException("Attendance record not found.");
            }

            existingRecord.Status = attendanceDto.Status; 

           
            await _attendanceRepository.UpdateAttendance(existingRecord);
        }


        public async Task DeleteAttendance(int employeeId, DateTime attendanceDate)
        {
            await _attendanceRepository.DeleteAttendance(employeeId, attendanceDate);
        }

    }
}
