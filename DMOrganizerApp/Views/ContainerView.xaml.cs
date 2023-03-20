using CSToolbox.Weak;
using DMOrganizerModel.Interface.Items;
using DMOrganizerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMOrganizerApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ContainerView.xaml
    /// </summary>
    public partial class ContainerView : UserControl
    {

        public ContainerView()
        {
            InitializeComponent();

        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ObjectContainerViewModel model = this.DataContext as ObjectContainerViewModel;
            double yadjust = this.Height + e.VerticalChange;
            double xadjust = this.Width + e.HorizontalChange;
            //hard coded page values, need to fix
            if ((xadjust >= this.MinWidth) && (yadjust >= MinHeight) 
            && (xadjust + (double)model.Width.Value + (double)model.CoordX.Value <= 1240) 
            && (yadjust + (double)model.Height.Value + (double)model.CoordY.Value <= 1753))
            {
            this.Width = xadjust;
            this.Height = yadjust;
            }

        }

        private void ResizeThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

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
            
            //double left = Canvas.GetLeft(item);
            //double top = Canvas.GetTop(item);

            //Canvas.SetLeft(item, left + e.HorizontalChange);
            //Canvas.SetTop(item, top + e.VerticalChange);
            model.CoordX.Value = model.CoordX.Value + (int)e.HorizontalChange;
            model.CoordY.Value = model.CoordX.Value + (int)e.VerticalChange;
        }

        private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {

        }

        
    }
    //public class MoveThumb : Thumb
    //{
    //    public MoveThumb()
    //    {
    //        DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
    //    }

    //    private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
    //    {
    //        Control item = this.DataContext as Control;

    //        if (item != null)
    //        {
    //            double left = Canvas.GetLeft(item);
    //            double top = Canvas.GetTop(item);

    //            Canvas.SetLeft(item, left + e.HorizontalChange);
    //            Canvas.SetTop(item, top + e.VerticalChange);
    //        }
    //    }
    //}
}
