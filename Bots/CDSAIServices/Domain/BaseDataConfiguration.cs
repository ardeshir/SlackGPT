using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CDSAIServices.Domain
{
    public class BaseDataConfiguration
    {
        ///public List<VMSize> VMSizes { get; set; }
        public List<string> AdminUsers { get; set; }
        //public List<Project> Projects { get; set; }
        //public List<Project> AdminProjectsList { get; set; }
        public BaseDataConfiguration(IConfiguration configuration)
        {
            // var adminProjectsJson = configuration.GetValue<string>("AdminProjects");
            var adminUsersJson = configuration.GetValue<string>("AdminUsers");
            // var projectsJson = configuration.GetValue<string>("Projects");
            // var vmSizesJson = configuration.GetValue<string>("VMSizes");

            //VMSizes = JsonSerializer.Deserialize<List<VMSize>>(vmSizesJson);
            //Projects = JsonSerializer.Deserialize<List<Project>>(projectsJson);
            //AdminProjectsList = JsonSerializer.Deserialize<List<Project>>(adminProjectsJson);
            AdminUsers = JsonSerializer.Deserialize<List<string>>(adminUsersJson);
        }

        //public List<string> VMSizeChoices => VMSizes.Select(vmSize => vmSize.Name).ToList();
        //public List<string> ProjectChoices => Projects.Select(project => project.Name).ToList();
        /* public List<Project> AdminProjects
        {
            get
            {
                var projects = new List<Project>();
                projects.AddRange(Projects);
                projects.AddRange(AdminProjectsList);
                return projects;
            }
        }

        public Project ProjectBasedOnVirtualMachineName(string virtualMachineName)
        {
            if (!string.IsNullOrWhiteSpace(virtualMachineName))
            {
                var splitVirtualMachineName = virtualMachineName.Split('-');

                if (splitVirtualMachineName.Length >= 2)
                {
                    var projectAbbreviation = splitVirtualMachineName[0];

                    if (AdminProjects.Any(x => x.Abbreviation == projectAbbreviation))
                    {
                        var project = AdminProjects.Where(project => project.Abbreviation == projectAbbreviation).FirstOrDefault();
                        return project;
                    }

                    if (AdminProjects.Any(x => x.VMNameAbbreviation == projectAbbreviation))
                    {
                        var project = AdminProjects.Where(project => project.VMNameAbbreviation == projectAbbreviation).FirstOrDefault();
                        return project;
                    }
                }
            }

            return null;
        } */
    }
}
