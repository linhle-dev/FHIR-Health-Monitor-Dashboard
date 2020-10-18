using System;
using System.Collections.Generic;
using System.Linq;
using FHIR_FIT3077.IRepository;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using FHIR_FIT3077.Models;
using Hl7.Fhir.Support;
using FHIR_FIT3077.ViewModel;

namespace FHIR_FIT3077.Repository
{
    public class PractitionerRepository : IPractitionerRepository
    {
        private static FhirClient _client;

        public static void InitializeClient()
        {
            _client = new FhirClient("https://fhir.monash.edu/hapi-fhir-jpaserver/fhir/");
            _client.Timeout = 120000;
        }

        public Dictionary<string, PatientModel> GetTotalPatients(string id)
        {
            InitializeClient();

            var patientList = new Dictionary<string, PatientModel>();
            Bundle result = _client.Search<Encounter>(new string[]
            {
                "participant.identifier=http://hl7.org/fhir/sid/us-npi|" + id.ToString()
            });
            while (result != null)
            {
                foreach (var e in result.Entry)
                {
                    Encounter p = (Encounter)e.Resource;
                    var res = p.Subject.Reference;
                    var patientId = res.Split('/')[1];
                    var patientName = p.Subject.Display;
                    var patient = new PatientModel() { Id = patientId, Name = patientName, Records = new List<RecordModel>() };

                    if (!patientList.ContainsKey(patientId))
                    {
                        var record = GetCholesterolRecordById(patientId);
                        patient.Records.Add(record);
                        GetPatientDetails(patient);
                        patientList.Add(patientId, patient);
                    }
                }

                result = _client.Continue(result);
            }
                


            return patientList;
        }

        public List<RecordModel> GetLatestRecords(string id)
        {
            var recordList = new List<RecordModel>();
            var cholesterolRecord = GetCholesterolRecordById( id);
            recordList.Add(cholesterolRecord);
            return recordList;
        }
        
        public PatientModel GetPatientDetails(PatientModel patient)
        {
            var patientId = patient.Id;
            var q = new SearchParams().Where("_id=" + patientId);
            Bundle result = _client.Search<Patient>(q);
            if(result.Total > 0)
            {
                Bundle.EntryComponent e = (Bundle.EntryComponent)result.Entry[0];
                Patient p = (Patient)e.Resource;
                var birthdate = p.BirthDate;
                var gender = p.Gender;
                var listAdr = p.Address;
                var adr = listAdr[0];
                var address = ((adr.Line).ToList())[0];
                var city = adr.City;
                var state = adr.State;
                var country = adr.Country;
                
                patient.Birthdate = birthdate;
                patient.Gender = gender;
                patient.Address = address;
                patient.City = city;
                patient.State = state;
                patient.Country = country;
                return patient;
            }
            return patient;
        }

        public RecordModel GetCholesterolRecordById(string id)
        {
            InitializeClient();
            var q = new SearchParams().Where("patient=" + id).Where("code=2093-3").OrderBy("-date");
            Bundle result = _client.Search<Observation>(q);

            if (result.Total > 0){
                Bundle.EntryComponent e = (Bundle.EntryComponent)result.Entry[0];
                Observation o = (Observation)e.Resource;
                var date = o.Issued.ToFhirDateTime();
                Quantity quantity = (Quantity)o.Value;
                var weight = quantity.Value.ToString();
                return new RecordModel()
                {
                    Name = RecordType.Cholesterol,
                    Value = weight,
                    Date = date
                };
            }
            
            return new RecordModel()
                {
                    Name = RecordType.Cholesterol,
                    Value = 0.ToString()
                };
            
            
        }
    
    }
}