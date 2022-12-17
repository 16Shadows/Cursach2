using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DMOrganizerApp.Resources
{
    public sealed class ColorScheme : INotifyPropertyChanged
    {
        #region Properties
        private Brush m_ApplicationBackground;
        public Brush ApplicationBackground
        {
            get => m_ApplicationBackground;
            set
            {
                m_ApplicationBackground = value ?? throw new ArgumentNullException(nameof(ApplicationBackground));
                InvokePropertyChanged(nameof(ApplicationBackground));
            }
        }

        private Brush m_ElementBackground;
        public Brush ElementBackground
        {
            get => m_ElementBackground;
            set
            {
                m_ElementBackground = value ?? throw new ArgumentNullException(nameof(ElementBackground));
                InvokePropertyChanged(nameof(ElementBackground));
            }
        }

        private Brush m_OrdinaryText;
        public Brush OrdinaryText
        {
            get => m_OrdinaryText;
            set
            {
                m_OrdinaryText = value ?? throw new ArgumentNullException(nameof(OrdinaryText));
                InvokePropertyChanged(nameof(OrdinaryText));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void InvokePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Constructors
        public ColorScheme()
        {
            m_ApplicationBackground = Brushes.LightGray;
            m_ElementBackground = Brushes.White;
            m_OrdinaryText = Brushes.Black;
        }
        #endregion

        #region Methods
        public void Load(ColorScheme scheme)
        {
            if (scheme == null)
                throw new ArgumentNullException(nameof(scheme));

            ApplicationBackground = scheme.ApplicationBackground;
            ElementBackground = scheme.ElementBackground;
            OrdinaryText = scheme.OrdinaryText;
        }
        #endregion
    }
}
