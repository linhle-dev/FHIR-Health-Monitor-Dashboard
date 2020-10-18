using System;
using System.Collections.Generic;
using System.Linq;
using FHIR_FIT3077.IRepositories;
using FHIR_FIT3077.Models;
using FHIR_FIT3077.Observer;
using FHIR_FIT3077.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace FHIR_FIT3077.Controllers
{
    public class MonitorController : Controller
    {
        private readonly ICacheRepository _cache;
        public MonitorController( ICacheRepository distributedCache)
        {
            _cache = distributedCache;
        }

        //This method registers a patient from the monitor list stored in cache by id
        [HttpPost]
        public IActionResult DeregisterPatient(string id, string pracId)
        {
            //Load cache
            string cachePatientKey = "Patients" + pracId;
            string cacheMonitorKey = "Monitor" + pracId;
            var patientList = _cache.GetObject<Dictionary<string, PatientModel>>(cachePatientKey);
            var monitorList = _cache.GetObject<List<MonitorModel>>(cacheMonitorKey);
            //Find monitor and patient to remove
            var monitorToRemove = monitorList.SingleOrDefault(monitor => monitor.Id == id);
            monitorList.Remove(monitorToRemove);
            //Set new object to cache
            _cache.SetObject(cacheMonitorKey, monitorList);
            _cache.SetObject(cachePatientKey, patientList);
            return RedirectToAction("UpdateMonitorView", "Monitor", new { pracId = pracId });
        }

        //This method register a monitor of patient into monitor list
        [HttpPost]
        public IActionResult RegisterPatient(string id, string pracId)
        {
            List<MonitorModel> monitorList;
            string cachePatientKey = "Patients" + pracId;
            string cacheMonitorKey = "Monitor" + pracId;
            var patientList = _cache.GetObject<Dictionary<string, PatientModel>>(cachePatientKey);
            var pat = patientList[id];
            //Create new monitor
            var monitor = new MonitorModel("Cholesterol" + id);
            monitor.Update(pat);
            //Check existing object in cache
            if (_cache.ExistObject<List<PatientViewModel>>(cacheMonitorKey) == true)
            {
                monitorList = _cache.GetObject<List<MonitorModel>>(cacheMonitorKey);

                bool notExist = true;
                for (int i = 0; i < monitorList.Count; i++)
                {
                    if (monitorList[i].Id == monitor.Id)
                    {
                        monitorList[i] = monitor;
                        notExist = false;
                    }
                }
                if (notExist)
                {
                    monitorList.Add(monitor);
                }
                
            }
            else
            {
                monitorList = new List<MonitorModel>
                {
                    monitor
                };
                
            }
            _cache.SetObject(cacheMonitorKey, monitorList);
            _cache.SetObject(cachePatientKey, patientList);
            return RedirectToAction("UpdateMonitorView", "Monitor", new { pracId = pracId});
        }

        public IActionResult UpdateMonitorView(string pracId)
        {
            string cacheMonitorKey = "Monitor" + pracId;
            var monitorViewModel = new PatientViewModel();
            monitorViewModel.MonitorList = new List<MonitorModel>();
            monitorViewModel.MonitorList = _cache.GetObject<List<MonitorModel>>(cacheMonitorKey);
            var max = GetHighCholesterol(monitorViewModel.MonitorList);
            ViewData["max"] = max;
            return PartialView("_MonitorSection", monitorViewModel);
        }
        private List<double> GetHighCholesterol(List<MonitorModel> monitorList)
        {
            double totalChol = 0.0;
            List<double> highCholVal = new List<double>();
            for (int i = 0; i < monitorList.Count; i++)
            {
                var value = Convert.ToDouble(monitorList[i].Records[0].Value);
                totalChol += value;
            }
            var averageChol = totalChol / monitorList.Count;
            foreach (var t in monitorList)
            {
                var value = Convert.ToDouble(t.Records[0].Value);
                if (value > averageChol)
                {
                    highCholVal.Add(value);
                }
            }
            return highCholVal; 
        }
    }
}