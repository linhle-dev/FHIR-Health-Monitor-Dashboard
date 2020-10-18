using System.Collections.Generic;

namespace FIT3077.Shared.Models
{
    public class Monitor
    {
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public Measurement MeasurementList { get; set; }

        public bool CholFlag { get; set; }

        public bool SystolicFlag { get; set; }

        public bool DiastolicFlag { get; set; }

        public Monitor(string patientId, string patientName)
        {
            this.PatientId = patientId;
            this.PatientName = patientName;
        }

    }
}