//using Microsoft.EntityFrameworkCore;
//using Planify_BackEnd.Models;

//namespace Planify_BackEnd.Repositories.JoinGroups
//{
//    public class JoinGroupRepository : IJoinGroupRepository
//    {
//        private readonly PlanifyContext _context;

//        public JoinGroupRepository(PlanifyContext context)
//        {
//            _context = context;
//        }

//        public async Task<Group?> GetGroupById(int groupId)
//        {
//            return await _context.Groups.FindAsync(groupId);
//        }

//        public async Task<bool> IsImplementerInGroup(Guid implementerId, int groupId)
//        {
//            return await _context.JoinGroups.AnyAsync(jg => jg.ImplementerId == implementerId && jg.GroupId == groupId);
//        }

//        public async Task<bool> AddImplementersToGroup(List<Guid> implementerIds, int groupId)
//        {
//            if (implementerIds == null || !implementerIds.Any())
//            {
//                return false; // Không có implementer nào để thêm
//            }

//            try
//            {
//                var joinGroups = implementerIds.Select(implementerId => new JoinGroup
//                {
//                    ImplementerId = implementerId,
//                    GroupId = groupId,
//                    TimeJoin = DateTime.Now,
//                    Status = 1 // Active
//                }).ToList();

//                _context.JoinGroups.AddRange(joinGroups);
//                var result = await _context.SaveChangesAsync();

//                return result > 0;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                return false;
//            }
//        }

//        public async Task<bool> IsUserInProject(Guid userId, int eventId)
//        {
//            return await _context.JoinProjects.AnyAsync(jp => jp.UserId == userId && jp.EventId == eventId);
//        }
//    }
//}
