using Planify_BackEnd.Models;

public interface ICostRepository
{
    Task<CostBreakdown> CreateCostAsync(CostBreakdown cost);
    Task<CostBreakdown> UpdateCostAsync(CostBreakdown cost);
    System.Threading.Tasks.Task DeleteCostAsync(int id);
    Task<CostBreakdown> GetCostByIdAsync(int id);
}

