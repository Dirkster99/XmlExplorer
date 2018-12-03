namespace XmlExplorerDemo
{
    using Settings.UserProfile;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MWindowLib.MetroWindow
                                     , IViewSize  // Implements saving and loading/repositioning of Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.PreviewMouseWheel += textEditor_PreviewMouseWheel;
        }

        /// <summary>
        /// This method is triggered on a MouseWheel preview event to check if the user
        /// is also holding down the CTRL Key and adjust the current font size if so.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double fontSize = XPathTreeView.FontSize + e.Delta / 25.0;

                if (fontSize < 6)
                    XPathTreeView.FontSize = 6;
                else
                {
                    if (fontSize > 200)
                        XPathTreeView.FontSize = 200;
                    else
                        XPathTreeView.FontSize = fontSize;
                }

                e.Handled = true;
            }
        }
    }
}
