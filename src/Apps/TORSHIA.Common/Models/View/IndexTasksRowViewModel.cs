using System.Collections.Generic;

namespace TORSHIA.App.Models.View
{
    public class IndexTasksRowViewModel
    {
        public IndexTasksRowViewModel()
        {
            this.Tasks = new List<IndexTaskViewModel>();
        }

        public List<IndexTaskViewModel> Tasks { get; set; }

        public string[] Empty => new string[5 - this.Tasks.Count];
    }
}
