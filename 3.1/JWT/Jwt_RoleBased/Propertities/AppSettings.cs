using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jwt_RoleBased.Propertities
{
    public class AppSettings
    {
        public JwtSetting JwtSetting { get; set; }
    }

    public class JwtSetting
    {
        public string SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpireSeconds { get; set; }
    }
}
