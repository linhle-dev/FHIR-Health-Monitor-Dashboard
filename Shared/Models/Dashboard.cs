using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace FIT3077.Shared.Models
{
    public class Dashboard
    {
        public Dictionary<string, Patient> Patients { get; private set; }
        public List<Monitor> Monitors { get; private set; }
        public Timer t { get; set; }
        private SysDiastolicThreshold SysDiastolicValues { get; set; } = new SysDiastolicThreshold();

        /// <summary>
        /// This method register a patient to a monitored list, when registering it will check the level of cholesterol and bloodpressure
        /// to display on the page appropriately
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="measurement"></param>
        public void RegisterMonitor(Patient patient, Measurement measurement)
        {
            Monitor monitor = new Monitor(patient.Id, patient.Name);
            monitor.MeasurementList = measurement;
            Monitors ??= new List<Monitor>();
            Monitors.Add(monitor);
            ChangePatientMonitorState(patient);
            HighCholFlag();
            HighBloodPressureFlag(SysDiastolicValues);
        }

        /// <summary>
        /// This method sets the flag if the patient has a high cholesterol level than the average cholesterol
        /// </summary>
        public void HighCholFlag()
        {
            double totalChol = 0.0;
            foreach (var m in Monitors)
            {
                var monitor = m.MeasurementList.CholesterolRecords.Records[0].CholesterolValue;
                var val = Convert.ToDouble(monitor);
                totalChol += val;
            }

            var averageChol = totalChol / Monitors.Count;
            foreach(var t in Monitors)
            {
                var val = Convert.ToDouble(t.MeasurementList.CholesterolRecords.Records[0].CholesterolValue);
                t.CholFlag = val > averageChol;
            }
        }
        
        /// <summary>
        /// this method sets the systolic and diastolic flag for patients who has high level of Systolic and Diastolic blood pressure
        /// compare to the input value that the practitioner put in
        /// </summary>
        /// <param name="highBloodValues"></param>
        public void HighBloodPressureFlag(SysDiastolicThreshold highBloodValues)
        {
            SysDiastolicValues = highBloodValues;
            var systolicValue = SysDiastolicValues.Systolic;
            var diastolicValue = SysDiastolicValues.Diastolic;

            foreach (var t in Monitors)
            {
                var systolicMonitor = int.Parse(t.MeasurementList.BloodPressureRecords.Records[0].SystolicValue);
                var diastolicMonitor = int.Parse(t.MeasurementList.BloodPressureRecords.Records[0].DiastolicValue);
                t.SystolicFlag = systolicMonitor > systolicValue;
                t.DiastolicFlag = diastolicMonitor > diastolicValue;
            }
            
        }

        /// <summary>
        /// This method deregister a patient from monitor list
        /// </summary>
        /// <param name="monitor"></param>
        public void DeregisterMonitor(Monitor monitor)
        {
            var patient = Patients.SingleOrDefault(p => p.Value.Id == monitor.PatientId).Value;
            Monitors?.Remove(Monitors.SingleOrDefault(m => m.PatientId == monitor.PatientId));
            patient.ChangePatientMonitorState();
            HighCholFlag();
        }

        /// <summary>
        /// this method change the AddedToMonitor flag attribute of the patient
        /// </summary>
        /// <param name="patient"></param>
        public void ChangePatientMonitorState(Patient patient)
        {
            patient.ChangePatientMonitorState();
        }

        /// <summary>
        /// this method updates the new measurement lists that indicates whether each list (cholesterol or bloodpressure) is monitored
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="measurementList"></param>
        public void UpdateMeasurement(Monitor monitor, Measurement measurementList)
        {
            var bloodPressureState = monitor.MeasurementList.BloodPressureRecords.IsMonitored;
            var cholesterolState = monitor.MeasurementList.CholesterolRecords.IsMonitored;
            monitor.MeasurementList = measurementList;
            monitor.MeasurementList.CholesterolRecords.IsMonitored = cholesterolState;
            monitor.MeasurementList.BloodPressureRecords.IsMonitored = bloodPressureState;
        }

        /// <summary>
        /// This methods assign patient list requested from backend assign it to Patients instance in Dashboard model
        /// </summary>
        /// <param name="patients"></param>
        public void FetchPatientList(Dictionary<string, Patient> patients)
        {
            Patients = patients;
        }

        /// <summary>
        /// This methods clear all monitors in Monitor list
        /// </summary>
        public void ClearMonitor()
        {
            Monitors?.Clear();
        }
    }
}
