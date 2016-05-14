using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Timesheet.Services;
using Timesheet.Model;
using System.Windows.Input;
using Xamarin.Forms;


namespace Timesheet.ViewModel
{
    public class ProjectListViewModel : BaseViewModel
    {
        private ProjectService _ProjectService = new ProjectService();
        private ObservableRangeCollection<Project> _Projects = new ObservableRangeCollection<Project>();

        private ICommand _refreshCommand, _loadMoreCommand = null;

        public ProjectListViewModel()
        {
            LoadProjects();
        }

        public async Task LoadProjects()
        {
            var newProjects = _ProjectService.Load(null);

            _Projects.ReplaceRange(newProjects);

            Title = $"Projects {_Projects.Count}";
        }

        public ObservableRangeCollection<Project> Projects
        {
            get { return _Projects; }

        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? new Command(async () => await ExecuteRefreshCommand(), () => CanExecuteRefreshCommand()); }
        }

        public bool CanExecuteRefreshCommand()
        {
            return IsNotBusy;
        }

        public async Task ExecuteRefreshCommand()
        {
            IsBusy = true;

            await LoadProjects();

            IsBusy = false;
        }

        public ICommand LoadMoreCommand
        {
            get { return _loadMoreCommand ?? new Command<Project>(ExecuteLoadMoreCommand, CanExecuteLoadMoreCommand); }
        }

        public bool CanExecuteLoadMoreCommand(Project item)
        {
            return IsNotBusy && _Projects.Count != 0 && _Projects.OrderByDescending(o => o.Created).Last().Created == item.Created;
        }

        public void ExecuteLoadMoreCommand(Project item)
        {
            IsBusy = true;
            var items = _ProjectService.Load(item.Created);
            _Projects.AddRange(items);
            Title = $"Projects {_Projects.Count}";
            IsBusy = false;
        }


    }
}
