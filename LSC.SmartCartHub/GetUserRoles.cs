using LSC.SmartCartHub.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LSC.SmartCartHub
{
    public partial class UserProfileFunction
    {

        [FunctionName("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUserRoles/{adObjId}")] HttpRequest req,
            string adObjId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processing GetUserRoles request.");

            var optionsBuilder = new DbContextOptionsBuilder<LearnSmartDbContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DbContext"));
            var learnSmartDbContext = new LearnSmartDbContext(optionsBuilder.Options);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (string.IsNullOrEmpty(adObjId))
            {
                return new BadRequestObjectResult("Please provide AdObjId in the request.");
            }

            var defaultRole = "Guest";

            var userRoles = learnSmartDbContext.UserRoles
                .Where(ur => ur.User.AdObjId == adObjId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role.RoleName)
                .ToList();

            if (!userRoles.Any()) 
                userRoles.Add(defaultRole);            

            return new OkObjectResult(userRoles);
        }
    }
}
