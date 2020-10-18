using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;

namespace FHIR_FIT3077.Models
{
    public class RecordModel
    {
        public RecordType Name { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }

    }

    public enum RecordType
    {
        Cholesterol
    }
}
