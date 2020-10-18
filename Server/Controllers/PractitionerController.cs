using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Patient = FIT3077.Shared.Models.Patient;

namespace FIT3077.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PractitionerController : ControllerBase
    {
        private readonly IFhirClient _client ;
        public PractitionerController()
        {
            _client = new FhirClient("https://fhir.monash.edu/hapi-fhir-jpaserver/fhir/");
        }


        /// <summary>
        /// This function query all the patient treated by the practitioner
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of all patients treated by this practitioner</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<Dictionary<string, Patient>> SearchAllPatients(string id)
        {
            var patientList = new Dictionary<string, Patient>();
            var result = await _client.SearchAsync<Encounter>(new string[]
            {
                "participant.identifier=http://hl7.org/fhir/sid/us-npi|" + id.ToString()
            });
            foreach (var e in result.Entry)
            {
                Encounter p = (Encounter)e.Resource;
                var res = p.Subject.Reference;
                var patientId = res.Split('/')[1];
                var patientName = p.Subject.Display;
                var patient = new Patient() { Id = patientId, Name = patientName };
                if (!patientList.ContainsKey(patientId))
                {
                    patientList.Add(patientId, patient);
                }
            }
            return patientList;
        }
    }
}
