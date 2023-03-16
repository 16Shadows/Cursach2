using System;
using System.Globalization;

namespace MVVMToolbox.Services
{
    public class InputBoxConfiguration<Scenarios, ResultType> where Scenarios : Enum
    {
        /// <summary>
        /// The delegate type of Converter
        /// </summary>
        /// <param name="value">User's input.</param>
        /// <param name="result">The conversion result.</param>
        /// <param name="culture">Active UI culture.</param>
        /// <returns>True on successful conversion.</returns>
        public delegate bool ConverterType(string value, out ResultType? result, CultureInfo culture);

        /// <summary>
        /// The scenario for which the InputBox is displayed.
        /// </summary>
        public Scenarios Scenario { get; }
        /// <summary>
        /// The resulting user's input.
        /// </summary>
        public ResultType? UserInput { get; set; }
        /// <summary>
        /// The method used to convert and validate user-input string to the desired value.
        /// </summary>
        public ConverterType Converter { get; }
        /// <summary>
        /// By default the input box is expected to forbid invalid input.
        /// If this is set to true,
        /// </summary>
        public bool AllowFaultyInput { get; set; } = false;

        public InputBoxConfiguration(Scenarios activeScenario, ConverterType converter)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Scenario = activeScenario;
        }
    }

    /// <summary>
    /// Possible results for <see cref="IInputBoxService{Scenarios}"/>'s results.
    /// </summary>
    public enum InputBoxResult
    {
        /// <summary>
        /// User input was successfully obtained.
        /// </summary>
        Success,
        /// <summary>
        /// The user's input could not be converted or validated.
        /// Can only be returned if AllowFaultyInput is set to true in configuration.
        /// </summary>
        FaultyInput,
        /// <summary>
        /// The user canceled input.
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Provides a way to forcibly ask the user for input, forbidding other actions until input is done.
    /// </summary>
    /// <typeparam name="Scenarios">The enumerator type which represents scenarios supported by this service.</typeparam>
    public interface IInputBoxService<Scenarios> where Scenarios : Enum
    {
        /// <summary>
        /// Asks the user for input
        /// </summary>
        /// <typeparam name="ReturnType">The final (converted) type of user's input.</typeparam>
        /// <param name="configuration">A configuration for this action.</param>
        /// <returns>A result based on user's actions. See <see cref="InputBoxResult"/></returns>
        InputBoxResult Show<ReturnType>(InputBoxConfiguration<Scenarios, ReturnType> configuration);
    }
}
