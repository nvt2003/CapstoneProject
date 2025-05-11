using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SendRequests;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Repositories.SendRequests;
using Planify_BackEnd.Services.Notification;

namespace Planify_BackEnd.Services.EventRequests
{
    public class SendRequestService : ISendRequestService
    {
        private readonly ISendRequestRepository _requestRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IHubContext<EventRequestHub> _hubContext;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;

        public SendRequestService(
            ISendRequestRepository requestRepository,
            IEventRepository eventRepository,
            IHubContext<EventRequestHub> hubContext,
            IHubContext<NotificationHub> notificationHubContext,
            IEmailSender emailSender,
            IUserRepository userRepository)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _notificationHubContext = notificationHubContext ?? throw new ArgumentNullException(nameof(notificationHubContext));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _userRepository = userRepository;
        }

        public async Task<ResponseDTO> GetRequestsAsync(int campusId)
        {
            var requests = await _requestRepository.GetRequestsAsync(campusId);
            return new ResponseDTO(200, "Lấy danh sách yêu cầu thành công", requests);
        }

        public async Task<ResponseDTO> CreateRequestAsync(SendRequestDTO requestDTO)
        {
            try
            {
                if (requestDTO == null)
                    return new ResponseDTO(400, "Dữ liệu không hợp lệ", null);

                if (requestDTO.EventId <= 0)
                    return new ResponseDTO(400, "Event ID không hợp lệ", null);

                var eventEntity = await _eventRepository.GetEventByIdAsync(requestDTO.EventId);
                if (eventEntity == null)
                    return new ResponseDTO(404, "Không tìm thấy sự kiện", null);

                if (eventEntity.Status != 0 && eventEntity.Status != -1)
                    return new ResponseDTO(400, "Sự kiện không thể tạo yêu cầu mới do trạng thái hiện tại", null);

                var existingRequest = await _requestRepository.GetRequestByIdAsync(requestDTO.EventId);
                if (existingRequest != null && existingRequest.Status == 0)
                    return new ResponseDTO(400, "Sự kiện đã có yêu cầu đang chờ duyệt", null);

                var request = new SendRequest
                {
                    EventId = requestDTO.EventId,
                    Status = 0, // Chờ duyệt
                    Reason = "N/A"
                };

                request = await _requestRepository.CreateRequestAsync(request);
                if (request == null)
                    return new ResponseDTO(500, "Không thể tạo yêu cầu", null);

                eventEntity.Status = 1; // Chờ duyệt
                await _eventRepository.UpdateEventAsync(eventEntity);

                // Gửi thông báo SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveEventRequest", request);

                return new ResponseDTO(201, "Tạo yêu cầu thành công", request);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Đã xảy ra lỗi khi tạo yêu cầu", null);
            }
        }

