﻿using Microsoft.AspNetCore.Identity;

namespace BetSolutionsProject.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrivateNumber { get; set; }
    }
}
