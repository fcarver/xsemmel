using System.ComponentModel;

namespace XSemmel.Helpers.WPF
{
    /// <summary>
    /// Base class implementing INotifyPropertyChanged interface handling
    /// </summary>
    public abstract class PropertyChangeNotifierBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        /// <summary>
        /// occurs if a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// signal a property changed event
        /// </summary>
        /// <param name="name">name of changed property</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler pc = PropertyChanged;
            if (pc != null)
                pc(this, new PropertyChangedEventArgs(name));
        }
    }
}
