using Microsoft.AspNetCore.Identity;

namespace StockMapSvelte.Infrastructure.Database.Seeding;

internal class UserRolesInit
{
    public List<IdentityUserRole<string>> GetRolesForAdmin()
    {
        List<IdentityUserRole<string>> adminUserRoles = new List<IdentityUserRole<string>>()
        {
            new IdentityUserRole<string>()
            {
                UserId = "702f52fd-f23b-462d-b84b-0e1acc17785c",
                RoleId = "1"
            },
            new IdentityUserRole<string>()
            {
                UserId = "702f52fd-f23b-462d-b84b-0e1acc17785c",
                RoleId = "2"
            },
            new IdentityUserRole<string>()
            {
                UserId = "702f52fd-f23b-462d-b84b-0e1acc17785c",
                RoleId = "3"
            }
        };

        return adminUserRoles;
    }


    public List<IdentityUserRole<string>> GetRolesForManager()
    {
        List<IdentityUserRole<string>> managerUserRoles = new List<IdentityUserRole<string>>()
        {
            new IdentityUserRole<string>()
            {
                UserId = "b3546cea-bc20-4f06-9341-15fb80768cf6",
                RoleId = "2"
            },
            new IdentityUserRole<string>()
            {
                UserId = "b3546cea-bc20-4f06-9341-15fb80768cf6",
                RoleId = "3"
            }
        };

        return managerUserRoles;
    }
    
    public List<IdentityUserRole<string>> GetRolesForDemoUser()
    {
        List<IdentityUserRole<string>> demoUserRoles = new List<IdentityUserRole<string>>()
        {
            new IdentityUserRole<string>()
            {
                UserId = "505aedf9-29e3-4728-a8f0-d295040dbaef",
                RoleId = "3"
            }
        };
            
        return demoUserRoles;
    }
}