using EmployeeAttendance.Application.Services;
using EmployeeAttendance.Core.Repositories;
using EmployeeAttendance.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml; 

var builder = WebApplication.CreateBuilder(args);


ExcelPackage.LicenseContext = LicenseContext.NonCommercial; 


builder.Services.AddScoped<MyEmpRepo, EmpRepo>();
builder.Services.AddScoped<IEmployeeAttendanceRepository, EmpAttendanceRepo>();

// Registering the EmployeeService
builder.Services.AddScoped<MyEmpService, EmpService>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("AttCon");
    return new EmpService(provider.GetRequiredService<MyEmpRepo>(), connectionString);
});

// Registering the EmployeeAttendanceService
builder.Services.AddScoped<IEmployeeAttendanceService>(provider =>
{
    var attendanceRepository = provider.GetRequiredService<IEmployeeAttendanceRepository>();
    var connectionString = builder.Configuration.GetConnectionString("AttCon");
    return new EmpAttendanceService(attendanceRepository, connectionString);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint != null)
    {
        Console.WriteLine($"Endpoint: {endpoint.DisplayName}");
    }
    await next();
});

app.MapControllerRoute(
    name: "export",
    pattern: "Attendance/ExportToExcel",
    defaults: new { controller = "Attendance", action = "ExportToExcel" }
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
