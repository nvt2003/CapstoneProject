using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SendRequests;
using Planify_BackEnd.Models;
using System;
using System.Globalization;
using System.Text;

namespace Planify_BackEnd.Repositories.SendRequests
{
    public class SendRequestRepository : ISendRequestRepository
    {
        private readonly PlanifyContext _context;

        public SendRequestRepository(PlanifyContext context)
        {
            _context = context;
        }

        public async Task<List<SendRequestWithEventDTO>> GetRequestsAsync(int campusId)
        {
            return await _context.SendRequests
                .Include(sr => sr.Event)
                .Where(sr=>sr.Event.CampusId==campusId)
                .Select(sr => new SendRequestWithEventDTO
                {
                    Id = sr.Id,
                    EventId = sr.EventId,
                    Reason = sr.Reason,
                    Status = sr.Event.Status,
                    ManagerId = sr.ManagerId,
                    EventTitle = sr.Event.EventTitle,
                    EventStartTime = sr.Event.StartTime,
                    EventEndTime = sr.Event.EndTime,
                    requestStatus = sr.Status
                })
                .ToListAsync();
        }

        public async Task<SendRequest> GetRequestByIdAsync(int id)
        {
            try
            {
                return await _context.SendRequests
                    .Include(sr => sr.Event).FirstOrDefaultAsync(sr => sr.Id == id);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SendRequest> CreateRequestAsync(SendRequest request)
        {
            _context.SendRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async System.Threading.Tasks.Task UpdateRequestAsync(SendRequest request)
        {
            _context.SendRequests.Update(request);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SendRequest>> GetRequestsByUserIdAsync(Guid userId, int campusId)
        {
            return await _context.SendRequests
                .Include(sr => sr.Event)
                .Where(sr => sr.Event.CreateBy == userId &&
                    sr.Event.CampusId == campusId)
                .ToListAsync();
        }

        public async Task<PageResultDTO<SendRequest>> SearchRequest(int page, int pageSize, int campusId, string? eventTitle, int? status, Guid? userId, int? requestStatus)
        {
            try
            {
                var query = await _context.SendRequests
                    .Include(sr=>sr.Event)
                    .Where(r =>
                        r.Event.CampusId == campusId &&
                        (!status.HasValue || r.Event.Status == status) &&
                        (!requestStatus.HasValue || r.Status == requestStatus) &&
                        (!userId.HasValue || r.Event.CreateBy.Equals(userId))
                    )
                    .ToListAsync();

                if (!string.IsNullOrEmpty(eventTitle))
                {
                    string normalizedEventName = RemoveDiacritics(eventTitle).ToLower();

                    query = query
                        .Where(r => !string.IsNullOrEmpty(r.Event.EventTitle) &&
                            RemoveDiacritics(r.Event.EventTitle).ToLower().Contains(normalizedEventName))
                        .ToList();
                }
                var totalCount = query.Count;

                var data = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                return new PageResultDTO<SendRequest>(data, totalCount, page,pageSize);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
