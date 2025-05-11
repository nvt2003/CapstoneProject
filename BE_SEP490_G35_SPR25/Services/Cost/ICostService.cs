using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.CostBreakdown;

public interface ICostService
{
    Task<ResponseDTO> CreateCostAsync(CostBreakdownCreateDTO costDto);
    Task<ResponseDTO> UpdateCostAsync(CostBreakdownUpdateDTO costDto);
    Task<ResponseDTO> DeleteCostAsync(int id);
}