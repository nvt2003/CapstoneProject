using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Campus;

namespace Planify_BackEnd.Services.Campus
{
    public interface ICampusService
    {
       Task<IEnumerable<CampusDTO>> GetAllCampus();
        Task<CampusDTO> GetCampusByName(string campusName);
        Task<ResponseDTO> GetCampusById(int id);
        Task<ResponseDTO> CreateCampus(CampusDTO campusDTO);
        Task<ResponseDTO> UpdateCampus(CampusDTO campusDTO);
        Task<ResponseDTO> DeleteCampus(int id);
    }
}
