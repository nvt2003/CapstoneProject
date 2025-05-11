using Microsoft.AspNetCore.Http.HttpResults;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.Events;
using Planify_BackEnd.Services.GoogleDrive;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Categories;
using Planify_BackEnd.Repositories.Tasks;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs.Medias;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Hub;
using Google.Apis.Drive.v3.Data;
using System.Net.NetworkInformation;
using System.Numerics;
using Planify_BackEnd.Services.Notification;
using Microsoft.AspNetCore.Identity;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITaskRepository _taskRepository;
    private readonly ISubTaskRepository _subTaskRepository;
    private readonly GoogleDriveService _googleDriveService;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IJoinProjectRepository _joinProjectRepository;
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    public EventService(IEventRepository eventRepository, IHttpContextAccessor httpContextAccessor, GoogleDriveService googleDriveService, ISubTaskRepository subTaskRepository, ITaskRepository taskRepository,IHubContext<NotificationHub> hubContext, IJoinProjectRepository joinProjectRepository,IEmailSender emailSender,IUserRepository userRepository)
    {
        _eventRepository = eventRepository;
        _httpContextAccessor = httpContextAccessor;
        _googleDriveService = googleDriveService;
        _subTaskRepository = subTaskRepository;
        _taskRepository = taskRepository;
        _hubContext = hubContext;
        _joinProjectRepository = joinProjectRepository;
        _emailSender = emailSender;
        _userRepository = userRepository;
    }
    /// <summary>
    /// get all event by campusId and paging
    /// </summary>
    /// <param name="campusId"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public PageResultDTO<EventGetListResponseDTO> GetAllEvent(int campusId, int page, int pageSize, Guid userId)
    {
        try
        {
            PageResultDTO<Event> events = _eventRepository.GetAllEvent(campusId, page, pageSize, userId);
            if (events.TotalCount == 0)

                return new PageResultDTO<EventGetListResponseDTO>(new List<EventGetListResponseDTO>(), 0, page, pageSize);
            List<EventGetListResponseDTO> eventList = new List<EventGetListResponseDTO>();
            foreach (var item in events.Items)
            {
                EventGetListResponseDTO eventDTO = new EventGetListResponseDTO
                {
                    Id = item.Id,
                    EventTitle = item.EventTitle,
                    EventDescription = item.EventDescription,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    AmountBudget = item.AmountBudget,
                    IsPublic = item.IsPublic,
                    TimePublic = item.TimePublic,
                    Status = item.Status,
                    CampusId = item.CampusId,
                    CategoryEventId = item.CategoryEventId,
                    Placed = item.Placed,
                    CreateBy = item.CreateBy,
                    CreatedAt = item.CreatedAt,
                    ManagerId = item.ManagerId,
                    MeasuringSuccess = item.MeasuringSuccess,
                    Goals = item.Goals,
                    MonitoringProcess = item.MonitoringProcess,
                    SizeParticipants = item.SizeParticipants,
                    EventMedias = item.EventMedia == null ? null : item.EventMedia.Select(em => new Planify_BackEnd.DTOs.Medias.EventMediumViewMediaModel
                    {
                        Id = em.Id,
                        EventId = em.Id,
                        MediaId = em.Id,
                        Status = em.Status,
                        MediaDTO = em.Media == null ? null : new Planify_BackEnd.DTOs.Medias.MediaItemDTO
                        {
                            Id = em.Media.Id,
                            MediaUrl = em.Media.MediaUrl
                        }
                    }).ToList(),
                    isFavorite = item.FavouriteEvents.Count!=0
                };
                eventList.Add(eventDTO);
                
            }
            return new PageResultDTO<EventGetListResponseDTO>(eventList, events.TotalCount, page, pageSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString()); 
            throw;
        }
    }

    public async Task<ResponseDTO> CreateEventAsync(EventCreateRequestDTO eventDTO, Guid organizerId)
    {
        using var transaction = await _eventRepository.BeginTransactionAsync();
        try
        {
            if (eventDTO == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(eventDTO.EventTitle))
                return new ResponseDTO(400, "Tên sự kiện là bắt buộc.", null);

            if (eventDTO.StartTime < DateTime.Now.AddMonths(2))
                return new ResponseDTO(400, "Thời gian bắt đầu phải cách thời gian hiện tại ít nhất 2 tháng.", null);

            if (eventDTO.StartTime >= eventDTO.EndTime)
                return new ResponseDTO(400, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc.", null);

            if (eventDTO.SizeParticipants < 0)
                return new ResponseDTO(400, "Số lượng người tham gia không thể âm.", null);

            var campusIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("campusId")?.Value;
            if (string.IsNullOrEmpty(campusIdClaim) || !int.TryParse(campusIdClaim, out int campusId))
                return new ResponseDTO(400, "ID campus không hợp lệ.", null);

            var category = await _eventRepository.GetCategoryEventAsync(eventDTO.CategoryEventId, campusId);
            if (category == null)
                return new ResponseDTO(400, "Danh mục không tồn tại hoặc không thuộc campus này.", null);

            decimal totalBudget = 0;
            if (eventDTO.CostBreakdowns != null && eventDTO.CostBreakdowns.Any())
            {
                foreach (var costBreakdown in eventDTO.CostBreakdowns)
                {
                    if (costBreakdown.Quantity.HasValue && costBreakdown.PriceByOne.HasValue)
                    {
                        totalBudget += costBreakdown.Quantity.Value * costBreakdown.PriceByOne.Value;
                    }
                }
            }

            // Tạo sự kiện mới
            var newEvent = new Event
            {
                EventTitle = eventDTO.EventTitle,
                EventDescription = eventDTO.EventDescription,
                StartTime = eventDTO.StartTime,
                EndTime = eventDTO.EndTime,
                AmountBudget = totalBudget,
                IsPublic = 0,
                TimePublic = null,
                Status = 0,
                CampusId = campusId,
                CategoryEventId = eventDTO.CategoryEventId,
                Placed = eventDTO.Placed,
                CreateBy = organizerId,
                CreatedAt = DateTime.Now,
                MeasuringSuccess = eventDTO.MeasuringSuccess,
                Goals = eventDTO.Goals,
                MonitoringProcess = eventDTO.MonitoringProcess,
                SizeParticipants = eventDTO.SizeParticipants,
                PromotionalPlan = eventDTO.PromotionalPlan,
                TargetAudience = eventDTO.TargetAudience,
                SloganEvent = eventDTO.SloganEvent
            };

            await _eventRepository.CreateEventAsync(newEvent);

            // Tạo các Task và SubTask
            if (eventDTO.Tasks != null && eventDTO.Tasks.Any())
            {
                foreach (var task in eventDTO.Tasks)
                {
                    if (string.IsNullOrWhiteSpace(task.TaskName))
                        return new ResponseDTO(400, "Tên task là bắt buộc.", null);

                    if (task.Deadline.HasValue && task.Deadline <= DateTime.Now)
                        return new ResponseDTO(400, $"Hạn chót của task '{task.TaskName}' phải sau thời gian hiện tại.", null);

                    if (task.Budget < 0)
                        return new ResponseDTO(400, $"Ngân sách của task '{task.TaskName}' không thể âm.", null);

                    var newTask = new Planify_BackEnd.Models.Task
                    {
                        EventId = newEvent.Id,
                        TaskName = task.TaskName,
                        TaskDescription = task.Description,
                        StartTime = task.StartTime ?? newEvent.StartTime,
                        Deadline = task.Deadline,
                        AmountBudget = task.Budget,
                        CreateBy = organizerId,
                        CreateDate = DateTime.Now,
                        Status = 0
                    };
                    await _taskRepository.CreateTaskAsync(newTask);

                    if (task.SubTasks != null && task.SubTasks.Any())
                    {
                        foreach (var subTask in task.SubTasks)
                        {
                            if (string.IsNullOrWhiteSpace(subTask.SubTaskName))
                                return new ResponseDTO(400, "Tên subtask là bắt buộc.", null);

                            if (subTask.StartTime.HasValue && subTask.Deadline.HasValue && subTask.StartTime >= subTask.Deadline)
                                return new ResponseDTO(400, $"Thời gian bắt đầu của subtask '{subTask.SubTaskName}' phải sớm hơn hạn chót.", null);

                            if (subTask.Budget < 0)
                                return new ResponseDTO(400, $"Ngân sách của subtask '{subTask.SubTaskName}' không thể âm.", null);

                            var newSubTask = new SubTask
                            {
                                TaskId = newTask.Id,
                                SubTaskName = subTask.SubTaskName,
                                SubTaskDescription = subTask.Description,
                                StartTime = subTask.StartTime ?? newTask.StartTime,
                                Deadline = subTask.Deadline,
                                AmountBudget = subTask.Budget,
                                CreateBy = organizerId,
                                Status = 0
                            };
                            await _subTaskRepository.CreateSubTaskAsync(newSubTask);
                        }
                    }
                }
            }

            // Tạo các rủi ro
            if (eventDTO.Risks != null && eventDTO.Risks.Any())
            {
                foreach (var risk in eventDTO.Risks)
                {
                    if (string.IsNullOrWhiteSpace(risk.Name))
                        return new ResponseDTO(400, "Tên rủi ro là bắt buộc.", null);

                    var newRisk = new Risk
                    {
                        EventId = newEvent.Id,
                        Name = risk.Name,
                        Reason = risk.Reason,
                        Solution = risk.Solution,
                        Description = risk.Description
                    };
                    await _eventRepository.CreateRiskAsync(newRisk);
                }
            }
            // Tạo các hoạt động
            if (eventDTO.Activities != null && eventDTO.Activities.Any())
            {
                foreach (var activity in eventDTO.Activities)
                {
                    if (string.IsNullOrWhiteSpace(activity.Name))
                        return new ResponseDTO(400, "Tên hoạt động là bắt buộc.", null);

                    var newActivity = new Activity
                    {
                        EventId = newEvent.Id,
                        Name = activity.Name,
                        Content = activity.Content
                    };
                    await _eventRepository.CreateActivityAsync(newActivity);
                }
            }
            Console.WriteLine("sang " + eventDTO.CostBreakdowns.Count());
            // Tạo các chi phí
            if (eventDTO.CostBreakdowns != null && eventDTO.CostBreakdowns.Any())
            {
                foreach (var costBreakdown in eventDTO.CostBreakdowns)
                {
                    if (string.IsNullOrWhiteSpace(costBreakdown.Name))
                        return new ResponseDTO(400, "Tên chi phí là bắt buộc.", null);

                    if (costBreakdown.Quantity.HasValue && costBreakdown.Quantity < 0)
                        return new ResponseDTO(400, $"Số lượng của chi phí '{costBreakdown.Name}' không thể âm.", null);

                    if (costBreakdown.PriceByOne.HasValue && costBreakdown.PriceByOne < 0)
                        return new ResponseDTO(400, $"Đơn giá của chi phí '{costBreakdown.Name}' không thể âm.", null);

                    var newCostBreakdown = new CostBreakdown
                    {
                        EventId = newEvent.Id,
                        Name = costBreakdown.Name,
                        Quantity = costBreakdown.Quantity,
                        PriceByOne = costBreakdown.PriceByOne
                    };
                    Console.WriteLine("thanh :"+newCostBreakdown.EventId);
                   await _eventRepository.CreateCostBreakdownAsync(newCostBreakdown);
                }
            }

            await transaction.CommitAsync();
            return new ResponseDTO(201, "Tạo sự kiện thành công!", newEvent);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Lỗi khi tạo sự kiện: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return new ResponseDTO(500, "Đã xảy ra lỗi khi tạo sự kiện.", ex.Message);
        }
    }

    public async Task<ResponseDTO> UploadImageAsync(UploadImageRequestDTO imageDTO)
    {
        if (imageDTO.EventMediaFiles != null && imageDTO.EventMediaFiles.Any())
        {
            foreach (var file in imageDTO.EventMediaFiles)
            {
                using var stream = file.OpenReadStream();
                string contentType = file.ContentType;
                string driveUrl;
                driveUrl = await _googleDriveService.UploadFileAsync(stream, file.FileName, contentType);
                if (string.IsNullOrEmpty(driveUrl))
                {
                    Console.WriteLine($"❌ Upload thất bại cho file {file.FileName}");
                    throw new Exception("Upload failed, MediaURL is null or empty.");
                }
                Console.WriteLine($"✅ File {file.FileName} đã upload thành công: {driveUrl}");
                var mediaItem = new Medium { MediaUrl = driveUrl };
                await _eventRepository.CreateMediaItemAsync(mediaItem);
              
                var eventMedia = new EventMedium
                {
                    EventId = imageDTO.EventId,
                    MediaId = mediaItem.Id,
                    Status = 1
                };
                await _eventRepository.AddEventMediaAsync(eventMedia);
            }
        }

        return new ResponseDTO(201, "Upload Image successfully!", null);
    }


    public async Task<ResponseDTO> GetEventDetailAsync(int eventId)
    {
        try
        {
            var eventDetail = await _eventRepository.GetEventDetailAsync(eventId);
            if (eventDetail == null)
            {
                return new ResponseDTO(404, $"Không tìm thấy sự kiện với ID là {eventId}.", eventDetail);
            }
            return new ResponseDTO(200, "Get event detail successfully!", eventDetail);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Error orcurs while getting event detail!", ex.Message);
        }
    }

    public async Task<ResponseDTO> UpdateEventAsync(EventUpdateDTO e)
    {
        try
        {
            Event updateEvent = new Event
            {
                Id = e.Id,
                AmountBudget = e.AmountBudget,
                CampusId = e.CampusId,
                CategoryEventId = e.CategoryEventId,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                EventDescription = e.EventDescription,
                EventTitle = e.EventTitle,
                Placed = e.Placed,
                Status = e.Status,
                UpdateBy = e.UpdateBy,
                UpdatedAt = DateTime.Now,
                MeasuringSuccess = e.MeasuringSuccess,
                Goals = e.Goals,
                MonitoringProcess = e.MonitoringProcess,
                SizeParticipants = e.SizeParticipants,
                PromotionalPlan = e.PromotionalPlan,
                TargetAudience = e.TargetAudience,
                SloganEvent = e.SloganEvent,
            };
            Event updatedEvent = await _eventRepository.UpdateEventAsync(updateEvent);
            EventDetailDto eventDetailResponseDTO = new EventDetailDto
            {
                Id = updatedEvent.Id,
                EventTitle = updatedEvent.EventTitle,
                EventDescription = updatedEvent.EventDescription,
                StartTime = updatedEvent.StartTime,
                EndTime = updatedEvent.EndTime,
                AmountBudget = updatedEvent.AmountBudget,
                IsPublic = updatedEvent.IsPublic,
                TimePublic = updatedEvent.TimePublic,
                Status = updatedEvent.Status,
                Placed = updatedEvent.Placed,
                CreatedAt = updatedEvent.CreatedAt,
                UpdatedAt = updatedEvent.UpdatedAt,
                CampusName = updatedEvent.Campus.CampusName,
                CategoryEventName = updatedEvent.CategoryEvent.CategoryEventName,
                MeasuringSuccess = updatedEvent.MeasuringSuccess,
                Goals = updatedEvent.Goals,
                MonitoringProcess = updatedEvent.Goals,
                SizeParticipants = updatedEvent.SizeParticipants,
                PromotionalPlan = updatedEvent.PromotionalPlan,
                TargetAudience = updatedEvent.TargetAudience,
                SloganEvent = updatedEvent.SloganEvent,
                CreatedBy = new UserDto
                {
                    Id = updatedEvent.CreateByNavigation.Id,
                    FirstName = updatedEvent.CreateByNavigation.FirstName,
                    LastName = updatedEvent.CreateByNavigation.LastName,
                    Email = updatedEvent.CreateByNavigation.Email
                },
                UpdatedBy = new UserDto
                {
                    Id = updatedEvent.UpdateByNavigation.Id,
                    FirstName = updatedEvent.UpdateByNavigation.FirstName,
                    LastName = updatedEvent.UpdateByNavigation.LastName,
                    Email = updatedEvent.UpdateByNavigation.Email
                },
                EventMedia = updatedEvent.EventMedia.Select(em => new EventMediaDto
                {
                    Id = em.Id,
                    MediaUrl = em.Media.MediaUrl
                }).ToList()
            };
            return new ResponseDTO(200, "Update event successfully!", eventDetailResponseDTO);
        }
        catch (Exception ex)
        {
            return new ResponseDTO(500, "Error update event!", ex.Message);
        }
    }

    public async Task<bool> DeleteEventAsync(int eventId)
    {
        try
        {
            var response = await _eventRepository.DeleteEventAsync(eventId);
            //notification
            if (response)
            {
                try
                {

                    var e = await _eventRepository.GetEventByIdAsync(eventId);
                    var listJoined = await _joinProjectRepository.GetUserIdJoinedEvent(eventId);
                    var message = "Đã xóa sự kiện " + (e.EventTitle.Length > 40 ? e.EventTitle.Substring(0, 40) + ".." : e.EventTitle) + "!";
                    var link = "https://fptu-planify.com/event-detail-EOG/" + eventId;

                    string htmlContent = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                      <meta charset='UTF-8'>
                      <title>Thông báo bị loại khỏi sự kiện</title>
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

                        <h1>Sự kiện đã bị xóa</h1>

                        <p class='description'>
                          {message}<br/><br/>
                          Nếu bạn có bất kỳ thắc mắc nào, vui lòng liên hệ với ban tổ chức để được hỗ trợ thêm.
                        </p>

                        <div class='button'>
                          <a href='{link}'>Xem chi tiết sự kiện</a>
                        </div>

                        <br><br>
                        <p class='description'>Trân trọng, hệ thống tự động</p>
                      </div>
                    </body>
                    </html>";
                    foreach (var id in listJoined)
                    {
                        await _hubContext.Clients.User(id + "").SendAsync("ReceiveNotification",
                        message,
                        link);
                        var user = await _userRepository.GetUserByIdAsync(id);
                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Thông báo: Sự kiện đã bị xóa ",
                            htmlContent);
                    }
                }
                catch { }
            }
            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PageResultDTO<EventGetListResponseDTO>> SearchEventAsync(int page, int pageSize, 
        string? title, DateTime? startTime, DateTime? endTime, decimal? minBudget, decimal? maxBudget, 
        int? isPublic, int? status, int? CategoryEventId, string? placed, Guid userId, int campusId, Guid? createBy)
    {
        try
        {
            var resultEvents = await _eventRepository.SearchEventAsync(page, pageSize, title, startTime, endTime,
                minBudget, maxBudget, isPublic, status, CategoryEventId, placed, userId, campusId, createBy);
            var eventDTOs = resultEvents.Items.Select(e => new EventGetListResponseDTO
            {
                Id = e.Id,
                EventTitle = e.EventTitle,
                EventDescription = e.EventDescription,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                AmountBudget = e.AmountBudget,
                IsPublic = e.IsPublic,
                TimePublic = e.TimePublic,
                Status = e.Status,
                CampusId = e.CampusId,
                CategoryEventId = e.CategoryEventId,
                Placed = e.Placed,
                CreateBy = e.CreateBy,
                CreatedAt = e.CreatedAt,
                ManagerId = e.ManagerId,
                EventMedias = e.EventMedia.Select(em=> new EventMediumViewMediaModel
                {
                    Id=em.Id,
                    MediaId = em.MediaId,
                    MediaDTO = new MediaItemDTO
                    {
                        Id = em.Media.Id,
                        MediaUrl = em.Media.MediaUrl,
                    }
                }).ToList(),
                isFavorite = e.FavouriteEvents.Count != 0,
            }).ToList();

            return new PageResultDTO<EventGetListResponseDTO>(eventDTOs,resultEvents.TotalCount,page,pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<ResponseDTO> CreateSaveDraft(EventCreateRequestDTO eventDTO, Guid organizerId)
    {
        using var transaction = await _eventRepository.BeginTransactionAsync();
        try
        {
            if (eventDTO == null)
                return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

            if (string.IsNullOrWhiteSpace(eventDTO.EventTitle))
                return new ResponseDTO(400, "Tên sự kiện là bắt buộc.", null);

            if (eventDTO.StartTime < DateTime.Now.AddMonths(2))
                return new ResponseDTO(400, "Thời gian bắt đầu phải cách thời gian hiện tại ít nhất 2 tháng.", null);

            if (eventDTO.StartTime >= eventDTO.EndTime)
                return new ResponseDTO(400, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc.", null);

            if (eventDTO.SizeParticipants < 0)
                return new ResponseDTO(400, "Số lượng người tham gia không thể âm.", null);

            var campusIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("campusId")?.Value;
            if (string.IsNullOrEmpty(campusIdClaim) || !int.TryParse(campusIdClaim, out int campusId))
                return new ResponseDTO(400, "ID campus không hợp lệ.", null);

            var category = await _eventRepository.GetCategoryEventAsync(eventDTO.CategoryEventId, campusId);
            if (category == null)
                return new ResponseDTO(400, "Danh mục không tồn tại hoặc không thuộc campus này.", null);

            decimal totalBudget = 0;
            if (eventDTO.CostBreakdowns != null && eventDTO.CostBreakdowns.Any())
            {
                foreach (var costBreakdown in eventDTO.CostBreakdowns)
                {
                    if (costBreakdown.Quantity.HasValue && costBreakdown.PriceByOne.HasValue)
                    {
                        totalBudget += costBreakdown.Quantity.Value * costBreakdown.PriceByOne.Value;
                    }
                }
            }

            var newEvent = new Event
            {
                EventTitle = eventDTO.EventTitle,
                EventDescription = eventDTO.EventDescription,
                StartTime = eventDTO.StartTime,
                EndTime = eventDTO.EndTime,
                AmountBudget = totalBudget,
                IsPublic = 0,
                TimePublic = null,
                Status = 0, // Bản nháp
                CampusId = campusId,
                CategoryEventId = eventDTO.CategoryEventId,
                Placed = eventDTO.Placed,
                CreateBy = organizerId,
                CreatedAt = DateTime.Now,
                MeasuringSuccess = eventDTO.MeasuringSuccess,
                Goals = eventDTO.Goals,
                MonitoringProcess = eventDTO.MonitoringProcess,
                SizeParticipants = eventDTO.SizeParticipants,
                PromotionalPlan = eventDTO.PromotionalPlan,
                TargetAudience = eventDTO.TargetAudience,
                SloganEvent = eventDTO.SloganEvent
            };

            // Lưu sự kiện
            await _eventRepository.CreateEventAsync(newEvent);

            // Tạo Tasks và SubTasks
            if (eventDTO.Tasks != null && eventDTO.Tasks.Any())
            {
                foreach (var task in eventDTO.Tasks)
                {
                    if (string.IsNullOrWhiteSpace(task.TaskName))
                        return new ResponseDTO(400, "Tên task là bắt buộc.", null);

                    if (task.Deadline > DateTime.Now)
                        return new ResponseDTO(400, $"Deadline of task '{task.TaskName}' must be after now.", null);

                    if (task.Budget < 0)
                        return new ResponseDTO(400, $"Ngân sách của task '{task.TaskName}' không thể âm.", null);

                    var newTask = new Planify_BackEnd.Models.Task
                    {
                        EventId = newEvent.Id,
                        TaskName = task.TaskName,
                        TaskDescription = task.Description,
                        StartTime = task.StartTime ?? newEvent.StartTime,
                        Deadline = task.Deadline,
                        AmountBudget = task.Budget,
                        CreateBy = organizerId,
                        CreateDate = DateTime.Now,
                        Status = 0
                    };
                    await _taskRepository.CreateTaskAsync(newTask);

                    if (task.SubTasks != null && task.SubTasks.Any())
                    {
                        foreach (var subTask in task.SubTasks)
                        {
                            if (string.IsNullOrWhiteSpace(subTask.SubTaskName))
                                return new ResponseDTO(400, "Tên subtask là bắt buộc.", null);

                            if (subTask.StartTime.HasValue && subTask.Deadline.HasValue && subTask.StartTime >= subTask.Deadline)
                                return new ResponseDTO(400, $"Thời gian bắt đầu của subtask '{subTask.SubTaskName}' phải sớm hơn thời hạn.", null);

                            if (subTask.Budget < 0)
                                return new ResponseDTO(400, $"Ngân sách của subtask '{subTask.SubTaskName}' không thể âm.", null);

                            var newSubTask = new SubTask
                            {
                                TaskId = newTask.Id,
                                SubTaskName = subTask.SubTaskName,
                                SubTaskDescription = subTask.Description,
                                StartTime = subTask.StartTime ?? newTask.StartTime,
                                Deadline = subTask.Deadline,
                                AmountBudget = subTask.Budget,
                                CreateBy = organizerId,
                                Status = 0
                            };
                            await _subTaskRepository.CreateSubTaskAsync(newSubTask);
                        }
                    }
                }
            }

            // Tạo Risks
            if (eventDTO.Risks != null && eventDTO.Risks.Any())
            {
                foreach (var risk in eventDTO.Risks)
                {
                    if (string.IsNullOrWhiteSpace(risk.Name))
                        return new ResponseDTO(400, "Tên rủi ro là bắt buộc.", null);

                    var newRisk = new Risk
                    {
                        EventId = newEvent.Id,
                        Name = risk.Name,
                        Reason = risk.Reason,
                        Solution = risk.Solution,
                        Description = risk.Description
                    };
                    await _eventRepository.CreateRiskAsync(newRisk);
                }
            }
            // Tạo các hoạt động
            if (eventDTO.Activities != null && eventDTO.Activities.Any())
            {
                foreach (var activity in eventDTO.Activities)
                {
                    if (string.IsNullOrWhiteSpace(activity.Name))
                        return new ResponseDTO(400, "Tên hoạt động là bắt buộc.", null);

                    var newActivity = new Activity
                    {
                        EventId = newEvent.Id,
                        Name = activity.Name,
                        Content = activity.Content
                    };
                    await _eventRepository.CreateActivityAsync(newActivity);
                }
            }
            // Tạo CostBreakdowns
            if (eventDTO.CostBreakdowns != null && eventDTO.CostBreakdowns.Any())
        {
                foreach (var costBreakdown in eventDTO.CostBreakdowns)
                {
                    if (string.IsNullOrWhiteSpace(costBreakdown.Name))
                        return new ResponseDTO(400, "Tên chi phí là bắt buộc.", null);

                    if (costBreakdown.Quantity.HasValue && costBreakdown.Quantity < 0)
                        return new ResponseDTO(400, $"Số lượng của chi phí '{costBreakdown.Name}' không thể âm.", null);

                    if (costBreakdown.PriceByOne.HasValue && costBreakdown.PriceByOne < 0)
                        return new ResponseDTO(400, $"Đơn giá của chi phí '{costBreakdown.Name}' không thể âm.", null);

                    var newCostBreakdown = new CostBreakdown
                    {
                        EventId = newEvent.Id,
                        Name = costBreakdown.Name,
                        Quantity = costBreakdown.Quantity,
                        PriceByOne = costBreakdown.PriceByOne
                    };
                    await _eventRepository.CreateCostBreakdownAsync(newCostBreakdown);
                }
            }

            await transaction.CommitAsync();
            return new ResponseDTO(201, "Lưu bản nháp thành công!", newEvent);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Lỗi khi lưu bản nháp: {ex.Message}\nStackTrace: {ex.StackTrace}");
            return new ResponseDTO(500, "Đã xảy ra lỗi khi lưu bản nháp.", ex.Message);
        }
    }
    //public async Task<ResponseDTO> UpdateSaveDraft(EventUpdateDTO eventDTO)
    //{

    //    try
    //    {
    //        var campus = await _campusRepository.GetCampusByName(eventDTO.CampusName);
    //        //var category = await _categoryRepository.GetCategoryByName(eventDTO.CategoryEventName, campus.Id);
    //        Event updateEvent = new Event
    //        {
    //            Id = eventDTO.Id,
    //            AmountBudget = eventDTO.AmountBudget,
    //            CampusId = campus.Id,
    //            CategoryEventId = (int)eventDTO.CategoryEventId,
    //            StartTime = eventDTO.StartTime,
    //            EndTime = eventDTO.EndTime,
    //            EventDescription = eventDTO.EventDescription,
    //            EventTitle = eventDTO.EventTitle,
    //            IsPublic = eventDTO.IsPublic,
    //            Placed = eventDTO.Placed,
    //            Status = eventDTO.Status,
    //            TimePublic = eventDTO.TimePublic,
    //            UpdateBy = eventDTO.UpdateBy,
    //            UpdatedAt = DateTime.Now
    //        };
    //        Event updatedEvent = await _eventRepository.UpdateSaveDraft(updateEvent);
    //        var e = new Event
    //        {
    //            EventTitle = updatedEvent.EventTitle,
    //            EventDescription = updatedEvent.EventDescription,
    //            StartTime = updatedEvent.StartTime,
    //            EndTime = updatedEvent.EndTime,
    //            AmountBudget = updatedEvent.AmountBudget,
    //            IsPublic = updatedEvent.IsPublic,
    //            TimePublic = updatedEvent.TimePublic,
    //            Status = updatedEvent.Status,
    //            CampusId = updatedEvent.CampusId,
    //            CategoryEventId = updatedEvent.CategoryEventId,
    //            Placed = updatedEvent.Placed,
    //            CreateBy = updatedEvent.CreateBy,
    //            CreatedAt = updatedEvent.CreatedAt
    //        };

    //        return new ResponseDTO(201, "Save draft successfully!", updatedEvent);
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ResponseDTO(500, "Error orcurs while save draft event!", ex.Message);
    //    }
    //}
    //public async Task<ResponseDTO> GetSaveDraft(Guid createBy)
    //{
    //    try
    //    {
    //        var eventDetail = await _eventRepository.GetSaveDraft(createBy);
    //        if (eventDetail == null)
    //        {
    //            return new ResponseDTO(404, "Don't exist any save draft!", eventDetail);
    //        }
    //        return new ResponseDTO(200, "Get save draft successfully!", eventDetail);
    //    }
    //    catch (Exception ex)
    //    {
    //        return new ResponseDTO(500, "Error orcurs while getting save draft!", ex.Message);
    //    }
    //}
    public async Task<ResponseDTO> DeleteImagesAsync(DeleteImagesRequestDTO request)
    {
        try
        {
            if (request.EventId <= 0 || !request.MediaIds.Any())
            {
                return new ResponseDTO(400, "Invalid eventId or empty mediaIds list", null);
            }

            // Kiểm tra các EventMedium tồn tại
            var eventMediaList = await _eventRepository.GetEventMediaByIdsAsync(request.EventId, request.MediaIds);
            if (!eventMediaList.Any() || eventMediaList.Count != request.MediaIds.Count)
            {
                return new ResponseDTO(404, "One or more images not found in event", null);
            }

            // Lấy danh sách Medium để xóa trên Drive
            var mediaItems = new List<Medium>();
            foreach (var mediaId in request.MediaIds)
            {
                var mediaItem = await _eventRepository.GetMediaItemAsync(mediaId);
                if (mediaItem == null || string.IsNullOrEmpty(mediaItem.MediaUrl))
                {
                    return new ResponseDTO(404, $"Media item {mediaId} not found or has no URL", null);
                }
                mediaItems.Add(mediaItem);
            }

            // Xóa files trên Google Drive
            var failedDeletions = new List<string>();
            foreach (var mediaItem in mediaItems)
            {
                string fileId = ExtractFileIdFromDriveUrl(mediaItem.MediaUrl);
                bool deleteSuccess = await _googleDriveService.DeleteFileAsync(fileId);
                if (!deleteSuccess)
                {
                    failedDeletions.Add(mediaItem.MediaUrl);
                    Console.WriteLine($"❌ Failed to delete file on Drive: {mediaItem.MediaUrl}");
                }
                else
                {
                    Console.WriteLine($"✅ Successfully deleted file on Drive: {mediaItem.MediaUrl}");
                }
            }

            if (failedDeletions.Any())
            {
                return new ResponseDTO(400, $"Failed to delete some files on Drive: {string.Join(", ", failedDeletions)}", null);
            }

            using (var transaction = await _eventRepository.BeginTransactionAsync())
            {
                try
                {
                    await _eventRepository.DeleteEventMediaListAsync(request.EventId, request.MediaIds);
                    await _eventRepository.DeleteMediaItemsAsync(request.MediaIds);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Database deletion failed: {ex.Message}");
                }
            }

            return new ResponseDTO(200, "Images deleted successfully!", null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting images: {ex.Message}");
            return new ResponseDTO(500, $"Error deleting images: {ex.Message}", null);
        }
    }

    private string ExtractFileIdFromDriveUrl(string driveUrl)
    {
        try
        {
            var uri = new Uri(driveUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var fileId = query["id"];

            if (!string.IsNullOrEmpty(fileId))
            {
                return fileId;
            }

            throw new Exception("Invalid Google Drive URL format - No file ID found");
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to extract file ID from URL: {ex.Message}");
        }
    }

    public async Task<bool> EventIncomingNotification(Guid userId,string?role)
    {
        try
        {
            var listEventIncoming = new List<EventIncomingNotification>();
            if (role.Equals("Campus Manager")|| role.Equals("Implementer") || role.Equals("Event Organizer"))
            {
                listEventIncoming = await _eventRepository.GetEventIncomings(userId);
                foreach (var item in listEventIncoming)
                {
                    var message = "Tháng này! Sự kiện mà bạn tham gia: " + (item.EventTitle.Length > 20 ? item.EventTitle.Substring(0, 20) + "..." : item.EventTitle) + "!\n"
                        + "Ngày bắt đầu: " + item.StartTime;
                    var link = "https://fptu-planify.com/event-detail-EOG/" + item.EventId;
                    await _hubContext.Clients.User(userId + "").SendAsync("ReceiveNotification",
                        message,
                        link);
                }
            }
            if (role.Equals("Spectator"))
            {
                listEventIncoming = await _eventRepository.GetEventParticipantIncomings(userId);
                foreach (var item in listEventIncoming)
                {
                    var message = "Tháng này! Sự kiện mà bạn tham gia: " + (item.EventTitle.Length > 20 ? item.EventTitle.Substring(0, 20) + "..." : item.EventTitle) + "!\n"
                        + "Ngày bắt đầu: " + item.StartTime;
                    var link = "https://fptu-planify.com/event-detail-spec/" + item.EventId;
                    await _hubContext.Clients.User(userId + "").SendAsync("ReceiveNotification",
                        message,
                        link);
                }
            }
            if (listEventIncoming.Count==0)
            {
                return false;
            }
            return true;
            
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<PageResultDTO<EventGetListResponseDTO>> MyEvent(int page, int pageSize, Guid createBy, int campusId)
    {
        try
        {
            var resultEvents = await _eventRepository.MyEvent(page, pageSize, createBy, campusId);
            var eventDTOs = resultEvents.Items.Select(e => new EventGetListResponseDTO
            {
                Id = e.Id,
                EventTitle = e.EventTitle,
                EventDescription = e.EventDescription,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                AmountBudget = e.AmountBudget,
                IsPublic = e.IsPublic,
                TimePublic = e.TimePublic,
                Status = e.Status,
                CampusId = e.CampusId,
                CategoryEventId = e.CategoryEventId,
                Placed = e.Placed,
                CreateBy = e.CreateBy,
                CreatedAt = e.CreatedAt,
                ManagerId = e.ManagerId,
                EventMedias = e.EventMedia.Select(em => new EventMediumViewMediaModel
                {
                    Id = em.Id,
                    MediaId = em.MediaId,
                    MediaDTO = new MediaItemDTO
                    {
                        Id = em.Media.Id,
                        MediaUrl = em.Media.MediaUrl,
                    }
                }).ToList(),
                isFavorite = e.FavouriteEvents.Count != 0,
            }).ToList();

            return new PageResultDTO<EventGetListResponseDTO>(eventDTOs, resultEvents.TotalCount, page, pageSize);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

