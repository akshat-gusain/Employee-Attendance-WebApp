using EmployeeAttendance.Application.DTOs;
using EmployeeAttendance.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.IO;

namespace EmployeeAttendance.WebUI.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IEmployeeAttendanceService _attendanceService;
        private readonly MyEmpService _employeeService; 

        public AttendanceController(IEmployeeAttendanceService attendanceService, MyEmpService employeeService)
        {
            _attendanceService = attendanceService;
            _employeeService = employeeService;
        }

        // GET For Attendance
        public async Task<IActionResult> Index()
        {
            IEnumerable<EmpAttendanceDTO> attendanceRecords = await _attendanceService.GetAllAttendanceRecords();
            return View(attendanceRecords); 
        }

        public async Task<IActionResult> EmployeeAttendance(int employeeId)
        {
            var attendanceRecords = await _attendanceService.GetAttendanceByEmployeeId(employeeId);
            ViewBag.Employee = await _employeeService.GetEmployeeById(employeeId);
            return View(attendanceRecords);
        }


        // Show Form For Adding Attendance
        public async Task<IActionResult> AddAttendance(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            ViewBag.Employee = employee; 
            return View(new EmpAttendanceDTO { EmployeeId = employeeId });
        }

        // POST For Adding Attendance Record
        [HttpPost]
        public async Task<IActionResult> AddAttendance(EmpAttendanceDTO attendanceDto)
        {
            if (ModelState.IsValid)
            {
                var employee = await _employeeService.GetEmployeeById(attendanceDto.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError(string.Empty, "The specified Employee ID does not exist.");
                    var attendancesRecords = await _attendanceService.GetAllAttendanceRecords();
                    return View("Index", attendancesRecords);
                }

                if (attendanceDto.FirstName != employee.FirstName)
                {
                    ModelState.AddModelError("FirstName", "The provided name does not match the Employee ID.");
                }
                else
                {
                    attendanceDto.FirstName = employee.FirstName;

                    bool exceptionHandled = false; 

                    try
                    {
                        await _attendanceService.AddAttendance(attendanceDto);
                        TempData["SuccessMessage"] = "Attendance record added successfully.";
                        return RedirectToAction("Index");
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627 || ex.Number == 2601) // SQL Error codes for unique constraint violations
                        {
                            ModelState.AddModelError("", "An attendance record for this date already exists for the employee.");
                            exceptionHandled = true; 
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty,ex.Message);
                            exceptionHandled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!exceptionHandled)
                        {
                            ModelState.AddModelError(string.Empty,ex.Message);
                        }
                    }
                }
            }

            var attendanceRecords = await _attendanceService.GetAllAttendanceRecords();
            return View("Index", attendanceRecords);
        }



        public async Task<IActionResult> EditAttendance(int employeeId)
        {
            var attendanceRecords = await _attendanceService.GetAttendanceByEmployeeId(employeeId);
            var attendance = attendanceRecords.FirstOrDefault();

            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }


        [HttpPost]
        public async Task<IActionResult> EditAttendance(EmpAttendanceDTO attendanceDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _attendanceService.UpdateAttendance(attendanceDto);
                    TempData["SuccessMessage"] = "Attendance record updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    
                    ModelState.AddModelError("", ex.Message);
                }
                catch (SqlException ex) when (ex.Message.Contains("UQ_Employee_AttendanceDate"))
                {
                
                    ModelState.AddModelError("", "An attendance record for this employee on this date already exists.");
                }
                catch (Exception ex)
                {
                   
                    ModelState.AddModelError("", "An error occurred while updating the attendance record. Please try again.");
                }
            }

           
            return View(attendanceDto);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteAttendance(int employeeId, DateTime attendanceDate)
        {
            await _attendanceService.DeleteAttendance(employeeId, attendanceDate);
            TempData["SuccessMessage"] = "Attendance record deleted successfully.";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> ExportToExcel()
        {

            var attendanceRecords = await _attendanceService.GetAllAttendanceRecords();

            using (var package = new ExcelPackage())
            {

                var worksheet = package.Workbook.Worksheets.Add("Attendance Records");


                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "First Name";
                worksheet.Cells[1, 3].Value = "Attendance Date";
                worksheet.Cells[1, 4].Value = "Status";


                int row = 2;
                foreach (var record in attendanceRecords)
                {
                    worksheet.Cells[row, 1].Value = record.EmployeeId;
                    worksheet.Cells[row, 2].Value = record.FirstName;
                    worksheet.Cells[row, 3].Value = record.AttendanceDate.ToShortDateString();
                    worksheet.Cells[row, 4].Value = record.Status;
                    row++;
                }


                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = "AttendanceRecords.xlsx";
                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }



    }
}
