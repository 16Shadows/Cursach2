using System;
using System.ComponentModel;
using CSToolbox.Weak;

namespace CSToolbox
{
    /// <summary>
    /// Represents a lazy property which doesn't have a value until it's fetched or set for the first time
    /// </summary>
    /// <typeparam name="T">The type of this property</typeparam>
    public class LazyProperty<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// The method to use for initialization. May never be called if the value is set before first fetch.
        /// </summary>
        /// <param name="property">A reference to the property which is being initialized</param>
        public delegate void LoaderType(LazyProperty<T> property);

        /// <summary>
        /// Raised when this property changes. 
        /// Implemented using <see cref="WeakEvent"/> to allow the object holding this property to subscribe to it without causing a memory leak.
        /// </summary>
        public WeakEvent<LazyProperty<T>> WeakPropertyChanged { get; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        private LoaderType m_Loader;
        private T? m_Value;
        private bool m_Loaded;

        /// <summary>
        /// The value this property has
        /// </summary>
        public T? Value
        {
            get
            {
                if (!m_Loaded)
                {
                    lock (WeakPropertyChanged)
                    {
                        if (!m_Loaded)
                        {
                            m_Loader(this);
                        }
                    }
                }
                return m_Value;
            }
            set
            {
                if (m_Value?.Equals(value) == true)
                    return;

                if (!m_Loaded)
                {
                    lock (WeakPropertyChanged)
                    {
                        if (!m_Loaded)
                        {
                            m_Loaded = true;
                            m_Loader = null;
                        }
                    }
                }
                m_Value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                WeakPropertyChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Create a lazy property
        /// </summary>
        /// <param name="loader">Initialization method</param>
        /// <exception cref="ArgumentNullException">Can be thrown if loader is null</exception>
        public LazyProperty(LoaderType loader)
        {
            m_Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }
    }

    /// <summary>
    /// Represents a lazy property which doesn't have a value until it's fetched or set for the first time. After being initializer, the value cannot be set.
    /// </summary>
    /// <typeparam name="T">The type of this property</typeparam>
    public class ReadOnlyLazyProperty<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// The method to use for initialization. May never be called if the value is set before first fetch.
        /// </summary>
        /// <param name="property">A reference to the property which is being initialized</param>
        public delegate void LoaderType(ReadOnlyLazyProperty<T> property);

        /// <summary>
        /// Raised when this property changes. 
        /// Implemented using <see cref="WeakEvent"/> to allow the object holding this property to subscribe to it without causing a memory leak.
        /// </summary>
        public WeakEvent<ReadOnlyLazyProperty<T>> WeakPropertyChanged { get; } = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        private LoaderType m_Loader;
        private T? m_Value;
        private bool m_Loaded;

        /// <summary>
        /// The value this property has
        /// </summary>
        /// <exception cref="InvalidOperationException">Can be raised when attempting to set the value of an already initializer property</exception>
        public T? Value
        {
            get
            {
                if (!m_Loaded)
                {
                    lock (WeakPropertyChanged)
                    {
                        if (!m_Loaded)
                        {
                            m_Loader(this);
                        }
                    }
                }
                return m_Value;
            }
            set
            {
                if (m_Loaded)
                    throw new InvalidOperationException();
                lock (WeakPropertyChanged)
                {
                    if (!m_Loaded)
                    {
                        m_Value = value;
                        m_Loaded = true;
                        m_Loader = null;
                    }
                }
                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                WeakPropertyChanged.Invoke(this);
            }
        }

        /// <summary>
        /// Create a lazy property
        /// </summary>
        /// <param name="loader">Initialization method</param>
        /// <exception cref="ArgumentNullException">Can be thrown if loader is null</exception>
        public ReadOnlyLazyProperty(LoaderType loader)
        {
            m_Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }
    }
}
