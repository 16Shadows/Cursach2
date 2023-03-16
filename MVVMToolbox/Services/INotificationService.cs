using System;

namespace MVVMToolbox.Services
{
    public class NotificationConfiguration<Scenarios> where Scenarios : Enum
    {
        public Scenarios Scenario { get; }
        
        public NotificationConfiguration(Scenarios scenario)
        {
            Scenario = scenario;
        }
    }

    public interface INotificationService<Scenarios> where Scenarios : Enum
    {
        void Show(NotificationConfiguration<Scenarios> configuration);
    }
}
