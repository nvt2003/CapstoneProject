using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.CostBreakdown;
using Planify_BackEnd.Models;

public class CostService : ICostService
{
    private readonly ICostRepository _costRepository;

    public CostService(ICostRepository costRepository)
    {
        _costRepository = costRepository;
    }

    public async Task<ResponseDTO> CreateCostAsync(CostBreakdownCreateDTO costDto)
    {
        try
        {
            if (costDto == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (costDto.EventId <= 0)
                return new ResponseDTO(400, "Event ID không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(costDto.Name))
                return new ResponseDTO(400, "Tên chi phí là bắt buộc", null);

            if (costDto.Quantity.HasValue && costDto.Quantity < 0)
                return new ResponseDTO(400, "Số lượng không thể âm", null);

            if (costDto.PriceByOne.HasValue && costDto.PriceByOne < 0)
                return new ResponseDTO(400, "Đơn giá không thể âm", null);

            var cost = new CostBreakdown
            {
                Name = costDto.Name,
                Quantity = costDto.Quantity,
                PriceByOne = costDto.PriceByOne,
                EventId = costDto.EventId
            };

            var createdCost = await _costRepository.CreateCostAsync(cost);
            return new ResponseDTO(201, "Create Cost Successfully", createdCost);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi tạo chi phí", null);
        }
    }

    public async Task<ResponseDTO> UpdateCostAsync(CostBreakdownUpdateDTO costDto)
    {
        try
        {
            if (costDto == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (costDto.Id <= 0)
                return new ResponseDTO(400, "Cost ID không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(costDto.Name))
                return new ResponseDTO(400, "Tên chi phí là bắt buộc", null);

            if (costDto.Quantity.HasValue && costDto.Quantity < 0)
                return new ResponseDTO(400, "Số lượng không thể âm", null);

            if (costDto.PriceByOne.HasValue && costDto.PriceByOne < 0)
                return new ResponseDTO(400, "Đơn giá không thể âm", null);

            var existingCost = await _costRepository.GetCostByIdAsync(costDto.Id);
            if (existingCost == null)
                return new ResponseDTO(404, $"Cost with ID {costDto.Id} not found", null);

            existingCost.Name = costDto.Name;
            existingCost.Quantity = costDto.Quantity;
            existingCost.PriceByOne = costDto.PriceByOne;

            var updatedCost = await _costRepository.UpdateCostAsync(existingCost);
            return new ResponseDTO(200, "Update Cost Successfully", updatedCost);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi cập nhật chi phí", null);
        }
    }

    public async Task<ResponseDTO> DeleteCostAsync(int id)
    {
        try
        {
            if (id <= 0)
                return new ResponseDTO(400, "Cost ID không hợp lệ", null);

            var existingCost = await _costRepository.GetCostByIdAsync(id);
            if (existingCost == null)
                return new ResponseDTO(404, $"Cost with ID {id} not found", null);

            await _costRepository.DeleteCostAsync(id);
            return new ResponseDTO(200, "Delete Cost Successfully", null);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Đã xảy ra lỗi khi xóa chi phí", null);
        }
    }
}