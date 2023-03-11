using System;

namespace MVVMToolbox
{
    /// <summary>
    /// Provides an implementation for a safe method to perform changes to the UI
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Performs a change synchronously
        /// </summary>
        /// <param name="callback">The change to perform</param>
        void Invoke(Action callback);
        /// <summary>
        /// Starts performing a change asynchronously
        /// </summary>
        /// <param name="callback">The change to perform</param>
        void BeginInvoke(Action callback);
    }
}
