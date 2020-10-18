using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FHIR_FIT3077.IRepositories;
using FHIR_FIT3077.IRepository;
using FHIR_FIT3077.Models;
using FHIR_FIT3077.Observer;
using FHIR_FIT3077.Repository;
using FHIR_FIT3077.ViewModel;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace FHIR_FIT3077.Controllers
{
    public class PractitionerController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        private readonly IPractitionerRepository _practitioner;
        private readonly ICacheRepository _cache;

        public PractitionerController(IPractitionerRepository practitioner, ICacheRepository distributedCache)
        {
            _practitioner = practitioner;
            _cache = distributedCache;
        }

        [HttpPost]
        [ActionName("SubmitId")]
        public IActionResult SubmitId(PractitionerModel model)
        {
            if (ModelState.IsValid)
            {
                _cache.Remove("Patient" + model.Id);
                _cache.Remove("Monitor" + model.Id);
                return RedirectToAction("LoadPatient", new {id = model.Id});
            }

            return Index();
        }


        //This method load all unique patients from the server and store the list in cache
        public IActionResult LoadPatient(string id)
        {
            string cacheKey = "Patients" + id;
            var patientViewModel = new PatientViewModel();
            if (_cache.ExistObject<Dictionary<string, PatientModel>>(cacheKey) == true)
            {
                Console.WriteLine("exist cache");
                var patientList = _cache.GetObject<Dictionary<string, PatientModel>>(cacheKey);
                patientViewModel.PatientList = patientList;
                TempData["PracId"] = id;
                ViewData["PractitionerId"] = id;
                return View(patientViewModel);
            }
            else
            {
                Console.WriteLine("create new cache");
                patientViewModel.PatientList = _practitioner.GetTotalPatients(id);
                _cache.SetObject(cacheKey, patientViewModel.PatientList);
                TempData["PracId"] = id;
                ViewData["PractitionerId"] = id;
                return View(patientViewModel);
            }
        }

        public IActionResult Update(string pracId)
        {
            string cacheMonitorKey = "Monitor" + pracId;
            string cachePatientKey = "Patients" + pracId;
            var monitorList = _cache.GetObject<List<MonitorModel>>(cacheMonitorKey);
            var patientList = _cache.GetObject<Dictionary<string, PatientModel>>(cachePatientKey);

            foreach (var pat in patientList)
            {
                pat.Value.Records = _practitioner.GetLatestRecords(pat.Value.Id);
            }
            _cache.SetObject(cachePatientKey, patientList);

            if (monitorList != null)
            {
                foreach (var monitor in monitorList)
                {
                    monitor.Update(patientList[monitor.Id]);
                }
                _cache.SetObject(cacheMonitorKey, monitorList);
                return RedirectToAction("UpdateMonitorView", "Monitor", new { pracId = pracId });
            }
            else
            {
                return new EmptyResult();
            }
            
           
        }
        
    }
}