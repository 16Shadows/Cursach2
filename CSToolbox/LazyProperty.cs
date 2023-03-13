using System;

namespace CSToolbox
{
    /// <summary>
    /// Represents a lazy property which doesn't have a value until its fetched or set for the first time
    /// </summary>
    /// <typeparam name="T">The type of this property</typeparam>
    public class LazyProperty<T>
    {
        public delegate void LoaderType(LazyProperty<T> property);
        public WeakEvent<LazyProperty<T>> PropertyChanged { get; } = new();

        private LoaderType m_Loader;
        private T? m_Value;
        private bool m_Loaded;
        public T? Value
        {
            get
            {
                if (!m_Loaded)
                {
                    m_Loader(this);
                    m_Loader = null;
                    m_Loaded = true;
                }
                return m_Value;
            }
            set
            {
                if (m_Value?.Equals(value) == true)
                    return;
                m_Value = value;
                m_Loaded = true;
                PropertyChanged.Invoke(this);
            }
        }

        public LazyProperty(LoaderType loader)
        {
            m_Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }
    }
}
