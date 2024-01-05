using LSC.SmartCartHub.Entities;
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
using System.Threading.Tasks;

namespace LSC.SmartCartHub
{
    public partial class UserProfileFunction
    {

        [FunctionName("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetUserRoles/{adObjId}")] HttpRequest req,
            string adObjId,
            ILogger log, ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processing GetUserRoles request.");
            var userRoles = new List<string>();

            // Get the connection string from the configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            try
            {
                string connectionString = config.GetConnectionString("DbContext");

                var optionsBuilder = new DbContextOptionsBuilder<LearnSmartDbContext>();
                optionsBuilder.UseSqlServer(connectionString);
                var learnSmartDbContext = new LearnSmartDbContext(optionsBuilder.Options);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(adObjId))
                {
                    return new BadRequestObjectResult("Please provide AdObjId in the request.");
                }

                var defaultRole = "Guest";

                userRoles = learnSmartDbContext.UserRoles
                    .Where(ur => ur.User.AdObjId == adObjId)
                    .Include(ur => ur.Role)
                    .Select(ur => ur.Role.RoleName)
                    .ToList();

                if (!userRoles.Any())
                    userRoles.Add(defaultRole);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
            }

            var rolesSeparatedByComma = string.Join(',', userRoles);
            return new OkObjectResult(rolesSeparatedByComma);
        }
    }
}
