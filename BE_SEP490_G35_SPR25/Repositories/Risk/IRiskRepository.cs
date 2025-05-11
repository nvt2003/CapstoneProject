using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Risk
{
    public interface IRiskRepository
    {
        Task<Models.Risk> CreateRiskAsync(Models.Risk risk);
        Task<Models.Risk> UpdateRiskAsync(Models.Risk risk);
        System.Threading.Tasks.Task DeleteRiskAsync(int id);
        Task<Models.Risk> GetRiskByIdAsync(int id);
    }
}
