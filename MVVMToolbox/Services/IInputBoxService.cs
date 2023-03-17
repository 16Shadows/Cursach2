using System;
using System.Globalization;

namespace MVVMToolbox.Services
{
    public class InputBoxConfiguration<Scenarios, ResultType> where Scenarios : Enum
    {
        /// <summary>
        /// The delegate type of <see cref="Converter"/>
        /// </summary>
        /// <param name="value">User-input input.</param>
        /// <param name="culture">Active culture.</param>
        /// <returns>Conversion result.</returns>
        public delegate ResultType ConverterType(string value, CultureInfo culture);
        /// <summary>
        /// The delegate type of <see cref="Validator"/>
        /// </summary>
        /// <param name="value">User-input string.</param>
        /// <param name="culture">Active culture.</param>
        /// <returns>True if the result is valid for conversion.</returns>
        public delegate bool ValidatorType(string value, CultureInfo culture);

        /// <summary>
        /// The scenario for which the InputBox is displayed.
        /// </summary>
        public Scenarios Scenario { get; }
        /// <summary>
        /// The resulting user's input.
        /// </summary>
        public ResultType? UserInput { get; set; }
        /// <summary>
        /// The method used to convert user-input string to the associated value.
        /// Do note that the method is expected to run synchronously.
        /// </summary>
        public ConverterType Converter { get; }
        /// <summary>
        /// The method used to check whether the user-input string is valid for conversion.
        /// Do note that the method is expected to run synchronously.
        /// </summary>
        public ValidatorType Validator { get; }
        /// <summary>
        /// By default the input box is expected to forbid invalid input.
        /// If this is set to true,
        /// </summary>
        public bool AllowFaultyInput { get; set; } = false;

        public InputBoxConfiguration(Scenarios activeScenario, ConverterType converter, ValidatorType validator = null)
        {
            Converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Validator = validator ?? ((_, _) => true);
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
