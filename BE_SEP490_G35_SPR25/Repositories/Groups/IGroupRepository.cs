//using Planify_BackEnd.Models;
//﻿namespace Planify_BackEnd.Repositories.Groups
//{
//    public interface IGroupRepository
//    {
//        bool AllocateCostToGroup(int groupId, decimal cost);
//        Task<Group> CreateGroupAsync(Group newGroup);
//        bool IsGroupExists(int groupId);
//        System.Threading.Tasks.Task<bool> AddImplementerToGroupAsync(JoinGroup joinGroup);
//        bool AddLeadGroup(int GroupId, Guid ImplementerId);
//        bool RemoveLeadGroup(int GroupId, Guid ImplementerId);
//        Task<Group> UpdateGroupAsync(Group group);
//        Task<bool> DeleteGroupAsync(int GroupId);
//        Task<Group> GetGroupByIdAsync(int GroupId);
//        Task<bool> CheckLeadGroup(Guid userId, int groupId);
//    }
//}
