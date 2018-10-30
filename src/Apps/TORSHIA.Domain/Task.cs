using System;
using System.Collections.Generic;

namespace TORSHIA.Domain
{
    public class Task
    {
        public Task()
        {
            this.AffectedSectors = new List<TaskSector>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        public bool IsReported { get; set; }

        /// <summary>
        /// By Description - this is stored however we want. I decided to store it in a simple String.
        /// An alternative to this may be just another simple entity - TaskParticipant
        /// </summary>
        public string ParticipantsString { get; set; }

        public ICollection<TaskSector> AffectedSectors { get; set; }
    }
}
