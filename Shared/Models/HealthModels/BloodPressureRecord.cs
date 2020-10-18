using System;

namespace FIT3077.Shared.Models
{
    public class BloodPressureRecord : Record
    {
        public string SystolicValue { get; set; }

        public string DiastolicValue { get; set; }
    }
}