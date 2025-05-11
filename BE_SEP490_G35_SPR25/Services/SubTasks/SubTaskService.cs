using Azure.Core;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Services.Notification;
using System.Drawing;
using System.Threading.Tasks;

namespace Planify_BackEnd.Services.SubTasks
{
    public class SubTaskService : ISubTaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ISubTaskRepository _subTaskRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IJoinProjectRepository _joinProjectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        public SubTaskService(ISubTaskRepository subTaskRepository, IHttpContextAccessor httpContextAccessor, ITaskRepository taskRepository, IHubContext<NotificationHub> hubContext,IJoinProjectRepository joinProjectRepository,IUserRepository userRepository,IEmailSender emailSender)
        {
            _taskRepository = taskRepository;
            _subTaskRepository = subTaskRepository;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _joinProjectRepository = joinProjectRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
        }
        /// <summary>
        /// Create a new sub-task
        /// </summary>
        /// <param name="subTaskDTO"></param>
        /// <param name="implementerId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ResponseDTO> CreateSubTaskAsync(SubTaskCreateRequestDTO subTaskDTO, Guid implementerId)
        {
            try
            {
                if (!_taskRepository.IsTaskExists(subTaskDTO.TaskId))
                {
                    throw new ArgumentException("Task does not exists.");
                }
                if (string.IsNullOrWhiteSpace(subTaskDTO.SubTaskName))
                {
                    return new ResponseDTO(400, "Sub-task name is required.", null);
                }

                if (subTaskDTO.StartTime >= subTaskDTO.Deadline)
                {
                    return new ResponseDTO(400, "Start time must be earlier than deadline.", null);
                }
                var newSubTask = new SubTask
                {
                    TaskId = subTaskDTO.TaskId,
                    SubTaskName = subTaskDTO.SubTaskName,
                    SubTaskDescription = subTaskDTO.SubTaskDescription,
                    StartTime = subTaskDTO.StartTime,
                    Deadline = subTaskDTO.Deadline,
                    AmountBudget = subTaskDTO.AmountBudget,
                    Status = 0,
                    CreateBy = implementerId,
               
                };

                try
                {
                    await _subTaskRepository.CreateSubTaskAsync(newSubTask);
                }
                catch (Exception dbEx)
                {
                    return new ResponseDTO(500, "Database error while creating sub-task!", dbEx.Message);
                }

                return new ResponseDTO(201, "Sub-task creates successfully!", newSubTask);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error orcurs while creating sub-task!", ex.Message);
            }
        }

        public bool UpdateActualSubTaskAmount(int subTaskId, decimal amount)
        {
            try
            {
                return _subTaskRepository.UpdateActualSubTaskAmount(subTaskId, amount);
            }catch(Exception ex)
            {
                Console.WriteLine("subtask service - update actual subtask amount: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Get sub-task by ID
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> GetSubTaskByIdAsync(int subTaskId)
        {
            try
            {
                var subTask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskId);
                if (subTask == null)
                {
                    return new ResponseDTO(404, "Sub-task not found", null);
                }

                var subTaskResponse = new SubTaskResponseDTO
                {
                    Id = subTask.Id,
                    TaskId = subTask.TaskId,
                    SubTaskName = subTask.SubTaskName,
                    SubTaskDescription = subTask.SubTaskDescription,
                    StartTime = subTask.StartTime,
                    Deadline = subTask.Deadline,
                    AmountBudget = subTask.AmountBudget,
                    Status = subTask.Status,
                    CreateBy = subTask.CreateBy
                };

                return new ResponseDTO(200, "Sub-task retrieved successfully", subTaskResponse);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurred while retrieving sub-task", ex.Message);
            }
        }
        /// <summary>
        /// Update sub-task
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <param name="subTaskDTO"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> UpdateSubTaskAsync(int subTaskId, SubTaskUpdateRequestDTO subTaskDTO)
        {
            try
            {
                var existingSubTask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskId);
                if (existingSubTask == null)
                {
                    return new ResponseDTO(404, "Sub-task not found.", null);
                }

                existingSubTask.SubTaskName = subTaskDTO.SubTaskName;
                existingSubTask.SubTaskDescription = subTaskDTO.SubTaskDescription;
                existingSubTask.StartTime = subTaskDTO.StartTime;
                existingSubTask.Deadline = subTaskDTO.Deadline;
                existingSubTask.AmountBudget = subTaskDTO.AmountBudget;

                var updatedSubTask = await _subTaskRepository.UpdateSubTaskAsync(subTaskId, existingSubTask);

                return new ResponseDTO(200, "Sub-task updated successfully!", updatedSubTask);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while updating sub-task!", ex.Message);
            }
        }
        /// <summary>
        /// update sub-task status
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> UpdateSubTaskStatusAsync(int subTaskId, int newStatus)
        {
            try
            {
                var existingSubTask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskId);
                if (existingSubTask == null)
                {
                    return new ResponseDTO(404, "Sub-task not found.", null);
                }

                existingSubTask.Status = newStatus;

                var updatedSubTask = await _subTaskRepository.UpdateSubTaskAsync(subTaskId, existingSubTask);

                return new ResponseDTO(200, "Sub-task status updated successfully!", updatedSubTask);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while updating sub-task status!", ex.Message);
            }
        }

