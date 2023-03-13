using System;

namespace MVVMToolbox.Services
{
    /// <summary>
    /// Provides a way to ask the user for input in a message-box style way
    /// </summary>
    public interface IInputBoxService
    {
        void Show(Enum scenario);
        Enum GetSupportedScenarios();
    }
}
