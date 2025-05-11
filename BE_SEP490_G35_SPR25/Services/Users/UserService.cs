using Azure;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.DTOs.Roles;
using Planify_BackEnd.DTOs.Users;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Planify_BackEnd.Services.Users
{
    public class UserService : IUserservice
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ResponseDTO> UpdateManagerAsync(UserUpdateDTO user, Guid id)
        {
            try
            {
                var updateUser = await _userRepository.UpdateManagerAsync(id, user);

                if (updateUser == null)
                {
                    return new ResponseDTO(400, "User not found!", null);
                }
                return new ResponseDTO(200, "User updated successfully!", updateUser);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while updating user!", ex.Message);
            }
        }
        public async Task<ResponseDTO> CreateManagerAsync(UserDTO user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    return new ResponseDTO(400, "First name is required.", null);
                }

                var check = await _userRepository.GetUserByEmailAsync(user.Email);
                if (check != null)
                {
                    return new ResponseDTO(400, "Email exists!", user);
                }
                var newUser = new Models.User
                {
                    Id = Guid.NewGuid(),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = DateTime.Now,
                    CampusId = user.CampusId,
                    Status = 1,
                    Gender = user.Gender,
                };
                try
                {
                    await _userRepository.CreateManagerAsync(newUser);
                }
                catch (Exception dbEx)
                {
                    return new ResponseDTO(500, "Database error while creating user!", dbEx.Message);
                }
                return new ResponseDTO(201, "Campus Manager creates successfully!", newUser);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public PageResultDTO<UserListDTO> GetListUser(int page, int pageSize)
        {
            try
            {
                var users = _userRepository.GetListUser(page, pageSize);
                if (users.TotalCount == 0)
                {
                    return new PageResultDTO<UserListDTO>(new List<UserListDTO>(), 0, page, pageSize);
                }
                List<UserListDTO> result = new List<UserListDTO>();
                foreach (var c in users.Items)
                {
                    UserListDTO user = new UserListDTO
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        DateOfBirth = c.DateOfBirth,
                        PhoneNumber = c.PhoneNumber,
                        AddressId = c.AddressId,
                        AvatarId = c.AvatarId,
                        CreatedAt = c.CreatedAt,
                        CampusId = c.CampusId,
                        Status = c.Status,
                        Gender = c.Gender,
                        Avatar = c.Avatar == null ? new DTOs.Medias.MediaItemDTO() :
                        new DTOs.Medias.MediaItemDTO
                        {
                            Id = c.Avatar.Id,
                            MediaUrl = c.Avatar.MediaUrl
                        },
                        RoleName = c.UserRoles.Count == 0 ? "" : c.UserRoles.First().Role.RoleName,
                    };
                    result.Add(user);
                }
                return new PageResultDTO<UserListDTO>(result, users.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public async Task<UserDetailDTO> GetUserDetailAsync(Guid id)
        {
            var c = await _userRepository.GetUserDetailAsync(id);
            if (c == null)
            {
                return null;
            }

            var userDTO = new UserDetailDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Password = c.Password,
                DateOfBirth = c.DateOfBirth,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address.AddressDetail,
                AvatarId = c.AvatarId,
                CreatedAt = c.CreatedAt,
                CampusName = c.Campus.CampusName,
                Status = c.Status,
                Gender = c.Gender ? "Male" : "Female"

            };

            return userDTO;
        }
        public async Task<ResponseDTO> UpdateUserStatusAsync(Guid id)
        {
            try
            {
                var updateUser = await _userRepository.UpdateUserStatusAsync(id);

                return new ResponseDTO(200, "User status updated successfully!", updateUser);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, "Error occurs while updating user atus!", ex.Message);
            }
        }

        public PageResultDTO<UserListDTO> GetListImplementer(int eventId, int page, int pageSize)
        {
            try
            {
                var users = _userRepository.GetListImplementer(eventId, page, pageSize);
                if (users.TotalCount == 0)
                {
                    return new PageResultDTO<UserListDTO>(new List<UserListDTO>(), 0, page, pageSize);
                }
                List<UserListDTO> result = new List<UserListDTO>();
                foreach (var c in users.Items)
                {
                    UserListDTO user = new UserListDTO
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        DateOfBirth = c.DateOfBirth,
                        PhoneNumber = c.PhoneNumber,
                        AddressId = c.AddressId,
                        AvatarId = c.AvatarId,
                        CreatedAt = c.CreatedAt,
                        CampusId = c.CampusId,
                        Status = c.Status,
                        Gender = c.Gender
                    };
                    result.Add(user);
                }
                return new PageResultDTO<UserListDTO>(result, users.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<PageResultDTO<UserListDTO>> GetUserByNameOrEmailAsync(string input, int campusId)

        {
            try
            {
                var users = await _userRepository.GetUserByNameOrEmail(input, campusId);

                if (users.TotalCount == 0)
                {
                    return new PageResultDTO<UserListDTO>(new List<UserListDTO>(), 0, 0, 0);
                }
                List<UserListDTO> result = new List<UserListDTO>();
                foreach (var c in users.Items)
                {
                    UserListDTO user = new UserListDTO
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        DateOfBirth = c.DateOfBirth,
                        PhoneNumber = c.PhoneNumber,
                        AddressId = c.AddressId,
                        AvatarId = c.AvatarId,
                        CreatedAt = c.CreatedAt,
                        CampusId = c.CampusId,
                        Status = c.Status,
                        Gender = c.Gender,
                        Avatar = c.Avatar == null ? new DTOs.Medias.MediaItemDTO() :
                        new DTOs.Medias.MediaItemDTO
                        {
                            Id = c.Avatar.Id,
                            MediaUrl = c.Avatar.MediaUrl
                        },
                        RoleName = c.UserRoles.Count == 0 ? "" : c.UserRoles.First().Role.RoleName,
                    };
                    result.Add(user);
                }
                return new PageResultDTO<UserListDTO>(result, users.TotalCount, 0, 0);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<UserRoleDTO> AddUserRole(UserRoleDTO roleDTO)
        {
            try
            {
                UserRole role = new UserRole
                {
                    Id = roleDTO.Id,
                    RoleId = roleDTO.RoleId,
                    UserId = roleDTO.UserId,
                };
                var r = await _userRepository.AddUserRole(role);
                roleDTO.Id = r.Id;
                roleDTO.UserId = r.UserId;
                roleDTO.RoleId = r.RoleId;
                return roleDTO;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO> CreateEventOrganizer(UserDTO userDTO)
        {
            try
            {
                var check = await _userRepository.GetUserByEmailAsync(userDTO.Email);
                if (check != null)
                {
                    return new ResponseDTO(400, "Email exists!", userDTO);
                }
                Models.User user = new Models.User
                {
                    Id = Guid.NewGuid(),
                    CampusId = userDTO.CampusId,
                    DateOfBirth = userDTO.DateOfBirth,
                    Email = userDTO.Email,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    PhoneNumber = userDTO.PhoneNumber,
                    Gender = userDTO.Gender,
                    Status = 1,
                    CreatedAt = DateTime.Now,
                };
                var u = await _userRepository.CreateEventOrganizer(user);
                UserListDTO userListDTO = new UserListDTO
                {
                    Id = u.Id,
                    AddressId = u.AddressId,
                    AvatarId = u.AvatarId,
                    CampusId = u.CampusId,
                    CreatedAt = u.CreatedAt,
                    DateOfBirth = u.DateOfBirth,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    UserName = u.UserName,
                    Password = u.Password,
                };
                return new ResponseDTO(200,"Create successfully!", userListDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(400, ex.Message, null);
            }
        }

        public async Task<ResponseDTO> UpdateEventOrganizer(UserDTO userDTO)
        {
            try
            {
                var check = await _userRepository.GetUserByEmailAsync(userDTO.Email);
                if (check == null)
                {
                    return new ResponseDTO(404, "Cannot found user", userDTO);
                }
                Models.User user = new Models.User
                {
                    Id = userDTO.Id,
                    CampusId = userDTO.CampusId,
                    DateOfBirth = userDTO.DateOfBirth,
                    Email = userDTO.Email,
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    PhoneNumber = userDTO.PhoneNumber,
                    Gender = userDTO.Gender,
                };
                var u = await _userRepository.UpdateEventOrganizer(user);
                UserListDTO userListDTO = new UserListDTO
                {
                    Id = u.Id,
                    AddressId = u.AddressId,
                    AvatarId = u.AvatarId,
                    CampusId = u.CampusId,
                    CreatedAt = u.CreatedAt,
                    DateOfBirth = u.DateOfBirth,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Gender = u.Gender,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    UserName = u.UserName,
                    Password = u.Password,
                };
                return new ResponseDTO(200, "Update successfully!", userListDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(400, ex.Message, null);
            }
        }

        public Task<PageResultDTO<EventOrganizerVM>> GetEventOrganizer(int page, int pageSize, int campusId)
        {
            try
            {
                var result = _userRepository.GetEventOrganizer(page, pageSize, campusId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> UpdateEOGRole(Guid userId, int roleId)
        {
            try
            {
                var result = _userRepository.UpdateRoleEOG(userId, roleId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<bool> UpdateCampusManagerRole(Guid userId, int roleId)
        {
            try
            {
                var result = _userRepository.UpdateRoleCampusManager(userId, roleId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<PageResultDTO<EventOrganizerVM>> GetCampusManager(int page, int pageSize/*, int campusId*/)
        {
            try
            {
                var result = _userRepository.GetCampusManager(page, pageSize);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageResultDTO<UserListDTO>> SearchUser(int page, int pageSize, string? input, string? roleName, int? campusId)
        {
            try
            {
                var users = await _userRepository.SearchUser(page, pageSize, input, roleName, campusId);

                if (users.TotalCount == 0)
                {
                    return new PageResultDTO<UserListDTO>(new List<UserListDTO>(), 0, page, pageSize);
                }
                List<UserListDTO> result = new List<UserListDTO>();
                foreach (var c in users.Items)
                {
                    UserListDTO user = new UserListDTO
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        DateOfBirth = c.DateOfBirth,
                        PhoneNumber = c.PhoneNumber,
                        AddressId = c.AddressId,
                        AvatarId = c.AvatarId,
                        CreatedAt = c.CreatedAt,
                        CampusId = c.CampusId,
                        Status = c.Status,
                        Gender = c.Gender,
                        Avatar = c.Avatar == null ? new DTOs.Medias.MediaItemDTO() :
                        new DTOs.Medias.MediaItemDTO
                        {
                            Id = c.Avatar.Id,
                            MediaUrl = c.Avatar.MediaUrl
                        },
                        RoleName = c.UserRoles.Count == 0 ? "" : c.UserRoles.First().Role.RoleName,
                    };
                    result.Add(user);
                }
                return new PageResultDTO<UserListDTO>(result, users.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO> ImportUsersAsync(int campusId, IFormFile excelFile)
        {
            // Validate campus
            var campus = await _userRepository.GetCampusByIdAsync(campusId);
            if (campus == null)
            {
                return new ResponseDTO(400, "ID campus không hợp lệ.", null);
            }

            if (excelFile == null || excelFile.Length == 0)
            {
                return new ResponseDTO(400, "Không có file được tải lên.", null);
            }

            if (!excelFile.FileName.EndsWith(".xlsx"))
            {
                return new ResponseDTO(400, "Định dạng file không hợp lệ. Vui lòng tải lên file Excel (.xlsx).", null);
            }

            var users = new List<Models.User>();
            var errors = new List<string>();
            int rowIndex = 2;

            try
            {
                using var stream = new MemoryStream();
                await excelFile.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                // Validate headers
                var expectedHeaders = new[] { "Email", "FirstName", "LastName", "DateOfBirth", "PhoneNumber", "Gender" };
                for (int i = 1; i <= expectedHeaders.Length; i++)
                {
                    if (worksheet.Cells[1, i].Text != expectedHeaders[i - 1])
                    {
                        return new ResponseDTO(400, $"Tiêu đề cột không hợp lệ tại cột {i}. Mong đợi: {expectedHeaders[i - 1]}.", null);
                    }
                }

                while (!string.IsNullOrEmpty(worksheet.Cells[rowIndex, 1].Text))
                {
                    var userDto = new UserImportDTO
                    {
                        Email = worksheet.Cells[rowIndex, 1].Text?.Trim(),
                        FirstName = worksheet.Cells[rowIndex, 2].Text?.Trim(),
                        LastName = worksheet.Cells[rowIndex, 3].Text?.Trim(),
                        DateOfBirth = DateTime.TryParseExact(
                            worksheet.Cells[rowIndex, 4].Text?.Trim(),
                            new[] { "dd/MM/yyyy", "yyyy-MM-dd" },
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out var dob) ? dob : default,
                        PhoneNumber = worksheet.Cells[rowIndex, 5].Text?.Trim(),
                        Gender = worksheet.Cells[rowIndex, 6].Text?.Trim()?.ToLower() == "male"
                    };

                    // Chuẩn hóa PhoneNumber
                    if (!string.IsNullOrWhiteSpace(userDto.PhoneNumber))
                    {
                        userDto.PhoneNumber = userDto.PhoneNumber.Replace(" ", ""); // Loại bỏ khoảng trắng
                        if (userDto.PhoneNumber.StartsWith("84"))
                        {
                            userDto.PhoneNumber = "0" + userDto.PhoneNumber.Substring(2);
                        }
                        else if (userDto.PhoneNumber.StartsWith("+84"))
                        {
                            userDto.PhoneNumber = "0" + userDto.PhoneNumber.Substring(3);
                        }
                    }

                    // Validation
                    var validationErrors = ValidateUser(userDto, rowIndex);
                    if (validationErrors.Any())
                    {
                        errors.AddRange(validationErrors);
                        rowIndex++;
                        continue;
                    }

                    // Check email uniqueness
                    if (await _userRepository.EmailExistsAsync(userDto.Email))
                    {
                        errors.Add($"Dòng {rowIndex}: Email {userDto.Email} đã tồn tại.");
                        rowIndex++;
                        continue;
                    }

                    var user = new Models.User
                    {
                        Id = Guid.NewGuid(),
                        Email = userDto.Email,
                        FirstName = userDto.FirstName,
                        LastName = userDto.LastName,
                        DateOfBirth = userDto.DateOfBirth,
                        PhoneNumber = userDto.PhoneNumber,
                        Gender = userDto.Gender,
                        CampusId = campusId,
                        Status = 1, // Active status
                        CreatedAt = DateTime.Now
                    };

                    users.Add(user);
                    rowIndex++;
                }

                if (errors.Any())
                {
                    return new ResponseDTO(400, "Có lỗi xác thực dữ liệu.", errors);
                }

                foreach (var user in users)
                {
                    await _userRepository.AddUserAsync(user);
                    await _userRepository.AddUserRoleAsync(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = 5
                    });
                }

                return new ResponseDTO(200, $"Đã nhập thành công {users.Count} người dùng.", null);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(500, $"Lỗi khi xử lý file: {ex.Message}", null);
            }
        }

        private List<string> ValidateUser(UserImportDTO user, int rowIndex)
        {
            var errors = new List<string>();

            // Email validation
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add($"Dòng {rowIndex}: Email là bắt buộc.");
            }
            else if (!IsValidEmail(user.Email))
            {
                errors.Add($"Dòng {rowIndex}: Định dạng email không hợp lệ.");
            }

            // FirstName validation
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                errors.Add($"Dòng {rowIndex}: Tên là bắt buộc.");
            }
            else if (user.FirstName.Length > 100)
            {
                errors.Add($"Dòng {rowIndex}: Tên không được vượt quá 100 ký tự.");
            }

            // LastName validation
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                errors.Add($"Dòng {rowIndex}: Họ là bắt buộc.");
            }
            else if (user.LastName.Length > 100)
            {
                errors.Add($"Dòng {rowIndex}: Họ không được vượt quá 100 ký tự.");
            }

            // DateOfBirth validation
            if (user.DateOfBirth == default)
            {
                errors.Add($"Dòng {rowIndex}: Định dạng ngày sinh không hợp lệ. Sử dụng định dạng dd/MM/yyyy (ví dụ: 15/03/2000) hoặc yyyy-MM-dd (ví dụ: 2000-03-15).");
            }
            else if (user.DateOfBirth > DateTime.Now.AddYears(-17))
            {
                errors.Add($"Dòng {rowIndex}: Người dùng phải ít nhất 17 tuổi.");
            }

            // PhoneNumber validation
            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                errors.Add($"Dòng {rowIndex}: Số điện thoại là bắt buộc.");
            }
            else if (!IsValidPhoneNumber(user.PhoneNumber))
            {
                errors.Add($"Dòng {rowIndex}: Định dạng số điện thoại không hợp lệ. Sử dụng 10 chữ số, bắt đầu bằng 03|05|07|08|09 (ví dụ: 0912345678).");
            }

            return errors;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            // Định dạng: 10 chữ số, bắt đầu bằng 03|05|07|08|09
            string pattern = @"^0(3|5|7|8|9)\d{8}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 6)
            {
                throw new ArgumentException("Mật khẩu mới phải có ít nhất 6 ký tự.");
            }

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                throw new ArgumentException("Mật khẩu mới và xác nhận mật khẩu không khớp.");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Người dùng không tồn tại.");
            }

            if (string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
            {
                throw new ArgumentException("Mật khẩu cũ không chính xác.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return true;
        }

        public async Task<PageResultDTO<UserListDTO>> GetSpectatorAndImplementer(int page, int pageSize, string? input, int campusId)
        {
            try
            {
                var users = await _userRepository.GetSpectatorAndImplementer(page, pageSize, input, campusId);

                if (users.TotalCount == 0)
                {
                    return new PageResultDTO<UserListDTO>(new List<UserListDTO>(), 0, page, pageSize);
                }
                List<UserListDTO> result = new List<UserListDTO>();
                foreach (var c in users.Items)
                {
                    UserListDTO user = new UserListDTO
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        Email = c.Email,
                        FirstName = c.FirstName,
                        LastName = c.LastName,
                        Password = c.Password,
                        DateOfBirth = c.DateOfBirth,
                        PhoneNumber = c.PhoneNumber,
                        AddressId = c.AddressId,
                        AvatarId = c.AvatarId,
                        CreatedAt = c.CreatedAt,
                        CampusId = c.CampusId,
                        Status = c.Status,
                        Gender = c.Gender,
                        Avatar = c.Avatar == null ? new DTOs.Medias.MediaItemDTO() :
                        new DTOs.Medias.MediaItemDTO
                        {
                            Id = c.Avatar.Id,
                            MediaUrl = c.Avatar.MediaUrl
                        },
                        RoleName = c.UserRoles.Count == 0 ? "" : c.UserRoles.First().Role.RoleName,
                    };
                    result.Add(user);
                }
                return new PageResultDTO<UserListDTO>(result, users.TotalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ResponseDTO> SetRoleEOG(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                var role = user.UserRoles.FirstOrDefault(ur => ur.RoleId == 3);
                if (role != null)
                {
                    return new ResponseDTO(200, "User already event organizer!", null);
                }
                var userRole = new UserRole
                {
                    RoleId = 3,
                    UserId = userId
                };
                var ur = await _userRepository.AddUserRole(userRole);
                if (ur.Id == 0)
                {
                    return new ResponseDTO(400, "Error occurred while set user to event organizer!", null);
                }
                return new ResponseDTO(200, "Set role successfully!", userRole);
            }catch(Exception ex)
            {
                return new ResponseDTO(500, "Error occurred while set user to event organizer!", null);
            }
        }
    }
}
