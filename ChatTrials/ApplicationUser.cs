using Microsoft.AspNetCore.Identity;

namespace ChatTrials
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}