using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Tasks;

namespace Planify_BackEnd.Services.Tasks
{
    public interface ITaskService
    {
        Task<ResponseDTO> CreateTaskAsync(TaskCreateRequestDTO taskDTO, Guid organizerId);
        bool UpdateActualTaskAmount(int taskId, decimal amount);
        Task<List<TaskSearchResponeDTO>> SearchTaskOrderByStartDateAsync(int page, int pageSize, string? name, DateTime startDate, DateTime endDate);
        Task<ResponseDTO> UpdateTaskAsync(int taskId, TaskUpdateRequestDTO taskDTO);
        Task<ResponseDTO> DeleteTaskAsync(int taskId);
        PageResultDTO<TaskSearchResponeDTO> GetAllTasks(int eventId, int page, int pageSize);
        public TaskDetailVM GetTaskById(int taskId);
        Task<bool> changeStatus(int taskId, int status);
        Task<PageResultDTO<SubTaskResponseDTO>> SearchSubTaskByImplementerId(int page, int pageSize,Guid implementerId, DateTime startDate, DateTime endDate);
        Task<ResponseDTO> DeleteTaskV2(int taskId);
    }
}