        /// <summary>
        /// Delete sub-task
        /// </summary>
        /// <param name="subTaskId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> DeleteSubTaskAsync(int subTaskId)
        {
            try
            {
                var isDeleted = await _subTaskRepository.DeleteSubTaskAsync(subTaskId);
                if (!isDeleted)
                {
                    return new ResponseDTO(404, "Sub-task not found.", null);
                }
                try
                {
                    //notification
                    var listJoinUserId = await _subTaskRepository.GetJoinedIdBySubTaskIdAsync(subTaskId);
                    var eventId = await _subTaskRepository.GetEventIdBySubtaskId(subTaskId);
                    var subtask = await _subTaskRepository.GetSubTaskByIdAsync(subTaskId);
                    var message = "Nhiệm vụ con " + subtask.SubTaskName + " đã bị xóa!";
                    var link = "https://fptu-planify.com/event-detail-EOG/" + eventId;
                    foreach (var id in listJoinUserId)
                    {
                        await _hubContext.Clients.User(id + "").SendAsync("ReceiveNotification",
                            message,
                            link);
                        var user = await _userRepository.GetUserByIdAsync(id);
                        //await _emailSender.SendEmailAsync(user.Email, "Một nhiệm vụ con đã bị xóa",
                        //    message+" Trong sự kiện "+subtask.Task.Event.EventTitle);
                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Một nhiệm vụ con đã bị xóa",
                            $@"
                            <!DOCTYPE html>
                            <html lang='vi'>
                            <head>
                              <meta charset='UTF-8'>
                              <title>Thông báo nhiệm vụ con bị xóa</title>
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
                                  margin: 0;
                                  color: #cc0000;
                                }}

                                .description {{
                                  font-size: 15px;
                                  color: #333;
                                  margin-top: 30px;
                                  margin-bottom: 20px;
                                  line-height: 1.6;
                                  max-width: 500px;
                                  margin-left: auto;
                                  margin-right: auto;
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

                                <h1>Nhiệm vụ con đã bị xóa</h1>

                                <p class='description'>
                                  Một nhiệm vụ con mà bạn đang tham gia đã bị xóa trong sự kiện <strong>{subtask.Task.Event.EventTitle}</strong>.<br/><br/>
                                  {message}
                                </p>

                                <div class='button'>
                                  <a href='https://fptu-planify.com/event-detail-EOG/{subtask.Task.EventId}'>Xem chi tiết sự kiện</a>
                                </div>

                                <br><br>
                                <p class='description'>Trân trọng, hệ thống tự động</p>
                              </div>
                            </body>
                            </html>"
                        );

                    }
                }
                catch{ }
                return new ResponseDTO(200, "Sub-task deleted successfully!", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while deleting sub-task!", ex.Message);
            }
        }
        /// <summary>
        /// Get sub-tasks by task ID
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<ResponseDTO> GetSubTasksByTaskIdAsync(int taskId)
        {
            try
            {
                var subTasks = await _subTaskRepository.GetSubTasksByTaskIdAsync(taskId);
                if (subTasks == null || !subTasks.Any())
                {
                    return new ResponseDTO(404, "No sub-tasks found for this task", null);
                }

                var subTaskResponses = subTasks.Select(subTask => new SubTaskResponseDTO
                {
                    Id = subTask.Id,
                    TaskId = subTask.TaskId,
                    SubTaskName = subTask.SubTaskName,
                    SubTaskDescription = subTask.SubTaskDescription,
                    StartTime = subTask.StartTime,
                    Deadline = subTask.Deadline,
                    AmountBudget = subTask.AmountBudget,
                    Status = subTask.Status,
                    CreateBy = subTask.CreateBy
                }).ToList();

                return new ResponseDTO(200, "Sub-tasks retrieved successfully", subTaskResponses);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurred while retrieving sub-tasks", ex.Message);
            }
        }

