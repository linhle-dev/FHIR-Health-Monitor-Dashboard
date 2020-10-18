using System.Collections.Generic;

namespace FIT3077.Shared.Models
{
    public class BloodPressureList : RecordList
    {
        public new List<BloodPressureRecord> Records { get; set; }
    }
}