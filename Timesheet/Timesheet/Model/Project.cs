using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.Model
{
    class Project
    {
        public Project()
        {
        }

        public int ProjectId  { get; set; }

        public string ProjectName { get; set; }
        public string ProjectDesc { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

    }
}
