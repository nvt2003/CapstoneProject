using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Planify_BackEnd.Services.Campus
{
    public class CampusService : ICampusService
    {
        private ICampusRepository _campusRepository;
        public CampusService(ICampusRepository campusRepository)
        {
            _campusRepository = campusRepository;
        }

        public async Task<ResponseDTO> CreateCampus(CampusDTO campusDTO)
        {
            try
            {
                
                var campus = await _campusRepository.GetCampusByName(campusDTO.CampusName);
                if (campus!=null && campus.Status == 0)
                {
                    campus.Status = 1;
                    var update = await _campusRepository.UpdateCampus(campus);
                    if (update)
                    {
                        return new ResponseDTO(200, "", campusDTO);
                    }
                    else
                    {
                        return new ResponseDTO(500, "Lỗi khi tạo cơ sở!", campusDTO);
                    }
                }
                if (campus!=null) return new ResponseDTO(400, "Cơ sở đã tồn tại!", campusDTO);
                var newCampus = new Models.Campus
                {
                    CampusName = campusDTO.CampusName,
                    Status = 1,
                };
                var response = await _campusRepository.CreateCampus(newCampus);
                if (response)
                {
                    return new ResponseDTO(200, "", campusDTO);
                }
                else
                {
                    return new ResponseDTO(500, "Lỗi khi tạo cơ sở!", campusDTO);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, ex.Message, null);
            }
        }

        public async Task<ResponseDTO> DeleteCampus(int id)
        {
            try
            {
                var response = await _campusRepository.DeleteCampus(id);
                if (response)
                {
                    return new ResponseDTO(200, "Xóa thành công!", null);
                }
                else
                {
                    return new ResponseDTO(400, "Xóa không thành công!", null);
                }
            }catch(Exception ex)
            {
                return new ResponseDTO(500, ex.Message,null);
            }
        }

        public async Task<IEnumerable<CampusDTO>> GetAllCampus()
        {
            var campuses = await _campusRepository.getAllCampus();
            var campusDTOs = campuses.Select(c => new CampusDTO
            {
                Id = c.Id,
                CampusName = c.CampusName,
                Status = c.Status
            }).ToList();
            return campusDTOs;
        }

        public async Task<ResponseDTO> GetCampusById(int id)
        {
            try
            {
                var campus = await _campusRepository.GetCampusById(id);
                if (campus == null) return new ResponseDTO(404, "Không tìm thấy cơ sở nào có id " + id, null);
                var campusDTO = new CampusDTO
                {
                    Id = campus.Id,
                    CampusName = campus.CampusName,
                    Status = campus.Status
                };
                return new ResponseDTO(200,"",campusDTO);
            }catch (Exception ex)
            {
                return new ResponseDTO(500, ex.Message, null);
            }
        }

        public async Task<CampusDTO> GetCampusByName(string campusName)
        {
            try{
                var campus = await _campusRepository.GetCampusByName(campusName);
                CampusDTO campusDTO = new CampusDTO
                {
                    Id = campus.Id,
                    CampusName = campus.CampusName,
                    Status = campus.Status
                };
                return campusDTO;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<ResponseDTO> UpdateCampus(CampusDTO campusDTO)
        {
            try
            {

                var campus = await _campusRepository.GetCampusByName(campusDTO.CampusName);
                if (campus != null) return new ResponseDTO(400, "Cơ sở đã tồn tại!", campusDTO);
                var newCampus = new Models.Campus
                {
                    Id = campusDTO.Id,
                    CampusName = campusDTO.CampusName,
                    Status = int.Parse(campusDTO.Status+""),
                };
                var response = await _campusRepository.UpdateCampus(newCampus);
                if (response)
                {
                    return new ResponseDTO(200, "", campusDTO);
                }
                else
                {
                    return new ResponseDTO(500, "Lỗi khi cập nhật cơ sở!", campusDTO);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, ex.Message, null);
            }
        }
    }
}
