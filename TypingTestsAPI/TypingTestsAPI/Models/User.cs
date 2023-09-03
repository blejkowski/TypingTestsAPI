using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using static System.Net.WebRequestMethods;

namespace TypingTestsAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ProfileImage { get; set; } = "https://openseauserdata.com/files/531003390ee8fa9a0cf3b7fbc36f5960.png";
        public double AverageWpm { get; set; } = 0;
        public double BestWpm { get; set; } = 0;
        public int TypingTests { get; set; } = 0;
        public List<Test> tests { get; set; } = new List<Test>();

        public UserPreferences Preferences = new UserPreferences();

        [Serializable]
        public class UserPreferences
        {
            public string BackgroundColor { get; set; } = "#323437";
            public string PrimaryColor { get; set; } = "#e2b714";
            public string SecondaryColor { get; set; } = "#646669";
            public string TextColor { get; set; } = "#d1d0c5";
           
        }

        public class Test
        {
            public double WordsPerMinute { get; set; } = 0;
            public double Accuracy { get; set; } = 0;
        }
    }
}
