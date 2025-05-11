using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs.JoinedProjects;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Services.Notification;
using static Microsoft.IO.RecyclableMemoryStreamManager;

namespace Planify_BackEnd.Services.JoinProjects
{
    public class JoinProjectService : IJoinProjectService
    {
        private readonly IJoinProjectRepository _joinProjectRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEventRepository _eventRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        public JoinProjectService(IJoinProjectRepository joinProjectRepository, IHttpContextAccessor httpContextAccessor, IHubContext<NotificationHub> hubContext, IEventRepository eventRepository, IEmailSender emailSender,IUserRepository userRepository)
        {
            _joinProjectRepository = joinProjectRepository;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
            _eventRepository = eventRepository;
            _emailSender = emailSender;
            _userRepository = userRepository;
        }
        public PageResultDTO<JoinedProjectDTO> JoiningEvents(int page, int pageSize, Guid userId)
        {
            try
            {
                var joinedProject = _joinProjectRepository.JoiningEvents(userId,page, pageSize);
                if (joinedProject.TotalCount == 0)

                    return new PageResultDTO<JoinedProjectDTO>(new List<JoinedProjectDTO>(), 0, page, pageSize);
                List<JoinedProjectDTO> joiningList = new List<JoinedProjectDTO>();
                foreach (var item in joinedProject.Items)
                {
                    var joinedProjectDTO = new JoinedProjectDTO
                    {
                        Id = item.Id,
                        EventId = item.EventId,
                        UserId = item.UserId,
                        TimeJoinProject = item.TimeJoinProject,
                        TimeOutProject = item.TimeOutProject,
                        EventTitle = item.Event.EventTitle,
                        EventDescription = item.Event.EventDescription,
                        StartTime = item.Event.StartTime,
                        EndTime = item.Event.EndTime,
                        AmountBudget = item.Event.AmountBudget,
                        IsPublic = item.Event.IsPublic,
                        TimePublic = item.Event.TimePublic,
                        Status = item.Event.Status,
                        CampusId = item.Event.CampusId,
                        CategoryEventId = item.Event.CategoryEventId,
                        Placed = item.Event.Placed,
                        CreatedAt = item.Event.CreatedAt,
                        CreateBy = item.Event.CreateBy,
                        UpdatedAt = item.Event.UpdatedAt,
                        UpdateBy = item.Event.UpdateBy
                    };
                    joiningList.Add(joinedProjectDTO);
                }

                return new PageResultDTO<JoinedProjectDTO>(joiningList, joinedProject.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public PageResultDTO<JoinedProjectDTO> AttendedEvents(int page, int pageSize, Guid userId)
        {
            try
            {
                var joinedProject = _joinProjectRepository.AttendedEvents(page, pageSize, userId);
                if(joinedProject.TotalCount == 0)

                    return new PageResultDTO<JoinedProjectDTO>(new List<JoinedProjectDTO>(), 0, page, pageSize);
                List<JoinedProjectDTO> joinedList = new List<JoinedProjectDTO>();
                foreach (var item in joinedProject.Items)
                {
                    var joinedProjectDTO = new JoinedProjectDTO
                    {
                        Id = item.Id,
                        EventId = item.EventId,
                        UserId = item.UserId,
                        TimeJoinProject = item.TimeJoinProject,
                        TimeOutProject = item.TimeOutProject,
                        EventTitle = item.Event.EventTitle,
                        EventDescription = item.Event.EventDescription,
                        StartTime = item.Event.StartTime,
                        EndTime = item.Event.EndTime,
                        AmountBudget = item.Event.AmountBudget,
                        IsPublic = item.Event.IsPublic,
                        TimePublic = item.Event.TimePublic,
                        Status = item.Event.Status,
                        CampusId = item.Event.CampusId,
                        CategoryEventId = item.Event.CategoryEventId,
                        CategoryName = item.Event.CategoryEvent.CategoryEventName,
                        Placed = item.Event.Placed,
                        CreatedAt = item.Event.CreatedAt,
                        CreateBy = item.Event.CreateBy,
                        UpdatedAt = item.Event.UpdatedAt,
                        UpdateBy = item.Event.UpdateBy
                    };
                    joinedList.Add(joinedProjectDTO);
                }
              
                return new PageResultDTO<JoinedProjectDTO>(joinedList, joinedProject.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteImplementerFromEvent(Guid userId, int eventId)
        {
            try
            {
                var response = await _joinProjectRepository.DeleteImplementerFromEvent(userId, eventId);
                //notification
                var e = await _eventRepository.GetEventByIdAsync(eventId);
                var message = "Bạn đã bị loại khỏi sự kiện " + (e.EventTitle.Length > 40 ? e.EventTitle.Substring(0, 40) + ".." : e.EventTitle) + "!";
                if (response)
                {
                    var user = await _userRepository.GetUserByIdAsync(userId);
                    await _hubContext.Clients.User(userId + "").SendAsync("ReceiveNotification",
                    message,
                    "https://fptu-planify.com/event-detail-EOG/" + eventId);
                    //await _emailSender.SendEmailAsync(user.Email, 
                    //    "Thông báo: Bạn đã bị loại khỏi sự kiện " + e.EventTitle,
                    //    $"<p>Chúng tôi xin thông báo rằng bạn đã bị loại khỏi sự kiện <strong>{e.EventTitle}</strong>.</p>" +
                    //    $"<p>Nếu bạn có bất kỳ thắc mắc nào về quyết định này, vui lòng liên hệ với ban tổ chức để được giải đáp thêm.</p>" +
                    //    $"<p>Bạn có thể xem lại chi tiết sự kiện tại liên kết sau: <a href=\"/event-detail-EOG/{eventId}\">Xem chi tiết sự kiện</a></p>"
                    //    );
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Thông báo: Bạn đã bị loại khỏi sự kiện " + e.EventTitle,
                        $@"
                        <!DOCTYPE html>
                        <html lang='vi'>
                        <head>
                          <meta charset='UTF-8'>
                          <title>Thông báo loại khỏi sự kiện</title>
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

                            <h1>Bạn đã bị loại khỏi sự kiện</h1>

                            <p class='description'>
                              Chúng tôi xin thông báo rằng bạn đã bị loại khỏi sự kiện <strong>{e.EventTitle}</strong>.
                              <br/><br/>
                              Nếu bạn có bất kỳ thắc mắc nào về quyết định này, vui lòng liên hệ với ban tổ chức để được giải đáp thêm.
                            </p>

                            <div class='button'>
                              <a href='https://fptu-planify.com/event-detail-EOG/{eventId}'>Xem chi tiết sự kiện</a>
                            </div>

                            <br><br>
                            <p class='description'>Trân trọng, hệ thống tự động</p>
                          </div>
                        </body>
                        </html>"
                    );

                }
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("join project service - delete implementer from event: " + ex.Message);
                return false;
            }
        }

        public async Task<ResponseDTO> AddImplementersToEventAsync(AddImplementersToEventDTO request)
        {
            if (request == null || request.UserIds == null || !request.UserIds.Any())
            {
                return new ResponseDTO(400, "Invalid request data. UserIds list cannot be empty.", null);
            }

            if (!await _joinProjectRepository.EventExistsAsync(request.EventId))
            {
                return new ResponseDTO(404, $"Event with ID {request.EventId} not found.", null);
            }

            var invalidUserIds = await _joinProjectRepository.GetInvalidUserIdsAsync(request.UserIds);
            if (invalidUserIds.Any())
            {
                return new ResponseDTO(400, $"The following UserIds do not exist: {string.Join(", ", invalidUserIds)}", null);
            }

            var existingUserIds = await _joinProjectRepository.GetExistingImplementerIdsAsync(request.EventId);
            var newUserIds = request.UserIds.Except(existingUserIds).ToList();

            if (!newUserIds.Any())
            {
                return new ResponseDTO(200, "All provided users are already implementers for this event.", null);
            }

            var addImplementersSuccess = await _joinProjectRepository.AddImplementersToProject(newUserIds, request.EventId);
            if (!addImplementersSuccess)
            {
                return new ResponseDTO(500, "Failed to add implementers to the event.", null);
            }

            var addRolesSuccess = await _joinProjectRepository.AddRoleImplementers(newUserIds);
            if (!addRolesSuccess)
            {
                return new ResponseDTO(500, "Failed to assign Implementer role to users.", null);
            }

            var result = new { AddedCount = newUserIds.Count, EventId = request.EventId };
            try
            {
                //notification
                var e = await _eventRepository.GetEventByIdAsync(request.EventId);
                var message = "Bạn đã được thêm vào sự kiện " + (e.EventTitle.Length > 40 ? e.EventTitle.Substring(0, 40) + ".." : e.EventTitle) + "!";
                var link = "https://fptu-planify.com/event-detail-EOG/" + request.EventId;
                foreach (var id in newUserIds)
                {
                    await _hubContext.Clients.User(id + "").SendAsync("ReceiveNotification",
                        message,
                        link);
                    var user = await _userRepository.GetUserByIdAsync(id);
                    //await _emailSender.SendEmailAsync(user.Email,
                    //        "Thông báo: Bạn đã được thêm vào sự kiện " + e.EventTitle,
                    //        $"<p>Bạn đã được thêm vào sự kiện <strong>{e.EventTitle}</strong>.</p>" +
                    //        $"<p>Nếu bạn có bất kỳ thắc mắc nào về quyết định này, vui lòng liên hệ với ban tổ chức để được giải đáp thêm.</p>"
                    //        );
                    await _emailSender.SendEmailAsync(
                        user.Email,
                        "Thông báo: Bạn đã được thêm vào sự kiện " + e.EventTitle,
                        $@"
                        <!DOCTYPE html>
                        <html lang='vi'>
                        <head>
                          <meta charset='UTF-8'>
                          <title>Thông báo thêm vào sự kiện</title>
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
                              color: #008000;
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

                            <h1>Bạn đã được thêm vào sự kiện</h1>

                            <p class='description'>
                              Xin chúc mừng! Bạn đã được thêm vào sự kiện <strong>{e.EventTitle}</strong>.<br/><br/>
                              Nếu bạn có bất kỳ thắc mắc nào về quyết định này, vui lòng liên hệ với ban tổ chức để được giải đáp thêm.
                            </p>

                            <div class='button'>
                              <a href='https://fptu-planify.com/event-detail-EOG/{e.Id}'>Xem chi tiết sự kiện</a>
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
            return new ResponseDTO(200, $"Successfully added {newUserIds.Count} implementer(s) to event {request.EventId}.", result);
        }

        public async Task<PageResultDTO<JoinedProjectVM>> SearchImplementerJoinedEvent(
            int page, int pageSize, int? eventId, string? email, string? name)
        {
            try
            {
                var listImplement = await _joinProjectRepository
                    .SearchImplementerJoinedEvent(page, pageSize, eventId, email, name);
                List<JoinedProjectVM> list = listImplement.Items.Select(jp => new JoinedProjectVM
                {
                    Id = jp.Id,
                    EventId = jp.EventId,
                    UserId = jp.UserId,
                    TimeJoinProject = jp.TimeJoinProject,
                    TimeOutProject = jp.TimeOutProject,
                    User = new UserNameVM
                    {
                        Id = jp.User.Id,
                        Email = jp.User.Email,
                        FirstName = jp.User.FirstName,
                        LastName = jp.User.LastName,
                    },
                }).ToList();
                return new PageResultDTO<JoinedProjectVM>(list, listImplement.TotalCount, page, pageSize);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

   
}
