using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;
using TaskModel = Planify_BackEnd.Models.Task;
namespace Planify_BackEnd.Repositories.Tasks
{
    public interface ITaskRepository
    {
        Task<TaskModel> CreateTaskAsync(TaskModel newTask);
        Task<List<Models.Task>> SearchTaskOrderByStartDateAsync(int page, int pageSize, string? name, DateTime startDate, DateTime endDate);

        Task<Models.Task?> UpdateTaskAsync(int taskId, Models.Task updatedTask);
        Task<bool> DeleteTaskAsync(int taskId);
        PageResultDTO<Models.Task> GetAllTasks(int eventId, int page, int pageSize);

        bool IsTaskExists(int taskId);
        bool UpdateActualTaskAmount(int taskId, decimal amount);
        public Models.Task GetTaskById(int taskId);
        public Task<bool> changeStatus(int taskId, int status);
        Task<PageResultDTO<SubTask>> SearchSubTaskByImplementerId(int page, int pageSize, Guid implementerId, DateTime startDate, DateTime endDate);
        Task<bool> DeleteTaskV2(int taskId);
        Task<List<Guid>> GetJoinedIdByTaskId(int taskId);

    }
}
