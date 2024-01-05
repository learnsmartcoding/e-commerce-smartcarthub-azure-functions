using LSC.SmartCartHub.Entities;
using LSC.SmartCartHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LSC.SmartCartHub
{
    public partial class UserProfileFunction
    {
       
        [FunctionName("UpdateUserProfile")]
        public static async Task<IActionResult> UpdateUserProfile(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "UpdateUserProfile")] HttpRequest req,
           ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var userProfileResponse = new Profile();

            try
            {
                // Get the connection string from the configuration
                var config = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                string connectionString = config.GetConnectionString("DbContext");

                var optionsBuilder = new DbContextOptionsBuilder<LearnSmartDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                var learnSmartDbContext = new LearnSmartDbContext(optionsBuilder.Options);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBody))
                {
                    return new BadRequestObjectResult("Invalid request body. Please provide a valid Profile. Body cannot be empty");
                }

                Profile? profile = JsonSerializer.Deserialize<Profile>(requestBody);

                if (profile == null)
                {
                    return new BadRequestObjectResult("Invalid request body. Please provide a valid Profile.");
                }

                string adObjId = profile.AdObjId;

                if (string.IsNullOrEmpty(adObjId))
                {
                    return new BadRequestObjectResult("Please provide AdObjId in the request body.");
                }

                var adminUsers = new List<string>() { "learnsmartcoding@gmail.com" };
                var supportUsers = new List<string>() { "karthiktechblog@gmail.com" };

                // Check if UserProfile with given AdObjId exists
                var userProfile = await learnSmartDbContext.UserProfiles.FirstOrDefaultAsync(u => u.AdObjId == adObjId);

                if (userProfile == null)
                {
                    // If not exists, create a new UserProfile
                    userProfile = new UserProfile
                    {
                        AdObjId = adObjId,
                        DisplayName = profile.DisplayName,
                        FirstName = profile.FirstName,
                        LastName = profile.LastName,
                        Email = profile.Email
                    };
                    var guestRoleId = learnSmartDbContext.Roles.FirstOrDefault(w => w.RoleName == "Guest")?.RoleId;//Assuming defualt role is inserted via script
                    userProfile.UserRoles = new List<UserRole>() { new UserRole() { RoleId = guestRoleId } };


                    //This ideally executes only one time for particular users. we should not hard code instead derive from DB
                    var roleNameToDerive = adminUsers.Contains(profile.Email) ? "Admin" :
                        supportUsers.Contains(profile.Email) ? "Support" : "";
                    if (!string.IsNullOrEmpty(roleNameToDerive))
                    {
                        var specialUserRoleId = learnSmartDbContext.Roles.FirstOrDefault(w => w.RoleName == roleNameToDerive)?.RoleId;
                        userProfile.UserRoles.Add(new UserRole() { RoleId = specialUserRoleId });
                    }

                    learnSmartDbContext.UserProfiles.Add(userProfile);
                }
                else
                {
                    // If exists, update the existing UserProfile
                    // You can update other properties here if needed
                    userProfile.DisplayName = profile.DisplayName;
                    userProfile.FirstName = profile.FirstName;
                    userProfile.LastName = profile.LastName;
                    userProfile.Email = profile.Email;
                }

                await learnSmartDbContext.SaveChangesAsync();

                userProfileResponse = new Profile()
                {
                    UserId = userProfile.UserId,
                    AdObjId = userProfile.AdObjId,
                    DisplayName = userProfile.DisplayName,
                    Email = userProfile.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName
                };

            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }
            

            return new OkObjectResult(userProfileResponse);
        }
    }
}
