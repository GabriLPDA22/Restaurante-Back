using CineAPI.Models;
using CineAPI.Repositories;
using CineAPI.Repositories.Interfaces; // Add this using directive
using System.Collections.Generic;

namespace CineAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public List<Admins> GetAllAdmins() => _adminRepository.GetAll();

        public Admins GetAdminById(int id) => _adminRepository.GetById(id);

        public void CreateAdmin(Admins admin) => _adminRepository.Add(admin);

        public bool UpdateAdmin(int id, Admins updatedAdmin) => _adminRepository.Update(id, updatedAdmin);

        public bool DeleteAdmin(int id) => _adminRepository.Delete(id);
    }
}
