using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Core
{
    public class UserService : ServiceBase
    {
        private readonly HotelDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(HotelDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private User _user;
        public User CurrentUser
        {
            get
            {
                if (_user == null)
                {
                    var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    _user = string.IsNullOrEmpty(userId) ? null : _dbContext.Users.Find(int.Parse(userId));
                    if (_user == null)
                        throw new Exception("can't find user with id:" + userId);
                }
                return _user;
            }
        }

        public User FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var user = _dbContext.Users.FirstOrDefault(x => x.Email == email);
            return user;
        }

        public User FindByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var user = _dbContext.Users.FirstOrDefault(x => x.Name == name);
            return user;
        }

        public IList<User> GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return users;
        }

        public IList<Reservation> GetReservations()
        {
            var reservations = GetReservations(CurrentUser);
            return reservations;
        }

        public IList<Reservation> GetReservations(User user = null)
        {
            if (user == null)
                return null;
            var reservations = _dbContext.Reservations.Where(r => user.UserType == UserType.Employee ? true : r.GuestId == user.Id).ToList();
            return reservations;
        }

        public IList<Reservation> GetReservations(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var user = FindByEmail(email);
            var reservations = GetReservations(user);
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
                var admin = new User { CreatedTime = now, Email = "admin@qq.com", Name = "admin", Password = "1", UserType = UserType.Employee };
                var table1 = new Table { Name = "Table1", Size = 2, CreatedTime = now };
                _dbContext.Users.AddRange(admin, new User { CreatedTime = now, Email = "user1@qq.com", Name = "user1", Password = "1" });
                _dbContext.Tables.AddRange(table1, new Table { Name = "Table1", Size = 4, CreatedTime = now },
                new Table { Name = "Table1", Size = 6, CreatedTime = now });
                _dbContext.Reservations.Add(new Reservation { CreatedTime = now, Guest = admin, Table = table1, ReservationTime = now.AddDays(2), ReservationStatus = ReservationStatus.UnHandled });
                _dbContext.SaveChanges();
            }
        }
    }
}
