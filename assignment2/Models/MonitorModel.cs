using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FHIR_FIT3077.Models;
using Hl7.Fhir.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FHIR_FIT3077.Observer
{
    [Serializable()]
    public class MonitorModel 
    {
        //JsonProperty added for deserialization of private field
        [JsonProperty(PropertyName = "Id")]
        public string Id { get; private set; }
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; private set; }
        [JsonProperty]
        public string Birthdate { get; private set; }
        [JsonProperty]
        public AdministrativeGender? Gender { get; private set; }
        [JsonProperty]
        public string Address { get; private set; }
        [JsonProperty]
        public string City { get; private set; }
        [JsonProperty]
        public string State { get; private set; }
        [JsonProperty]
        public string Country { get; private set; }
        [JsonProperty]
        public List<RecordModel> Records { get; private set; }


        public string Identifier { get; set; }

        public MonitorModel(string identifier)
        {
            this.Identifier = identifier;
        }

        public void Update(PatientModel value)
        {
            this.Id = value.Id;
            this.Name = value.Name;
            this.Birthdate = value.Birthdate;
            this.Gender = value.Gender;
            this.Address = value.Address;
            this.City = value.City;
            this.State = value.State;
            this.Country = value.Country;
            this.Records = value.Records;
        }


    }
}
