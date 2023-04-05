using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Core
{
    public class UserService : ServiceBase
    {
        HotelContext _dbContext;
        public UserService(HotelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            return user;
        }

        public IList<User> GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return users;
        }

        public object GetUser(string name)
        {
            throw new NotImplementedException();
        }

        public IList<Reservation> GetReservations(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var user = FindByEmail(name);
            if (user == null)
                return null;

            var reservations = _dbContext.Reservations.Where(r => user.UserType == UserType.Employee ? true : r.GuestId == user.Id).ToList();
            return reservations;
        }

        public async Task<int> CreateUserAsync(User newUser, string password)
        {
            newUser.Password = password;
            _dbContext.Users.Add(newUser);
            return await _dbContext.SaveChangesAsync();
        }

        public void DbSeed()
        {
            if (_dbContext.Database.EnsureCreated())
            {
                var now = DateTime.Now;
                _dbContext.Users.AddRange(new User { CreatedTime = now, Email = "user1@qq.com", Name = "user1", Password = "1" },
                    new User { CreatedTime = now, Email = "admin@qq.com", Name = "admin", Password = "1", UserType = UserType.Employee });
                _dbContext.Tables.AddRange(new Table { Name = "Table1", Size = 2, CreatedTime = now },
                new Table { Name = "Table1", Size = 4, CreatedTime = now },
                new Table { Name = "Table1", Size = 6, CreatedTime = now });
                _dbContext.SaveChanges();
            }
        }
    }
}
