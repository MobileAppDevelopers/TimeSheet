using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Timesheet.Model;


namespace Timesheet.Services
{   

    public class ProjectService
    {
        List<Project> _ProjectsContainer;

        public ProjectService()
        {

            _ProjectsContainer = new List<Project>(FillContainer());

        }

        private IEnumerable<Project> FillContainer()
        {
            var random = new Random(0);
            var startDate = new DateTime(2016, 1, 1);

            for (int i = 0; i < 200; i++)
            {
                yield return new Project()
                {
                    ProjectId = i,
                    ProjectName = "Carlsberg 33cl",
                    CreatedOn = startDate.AddDays(i)
                };
            }
        }

        public IEnumerable<Project> Load(DateTime? fromDate)
        {
            if (!fromDate.HasValue)
                return _ProjectsContainer.OrderByDescending(Project => Project.CreatedOn).Take(20);

            if (!_ProjectsContainer.Any(o => o.CreatedOn < fromDate))
                return new List<Project>();

            return _ProjectsContainer.Where(Project => Project.CreatedOn <= fromDate).OrderByDescending(Project => Project.CreatedOn).Take(20);

        }
    }
}
