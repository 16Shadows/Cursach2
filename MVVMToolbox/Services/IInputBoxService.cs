using System;

namespace MVVMToolbox.Services
{
    public class InputBoxConfiguration<Scenarios> where Scenarios : Enum
    {
        public Scenarios ActiveScenario { get; }

        public InputBoxConfiguration(Scenarios activeScenario)
        {
            ActiveScenario = activeScenario;
        }
    }

    /// <summary>
    /// Provides a way to ask the user for input in a message-box style way
    /// </summary>
    public interface IInputBoxService<Scenarios> where Scenarios : Enum
    {
        object Show(InputBoxConfiguration<Scenarios> configuration);
    }
}
