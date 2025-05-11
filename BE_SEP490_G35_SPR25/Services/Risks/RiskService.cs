using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Risk;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Risk;

public class RiskService : IRiskService
{
    private readonly IRiskRepository _riskRepository;

    public RiskService(IRiskRepository riskRepository)
    {
        _riskRepository = riskRepository;
    }

    public async Task<ResponseDTO> CreateRiskAsync(RiskCreateDTO riskDto)
    {
        try
        {
            if (riskDto == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (riskDto.EventId <= 0)
                return new ResponseDTO(400, "Event ID không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(riskDto.Name))
                return new ResponseDTO(400, "Tên rủi ro là bắt buộc", null);

            var risk = new Risk
            {
                EventId = riskDto.EventId,
                Name = riskDto.Name,
                Reason = riskDto.Reason,
                Solution = riskDto.Solution,
                Description = riskDto.Description
            };

            var createdRisk = await _riskRepository.CreateRiskAsync(risk);
            return new ResponseDTO(201, "Create Risk Successfully", createdRisk);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi tạo rủi ro", null);
        }
    }

    public async Task<ResponseDTO> UpdateRiskAsync(RiskUpdateDTO riskDto)
    {
        try
        {
            if (riskDto == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (riskDto.Id <= 0)
                return new ResponseDTO(400, "Risk ID không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(riskDto.Name))
                return new ResponseDTO(400, "Tên rủi ro là bắt buộc", null);

            var existingRisk = await _riskRepository.GetRiskByIdAsync(riskDto.Id);
            if (existingRisk == null)
                return new ResponseDTO(404, $"Risk with ID {riskDto.Id} not found", null);

            existingRisk.Name = riskDto.Name;
            existingRisk.Reason = riskDto.Reason;
            existingRisk.Solution = riskDto.Solution;
            existingRisk.Description = riskDto.Description;

            var updatedRisk = await _riskRepository.UpdateRiskAsync(existingRisk);
            return new ResponseDTO(200, "Update Risk Successfully", updatedRisk);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi cập nhật rủi ro", null);
        }
    }

    public async Task<ResponseDTO> DeleteRiskAsync(int id)
    {
        try
        {
            if (id <= 0)
                return new ResponseDTO(400, "Risk ID không hợp lệ", null);

            var existingRisk = await _riskRepository.GetRiskByIdAsync(id);
            if (existingRisk == null)
                return new ResponseDTO(404, $"Risk with ID {id} not found", null);

            await _riskRepository.DeleteRiskAsync(id);
            return new ResponseDTO(200, "Delete Risk Successfully", null);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi xóa rủi ro", null);
        }
    }
}