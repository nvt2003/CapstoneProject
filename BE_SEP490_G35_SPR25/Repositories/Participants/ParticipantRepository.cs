using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Participants
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly PlanifyContext _context;

        public ParticipantRepository(PlanifyContext context)
        {
            _context = context;
        }

        public (List<Participant> Participants, int TotalCount) GetParticipantsWithDetails(int eventId, int pageNumber, int pageSize)
        {
            var query = _context.Participants
                .Include(p => p.User)
                .Where(p => p.EventId == eventId)
                .OrderByDescending(p => p.RegistrationTime);

            int totalCount = query.Count();
            var participants = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (participants, totalCount);
        }

        public void RegisterParticipant(Participant participant)
        {
            _context.Participants.Add(participant);
            _context.SaveChanges();
        }

        public List<Participant> GetRegisteredEvents(Guid userId)
        {
            return _context.Participants
                .Include(p => p.Event).ThenInclude(e=>e.EventMedia).ThenInclude(em=>em.Media)
                .Include(p=>p.Event).ThenInclude(e=>e.FavouriteEvents.Where(fe => fe.UserId == userId))
                .Where(p => p.UserId == userId)
                .ToList();
        }

        public bool EventExists(int eventId)
        {
            return _context.Events.Any(e => e.Id == eventId);
        }

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(u => u.Id == userId);
        }

        public bool IsAlreadyRegistered(int eventId, Guid userId)
        {
            return _context.Participants.Any(p => p.EventId == eventId && p.UserId == userId);
        }
        public bool IsOrganizer(Guid userId, int eventId)
        {
            var eventEntity = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (eventEntity != null && eventEntity.CreateBy == userId)
                return true;

            var userProjects = _context.JoinProjects
                .Where(jp => jp.UserId == userId && jp.EventId == eventId)
                .ToList();

            if (!userProjects.Any())
                return false;

            return userProjects.Any(jp => jp.TimeOutProject == null);
        }
        public bool UnregisterParticipant(int eventId, Guid userId)
        {
            var participant = _context.Participants
                .FirstOrDefault(p => p.EventId == eventId && p.UserId == userId);

            if (participant == null)
                return false;

            _context.Participants.Remove(participant);
            _context.SaveChanges();
            return true;
        }
    }
}
