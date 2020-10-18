using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FHIR_FIT3077.IRepositories;
using FHIR_FIT3077.Observer;
using FHIR_FIT3077.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FHIR_FIT3077.Controllers
{
    public class PatientController : Controller
    {
        private readonly ICacheRepository _cache;
        public PatientController(ICacheRepository distributedCache)
        {
            _cache = distributedCache;
        }
        public IActionResult PatientDisplay(string id)
        {
            var pracId = TempData["PracId"];
            var monitorKey = "Monitor" + pracId;
            var monitoredList = _cache.GetObject<List<MonitorModel>>(monitorKey);
            var patientDetailModel = new PatientDetailsViewModel();
            for (int i = 0; i < monitoredList.Count; i++)
            {
                if(monitoredList[i].Id == id)
                {
                    patientDetailModel.Id = monitoredList[i].Id;
                    patientDetailModel.Name = monitoredList[i].Name;
                    patientDetailModel.Birthdate = monitoredList[i].Birthdate;
                    patientDetailModel.Gender = monitoredList[i].Gender;
                    patientDetailModel.Address = monitoredList[i].Address;
                    patientDetailModel.City = monitoredList[i].City;
                    patientDetailModel.State = monitoredList[i].State;
                    patientDetailModel.Country = monitoredList[i].Country;
                }
            }
            return View(patientDetailModel);
        }
    }
}