using DMOrganizerApp.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DMOrganizerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ColorScheme DefaultColorScheme;

        #region Properties
        private ColorScheme? m_ActiveColorScheme;
        public ColorScheme ActiveColorScheme
        {
            get => m_ActiveColorScheme;
            set
            {
                m_ActiveColorScheme.Load(value ?? DefaultColorScheme);
            }
        }
        #endregion

        #region Constructors
        static App()
        {
            DefaultColorScheme = new ColorScheme()
            {
                ApplicationBackground = Brushes.LightGray,
                OrdinaryText = Brushes.Black,
                ElementBackground = Brushes.White
            };
        }
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            m_ActiveColorScheme = ((ColorScheme)Resources["ActiveColorScheme"]);
            ActiveColorScheme = DefaultColorScheme;
        }
    }
}
