//using Microsoft.EntityFrameworkCore;
//using Planify_BackEnd.Models;
//namespace Planify_BackEnd.Repositories.Groups
//{
//    public class GroupRepository : IGroupRepository
//    {
//        private readonly PlanifyContext _context;
//        public GroupRepository(PlanifyContext context)
//        {
//            _context = context;
//        }
//        public bool AllocateCostToGroup(int groupId, decimal cost)
//        {
//            try
//            {
//                var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
//                group.AmountBudget = cost;
//                _context.Groups.Update(group);
//                _context.SaveChanges();
//                return true;
//            }catch (Exception ex)
//            {
//                return false;
//            }
//        }
//        public async Task<Group> CreateGroupAsync(Group newGroup)
//        {
//            try
//            {
//                await _context.Groups.AddAsync(newGroup);
//                await _context.SaveChangesAsync();
//                return newGroup;
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("An unexpected error occurred.", ex);
//            }
//        }
//        public bool IsGroupExists(int groupId)
//        {
//            return _context.Groups.Any(g => g.Id == groupId);
//        }
//        public async System.Threading.Tasks.Task<bool> AddImplementerToGroupAsync(JoinGroup joinGroup)
//        {
//            bool exists = await _context.JoinGroups
//                            .AnyAsync(jg => jg.GroupId == joinGroup.GroupId && jg.ImplementerId == joinGroup.ImplementerId);          
//            if (!exists)
//            {
//                _context.JoinGroups.Add(joinGroup);
//                await _context.SaveChangesAsync();
//                return true;
//            } else
//            {
//                return false;
//            }
//        }
//        public bool AddLeadGroup(int GroupId, Guid ImplementerId)
//        {
//            try
//            {
//                var joinGroup = _context.JoinGroups.FirstOrDefault(jg => jg.GroupId == GroupId && jg.ImplementerId == ImplementerId);
//                joinGroup.Status = -1;
//                _context.Update(joinGroup);
//                _context.SaveChanges();
//                return true;
//            }catch (Exception ex)
//            {
//                return false;
//            }
//        }
//        public bool RemoveLeadGroup(int GroupId, Guid ImplementerId)
//        {
//            try
//            {
//                var joinGroup = _context.JoinGroups.FirstOrDefault(jg => jg.GroupId == GroupId && jg.ImplementerId == ImplementerId);
//                joinGroup.Status = 1;
//                _context.Update(joinGroup);
//                _context.SaveChanges();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }

//        public async Task<Group> UpdateGroupAsync(Group group)
//        {
//            try
//            {
//                _context.Update(group);
//                await _context.SaveChangesAsync();
//                return group;
//            }catch(Exception ex)
//            {
//                Console.WriteLine("group repository - update group: "+ ex.Message);
//                return new Group();
//            }
//        }

//        public async Task<bool> DeleteGroupAsync(int GroupId)
//        {
//            try
//            {
//                var group = await _context.Groups
//                    .Include(g=>g.JoinGroups)
//                    .Include(g=>g.Tasks)
//                    .FirstOrDefaultAsync(g=>g.Id == GroupId);
//                group.JoinGroups = null;
//                group.Tasks = null;
//                _context.Groups.Remove(group); 
//                await _context.SaveChangesAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("group repository - delete group: " + ex.Message);
//                return false;
//            }
//        }

//        public async Task<Group> GetGroupByIdAsync(int GroupId)
//        {
//            try
//            {
//                var group = await _context.Groups
//                    .Include(g=>g.CreateByNavigation)
//                    .Include(g => g.JoinGroups).ThenInclude(jg=>jg.Implementer)
//                    .Include(g => g.Tasks)
//                    .FirstOrDefaultAsync(g => g.Id == GroupId);
//                return group;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("group repository - get group detail : " + ex.Message);
//                return new Group();
//            }
//        }

//        public async Task<bool> CheckLeadGroup(Guid userId, int groupId)
//        {
//            try
//            {
//                var userJoin = await _context.JoinGroups.FirstOrDefaultAsync(jg => jg.GroupId==groupId);
//                if (userJoin.Status == -1)
//                    return true;
//                return false;
//            }catch (Exception ex)
//            {
//                return false;
//            }
//        }
//    }
//}
