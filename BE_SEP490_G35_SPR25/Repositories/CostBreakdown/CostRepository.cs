using Planify_BackEnd.Models;

public class CostRepository : ICostRepository
{
    private readonly PlanifyContext _context;

    public CostRepository(PlanifyContext context)
    {
        _context = context;
    }

    public async Task<CostBreakdown> CreateCostAsync(CostBreakdown cost)
    {
        _context.CostBreakdowns.Add(cost);
        await _context.SaveChangesAsync();
        return cost;
    }

    public async Task<CostBreakdown> UpdateCostAsync(CostBreakdown cost)
    {
        _context.CostBreakdowns.Update(cost);
        await _context.SaveChangesAsync();
        return cost;
    }

    public async System.Threading.Tasks.Task DeleteCostAsync(int id)
    {
        var cost = await _context.CostBreakdowns.FindAsync(id);
        if (cost != null)
        {
            _context.CostBreakdowns.Remove(cost);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<CostBreakdown> GetCostByIdAsync(int id)
    {
        return await _context.CostBreakdowns.FindAsync(id);
    }
}