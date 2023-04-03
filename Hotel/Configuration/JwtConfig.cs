using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Configuration
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpireSeconds { get; set; }
    }
}
