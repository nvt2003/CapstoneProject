using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.JoinGroups;

namespace Planify_BackEnd.Services.JoinGroups
{
    public interface IJoinGroupService
    {
        Task<ResponseDTO> AddImplementersToGroup(JoinGroupRequestDTO request);
    }
}
