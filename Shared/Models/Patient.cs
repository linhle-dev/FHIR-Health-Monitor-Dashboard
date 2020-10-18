using System;
using System.Collections.Generic;
using System.Text;

namespace FIT3077.Shared.Models
{
    public class Patient
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool AddedToMonitor { get; private set; } = false;

        /// <summary>
        /// This function reverse the current state of AddedToMonitor value
        /// </summary>
        /// <returns></returns>
        public void ChangePatientMonitorState()
        {
            AddedToMonitor = !AddedToMonitor;
        }
    }
}
