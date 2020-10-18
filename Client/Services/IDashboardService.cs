using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FIT3077.Shared.Models;

namespace FIT3077.Client.Services
{
    public interface IDashboardService
    {
        public IReadOnlyList<Monitor> Monitors { get;  }
        public IReadOnlyDictionary<string, Patient> Patients {get;  }
        public bool SearchInProgress { get; }
        public event Action OnChange;
        public System.Timers.Timer t { get;  }

        Task Search(InputParameter patientId);
        Task AddToMonitors(Patient patient);
        void RemoveFromMonitors(Monitor monitor);
        void ModifyRecordState(RecordList recordList);
        Task Update();
        void SetTime(InputParameter timeInput);
        void ProcessHighBloodInput(SysDiastolicThreshold highBloodValues);
        void CreateTimer();
    }
}
