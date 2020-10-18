using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FHIR_FIT3077.ViewModels
{
    public class PatientDetailsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Birthdate { get; set; }
        public AdministrativeGender? Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}
