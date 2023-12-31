﻿#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Microsoft.AspNetCore.Identity;

namespace LinkSharp.Database;

public class ApplicationUser : IdentityUser
{
    public List<Link> Links { get; set; }
    public List<ApplicationUserCollectionAccess> Collections { get; set; }
}