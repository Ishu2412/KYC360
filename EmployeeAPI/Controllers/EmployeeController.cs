using EmployeeAPI.Data;
using EmployeeAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace EmployeeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly DataContext _context;

        public  EmployeeController(DataContext context)
        {
            _context = context; 
        }


        //Getting all Employees
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployee()
        {
            var employees = await _context.Employees.ToListAsync();

            return Ok(employees);
        }

        //Getting specific entitiy by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)   
        {
            var employee = await _context.Employees.FindAsync(id.ToString());
            
            if(employee == null)
            {
                return NotFound("Employee with this id is not present");
            }

            return Ok(employee);
        }

        //Creating new Employee
        [HttpPost]
        public async Task<ActionResult<List<Employee>>> AddEmployee(Employee emp)
        {
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return Ok(await _context.Employees.ToListAsync());
        }


        //Updating Employee
        [HttpPut]
        public async Task<ActionResult<List<Employee>>> UpdateEmployee(Employee emp)
        {
            var dbEmp = await _context.Employees.FindAsync(emp.Id.ToString());

            if (dbEmp == null)
            {
                return NotFound("Employee with this id is not present");
            }
            dbEmp.Addresses = emp.Addresses;
            dbEmp.Names = emp.Names;
            dbEmp.Gender = emp.Gender;
            dbEmp.Dates = emp.Dates;
            dbEmp.Deceased = emp.Deceased;

            await _context.SaveChangesAsync();

            return Ok(await _context.Employees.ToListAsync());
        }

        //Deleting Employee
        [HttpDelete]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            var emp = await _context.Employees.FindAsync(id.ToString());

            if (emp == null)
            {
                return NotFound("Employee with this id is not present");
            }

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return Ok(await _context.Employees.ToListAsync());
        }

        //Searching Entities
        [HttpGet("search")]
        public async Task<ActionResult<List<Employee>>> SearchEmployees([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                // If search parameter is not provided, return all employees
                var allEmployees = await _context.Employees.ToListAsync();
                return Ok(allEmployees);
            }

            var searchResults = await _context.Employees
            .Where(e =>
                e.Addresses.Any(a => EF.Functions.Like(a.Country, $"%{search}%")) ||
                e.Addresses.Any(a => EF.Functions.Like(a.AddressLine, $"%{search}%")) ||
                e.Names.Any(n => EF.Functions.Like(n.FirstName, $"%{search}%")) ||
                e.Names.Any(n => EF.Functions.Like(n.MiddleName, $"%{search}%")) ||
                e.Names.Any(n => EF.Functions.Like(n.Surname, $"%{search}%")))
            .ToListAsync();

            if (searchResults == null)
            {
                return NotFound("No matching employees found for the given search criteria.");
            }

            return Ok(searchResults);
        }


        //Advanced Filtering
        [HttpGet("filter")]
        public async Task<ActionResult<List<Employee>>> FilterEmployees(
        [FromQuery] string gender,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] List<string> countries)
        {
            var filteredResults = _context.Employees.AsQueryable();

            // Applying gender filter
            if (!string.IsNullOrWhiteSpace(gender))
            {
                filteredResults = filteredResults.Where(e => e.Gender == gender);
            }

            // Applying date range filter
            if (startDate.HasValue && endDate.HasValue)
            {
                filteredResults = filteredResults.Where(e =>
                    e.Dates.Any(d => d.DateValue >= startDate && d.DateValue <= endDate));
            }

            // Applying countries filter
            if (countries != null && countries.Any())
            {
                filteredResults = filteredResults.Where(e =>
                    e.Addresses.Any(a => countries.Contains(a.Country)));
            }

            var filteredEmployees = await filteredResults.ToListAsync();

            if (filteredEmployees == null)
            {
                return NotFound("No matching employees found for the given filter criteria.");
            }

            return Ok(filteredEmployees);
        }

        //Bonus Chalenge 1

        //Pagination and Sorting
        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetPagedAndSortedEmployees(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
        {
            var query = _context.Employees.AsQueryable();

            // Sorting using Dynamic Linq
            query = query.OrderBy(e => e.Gender); // Adjust the sorting as needed

            // Pagination
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedAndSortedResults = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCount = totalCount,
                Data = pagedAndSortedResults
            };

            return Ok(result);
        }


    }
}
    