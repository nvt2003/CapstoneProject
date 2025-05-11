//using Microsoft.Extensions.Hosting;
//using Planify_BackEnd.DTOs;
//using Planify_BackEnd.DTOs.Groups;
//using Planify_BackEnd.DTOs.Implementers;
//using Planify_BackEnd.DTOs.Tasks;
//using Planify_BackEnd.Models;
//using Planify_BackEnd.Repositories;
//using Planify_BackEnd.Repositories.Groups;
//using System.Text.RegularExpressions;

//namespace Planify_BackEnd.Services.Groups
//{
//    public class GroupService : IGroupService
//    {
//        private readonly IGroupRepository _groupRepository;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        public GroupService(IGroupRepository groupRepository, IHttpContextAccessor httpContextAccessor)
//        {
//            _groupRepository = groupRepository;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        public bool AllocateCostToGroup(int groupId, decimal cost)
//        {
//            try
//            {
//                _groupRepository.AllocateCostToGroup(groupId, cost);
//                return true;
//            }catch (Exception ex)
//            {
//                Console.WriteLine("group service - allocate cost to group: "+ex.Message);
//                return false;
//            }
//        }
//        public async Task<ResponseDTO> CreateGroupAsync(GroupCreateRequestDTO groupDTO, Guid organizerId)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(groupDTO.GroupName))
//                {
//                    return new ResponseDTO(400, "Name of group is required.", null);
//                }

//                var newGroup = new Models.Group
//                {
//                    GroupName = groupDTO.GroupName,
//                    CreateBy = organizerId,
//                    EventId = groupDTO.EventId,
//                    AmountBudget = groupDTO.AmountBudget,
//                };
//                try
//                {
//                    await _groupRepository.CreateGroupAsync(newGroup);
//                }
//                catch (Exception ex)
//                {
//                    return new ResponseDTO(500, "Error orcurs while creating group!", ex.Message);
//                }

//                return new ResponseDTO(201, "Group creates successfully!", newGroup);
//            }
//            catch (Exception ex)
//            {
//                return new ResponseDTO(500, "Error orcurs while creating group!", ex.Message);
//            }
//        }

//        public bool AddLeadGroup(int GroupId, Guid ImplementerId)
//        {
//            try
//            {
//                return _groupRepository.AddLeadGroup(GroupId, ImplementerId);
//            }catch
//            {
//                return false;
//            }
//        }

//        public bool RemoveLeadGroup(int GroupId, Guid ImplementerId)
//        {
//            try
//            {
//                return _groupRepository.RemoveLeadGroup(GroupId, ImplementerId);
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<GroupDTO> UpdateGroupAsync(GroupDTO group)
//        {
//            try
//            {
//                Models.Group newGroup = new Models.Group
//                {
//                    Id = group.Id,
//                    AmountBudget = group.AmountBudget,
//                    CreateBy = group.CreateBy,
//                    EventId = group.EventId,
//                    GroupName = group.GroupName,
//                };
//                newGroup = await _groupRepository.UpdateGroupAsync(newGroup);
//                if (newGroup == null|| newGroup.Id==0)
//                {
//                    throw new Exception();
//                }
//                return group;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("group service - update group: " + ex.Message);
//                return new GroupDTO();
//            }
//        }

//        public async Task<bool> DeleteGroupAsync(int GroupId)
//        {
//            try
//            {
//                await _groupRepository.DeleteGroupAsync(GroupId);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("group service - delete group: " + ex.Message);
//                return false;
//            }
//        }

//        public async Task<GroupVM> GetGroupByIdAsync(int GroupId)
//        {
//            try
//            {
//                Models.Group group = await _groupRepository.GetGroupByIdAsync(GroupId);
//                GroupVM groupVm = new GroupVM
//                {
//                    Id = group.Id,
//                    AmountBudget = group.AmountBudget,
//                    CreateBy = group.CreateBy,
//                    EventId = group.EventId,
//                    GroupName = group.GroupName,
//                    CreateByNavigation = new DTOs.Users.UserNameVM
//                    {
//                        Id = group.CreateByNavigation.Id,
//                        Email = group.CreateByNavigation.Email,
//                        FirstName = group.CreateByNavigation.FirstName,
//                        LastName = group.CreateByNavigation.LastName,
//                    },
//                    JoinGroups = group.JoinGroups==null? new List<JoinGroupVM>() : group.JoinGroups.Select(jg => new JoinGroupVM
//                    {
//                        Id = jg.Id,
//                        ImplementerId = jg.ImplementerId,
//                        Status = jg.Status,
//                        TimeJoin = jg.TimeJoin,
//                        TimeOut = jg.TimeOut,
//                        Implementer = new DTOs.Users.UserNameVM
//                        {
//                            Id = jg.Implementer.Id,
//                            Email = jg.Implementer.Email,
//                            FirstName = jg.Implementer.FirstName,
//                            LastName = jg.Implementer.LastName,

//                        }
//                    }).ToList(),
//                    Tasks = group.Tasks == null ? new List<TaskSearchResponeDTO>() : group.Tasks.Select(jg => new TaskSearchResponeDTO
//                    {
//                        AmountBudget = jg.AmountBudget,
//                        Deadline = jg.Deadline,
//                        GroupId = jg.GroupId,
//                        Id = jg.Id,
//                        Progress = jg.Progress,
//                        StartTime = jg.StartTime,
//                        Status = jg.Status,
//                        TaskDescription = jg.TaskDescription,
//                        TaskName = jg.TaskName
//                    }).ToList(),
//                };
//                return groupVm;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("group service - get group by id: " + ex.Message);
//                return new GroupVM();
//            }
//        }

//        public async Task<bool> CheckLeadGroup(Guid userId, int groupId)
//        {
//            try
//            {
//                return await _groupRepository.CheckLeadGroup(userId, groupId);
//            }catch(Exception ex)
//            {
//                return false;
//            }
//        }
//    }
//}
