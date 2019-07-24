using System.Collections.Generic;

namespace TodoApi.Models
{
    public class User
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Child> Children {get; set;} = new HashSet<Child>();
        public string Username { get; set; }
        public string Password { get; set; }
    }
}