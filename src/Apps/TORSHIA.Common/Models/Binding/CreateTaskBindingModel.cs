using System;
using System.Collections.Generic;

namespace TORSHIA.App.Models.Binding
{
    public class CreateTaskBindingModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public string Participants { get; set; }

        public List<string> AffectedSectors { get; set; }
    }
}