        public async Task<bool> AssignSubTask(Guid assignUserId, Guid userId, int subtaskId)
        {
            try
            {
                var subtask = await _subTaskRepository.GetSubTaskByIdAsync(subtaskId);
                if (subtask == null) throw new Exception("Not found subtask!");
                var task = _taskRepository.GetTaskById(subtask.TaskId);
                if (!task.CreateBy.Equals(assignUserId)) throw new Exception("Assign user must be create user!");

                var isInProject = await _joinProjectRepository.IsImplementerInProject(userId, (int)task.EventId);
                if (!isInProject)
                {
                    await _joinProjectRepository.AddImplementerToProject(userId, (int)task.EventId);
                }

                JoinTask newJoinTask = new JoinTask
                {
                    UserId = userId,
                    TaskId = subtaskId,
                    CreatedAt = DateTime.Now
                };
                var response = await _subTaskRepository.AssignSubTask(newJoinTask);
                try
                {
                //notification
                    var user = await _userRepository.GetUserByIdAsync(userId);
                    if (response)
                    {
                        await _hubContext.Clients.User(userId + "").SendAsync("ReceiveNotification",
                            "Bạn đã được thêm vào nhiệm vụ con "+ subtask.SubTaskName+ "!",
                            "https://fptu-planify.com/event-detail-EOG/" + task.EventId);
                        //await _emailSender.SendEmailAsync(user.Email,
                        //    "Nhiệm vụ mới", "Bạn đã được thêm vào nhiệm vụ con tên " + subtask.SubTaskName + ", " + 
                        //    "trong sự kiện "+ subtask.Task.Event.EventTitle);
                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Nhiệm vụ mới",
                            $@"
                            <!DOCTYPE html>
                            <html lang='vi'>
                            <head>
                              <meta charset='UTF-8'>
                              <title>Thông báo nhiệm vụ mới</title>
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
                                  margin: 0;
                                  color: #0066cc;
                                }}

                                .description {{
                                  font-size: 15px;
                                  color: #333;
                                  margin-top: 30px;
                                  margin-bottom: 20px;
                                  line-height: 1.6;
                                  max-width: 500px;
                                  margin-left: auto;
                                  margin-right: auto;
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

                                <h1>Bạn đã được thêm vào nhiệm vụ mới</h1>

                                <p class='description'>
                                  Bạn đã được thêm vào nhiệm vụ mới <strong>{subtask.SubTaskName}</strong> trong sự kiện <strong>{subtask.Task.Event.EventTitle}</strong>.<br/><br/>
                                  Vui lòng kiểm tra chi tiết và thực hiện nhiệm vụ đúng thời hạn được giao.
                                </p>

                                <div class='button'>
                                  <a href='https://fptu-planify.com/event-detail-EOG/{subtask.Task.EventId}'>Xem chi tiết sự kiện</a>
                                </div>

                                <br><br>
                                <p class='description'>Trân trọng, hệ thống tự động</p>
                              </div>
                            </body>
                            </html>"
                        );

                    }
                }
                catch { }
                return response;

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteAssignedUser(Guid userId, int subTaskId)
        {
            try
            {
                return await _subTaskRepository.DeleteAssignedUser(userId, subTaskId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<PageResultDTO<SubTaskResponseDTO>> SearchSubTaskByImplementerId(Guid implementerId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var resultTasks = await _subTaskRepository.SearchSubTaskByImplementerId(implementerId, startDate, endDate);
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
                    CreateBy = item.CreateBy,
                    TaskName = item.Task.TaskName,
                    EventTitle = item?.Task?.Event?.EventTitle
                }).ToList();
                return new PageResultDTO<SubTaskResponseDTO>(tastDtos, resultTasks.TotalCount, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
