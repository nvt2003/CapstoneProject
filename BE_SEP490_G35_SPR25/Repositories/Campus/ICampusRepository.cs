using Planify_BackEnd.Models;

public interface ICampusRepository
    {
         Task<IEnumerable<Campus>> getAllCampus();
        Task<Campus> GetCampusByName(string campusName);
        Task<Campus?> GetCampusById(int id);
        Task<bool> CreateCampus(Campus campus);
        Task<bool> UpdateCampus(Campus campus);
        Task<bool> DeleteCampus(int id);

}

