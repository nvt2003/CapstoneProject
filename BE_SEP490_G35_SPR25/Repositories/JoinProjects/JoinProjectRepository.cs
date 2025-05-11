using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Planify_BackEnd.Repositories.JoinGroups
{
    public class JoinProjectRepository : IJoinProjectRepository
    {
        private readonly PlanifyContext _context;

        public JoinProjectRepository(PlanifyContext context)
        {
            _context = context;
        }

        public PageResultDTO<JoinProject> JoiningEvents(Guid userId, int page, int pageSize)
        {
            try
            {
                var count = _context.JoinProjects
                    .Where(jp => jp.UserId == userId && jp.TimeOutProject == null)
                    .Count();
                if (count == 0)
                    return new PageResultDTO<JoinProject>(new List<JoinProject>(), count, page, pageSize);
                var list = _context.JoinProjects
                    .Include(jp => jp.Event)
                    .Include(jp => jp.User)
                    .Where(jp => jp.UserId == userId && jp.TimeOutProject == null)
                    .Skip((page - 1) * pageSize).Take(pageSize)
                    .ToList();
                return new PageResultDTO<JoinProject>(list, count, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public PageResultDTO<JoinProject> AttendedEvents(int page, int pageSize, Guid userId)
        {
            try
            {
                var now = DateTime.Now;
                var count =  _context.JoinProjects
                    .Include(jp=>jp.Event)
                    .Where(jp => jp.UserId == userId &&
                                 jp.Event.Status == 2)
                    .Distinct()
                    .Count();
                if (count == 0)
                    return new PageResultDTO<JoinProject>(new List<JoinProject>(), count, page, pageSize);
                var list = _context.JoinProjects
                    .Include(jp => jp.Event).ThenInclude(jp=>jp.CategoryEvent)
                    .Include(jp => jp.User)
                    .Where(jp => jp.UserId == userId &&
                    jp.Event.Status == 2)
                    .Distinct()
                    .OrderByDescending(j => j.TimeJoinProject)
                    .Skip((page - 1) * pageSize).Take(pageSize)
                    .ToList();
                return new PageResultDTO<JoinProject>(list, count, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> DeleteImplementerFromEvent(Guid userId, int eventId)
        {
            try
            {
                var joinProject = _context.JoinProjects.FirstOrDefault(jp => jp.UserId == userId && jp.EventId == eventId && jp.TimeOutProject == null);
                joinProject.TimeOutProject = DateTime.Now;
                _context.JoinProjects.Update(joinProject);
                await _context.SaveChangesAsync();
                return true;
            }catch
            {
                return false;
            }
        }

        public async Task<bool> AddImplementersToProject(List<Guid> implementerIds, int eventId)
        {
            if (implementerIds == null || !implementerIds.Any())
            {
                return false;
            }

            try
            {
                var joinProjects = implementerIds.Select(implementerId => new JoinProject
                {
                    UserId = implementerId,
                    EventId = eventId,
                    TimeJoinProject = DateTime.Now,
                }).ToList();

                _context.JoinProjects.AddRange(joinProjects);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> AddImplementerToProject(Guid implementerId, int eventId)
        {
            if (implementerId == Guid.Empty)
            {
                return false;
            }

            try
            {
                var joinProjects =  new JoinProject
                {
                    UserId = implementerId,
                    EventId = eventId,
                    TimeJoinProject = DateTime.Now,
                };

                _context.JoinProjects.AddRange(joinProjects);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> AddRoleImplementer(Guid implementerId)
        {
            if (implementerId == Guid.Empty)
            {
                return false;
            }

            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == implementerId && ur.RoleId == 4);

                if (userRole == null)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = implementerId,
                        RoleId = 4
                    });
                }
                else
                {
                    return true;
                }

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> AddRoleImplementers(List<Guid> implementerIds)
        {
            if (implementerIds == null || !implementerIds.Any())
            {
                return false;
            }

            const int implementerRoleId = 4; // Nên lấy từ config hoặc db một lần

            try
            {
                // Lấy tất cả UserRole hiện có cho role Implementer
                var existingUserRoles = await _context.UserRoles
                    .Where(ur => ur.RoleId == implementerRoleId && implementerIds.Contains(ur.UserId))
                    .ToListAsync();

                var newUserRolesToAdd = new List<UserRole>();
                foreach (var implementerId in implementerIds)
                {
                    if (!existingUserRoles.Any(ur => ur.UserId == implementerId))
                    {
                        newUserRolesToAdd.Add(new UserRole
                        {
                            UserId = implementerId,
                            RoleId = implementerRoleId
                        });
                    }
                }

                if (newUserRolesToAdd.Any())
                {
                    _context.UserRoles.AddRange(newUserRolesToAdd);
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }

                return true; // Trả về true nếu không có gì để thêm (tất cả đã có role)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi thêm Implementer Roles: {ex.Message}");
                // Ghi log lỗi chi tiết hơn ở đây
                return false;
            }
        }

        public async Task<bool> EventExistsAsync(int eventId)
        {
            return await _context.Events.AnyAsync(e => e.Id == eventId);
        }

        public async Task<List<Guid>> GetInvalidUserIdsAsync(List<Guid> userIds)
        {
            return userIds.Where(id => !_context.Users.Any(u => u.Id == id)).ToList();
        }

        public async Task<List<Guid>> GetExistingImplementerIdsAsync(int eventId)
        {
            return await _context.JoinProjects
                .Where(jp => jp.EventId == eventId && jp.TimeOutProject == null)
                .Select(jp => jp.UserId)
                .ToListAsync();
        }

        public async Task<PageResultDTO<JoinProject>> SearchImplementerJoinedEvent(int page, int pageSize,
            int? eventId, string? email, string? name)
        {
            try
            {
                var count = _context.JoinProjects
                    .Include(jp => jp.User)
                    .AsEnumerable()
                    .Where(jp =>
                        (!eventId.HasValue || jp.EventId == eventId) &&
                        (string.IsNullOrEmpty(email) || RemoveDiacriticsAndToLower(jp.User.Email).Contains(RemoveDiacriticsAndToLower(email))) &&
                        (string.IsNullOrEmpty(name) ||
                        RemoveDiacriticsAndToLower(jp.User.FirstName).Contains(RemoveDiacriticsAndToLower(name)) ||
                        RemoveDiacriticsAndToLower(jp.User.LastName).Contains(RemoveDiacriticsAndToLower(name))) &&
                    jp.TimeOutProject == null)
                    .Count();
                var list = _context.JoinProjects
                    .Include(jp=>jp.User)
                    .AsEnumerable()
                    .Where(jp =>
                        (!eventId.HasValue || jp.EventId == eventId) &&
                        (string.IsNullOrEmpty(email) || RemoveDiacriticsAndToLower(jp.User.Email).Contains(RemoveDiacriticsAndToLower(email))) &&
                        (string.IsNullOrEmpty(name) ||
                        RemoveDiacriticsAndToLower(jp.User.FirstName).Contains(RemoveDiacriticsAndToLower(name)) ||
                        RemoveDiacriticsAndToLower(jp.User.LastName).Contains(RemoveDiacriticsAndToLower(name))) &&
                    jp.TimeOutProject == null)
                    .ToList();
                return new PageResultDTO<JoinProject>(list, count, page, pageSize);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string RemoveDiacriticsAndToLower(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLower();
        }

        public async Task<List<Guid>> GetUserIdJoinedEvent(int eventId)
        {
            try
            {
                var userIdJoinedEvent = await _context.JoinProjects
                    .Include(jp => jp.Event)
                    .Where(jp => jp.EventId == eventId &&
                    jp.TimeOutProject!=null &&
                    jp.Event.Status != -2)
                    .Select(jp => jp.UserId)
                    .ToListAsync();
                return userIdJoinedEvent;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> IsImplementerInProject(Guid implementerId, int eventId)
        {
            try
            {
                if (implementerId == Guid.Empty)
                {
                    return false;
                }

                return await _context.JoinProjects
                    .AnyAsync(jp => jp.UserId == implementerId && jp.EventId == eventId);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
