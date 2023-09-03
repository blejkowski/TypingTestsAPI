using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using TypingTestsAPI.Models;
using TypingTestsAPI.Helpers;
using BCrypt.Net;
using MongoDB.Bson;

namespace TypingTestsAPI.Services
{
    public class UserService
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

        internal bool UpdateUser(User userToUpdate)
        {
            try
            {
                return _users.ReplaceOne(x => x.Id == userToUpdate.Id, userToUpdate).ModifiedCount > 0 ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        internal User FindUserByName(string username)
        {
            try
            {
                User result = _users.Find(x => x.Username == username).Project<User>(Builders<User>.Projection.Exclude(x => x.Password)).FirstOrDefault();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal bool CreateUser(User user)
        {
            try
            {
                User createdUser = _users.Find(x => x.Username == user.Username).FirstOrDefault();
                if (createdUser != null)
                {
                    return false;
                }
                else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, 8);
                    _users.InsertOne(user);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool LoginUser(User userToLogin)
        {
            try
            {
                User user = _users.Find(x => x.Username == userToLogin.Username).FirstOrDefault();
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
        public int GetUserRank(User user)
        {
            try
            {
                var users = _users.AsQueryable<User>().ToList();
                users.Sort((a, b) => b.AverageWpm.CompareTo(a.AverageWpm));

                int userRank = users.FindIndex(x => x.Username == user.Username) + 1;
                return userRank;
            }catch(Exception)
            {
                return -2;
            }
        }

        public List<User> GetLeaderboardOfUsers()
        {
            try
            {
                var users = _users.AsQueryable<User>().ToList();
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
