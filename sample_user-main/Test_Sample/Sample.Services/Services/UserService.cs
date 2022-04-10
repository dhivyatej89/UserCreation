using AutoMapper;
using Sample.Common;
using Sample.Common.StaticHelpers;
using Sample.Models.DTOs;
using Sample.Models.Entities;
using Sample.Repository.Interfaces;
using Sample.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        List<string> validationmessage;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Guid> AddUser(AddUserDto addUser)
        {
            validationmessage = new List<string>();
            try
            {
                #region Validation
                if (string.IsNullOrEmpty(addUser.FullName?.Trim()))
                {
                    validationmessage.Add(Messages.MSG_NAME_REQUIRED);                    
                }

                if (!Validator.ValidateString(addUser.FullName.Trim()))
                {
                    validationmessage.Add(Messages.MSG_INVALID_NAME);                    
                }

                if (string.IsNullOrEmpty(addUser.Email?.Trim()))
                {
                    validationmessage.Add(Messages.MSG_EMAIL_REQUIRED);                   
                }

                if (!Validator.ValidateEmail(addUser.Email.Trim()))
                {
                    validationmessage.Add(Messages.MSG_INVALID_EMAIL);                   
                }

                bool isEmailExist = await _userRepository.IsUserEmailExist(addUser.Email);
                if (isEmailExist)
                {
                    validationmessage.Add(Messages.MSG_EMAIL_EXIST);                    
                }

                if (!string.IsNullOrEmpty(addUser.Phone?.Trim()))
                {
                    if (!Validator.ValidatePhone(addUser.Phone.Trim()))
                    {
                        validationmessage.Add(Messages.MSG_INVALID_PHONE);                      
                    }
                }

                if (addUser.Age != null && addUser.Age <= 0)
                {
                    validationmessage.Add(Messages.MSG_INVALID_AGE);                   
                }
                if(validationmessage.Count>0)
                {
                    throw new Exception(string.Join(", ", validationmessage));                  
                }
                #endregion

                return await _userRepository.AddUser(_mapper.Map<User>(addUser));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateUser(UpdateUserDto updateUser)
        {
            try
            {
                validationmessage = new List<string>();
                #region Validation
                if (updateUser.FullName != null && string.IsNullOrEmpty(updateUser.FullName.Trim()))
                {
                    validationmessage.Add(Messages.MSG_NAME_REQUIRED);                    
                }

                if (updateUser.FullName != null && !Validator.ValidateString(updateUser.FullName.Trim()))
                {
                    validationmessage.Add(Messages.MSG_INVALID_NAME);                    
                }
                if (!Validator.ValidatePhone(updateUser.Phone.Trim()))
                {
                    validationmessage.Add(Messages.MSG_INVALID_PHONE);
                }


                if (updateUser.Email != null && string.IsNullOrEmpty(updateUser.Email.Trim()))
                {
                    validationmessage.Add(Messages.MSG_EMAIL_REQUIRED);                    
                }

                if (updateUser.Email != null)
                {
                    if (!Validator.ValidateEmail(updateUser.Email.Trim()))
                    {
                        validationmessage.Add(Messages.MSG_INVALID_EMAIL);                        
                    }

                    bool isEmailExist = await _userRepository.IsUserEmailExist(updateUser.Email, updateUser.UserId);
                    if (isEmailExist)
                    {
                        validationmessage.Add(Messages.MSG_EMAIL_EXIST);                        
                    }
                }

                if (updateUser.Age != null && updateUser.Age <= 0)
                {
                    validationmessage.Add(Messages.MSG_INVALID_AGE);                    
                }
                if (validationmessage.Count > 0)
                {
                    throw new Exception(string.Join(", ", validationmessage));
                }

                #endregion

                var user = await _userRepository.GetUserByUserId(updateUser.UserId);
                if (user != null)
                {
                    user.FullName = updateUser.FullName ?? user.FullName;
                    user.Email = updateUser.Email ?? user.Email;
                    user.Phone = updateUser.Phone ?? user.Phone;
                    user.Age = updateUser.Age ?? user.Age;

                    return await _userRepository.UpdateUser(user);
                }
                else
                {                    
                    throw new Exception(Messages.MSG_NO_DATA);
                }

                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByUserId(userId);

                return await _userRepository.DeleteUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UserDto>> GetUsers(SearchDto search)
        {
            try
            {
                List<User> userList = await _userRepository.GetUsers(search.Email, search.Phone);
                List<UserDto> users = _mapper.Map<List<UserDto>>(userList);

                return Sorting.SortData(users, search.SortField, search.SortOrder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
