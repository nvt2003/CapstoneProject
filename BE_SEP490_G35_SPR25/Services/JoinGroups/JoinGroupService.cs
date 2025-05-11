//using Planify_BackEnd.DTOs;
//using Planify_BackEnd.DTOs.JoinGroups;
//using Planify_BackEnd.Models;
//using Planify_BackEnd.Repositories.JoinGroups;

//namespace Planify_BackEnd.Services.JoinGroups
//{
//    public class JoinGroupService : IJoinGroupService
//    {
//        private readonly IJoinGroupRepository _joinGroupRepository;
//        private readonly IJoinProjectRepository _joinProjectRepository;

//        public JoinGroupService(IJoinGroupRepository joinGroupRepository, IJoinProjectRepository joinProjectRepository)
//        {
//            _joinGroupRepository = joinGroupRepository;
//            _joinProjectRepository = joinProjectRepository;
//        }

//        public async Task<ResponseDTO> AddImplementersToGroup(JoinGroupRequestDTO request)
//        {
//            var group = await _joinGroupRepository.GetGroupById(request.GroupId);
//            if (group == null)
//                return new ResponseDTO(400, "Group không tồn tại!", null);

//            var failedImplementers = new List<Guid>();
//            var successfulImplementers = new List<Guid>();
//            var isNotInProject = new List<Guid>();

//            foreach (var implementerId in request.ImplementerIds)
//            {
//                bool isInGroup = await _joinGroupRepository.IsImplementerInGroup(implementerId, request.GroupId);
//                if (isInGroup)
//                {
//                    failedImplementers.Add(implementerId);
//                    continue;
//                }

//                bool isInGroup = await _joinGroupRepository.IsImplementerInGroup(implementerId, request.GroupId);
//                if (isInGroup)
//                {
//                    if (successfulImplementers.Count > 0)
//                    {
//                        bool result = await _joinGroupRepository.AddImplementersToGroup(successfulImplementers, request.GroupId);
//                        if (result)
//                        {
//                            foreach (var implementerId in successfulImplementers)
//                            {
//                                bool isInProject = await _joinGroupRepository.IsUserInProject(implementerId, group.EventId);
//                                if (!isInProject)
//                                {
//                                    isNotInProject.Add(implementerId);
//                                    continue;
//                                }
//                            }
//                            await _joinProjectRepository.AddImplementersToProject(isNotInProject, group.EventId);
//                            await _joinProjectRepository.AddRoleImplementers(successfulImplementers);
//                            var successMessage = $"Đã thêm {successfulImplementers.Count} Implementers vào Group thành công!";
//                            return new ResponseDTO(200, successMessage, successfulImplementers);
//                        }
//                    }
//                    {
//                        await _joinProjectRepository.AddImplementersToProject(successfulImplementers, group.EventId);
//                        await _joinProjectRepository.AddRoleImplementers(successfulImplementers);
//                        var successMessage = $"Đã thêm {successfulImplementers.Count} Implementers vào Group thành công!";
//                        return new ResponseDTO(200, successMessage, successfulImplementers);
//                    }
//                }

//                if (failedImplementers.Count == request.ImplementerIds.Count)
//                {
//                    return new ResponseDTO(400, "Tất cả Implementers không thể thêm vào Group!", null);
//                }

//                return new ResponseDTO(500, "Có lỗi xảy ra khi thêm Implementers vào Group!", null);
//            }
//        }
//    }
