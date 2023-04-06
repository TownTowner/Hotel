using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Core
{
    public abstract class ServiceBase
    {
        private HotelDbContext _db;
        internal HotelDbContext Db
        {
            get
            {
                if (_db == null)
                    _db = AppDbContextFactory.CreateDbContext();
                return _db;
            }
        }
    }

    public static class AppDbContextFactory
    {
        [ThreadStatic]
        static HotelDbContext _dbInstance;

        internal static HotelDbContext CreateDbContext()
        {
            //if (_dbInstance == null)
            //    _dbInstance = new HotelContext();
            return _dbInstance;
        }
    }
}
