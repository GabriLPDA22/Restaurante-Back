using CineAPI.Models;
using System.Collections.Generic;

namespace CineAPI.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        List<Admins> GetAll();
        Admins GetById(int id);  // Asegúrate de que está definido aquí
        void Add(Admins admin);
        bool Update(int id, Admins updatedAdmin);
        bool Delete(int id);
    }
}
