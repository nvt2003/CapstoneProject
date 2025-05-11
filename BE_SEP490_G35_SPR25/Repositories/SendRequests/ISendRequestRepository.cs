using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SendRequests;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.SendRequests
{
    public interface ISendRequestRepository
    {
        Task<List<SendRequestWithEventDTO>> GetRequestsAsync(int campusId);
        Task<SendRequest> GetRequestByIdAsync(int id);
        Task<SendRequest> CreateRequestAsync(SendRequest request);
        System.Threading.Tasks.Task UpdateRequestAsync(SendRequest request);
        Task<List<SendRequest>> GetRequestsByUserIdAsync(Guid userId, int campusId);
        Task<PageResultDTO<SendRequest>> SearchRequest(int page, int pageSize, int campusId,string? eventTitle, int? status, Guid? userId, int? requestStatus);
    }
}
