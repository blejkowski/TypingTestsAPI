using Microsoft.AspNetCore.Mvc;
using TypingTestsAPI.Models;
using System.Net;

namespace MoneyMeterAPI.Services
{
    public interface IUserService
    {
        Task<bool> UpdateUser(User user);
        Task<User> FindUserByName(string username);
        Task<bool> CreateUser(User user);
        Task<bool> LoginUser(User userToLogin);
        Task<int> GetUserRank(User user);
        Task<List<User>> GetLeaderboardOfUsers();
    }
}