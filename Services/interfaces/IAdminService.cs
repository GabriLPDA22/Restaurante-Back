using CineAPI.Models;
using System.Collections.Generic;

namespace CineAPI.Services
{
    public interface IAdminService
    {
        List<Admins> GetAllAdmins();
        Admins GetAdminById(int id);
        void CreateAdmin(Admins admin);
        bool UpdateAdmin(int id, Admins updatedAdmin);
        bool DeleteAdmin(int id);
    }
}
