using System.Threading.Tasks;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories
{
    public interface ISubTaskRepository
    {
        Task<SubTask> CreateSubTaskAsync(SubTask newSubTask);
        bool UpdateActualSubTaskAmount(int subTaskId, decimal amount);
        Task<SubTask?> UpdateSubTaskAsync(int subTaskId, SubTask updatedSubTask);
        Task<bool> UpdateSubTaskStatusAsync(int subTaskId, int newStatus);
        Task<bool> DeleteSubTaskAsync(int subTaskId);
        Task<SubTask?> GetSubTaskByIdAsync(int subTaskId);
        Task<List<SubTask>> GetSubTasksByTaskIdAsync(int taskId);
        Task<bool> AssignSubTask(JoinTask joinTask);
        Task<List<Guid>> GetJoinedIdBySubTaskIdAsync(int subtaskId);
        Task<int> GetEventIdBySubtaskId(int subtaskId);
        Task<bool> DeleteAssignedUser(Guid userId, int subTaskId);
        Task<PageResultDTO<SubTask>> SearchSubTaskByImplementerId(Guid implementerId, DateTime startDate, DateTime endDate);
    }
}
