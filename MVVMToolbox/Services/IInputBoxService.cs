using System;

namespace MVVMToolbox.Services
{
    /// <summary>
    /// Provides a way to ask the user for input in a message-box style way
    /// </summary>
    public interface IInputBoxService<Scenarios> where Scenarios : Enum
    {
        object Show(Scenarios scenario);
    }
}
