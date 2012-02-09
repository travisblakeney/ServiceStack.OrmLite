using System.Collections.Generic;

namespace ServiceStack.OrmLite.ModelConfiguration
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}