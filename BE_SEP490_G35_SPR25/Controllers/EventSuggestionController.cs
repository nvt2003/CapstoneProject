using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.Models;
using Planify_BackEnd.Services.ChatGPT;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventSuggestionController : ControllerBase
    {
        private readonly IChatGPTService _chatGPTService;
        private readonly PlanifyContext _dbContext;

        public EventSuggestionController(IChatGPTService chatGPTService, PlanifyContext dbContext)
        {
            _chatGPTService = chatGPTService;
            _dbContext = dbContext;
        }

        [HttpPost("get-full-event-suggestion")]
        public async Task<IActionResult> GetFullEventSuggestion([FromBody] SuggestionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                return BadRequest(new { Error = "Prompt không được để trống." });
            }

            try
            {
                var pastEvents = await _dbContext.Events
                    .Where(e => e.Status == 1 &&
                               (!request.CategoryEventId.HasValue || e.CategoryEventId == request.CategoryEventId))
                    .Select(e => new
                    {
                        e.EventTitle,
                        e.EventDescription,
                        e.StartTime,
                        e.EndTime,
                        e.CategoryEventId,
                        e.AmountBudget,
                        e.Placed,
                        e.SloganEvent,
                        e.Goals,
                        e.TargetAudience,
                        e.SizeParticipants,
                        e.MeasuringSuccess,
                        e.MonitoringProcess,
                        e.PromotionalPlan,
                        DurationHours = (e.EndTime - e.StartTime).TotalHours,
                        Tasks = _dbContext.Tasks.Where(t => t.EventId == e.Id)
                            .Select(t => new
                            {
                                t.TaskName,
                                t.TaskDescription,
                                t.Deadline,
                                t.AmountBudget,
                                SubTasks = _dbContext.SubTasks.Where(st => st.TaskId == t.Id)
                                    .Select(st => new
                                    {
                                        st.SubTaskName,
                                        st.SubTaskDescription,
                                        st.Deadline,
                                        st.AmountBudget,
                                    }).ToList()
                            }).ToList(),
                        CostBreakdowns = _dbContext.CostBreakdowns.Where(cb => cb.EventId == e.Id)
                            .Select(cb => new
                            {
                                cb.Name,
                                cb.Quantity,
                                cb.PriceByOne,
                                Total = cb.Quantity * cb.PriceByOne
                            }).ToList(),
                        Risks = _dbContext.Risks.Where(r => r.EventId == e.Id)
                            .Select(r => new
                            {
                                r.Name,
                                r.Reason,
                                r.Description,
                                r.Solution
                            }).ToList(),
                        Activities = _dbContext.Activities.Where(a => a.EventId == e.Id)
                            .Select(a => new
                            {
                                a.Name,
                                a.Content
                            }).ToList()
                    })
                    .Take(5)
                    .ToListAsync();

                string fullPrompt;

                if (!pastEvents.Any())
                {
                    fullPrompt = $"Không có dữ liệu lịch sử. Hãy tạo một sự kiện mới chi tiết và thực tế cho trường Đại học FPT.\n" +
                                 $"Yêu cầu từ người dùng: {request.Prompt}\n" +
                                 $"Trả về kết quả bằng tiếng Việt, thời gian phải luôn ở tương lai 2 tháng (sau ngày hiện tại {DateTime.Now:yyyy-MM-dd}), " +
                                 $"ngân sách thực tế (phù hợp với sự kiện tại trường đại học), và dưới dạng JSON với các trường sau:\n" +
                                 $"- EventTitle (string): Tiêu đề sự kiện\n" +
                                 $"- EventDescription (string): Mô tả chi tiết sự kiện\n" +
                                 $"- StartTime (string, định dạng 'yyyy-MM-dd HH:mm'): Thời gian bắt đầu\n" +
                                 $"- EndTime (string, định dạng 'yyyy-MM-dd HH:mm'): Thời gian kết thúc\n" +
                                 $"- CategoryEventId (int): ID danh mục sự kiện = {request.CategoryEventId}\n" +
                                 $"- Placed (string): Địa điểm tổ chức\n" +
                                 $"- SloganEvent (string): Khẩu hiệu sự kiện\n" +
                                 $"- Goals (string): Mục tiêu sự kiện\n" +
                                 $"- TargetAudience (string): Đối tượng tham gia\n" +
                                 $"- SizeParticipants (int): Số lượng người tham gia\n" +
                                 $"- MeasuringSuccess (string): Định lượng cụ thể như thế nào là sự kiện thành công, phải bao gồm một tỉ lệ phần trăm rõ ràng (ví dụ: 'Đạt ít nhất 85% phản hồi tích cực từ khảo sát sau sự kiện' hoặc 'Có ít nhất 90% người tham gia hoàn thành chương trình'). Đảm bảo tiêu chí thực tế, có thể đo lường được qua khảo sát, thống kê tham gia, hoặc kết quả hoạt động, phù hợp với mục tiêu sự kiện và bối cảnh trường Đại học FPT\n" +
                                 $"- MonitoringProcess (string): Quy trình giám sát\n" +
                                 $"- PromotionalPlan (string): Kế hoạch quảng bá\n" +
                                 $"- AmountBudget (decimal): Ngân sách tổng cho sự kiện\n" +
                                 $"- Tasks (mảng): Danh sách nhiệm vụ, mỗi nhiệm vụ bao gồm:\n" +
                                 $"  - **Quan trọng**: Deadline của nhiệm vụ con phải luôn nhỏ hơn deadline của nhiệm vụ lớn tương ứng (ví dụ: nếu nhiệm vụ có deadline là '2025-06-15 17:00', thì tất cả nhiệm vụ con phải có deadline sớm hơn, ví dụ: '2025-06-10 17:00').\n" +
                                 $"  - TaskName (string): Tên nhiệm vụ\n" +
                                 $"  - TaskDescription (string): Mô tả chi tiết nhiệm vụ\n" +
                                 $"  - Deadline (string, định dạng 'yyyy-MM-dd HH:mm'): Hạn chót nhiệm vụ\n" +
                                 $"  - AmountBudget (decimal): Ngân sách cho nhiệm vụ\n" +
                                 $"  - SubTasks (mảng): Danh sách nhiệm vụ con, mỗi nhiệm vụ con bao gồm:\n" +
                                 $"    - SubTaskName (string): Tên nhiệm vụ con\n" +
                                 $"    - SubTaskDescription (string): Mô tả chi tiết nhiệm vụ con\n" +
                                 $"    - Deadline (string, định dạng 'yyyy-MM-dd HH:mm'): Hạn chót của nhiệm vụ con, phải nhỏ hơn deadline của nhiệm vụ lớn\n" +
                                 $"    - AmountBudget (decimal): Ngân sách cho nhiệm vụ con\n" +
                                 $"- BudgetRows (mảng): Danh sách chi phí, mỗi mục bao gồm:\n" +
                                 $"  - Name (string): Tên mục chi phí\n" +
                                 $"  - Quantity (int): Số lượng\n" +
                                 $"  - Price (decimal): Giá mỗi đơn vị\n" +
                                 $"  - Total (decimal): Tổng chi phí (Quantity * Price)\n" +
                                 $"- Risks (mảng): Danh sách rủi ro, mỗi rủi ro bao gồm:\n" +
                                 $"  - Name (string): Tên rủi ro\n" +
                                 $"  - Reason (string): Lý do rủi ro\n" +
                                 $"  - Description (string): Mô tả rủi ro\n" +
                                 $"  - Solution (string): Giải pháp cho rủi ro\n" +
                                 $"- Activities (mảng): Danh sách hoạt động, mỗi hoạt động bao gồm:\n" +
                                 $"  - Name (string): Tên hoạt động\n" +
                                 $"  - Content (string): Nội dung hoạt động\n" +
                                 $"Lưu ý: Tất cả các số (AmountBudget, Price, Total, v.v.) phải được viết liền không có dấu phẩy phân cách hàng nghìn (ví dụ: 50000000 thay vì 50,000,000). Phản hồi phải cung cấp chi tiết đầy đủ, mô tả kỹ lưỡng kế hoạch, phù hợp với yêu cầu của người dùng và bối cảnh trường Đại học FPT.";
                }
                else
                {
                    string eventData = "Dữ liệu lịch sử các sự kiện trước đây tại trường Đại học FPT:\n";
                    foreach (var ev in pastEvents)
                    {
                        eventData += $"- Sự kiện: {ev.EventTitle}\n";
                        eventData += $"  Mô tả: {ev.EventDescription}\n";
                        eventData += $"  Thời gian: {ev.StartTime:yyyy-MM-dd HH:mm} - {ev.EndTime:yyyy-MM-dd HH:mm} ({ev.DurationHours:F2} giờ)\n";
                        eventData += $"  Danh mục: {ev.CategoryEventId}, Ngân sách: {ev.AmountBudget:N0} VNĐ\n";
                        eventData += $"  Địa điểm: {ev.Placed}\n";
                        eventData += $"  Khẩu hiệu: {ev.SloganEvent}\n";
                        eventData += $"  Mục tiêu: {ev.Goals}\n";
                        eventData += $"  Đối tượng: {ev.TargetAudience}\n";
                        eventData += $"  Số người tham gia: {ev.SizeParticipants}\n";
                        eventData += $"  Đo lường thành công: {ev.MeasuringSuccess}\n";
                        eventData += $"  Giám sát: {ev.MonitoringProcess}\n";
                        eventData += $"  Quảng bá: {ev.PromotionalPlan}\n";
                        eventData += "  Nhiệm vụ:\n";
                        foreach (var task in ev.Tasks)
                        {
                            eventData += $"    - {task.TaskName}: {task.TaskDescription}, Ngân sách: {task.AmountBudget} VNĐ, Thời hạn: {task.Deadline:yyyy-MM-dd HH:mm}\n";
                            eventData += "      Nhiệm vụ con:\n";
                            foreach (var subTask in task.SubTasks)
                            {
                                eventData += $"        - {subTask.SubTaskName}: {subTask.SubTaskDescription}, Ngân sách: {subTask.AmountBudget} VNĐ, Thời hạn: {subTask.Deadline:yyyy-MM-dd HH:mm}\n";
                            }
                        }
                        eventData += "  Chi phí:\n";
                        foreach (var cost in ev.CostBreakdowns)
                        {
                            eventData += $"    - {cost.Name}: Số lượng: {cost.Quantity}, Giá: {cost.PriceByOne} VNĐ, Tổng: {cost.Total} VNĐ\n";
                        }
                        eventData += "  Rủi ro:\n";
                        foreach (var risk in ev.Risks)
                        {
                            eventData += $"    - {risk.Name}: Lý do: {risk.Reason}, Mô tả: {risk.Description}, Giải pháp: {risk.Solution}\n";
                        }
                        eventData += "  Hoạt động:\n";
                        foreach (var activity in ev.Activities)
                        {
                            eventData += $"    - {activity.Name}: {activity.Content}\n";
                        }
                        eventData += "\n";
                    }

                    fullPrompt = $"{eventData}\n" +
                                 $"Yêu cầu từ người dùng: {request.Prompt}\n" +
                                 $"Trả về kết quả bằng tiếng Việt, thời gian phải luôn ở tương lai 2 tháng (sau ngày hiện tại {DateTime.Now:yyyy-MM-dd}), " +
                                 $"ngân sách thực tế (phù hợp với sự kiện tại trường đại học), và dưới dạng JSON với các trường sau:\n" +
                                 $"- EventTitle (string): Tiêu đề sự kiện\n" +
                                 $"- EventDescription (string): Mô tả chi tiết sự kiện\n" +
                                 $"- StartTime (string, định dạng 'yyyy-MM-dd HH:mm'): Thời gian bắt đầu\n" +
                                 $"- EndTime (string, định dạng 'yyyy-MM-dd HH:mm'): Thời gian kết thúc\n" +
                                 $"- CategoryEventId (int): ID danh mục sự kiện = {request.CategoryEventId}\n" +
                                 $"- Placed (string): Địa điểm tổ chức\n" +
                                 $"- SloganEvent (string): Khẩu hiệu sự kiện\n" +
                                 $"- Goals (string): Mục tiêu sự kiện\n" +
                                 $"- TargetAudience (string): Đối tượng tham gia\n" +
                                 $"- SizeParticipants (int): Số lượng người tham gia\n" +
                                 $"- MeasuringSuccess (string): Định lượng cụ thể như thế nào là sự kiện thành công, phải bao gồm một tỉ lệ phần trăm rõ ràng (ví dụ: 'Đạt ít nhất 85% phản hồi tích cực từ khảo sát sau sự kiện' hoặc 'Có ít nhất 90% người tham gia hoàn thành chương trình'). Đảm bảo tiêu chí thực tế, có thể đo lường được qua khảo sát, thống kê tham gia, hoặc kết quả hoạt động, phù hợp với mục tiêu sự kiện và bối cảnh trường Đại học FPT\n" +
                                 $"- MonitoringProcess (string): Quy trình giám sát\n" +
                                 $"- PromotionalPlan (string): Kế hoạch quảng bá\n" +
                                 $"- AmountBudget (decimal): Ngân sách tổng cho sự kiện\n" +
                                 $"- Tasks (mảng): Danh sách nhiệm vụ, mỗi nhiệm vụ bao gồm:\n" +
                                 $"  - **Quan trọng**: Deadline của nhiệm vụ con phải luôn nhỏ hơn deadline của nhiệm vụ lớn tương ứng (ví dụ: nếu nhiệm vụ có deadline là '2025-06-15 17:00', thì tất cả nhiệm vụ con phải có deadline sớm hơn, ví dụ: '2025-06-10 17:00').\n" +
                                 $"  - TaskName (string): Tên nhiệm vụ\n" +
                                 $"  - TaskDescription (string): Mô tả chi tiết nhiệm vụ\n" +
                                 $"  - Deadline (string, định dạng 'yyyy-MM-dd HH:mm'): Hạn chót nhiệm vụ\n" +
                                 $"  - AmountBudget (decimal): Ngân sách cho nhiệm vụ\n" +
                                 $"  - SubTasks (mảng): Danh sách nhiệm vụ con, mỗi nhiệm vụ con bao gồm:\n" +
                                 $"    - SubTaskName (string): Tên nhiệm vụ con\n" +
                                 $"    - SubTaskDescription (string): Mô tả chi tiết nhiệm vụ con\n" +
                                 $"    - Deadline (string, định dạng 'yyyy-MM-dd HH:mm'): Hạn chót của nhiệm vụ con, phải nhỏ hơn deadline của nhiệm vụ lớn\n" +
                                 $"    - AmountBudget (decimal): Ngân sách cho nhiệm vụ con\n" +
                                 $"- BudgetRows (mảng): Danh sách chi phí, mỗi mục bao gồm:\n" +
                                 $"  - Name (string): Tên mục chi phí\n" +
                                 $"  - Quantity (int): Số lượng\n" +
                                 $"  - Price (decimal): Giá mỗi đơn vị\n" +
                                 $"  - Total (decimal): Tổng chi phí (Quantity * Price)\n" +
                                 $"- Risks (mảng): Danh sách rủi ro, mỗi rủi ro bao gồm:\n" +
                                 $"  - Name (string): Tên rủi ro\n" +
                                 $"  - Reason (string): Lý do rủi ro\n" +
                                 $"  - Description (string): Mô tả rủi ro\n" +
                                 $"  - Solution (string): Giải pháp cho rủi ro\n" +
                                 $"- Activities (mảng): Danh sách hoạt động, mỗi hoạt động bao gồm:\n" +
                                 $"  - Name (string): Tên hoạt động\n" +
                                 $"  - Content (string): Nội dung hoạt động\n" +
                                 $"Lưu ý: Tất cả các số (AmountBudget, Price, Total, v.v.) phải được viết liền không có dấu phẩy phân cách hàng nghìn (ví dụ: 50000000 thay vì 50,000,000). Phản hồi phải cung cấp chi tiết đầy đủ, mô tả kỹ lưỡng kế hoạch, phù hợp với yêu cầu của người dùng và bối cảnh trường Đại học FPT.";
                }

                string suggestion = await _chatGPTService.GetSuggestion(fullPrompt);

                try
                {
                    var jsonSuggestion = JsonDocument.Parse(suggestion).RootElement;
                    return Ok(jsonSuggestion);
                }
                catch (JsonException ex)
                {
                    return Ok(new { Suggestion = suggestion, ParsingError = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }

        public class SuggestionRequest
        {
            public string Prompt { get; set; }
            public int? CategoryEventId { get; set; }
        }
    }
}