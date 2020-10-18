using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Timers;
using FIT3077.Shared.Models;

namespace FIT3077.Client.Services
{
    public class DashboardService : IDashboardService
    {
        private Dashboard Dashboard { get; } = new Dashboard();
        public IReadOnlyList<Monitor> Monitors => Dashboard.Monitors;

        public IReadOnlyDictionary<string, Patient> Patients => Dashboard.Patients;
        public Timer t => Dashboard.t;
        public bool SearchInProgress { get; private set; }
        public event Action OnChange;
        

        private readonly HttpClient _http;
        public DashboardService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        /// <summary>
        /// This function redirect to the search function in the practitioner controller to perform search query in FHIR server passing in the practitionerId as the input parameter
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public async Task Search(InputParameter patientId)
        {
            SearchInProgress = true;
            NotifyStateChanged();

            Dashboard.FetchPatientList(
                await _http.GetFromJsonAsync<Dictionary<string, Patient>>($"/api/practitioner/{patientId.Value}"));
            Dashboard.ClearMonitor();
            SearchInProgress = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// This function use the instance Dashboard to add a patient from patient list into monitor list and notify state change
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        public async Task AddToMonitors(Patient patient)
        {
            SearchInProgress = true;
            NotifyStateChanged();

            Dashboard.RegisterMonitor(patient, await FetchMeasurement(patient.Id));
            SearchInProgress = false;
            NotifyStateChanged();
        }

        /// <summary>
        /// This function removes a monitored patient from the monitored list by using an instance of Dashboard to call the DeregisterMonitor function
        /// </summary>
        /// <param name="monitor"></param>
        public void RemoveFromMonitors(Monitor monitor)
        {
            Dashboard.DeregisterMonitor(monitor);
            NotifyStateChanged();
        }

        /// <summary>
        /// This function redirects to the patient controller to fetch 2 types of measurements (cholesterol and bloodpressure) from the FHIR server
        /// and adds those 2 types of measurements in a measurement object and return that object
        /// </summary>
        /// <param name="id"></param>
        /// <returns>measurements</returns>
        private async Task<Measurement> FetchMeasurement(string id)
        {
            var fetchBloodPressureTask = _http.GetFromJsonAsync<List<BloodPressureRecord>>(
                $"/api/patient/{id}/measurement/blood-pressure");
            var fetchCholesterolTask = _http.GetFromJsonAsync<List<CholesterolRecord>>(
                $"/api/patient/{id}/measurement/cholesterol");
            await Task.WhenAll(fetchBloodPressureTask, fetchCholesterolTask);
            var measurement = new Measurement()
            {
                BloodPressureRecords = new BloodPressureList() { Records = await fetchBloodPressureTask },
                CholesterolRecords = new CholesterolList() { Records = await fetchCholesterolTask }
            };
            return measurement;
        }

        public void ModifyRecordState(RecordList recordList)
        {
            recordList.ChangeMeasurementMonitoreState();
            NotifyStateChanged();
        }

        /// <summary>
        /// this function calls the Updatemeasurement function in Dashboard to update the new measurement fetched from FHIR server
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if (Dashboard.Monitors != null && Dashboard.Monitors.Count > 1)
            {
                foreach (var m in Dashboard.Monitors)
                {
                    Dashboard.UpdateMeasurement(m, await FetchMeasurement(m.PatientId));
                }
                NotifyStateChanged();
            } 
        }

        /// <summary>
        /// This function set parameter as a new interval setting for the timer
        /// </summary>
        /// <param name="timeInput"></param>
        /// <returns></returns>
        public void SetTime(InputParameter timeInput)
        {
            try
            {
                t.Interval = 1000 * 60 * int.Parse(timeInput.Value);
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine("Value must be an integer");
                throw;
            }
        }

        /// <summary>
        /// this function takes in a SysDiastolicThreshold object as input and pass that object to the function inside Dashboard called HighBloodPressureFlag
        /// to set the flag of the blood pressure accordingly.
        /// </summary>
        /// <param name="highBloodValues"></param>
        public void ProcessHighBloodInput(SysDiastolicThreshold highBloodValues)
        {
            Dashboard.HighBloodPressureFlag(highBloodValues);
            NotifyStateChanged();
        }

        /// <summary>
        /// This function create a new timer instance in Dashboard model
        /// </summary>
        /// <returns></returns>
        public void CreateTimer()
        {
            Dashboard.t = new Timer();
        }
        /// <summary>
        /// This function invokes OnChange property itself
        /// </summary>
        /// <returns></returns>
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
