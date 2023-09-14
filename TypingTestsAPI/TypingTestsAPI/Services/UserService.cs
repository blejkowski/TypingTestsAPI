using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TypingTestsAPI.Models;
using TypingTestsAPI.Helpers;
using BCrypt.Net;
using MongoDB.Bson;
using MoneyMeterAPI.Services;

namespace TypingTestsAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IMongoCollection<User> _users = null!;

        public UserService(
             IOptions<UserStoreSettings> UserStoreSettings)
        {

            var mongoClient = new MongoClient(
                UserStoreSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                UserStoreSettings.Value.DatabaseName);

            _users = mongoDatabase.GetCollection<User>(
                UserStoreSettings.Value.UserCollectionName);

        }

        async public Task<bool> UpdateUser(User user)
        {
            try
            {
                await _users.ReplaceOneAsync(x => x.Id == user.Id, user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        async public Task<User> FindUserByName(string username)
        {
            try
            {
                User result = await _users.Find(x => x.Username == username).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        async public Task<bool> CreateUser(User user)
        {
            try
            {
                User userToRegister = await _users.Find(x => x.Username == user.Username).FirstOrDefaultAsync();

                if (userToRegister != null)
                    return false;

                userToRegister.Password = BCrypt.Net.BCrypt.HashPassword(userToRegister.Password, 8);
                await _users.InsertOneAsync(userToRegister);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public async Task<bool> LoginUser(User userToLogin)
        {
            try
            {
                User user = await _users.Find(x => x.Username == userToLogin.Username).FirstOrDefaultAsync();
                if (user == null)
                    return false;

                if (BCrypt.Net.BCrypt.Verify(userToLogin.Password, user.Password) == false)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<int> GetUserRank(User user)
        {
            try
            {
                var users = await _users.AsQueryable<User>().ToListAsync();
                users.Sort((a, b) => b.AverageWpm.CompareTo(a.AverageWpm));

                int userRank = users.FindIndex(x => x.Username == user.Username) + 1;
                return userRank;
            }catch(Exception)
            {
                return -2;
            }
        }

        public async Task<List<User>> GetLeaderboardOfUsers()
        {
            try
            {
                var users = await _users.AsQueryable<User>().ToListAsync();
                users.Sort((a, b) => b.AverageWpm.CompareTo(a.AverageWpm));
                return users.GetRange(0, 10);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }   

}
