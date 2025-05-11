using Microsoft.AspNetCore.Http.HttpResults;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Medias;
using Planify_BackEnd.DTOs.User;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Repositories.Address;
using Planify_BackEnd.Repositories.User;
using System.IdentityModel.Tokens.Jwt;

namespace Planify_BackEnd.Services.User
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IProvinceRepository _provinceRepository;
        public ProfileService(IProfileRepository profileRepository,IProvinceRepository provinceRepository)
        {
            _profileRepository = profileRepository;
            _provinceRepository = provinceRepository;
        }
        public ProfileViewModel getUserProfileById(Guid id)
        {
            try
            {
                Models.User user= _profileRepository.GetUserProfileById(id);
                ProfileViewModel profileViewModel = new ProfileViewModel
                {
                    Id = user.Id,
                    AvatarId = user.AvatarId,
                    Avatar = user.Avatar==null? null: new MediumDTO
                    {
                        Id = user.Avatar.Id,
                        MediaUrl = user.Avatar.MediaUrl
                    },
                    CampusDTO = user.Campus == null ? null : new DTOs.Campus.CampusDTO
                    {
                        Id = user.Campus.Id,
                        CampusName = user.Campus.CampusName,
                        Status = user.Campus.Status
                    },
                    CampusId = user.CampusId,
                    DateOfBirth = user.DateOfBirth,
                    CreatedAt = user.CreatedAt,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.UserName,
                    Password = user.Password,
                    Gender = user.Gender,
                    AddressId = user.AddressId,
                    AddressVM = user.Address==null? null: new DTOs.Andress.AddressVM
                    {
                        Id=user.Address.Id,
                        AddressDetail = user.Address.AddressDetail,
                        WardVM = new DTOs.Andress.WardVM
                        {
                            Id = user.Address.Ward.Id,
                            WardName = user.Address.Ward.WardName,
                            DistrictVM = new DTOs.Andress.DistrictVM
                            {
                                Id = user.Address.Ward.District.Id,
                                DistrictName = user.Address.Ward.District.DistrictName,
                                ProvinceVM = new DTOs.Andress.ProvinceVM
                                {
                                    Id = user.Address.Ward.District.Province.Id,
                                    ProvinceName = user.Address.Ward.District.Province.ProvinceName
                                }
                            },

                        }
                    },
                    UserRoleDTO = user.UserRoles==null?new List<UserRoleDTO>():user.UserRoles.Select(ur=>new UserRoleDTO
                    {
                        Id = ur.Id,
                        RoleId = ur.RoleId,
                        UserId = ur.UserId,
                        RoleDTO = new DTOs.Roles.RoleDTO
                        {
                            Id = ur.Role.Id,
                            RoleName = ur.Role.RoleName,
                        }
                    }).ToList(),
                    Status = user.Status
                };
                return profileViewModel;
            }catch(Exception ex)
            {
                Console.WriteLine("profile service - getUserProfileById: "+ex.Message);
                return new ProfileViewModel();
            }
        }

        public bool UpdateAvatar(Guid id, int avatarId)
        {
            try
            {
                return _profileRepository.UpdateAvatar(id, avatarId);
            }
            catch
            {
                return false;
            }
        }

        public ResponseDTO UpdateProfile(ProfileUpdateModel updateProfile)
        {
            try
            {
                var p = _profileRepository.GetUserProfileById(updateProfile.Id);
                //check if need update address
                if (updateProfile.addressVM.Id != 0||!updateProfile.addressVM.AddressDetail.Equals(""))
                {
                    bool isUpdateAddressSuccess = false;
                    if (p.AddressId == null)
                    {
                        Models.Address createAddress = new Models.Address
                        {
                            Id = 0,
                            AddressDetail = updateProfile.addressVM.AddressDetail,
                            WardId = updateProfile.addressVM.WardVM.Id
                        };
                        var addressId = _provinceRepository.CreateAddress(createAddress);
                        if (addressId == 0) return new ResponseDTO(400, "Error update address!", null);
                        updateProfile.AddressId = addressId;
                        isUpdateAddressSuccess = true;
                    }
                    if (isUpdateAddressSuccess == false)
                    {
                        if (updateProfile.addressVM.WardVM.Id != p.Address.Ward.Id
                        || !updateProfile.addressVM.AddressDetail.Equals(p.Address.AddressDetail))
                        {
                            Models.Address updateAddress = new Models.Address
                            {
                                Id = p.Address.Id,
                                AddressDetail = updateProfile.addressVM.AddressDetail,
                                WardId = updateProfile.addressVM.WardVM.Id
                            };
                            isUpdateAddressSuccess = _provinceRepository.UpdateAddress(updateAddress);
                        }
                        else
                        {
                            isUpdateAddressSuccess = true;
                        }
                    }

                    if (!isUpdateAddressSuccess) return new ResponseDTO(400, "Error update address!", null);
                }
                var profile = _profileRepository.UpdateProfile(updateProfile);
                ProfileUpdateModel updatedprofile = new ProfileUpdateModel
                {
                    Id = profile.Id,
                    AddressId = profile.AddressId,
                    DateOfBirth = profile.DateOfBirth,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Gender = profile.Gender,
                    PhoneNumber = profile.PhoneNumber,
                };
                return new ResponseDTO(200, "Updated successfully!", updatedprofile);
            }
            catch
            {
                throw new Exception();
            }
        }
    }
}
