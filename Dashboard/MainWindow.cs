using System.Windows;

namespace Dashboard
{
    internal class MainWindow
    {
        public WindowState WindowState { get; set; }
        public double Left { get; internal set; }
        public double Width { get; internal set; }
        public double Top { get; internal set; }
        public double Height { get; internal set; }

        internal void Show()
        {
            throw new NotImplementedException();
        }
    }
}