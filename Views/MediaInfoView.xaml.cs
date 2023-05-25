using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BingedIt.ViewModels;

namespace BingedIt.Views
{
    /// <summary>
    /// Interaction logic for MediaInfoView.xaml
    /// </summary>
    public partial class MediaInfoView : UserControl
    {
        public MediaInfoView()
        {
            InitializeComponent();
        }

        private static bool IsValidLink(DragEventArgs args) => args.AllowedEffects.HasFlag(DragDropEffects.Link) && (args.Data.GetDataPresent(DataFormats.Text) || args.Data.GetDataPresent(DataFormats.FileDrop));
        private void CheckImageOnDragEnter(object sender, DragEventArgs e)
        {
            if (IsValidLink(e))
            {
                e.Handled = true;
                return;
            }
            // Disable dropping because the path is invalid
            Image image = (Image)sender;
            image.AllowDrop = false;
            image.MouseLeave += UpdateImageOnMouseLeave;
        }
        private void UpdateImageOnMouseLeave(object sender, MouseEventArgs e)
        {
            // Re-enable dropping incase we disabled it
            Image image = (Image)sender;
            image.AllowDrop = true;
            image.MouseLeave -= UpdateImageOnMouseLeave;
        }
        private void AddImageOnDrop(object sender, DragEventArgs e)
        {
            MediaViewModel vm = (MediaViewModel)DataContext;
            string path = (string)e.Data.GetData(DataFormats.Text);
            if (!string.IsNullOrWhiteSpace(path))
                vm.Poster.Link = path;
            else
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
                string? newPath = paths.FirstOrDefault();
                if (newPath is not null)
                    vm.Poster.Link = newPath;
            }
            return;
        }

        private void UpdateToolTipOnMouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            Point mousePos = e.GetPosition(element);
            ToolTip tt = (ToolTip)element.ToolTip;
            tt.HorizontalOffset = mousePos.X + 5;
            tt.VerticalOffset = mousePos.Y + 5;
        }

        private void OnHideButtonClick(object sender, RoutedEventArgs e) => DataContext = null;
        private void OnDataContextChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Set Visibility to false if DataContext is null
            if (e.NewValue is not null)
            {
                Visibility = Visibility.Visible;
                Keyboard.ClearFocus();
                Keyboard.Focus(txtTitle);
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
