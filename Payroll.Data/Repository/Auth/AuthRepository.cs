using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Payroll.Data.Dtos.Users;

namespace Payroll.Data.Repository.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserInfoDto> _userCollection;

        public AuthRepository(IMongoDatabase database)
        {
            _database = database;
            _userCollection = _database.GetCollection<UserInfoDto>("Users"); // Specify your collection name here
        }


        public async ValueTask<UserInfoDto> GetByUsername(string userName)
        {
            try
            {
                // Define filter to find user by userName
                FilterDefinition<UserInfoDto> filter = Builders<UserInfoDto>.Filter.Eq(u => u.UserName, userName);

                // Perform a find operation to retrieve all documents
                UserInfoDto result = await _userCollection.Find(filter).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
         
        }


    }
}
