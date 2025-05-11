using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.DTOs;

namespace Planify_BackEnd.Services.Participants
{
    public interface IParticipantService
    {
        ResponseDTO GetParticipantCount(int eventId, int pageNumber, int pageSize);
        ResponseDTO RegisterParticipant(RegisterEventDTO registerDto);
        ResponseDTO GetRegisteredEvents(Guid userId);
        ResponseDTO UnregisterParticipant(RegisterEventDTO unregisterDto);
        ResponseDTO IsAlreadyRegistered(int eventId, Guid userId);
    }
}
