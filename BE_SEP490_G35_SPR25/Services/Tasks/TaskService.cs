using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.Models;
//using Planify_BackEnd.Repositories.Groups;
using Planify_BackEnd.Repositories.Tasks;
using TaskModel = Planify_BackEnd.Models.Task;
using Planify_BackEnd.DTOs.Groups;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs.Events;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Hub;
using Microsoft.AspNetCore.Identity;
using Planify_BackEnd.Services.Notification;
namespace Planify_BackEnd.Services.Tasks
{
    public class TaskService : ITaskService
    {
        //private readonly IGroupRepository _groupRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        public TaskService(ITaskRepository taskRepository, IHttpContextAccessor httpContextAccessor,IHubContext<NotificationHub> hubContext,IEmailSender emailSender, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _emailSender = emailSender;
            _userRepository = userRepository;
        }
        public async Task<ResponseDTO> CreateTaskAsync(TaskCreateRequestDTO taskDTO, Guid organizerId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(taskDTO.TaskName))
                {
                    return new ResponseDTO(400, "Task name is required.", null);
                }
                
                if (taskDTO.StartTime >= taskDTO.Deadline)
                {
                    return new ResponseDTO(400, "Start time must be earlier than deadline.", null);
                }
                try
                {
                    var newTask = new TaskModel
                    {
                        EventId = taskDTO.EventId,
                        TaskName = taskDTO.TaskName,
                        TaskDescription = taskDTO.TaskDescription,
                        StartTime = taskDTO.StartTime,
                        Deadline = taskDTO.Deadline,
                        AmountBudget = taskDTO.AmountBudget,
                        Status = 1,
                        CreateBy = organizerId,
                        CreateDate = DateTime.Now
                    };
                    await _taskRepository.CreateTaskAsync(newTask);
                    return new ResponseDTO(201, "Task creates successfully!", newTask);
                }
                catch (Exception dbEx)
                {
                    return new ResponseDTO(500, "Database error while creating task!", dbEx.Message);
                }


               
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error orcurs while creating task!", ex.Message);
            }
        }
        public async Task<List<TaskSearchResponeDTO>> SearchTaskOrderByStartDateAsync(int page, int pageSize, string? name, DateTime startDate, DateTime endDate)
        {
            try
            {
                var tasks = await _taskRepository.SearchTaskOrderByStartDateAsync(page, pageSize, name, startDate, endDate);

                return tasks.Select(item => new TaskSearchResponeDTO
                {
                    Id = item.Id,
                    TaskName = item.TaskName,
                    TaskDescription = item.TaskDescription,
                    StartTime = item.StartTime,
                    Deadline = item.Deadline,
                    EventId = item.EventId,
                    EventName = item.Event.EventTitle,
                    AmountBudget = item.AmountBudget,
                    Status = item.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("task - SearchTaskOrderByStartDateAsync: " + ex.Message);
                return new List<TaskSearchResponeDTO>();
            }
        }

        public PageResultDTO<TaskSearchResponeDTO> GetAllTasks(int eventId, int page, int pageSize)
        {
            try
            {
                var tasks =  _taskRepository.GetAllTasks(eventId, page, pageSize);
                if (tasks.TotalCount == 0)

                    return new PageResultDTO<TaskSearchResponeDTO>(new List<TaskSearchResponeDTO>(), 0, page, pageSize);
                List<TaskSearchResponeDTO> taskList = new List<TaskSearchResponeDTO>();
                foreach (var item in tasks.Items)
                {
                    TaskSearchResponeDTO task = new TaskSearchResponeDTO
                    {
                        Id = item.Id,
                        TaskName = item.TaskName,
                        TaskDescription = item.TaskDescription,
                        StartTime = item.StartTime,
                        Deadline = item.Deadline,
                        EventId = item.EventId,
                        AmountBudget = item.AmountBudget,
                        Status = item.Status,
                    };

              
                taskList.Add(task);

                }
                    return new PageResultDTO<TaskSearchResponeDTO>(taskList, tasks.TotalCount, page, pageSize);
                }
            catch (Exception ex)
            {
                 Console.WriteLine(ex.ToString()); 
            throw;
            }
        }


        public async Task<ResponseDTO> UpdateTaskAsync(int taskId, TaskUpdateRequestDTO taskDTO)
        {
            try
            {
                var existingTask = await _taskRepository.UpdateTaskAsync(taskId, new TaskModel
                {
                    TaskName = taskDTO.TaskName,
                    TaskDescription = taskDTO.TaskDescription,
                    StartTime = taskDTO.StartTime,
                    Deadline = taskDTO.Deadline,
                    AmountBudget = taskDTO.AmountBudget,
                    EventId = taskDTO.EventId
                    
                });

                if (existingTask == null)
                {
                    return new ResponseDTO(404, "Task not found.", null);
                }

                return new ResponseDTO(200, "Task updated successfully!", existingTask);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while updating task!", ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteTaskAsync(int taskId)
        {
            try
            {
                var isDeleted = await _taskRepository.DeleteTaskAsync(taskId);
                if (!isDeleted)
                {
                    return new ResponseDTO(404, "Task not found or already deleted.", null);
                }

                return new ResponseDTO(200, "Task deleted successfully!", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while deleting task!", ex.Message);
            }
        }

       


        public bool UpdateActualTaskAmount(int taskId, decimal amount)
        {
            try
            {
                return _taskRepository.UpdateActualTaskAmount(taskId, amount);
            }catch(Exception ex)
            {
                Console.WriteLine("task - update actual task amount" + ex.Message);
                return false;
            }
        }

        public TaskDetailVM GetTaskById(int taskId)
        {
            try
            {
                var task = _taskRepository.GetTaskById(taskId);
                TaskDetailVM taskDetailVM = new TaskDetailVM
                {
                    Id = task.Id,
                    TaskName = task.TaskName,
                    TaskDescription = task.TaskDescription,
                    AmountBudget = task.AmountBudget,
                    CreateBy = task.CreateBy,
                    CreateByNavigation = task.CreateByNavigation == null ? new DTOs.Users.UserNameVM() : new DTOs.Users.UserNameVM
                    {
                        Id = task.CreateByNavigation.Id,
                        Email = task.CreateByNavigation.Email,
                        FirstName = task.CreateByNavigation.FirstName,
                        LastName = task.CreateByNavigation.LastName
                    },
                    CreateDate = task.CreateDate,
                    Deadline = task.Deadline,
                    EventId = task.EventId,
                    StartTime = task.StartTime,
                    Status = task.Status,
                    SubTasks = task.SubTasks == null ? new List<SubTaskVM>() : task.SubTasks.Select(st => new SubTaskVM
                    {
                        Id = st.Id,
                        SubTaskName = st.SubTaskName,
                        SubTaskDescription = st.SubTaskDescription,
                        AmountBudget = st.AmountBudget,
                        StartTime = st.StartTime,
                        Deadline = st.Deadline,
                        Status = st.Status,
                        TaskId = st.TaskId,
                        CreateBy = st.CreateBy,
                        CreateByNavigation = st.CreateByNavigation == null ? new DTOs.Users.UserNameVM() : new DTOs.Users.UserNameVM
                        {
                            Id = st.CreateByNavigation.Id,
                            Email = st.CreateByNavigation.Email,
                            FirstName = st.CreateByNavigation.FirstName,
                            LastName = st.CreateByNavigation.LastName
                        }
                    }).ToList(),
                };
                return taskDetailVM;
            }
            catch
            {
                return new TaskDetailVM();
            }
        }

        public async Task<bool> changeStatus(int taskId, int status)
        {
            try
            {
                return await _taskRepository.changeStatus(taskId, status);
            }catch
            {
                return false;
            }
        }

        public async Task<PageResultDTO<SubTaskResponseDTO>> SearchSubTaskByImplementerId(int page, int pageSize, Guid implementerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var resultTasks = await _taskRepository.SearchSubTaskByImplementerId(page,pageSize,implementerId,startDate, endDate);
                var tastDtos = resultTasks.Items.Select(item => new SubTaskResponseDTO
                {
                    Id = item.Id,
                    SubTaskName = item.SubTaskName,
                    SubTaskDescription = item.SubTaskDescription,
                    StartTime = item.StartTime,
                    Deadline = item.Deadline,
                    TaskId = item.TaskId,
                    AmountBudget = item.AmountBudget,
                    Status = item.Status,
                    CreateBy = item.CreateBy
                }).ToList();
                return new PageResultDTO<SubTaskResponseDTO>(tastDtos, resultTasks.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteTaskV2(int taskId)
        {
            try
            {
                var isDeleted = await _taskRepository.DeleteTaskV2(taskId);
                if (!isDeleted)
                {
                    return new ResponseDTO(404, "Task not found or already deleted.", null);
                }
                //notification
                var task = _taskRepository.GetTaskById(taskId);
                var listJoined = await _taskRepository.GetJoinedIdByTaskId(taskId);
                var message = "Nhiệm vụ " + task.TaskName + "đã bị xóa!";
                var link = "https://fptu-planify.com/event-detail-EOG/" + task.EventId;

                string htmlContent = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                      <meta charset='UTF-8'>
                      <title>Thông báo nhiệm vụ bị xóa</title>
                      <style>
                        body {{
                          margin: 0;
                          font-family: Arial, sans-serif;
                          background-color: #f7f7ff;
                          text-align: center;
                          padding: 40px 20px;
                        }}
                        .container {{
                          background-color: white;
                          max-width: 600px;
                          margin: auto;
                          padding: 40px 20px;
                          border-radius: 8px;
                        }}
                        .logo img {{
                          width: 140px;
                          margin-bottom: 40px;
                        }}
                        h1 {{
                          font-size: 26px;
                          font-weight: bold;
                          color: #cc0000;
                        }}
                        .description {{
                          font-size: 15px;
                          color: #333;
                          margin-top: 30px;
                          margin-bottom: 20px;
                          line-height: 1.6;
                        }}
                        .button {{
                          margin-top: 30px;
                        }}
                        .button a {{
                          background-color: #6666ff;
                          color: white;
                          text-decoration: none;
                          padding: 12px 28px;
                          border-radius: 25px;
                          font-size: 16px;
                          font-weight: bold;
                        }}
                      </style>
                    </head>
                    <body>
                      <div class='container'>
                        <div class='logo'>
                           
                        </div>

                        <h1>Nhiệm vụ đã bị xóa</h1>

                        <p class='description'>
                          {message}<br/><br/>
                          Nếu bạn có thắc mắc hoặc cần thêm thông tin, vui lòng liên hệ với ban tổ chức.
                        </p>

                        <div class='button'>
                          <a href='{link}'>Xem chi tiết sự kiện</a>
                        </div>

                        <br><br>
                        <p class='description'>Trân trọng, hệ thống tự động</p>
                      </div>
                    </body>
                    </html>";
                if (listJoined != null&&listJoined.Count>0)
                {
                    foreach (var id in listJoined)
                    {
                        await _hubContext.Clients.User(id + "").SendAsync("ReceiveNotification",
                            message,
                            link);
                        var user = await _userRepository.GetUserByIdAsync(id);
                        if (user != null)
                        {

                            await _emailSender.SendEmailAsync(
                                user.Email,
                                $"Thông báo: Nhiệm vụ \"{task.TaskName}\" đã bị xóa",
                                htmlContent
                            );
                        }
                    }
                }
                return new ResponseDTO(200, "Task deleted successfully!", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while deleting task!", ex.Message);
            }
        }

    }
}
