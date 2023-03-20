using DMOrganizerViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ContainerView.xaml
    /// </summary>
    public partial class ContainerView : UserControl
    {
        public ObjectContainerViewModel dc;

        public ContainerView()
        {
            dc = this.DataContext as ObjectContainerViewModel;
            InitializeComponent();

        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ObjectContainerViewModel model = this.DataContext as ObjectContainerViewModel;
            double yadjust = this.Height + e.VerticalChange;
            double xadjust = this.Width + e.HorizontalChange;
            //hard coded page values, need to fix
            if ((xadjust >= this.MinWidth) && (yadjust >= MinHeight) 
            && (xadjust  + (double)model.CoordX.Value <= 1240) 
            && (yadjust  + (double)model.CoordY.Value <= 1753))
            {
                this.Width = xadjust;
                this.Height = yadjust;
            }
        }
        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
                UIElement thumb = e.Source as UIElement;
                Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
                Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }

        private void Thumb_DragDelta_1(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ObjectContainerViewModel model = this.DataContext as ObjectContainerViewModel;
            double yadjust = model.CoordY.Value + e.VerticalChange;
            double xadjust = model.CoordX.Value + e.HorizontalChange;
            //hard coded page values, need to fix
            if ((xadjust >= 0) && (xadjust + model.Width.Value <= 1240)
                && (yadjust >= 0) && (yadjust + model.Height.Value <= 1240))
            {
                model.CoordX.Value = (int)xadjust;
                model.CoordY.Value = (int)yadjust;
            }
            //model.CoordX.Value = model.CoordX.Value + (int)e.HorizontalChange;
            //model.CoordY.Value = model.CoordX.Value + (int)e.VerticalChange;
        }

        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {

        }

        
    }
}
