using Microsoft.AspNetCore.Mvc;
using EmployeeAttendance.Application.DTOs;
using EmployeeAttendance.Application.Services;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace EmployeeAttendance.WebUI.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly MyEmpService _employeeService;

        public EmployeeController(MyEmpService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployees();
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpDTO employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.AddEmployee(employeeDto);
                TempData["SuccessMessage"] = "Employee added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }
        
        public async Task<IActionResult> Edit(int employeeId) 
        {
            if (employeeId <= 0) 
            {
                return BadRequest("Invalid employee ID.");
            }

            var employee = await _employeeService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmpDTO employeeDto)
        {
            if (employeeDto == null || employeeDto.EmployeeId == 0)
            {
                return BadRequest("Invalid Employee ID.");
            }

            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployee(employeeDto);
                TempData["SuccessMessage"] = "Employee updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(employeeDto);
        }


        public async Task<IActionResult> Delete(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeById(employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int employeeId)
        {
            await _employeeService.DeleteEmployee(employeeId);
            TempData["SuccessMessage"] = "Employee deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
       
            var employeeRecords = await _employeeService.GetAllEmployees();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employee Records");

                
                worksheet.Cells[1, 1].Value = "Employee ID";
                worksheet.Cells[1, 2].Value = "Employee Code";
                worksheet.Cells[1, 3].Value = "First Name";
                worksheet.Cells[1, 4].Value = "Last Name";
                worksheet.Cells[1, 5].Value = "Date of Birth";
                worksheet.Cells[1, 6].Value = "PAN";
                worksheet.Cells[1, 7].Value = "Aadhar";

                int row = 2; 
                foreach (var employee in employeeRecords)
                {
                    worksheet.Cells[row, 1].Value = employee.EmployeeId;
                    worksheet.Cells[row, 2].Value = employee.EmployeeCode;
                    worksheet.Cells[row, 3].Value = employee.FirstName;
                    worksheet.Cells[row, 4].Value = employee.LastName;
                    worksheet.Cells[row, 5].Value = employee.DOB.ToShortDateString();
                    worksheet.Cells[row, 6].Value = employee.PAN;
                    worksheet.Cells[row, 7].Value = employee.Adhaar;
                    row++;
                }

         
                var stream = new MemoryStream();
                package.SaveAs(stream);
                var fileName = "EmployeeRecords.xlsx";
                stream.Position = 0; 
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


    }
}
