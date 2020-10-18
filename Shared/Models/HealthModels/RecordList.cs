using System;
using System.Collections.Generic;
using System.Text;

namespace FIT3077.Shared.Models
{
    public abstract class RecordList
    {
        public List<Record> Records { get; set; }
        public bool IsMonitored { get; set; } = true;

        /// <summary>
        /// This function reverse the current state of AddedToMonitor value
        /// </summary>
        /// <returns></returns>
        public void ChangeMeasurementMonitoreState() {
            IsMonitored = !IsMonitored;
        }
    }
}
