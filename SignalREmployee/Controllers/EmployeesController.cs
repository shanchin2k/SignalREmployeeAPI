using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalREmployee.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalREmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IDocumentDBRepository<Employee> Respository;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly string CollectionId;

        public EmployeesController(
            IDocumentDBRepository<Employee> Respository,
            IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _hubContext = hubContext;
            this.Respository = Respository;
            CollectionId = "Employee";            
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Employee>> Get()
        {
            return await Respository.GetItemsAsync(CollectionId);
        }

        [HttpGet("{id}/{cityname}")]
        public async Task<Employee> Get(string id, string cityname)
        {
            var employees = await Respository.GetItemsAsync(d => d.Id == id && d.Cityname == cityname, CollectionId);
            Employee employee = new Employee();
            foreach (var emp in employees)
            {
                employee = emp;
                break;
            }
            return employee;
        }

        [HttpPost]
        [Authorize]
        public async Task<bool> Post([FromBody]Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    employee.Id = null;
                    await Respository.CreateItemAsync(employee, CollectionId);
                    await _hubContext.Clients.All.BroadcastMessage();                    
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        [HttpPut]
        public async Task<bool> Put([FromBody]Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await Respository.UpdateItemAsync(employee.Id, employee, CollectionId);
                    await _hubContext.Clients.All.BroadcastMessage();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpDelete("{id}/{cityname}")]
        public async Task<bool> Delete(string id, string cityname)
        {
            try
            {
                await Respository.DeleteItemAsync(id, CollectionId, cityname);
                await _hubContext.Clients.All.BroadcastMessage();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}