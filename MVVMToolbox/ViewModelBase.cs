using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MVVMToolbox
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private int HostsCount { get; set;}

        protected IContext Context { get; }
        protected IServiceProvider ServiceProvider { get; }

        public ViewModelBase(IContext context, IServiceProvider serviceProvider)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Load()
        {
            if (HostsCount == 0)
                OnLoad();
            HostsCount++;
        }

        public void Unload()
        {
            HostsCount--;
            if (HostsCount == 0)
                OnUnload();
        }

        /// <summary>
        /// Is called just before this view model is hosted by a ViewModelHost for the first time after it had no hosts.
        /// </summary>
        protected virtual void OnLoad() {}

        /// <summary>
        /// Is called right after this view model is no longer hosted by any ViewModelHosts
        /// </summary>
        protected virtual void OnUnload() {}

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        protected void InvokeErrorsChanged(string? name)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(name));
        }
        #endregion

        #region Errors
        private Dictionary<string, object?> FailedValues { get; } = new Dictionary<string, object?>();
        /// <summary>
        /// Stores validators for properties which provides value for INotifyDataErrorInfo
        /// </summary>
        protected Dictionary<string, Func<object?, IEnumerable>> Validators { get; } = new Dictionary<string, Func<object?, IEnumerable>>();

        public bool HasErrors => FailedValues.Count > 0;
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                foreach (KeyValuePair<string, object?> value in FailedValues)
                    yield return Validators[value.Key](value.Value);
            }
            else if (FailedValues.TryGetValue(propertyName, out object? value))
                yield return Validators[propertyName](value);

            yield break;
        }

        /// <summary>
        /// Validates a property and indicates whether it should be updated
        /// </summary>
        /// <param name="name">The name of the property (must have an associated validator)</param>
        /// <param name="value">The new value for the property</param>
        /// <returns>True if the property should accept the value, false otherwise</returns>
        protected bool ValidateProperty(string name, object? value)
        {
            if (Validators[name](value).Any())
            {
                FailedValues[name] = value;
                return false;
            }
            FailedValues.Remove(name);
            return true;
        }

        /// <summary>
        /// Returns the current invalid value of the property or null if the property is valid
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The current invalid value of the property. May be null</returns>
        protected object? GetPropertyValue(string name)
        {
            if (FailedValues.TryGetValue(name, out object? v))
                return v;
            return null;
        }
        #endregion
    }
}
