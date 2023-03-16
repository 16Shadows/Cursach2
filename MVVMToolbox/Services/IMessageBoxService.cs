using System;

namespace MVVMToolbox.Services
{
    public enum MessageBoxType
    {
        /// <summary>
        /// A notification which can only be dismissed (informative)
        /// </summary>
        YesPrompt,
        /// <summary>
        /// A notification which expects the user to select from 'yes' or 'no' options (actual wording depends on the service's provider).
        /// Common use case is for actions which require confirmation (e.g. exiting the application).
        /// </summary>
        YesNoPrompt,
        /// <summary>
        /// A notification which expects the user to select from 'yes', 'no', 'cancel' options (actual wording depends on the service's provider).
        /// Common use case is for actions which cause other actions requiring confirmation (e.g. exiting the application with unsaved data).
        /// </summary>
        YesNoCancelPrompt
    }

    public enum MessageBoxResult
    {
        /// <summary>
        /// User selected 'yes' option in <see cref="MessageBoxType.YesPrompt"/>, <see cref="MessageBoxType.YesNoPrompt"/> or <see cref="MessageBoxType.YesNoCancelPrompt"/>.
        /// </summary>
        Yes,
        /// <summary>
        /// User selected 'no' option in <see cref="MessageBoxType.YesNoPrompt"/> or <see cref="MessageBoxType.YesNoCancelPrompt"/>.
        /// </summary>
        No,
        /// User selected 'Cancel' option in <see cref="MessageBoxType.YesNoCancelPrompt"/>.
        Cancel
    }

    public class MessageBoxConfiguration<Scenarios> where Scenarios : Enum
    {
        /// <summary>
        /// The scenarion for which this notification happens.
        /// </summary>
        public Scenarios Scenario { get; }
        /// <summary>
        /// The type of this notification.
        /// </summary>
        public MessageBoxType Type { get; }

        public MessageBoxConfiguration(Scenarios scenario)
        {
            Scenario = scenario;
        }
    }

    /// <summary>
    /// Provides a way to forcibly notify user about something, forbidding other actions until a decision is made.
    /// </summary>
    /// <typeparam name="Scenarios"></typeparam>
    public interface IMessageBoxService<Scenarios> where Scenarios : Enum
    {
        MessageBoxResult Show(MessageBoxConfiguration<Scenarios> configuration);
    }
}