        public async Task<ResponseDTO> ApproveRequestAsync(int id, Guid managerId, string reason)
        {
            try
            {
                if (id <= 0) return new ResponseDTO(400, "ID không hợp lệ", null);
                if (managerId == Guid.Empty) return new ResponseDTO(400, "Manager ID không hợp lệ", null);

                var request = await _requestRepository.GetRequestByIdAsync(id);
                if (request == null) return new ResponseDTO(404, "Không tìm thấy yêu cầu", null);

                if (request.Event.Status != 1)
                    return new ResponseDTO(400, "Yêu cầu không thể được duyệt vì không ở trạng thái chờ duyệt", null);

                var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId);
                if (eventEntity == null)
                    return new ResponseDTO(404, "Không tìm thấy sự kiện", null);

                if (eventEntity.Status != 1)
                    return new ResponseDTO(400, "Sự kiện không thể được duyệt do trạng thái hiện tại", null);

                request.Status = 1; // Đã duyệt
                request.ManagerId = managerId;
                request.Reason = reason;
                eventEntity.Status = 2; // Đã duyệt
                eventEntity.IsPublic = 1; // Public
                eventEntity.TimePublic = DateTime.Now;

                await _requestRepository.UpdateRequestAsync(request);
                await _eventRepository.UpdateEventAsync(eventEntity);
                await _hubContext.Clients.All.SendAsync("RequestApproved", request);
                //notification
                try
                {
                    await _notificationHubContext.Clients.User(eventEntity.CreateBy + "").SendAsync("ReceiveNotification",
                        "Your request has been approved!",
                        "https://fptu-planify.com/event-detail-EOG/" + request.EventId);
                    var user = await _userRepository.GetUserByIdAsync(eventEntity.CreateBy);
                    //if (user != null) await _emailSender.SendEmailAsync(user.Email,
                    //    "Thông Báo Kế Hoạch Của Bạn Đã Được Duyệt",
                    //    "Kính gửi " + user.LastName + " " + user.FirstName + ",<br/><br/>" +
                    //    "Chúng tôi xin thông báo rằng kế hoạch sự kiện " + eventEntity.EventTitle+
                    //    " của bạn đã được xem xét và đã được phê duyệt.<br/>" +
                    //    reason + "<br/><br/>" +
                    //    "Trân trọng,<br/>Hệ thống tự động");
                    if (user != null)
                    {
                        string htmlContent = $@"
                        <!DOCTYPE html>
                        <html lang='vi'>
                        <head>
                          <meta charset='UTF-8'>
                          <title>Thông Báo Kế Hoạch Được Duyệt</title>
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
                              font-size: 28px;
                              font-weight: bold;
                              margin: 0;
                              line-height: 1.2;
                              color: #000000;
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

                            <h1>Kế hoạch của bạn đã được duyệt</h1>

                            <p class='description'>
                              Kính gửi {user.LastName} {user.FirstName},<br/><br/>
                              Chúng tôi xin thông báo rằng kế hoạch sự kiện <strong>{eventEntity.EventTitle}</strong> của bạn đã được xem xét và <strong>đã được phê duyệt</strong>.<br/><br/>
                              Lý do: {reason}
                            </p>

                            <div class='button'>
                              <a href='{"https://fptu-planify.com/event-detail-EOG/" + request.EventId}'>Xem chi tiết</a>
                            </div>
                            <br></br>
                            <p class='description'>
                              Trân trọng, hệ thống tự động
                            </p>
                          </div>
                        </body>
                        </html>";

                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Thông Báo Kế Hoạch Của Bạn Đã Được Duyệt",
                            htmlContent
                        );
                    }

                }
                catch { }
                return new ResponseDTO(200, "Yêu cầu đã được duyệt", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error approving request: {ex.Message}");
                return new ResponseDTO(500, "Đã xảy ra lỗi khi duyệt yêu cầu", null);
            }
        }

        public async Task<ResponseDTO> RejectRequestAsync(int id, Guid managerId, string reason)
        {
            try
            {
                if (id <= 0) return new ResponseDTO(400, "ID không hợp lệ", null);
                if (managerId == Guid.Empty) return new ResponseDTO(400, "Manager ID không hợp lệ", null);

                var request = await _requestRepository.GetRequestByIdAsync(id);
                if (request == null) return new ResponseDTO(404, "Không tìm thấy yêu cầu", null);

                if (request.Event.Status != 1)
                    return new ResponseDTO(400, "Yêu cầu không thể bị từ chối vì không ở trạng thái chờ duyệt", null);

                var eventEntity = await _eventRepository.GetEventByIdAsync(request.EventId);
                if (eventEntity == null)
                    return new ResponseDTO(404, "Không tìm thấy sự kiện", null);

                if (eventEntity.Status != 1)
                    return new ResponseDTO(400, "Sự kiện không thể bị từ chối do trạng thái hiện tại", null);

                request.Status = -1; // Bị từ chối
                request.ManagerId = managerId;
                request.Reason = reason;
                eventEntity.Status = -1; // Không được duyệt

                await _requestRepository.UpdateRequestAsync(request);
                await _eventRepository.UpdateEventAsync(eventEntity);
                await _hubContext.Clients.All.SendAsync("RequestRejected", request);
                //notification
                try
                {
                    await _notificationHubContext.Clients.User(eventEntity.CreateBy + "").SendAsync("ReceiveNotification",
                        "Your request has been reject!",
                        "https://fptu-planify.com/event-detail-EOG/" + request.EventId);
                    var user = await _userRepository.GetUserByIdAsync(eventEntity.CreateBy);
                    if (user != null)
                    {
                        string htmlContent = $@"
                        <!DOCTYPE html>
                        <html lang='vi'>
                        <head>
                          <meta charset='UTF-8'>
                          <title>Thông Báo Kế Hoạch Bị Từ Chối</title>
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
                              font-size: 28px;
                              font-weight: bold;
                              margin: 0;
                              line-height: 1.2;
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

                            <h1>Kế hoạch của bạn đã bị từ chối</h1>

                            <p class='description'>
                              Kính gửi {user.LastName} {user.FirstName},<br/><br/>
                              Chúng tôi rất tiếc phải thông báo rằng kế hoạch sự kiện <strong>{eventEntity.EventTitle}</strong> của bạn <strong>đã bị từ chối</strong> sau quá trình xem xét.<br/><br/>
                              Lý do từ chối: {reason}
                            </p>

                            <div class='button'>
                              <a href='{"https://fptu-planify.com/event-detail-EOG/" + request.EventId}'>Xem chi tiết</a>
                            </div>
                            <br><br>
                            <p class='description'>
                              Trân trọng, hệ thống tự động
                            </p>
                          </div>
                        </body>
                        </html>";

                        await _emailSender.SendEmailAsync(
                            user.Email,
                            "Thông Báo Kế Hoạch Của Bạn Bị Từ Chối",
                            htmlContent
                        );
                    }


                }
                catch { }
                return new ResponseDTO(200, "Yêu cầu đã bị từ chối", request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rejecting request: {ex.Message}");
                return new ResponseDTO(500, "Đã xảy ra lỗi khi từ chối yêu cầu", null);
            }
        }
        public async Task<ResponseDTO> GetMyRequestsAsync(Guid userId, int campusId)
        {
            try
            {
                var requests = await _requestRepository.GetRequestsByUserIdAsync(userId,campusId);

                if (requests == null || !requests.Any())
                {
                    return new ResponseDTO(404, "No requests found for this user.", null);
                }

                var requestDtos = requests.Select(sr => new GetSendRequestDTO
                {
                    Id = sr.Id,
                    EventId = sr.EventId,
                    EventTitle = sr.Event.EventTitle,
                    ManagerId = sr.ManagerId,
                    Reason = sr.Reason,
                    Status = sr.Event.Status,
                    CreatedAt = sr.Event.CreatedAt,
                    EventStartTime = sr.Event.StartTime,
                    EventEndTime = sr.Event.EndTime,
                    requestStatus = sr.Status
                }).ToList();

                return new ResponseDTO(200, "Requests retrieved successfully.", requestDtos);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Error retrieving requests: {ex.Message}", null);
            }
        }

        public async Task<PageResultDTO<GetSendRequestDTO>> SearchRequest(int page, int pageSize, int campusId, string? eventTitle, int? status, Guid? userId, int? requestStatus)
        {
            try
            {
                var requests = await _requestRepository.SearchRequest(page,pageSize, campusId,eventTitle,status, userId, requestStatus);

                if (requests == null || requests.TotalCount==0)
                {
                    return new PageResultDTO<GetSendRequestDTO>(new List<GetSendRequestDTO>(),0,page,pageSize);
                }

                var requestDtos = requests.Items.Select(sr => new GetSendRequestDTO
                {
                    Id = sr.Id,
                    EventId = sr.EventId,
                    EventTitle = sr.Event.EventTitle,
                    ManagerId = sr.ManagerId,
                    Reason = sr.Reason,
                    Status = sr.Event.Status,
                    CreatedAt = sr.Event.CreatedAt,
                    EventStartTime = sr.Event.StartTime,
                    EventEndTime = sr.Event.EndTime,
                    requestStatus = sr.Status,
                }).ToList();

                return new PageResultDTO<GetSendRequestDTO>(requestDtos, requests.TotalCount, page, pageSize);
            }
            catch
            {
                return new PageResultDTO<GetSendRequestDTO>(new List<GetSendRequestDTO>(), 0, page, pageSize);
            }
        }
    }
}