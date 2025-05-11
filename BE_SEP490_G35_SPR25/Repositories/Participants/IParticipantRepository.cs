using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Participants
{
    public interface IParticipantRepository
    {
        (List<Participant> Participants, int TotalCount) GetParticipantsWithDetails(int eventId, int pageNumber, int pageSize);
        void RegisterParticipant(Participant participant);
        List<Participant> GetRegisteredEvents(Guid userId);
        bool EventExists(int eventId);
        bool UserExists(Guid userId);
        bool IsAlreadyRegistered(int eventId, Guid userId);
        bool IsOrganizer(Guid userId, int eventId);
        bool UnregisterParticipant(int eventId, Guid userId);
    }
}
