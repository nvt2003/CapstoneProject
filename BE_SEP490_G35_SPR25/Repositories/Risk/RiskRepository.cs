using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Risk
{
    public class RiskRepository : IRiskRepository
    {
        private readonly PlanifyContext _context;

        public RiskRepository(PlanifyContext context)
        {
            _context = context;
        }

        public async Task<Models.Risk> CreateRiskAsync(Models.Risk risk)
        {
            _context.Risks.Add(risk);
            await _context.SaveChangesAsync();
            return risk;
        }

        public async Task<Models.Risk> UpdateRiskAsync(Models.Risk risk)
        {
            _context.Risks.Update(risk);
            await _context.SaveChangesAsync();
            return risk;
        }

        public async System.Threading.Tasks.Task DeleteRiskAsync(int id)
        {
            var risk = await _context.Risks.FindAsync(id);
            if (risk != null)
            {
                _context.Risks.Remove(risk);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Models.Risk> GetRiskByIdAsync(int id)
        {
            return await _context.Risks.FindAsync(id);
        }
    }
}
