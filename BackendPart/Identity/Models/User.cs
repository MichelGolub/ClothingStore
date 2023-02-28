using Identity.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Identity.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
