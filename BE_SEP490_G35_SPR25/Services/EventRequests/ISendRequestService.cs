using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.SendRequests;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.EventRequests
{
    public interface ISendRequestService
    {
        Task<ResponseDTO> GetRequestsAsync(int campusId);
        Task<ResponseDTO> CreateRequestAsync(SendRequestDTO requestDTO);
        Task<ResponseDTO> ApproveRequestAsync(int id, Guid managerId, string reason);
        Task<ResponseDTO> RejectRequestAsync(int id, Guid managerId, string reason);
        Task<ResponseDTO> GetMyRequestsAsync(Guid userId, int campusId);
        Task<PageResultDTO<GetSendRequestDTO>> SearchRequest(int page, int pageSize, int campusId, string? eventTitle, int? status,Guid? userId,int? requestStatus);
    }
}
