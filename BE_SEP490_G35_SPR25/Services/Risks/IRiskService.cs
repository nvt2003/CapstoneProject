using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Risk;

public interface IRiskService
{
    Task<ResponseDTO> CreateRiskAsync(RiskCreateDTO riskDto);
    Task<ResponseDTO> UpdateRiskAsync(RiskUpdateDTO riskDto);
    Task<ResponseDTO> DeleteRiskAsync(int id);
}