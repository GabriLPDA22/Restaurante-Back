using Microsoft.AspNetCore.Mvc;
using CineAPI.Models;
using CineAPI.Services;

namespace CineAPI.Controllers
{
    [Route("api/admins")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminsController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_adminService.GetAllAdmins());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var admin = _adminService.GetAdminById(id);
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Admins admin)
        {
            admin.Id = 0; 
            _adminService.CreateAdmin(admin);
            return CreatedAtAction(nameof(GetById), new { id = admin.Id }, admin);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Admins admin)
        {
            if (_adminService.UpdateAdmin(id, admin)) return NoContent();
            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_adminService.DeleteAdmin(id)) return NoContent();
            return NotFound();
        }
    }
    
}
