using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FHIR_FIT3077.Observer;
using Hl7.Fhir.Model;

namespace FHIR_FIT3077.Models
{
    [Serializable()]

    public class PatientModel 
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Birthdate { get; set; }
        public AdministrativeGender? Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        

        public List<RecordModel> Records { get; set; }
       
    }
}
