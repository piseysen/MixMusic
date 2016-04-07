using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace MixMusic.Views
{
    /// <summary>
    /// A base UI page.
    /// </summary>
    public class BasePage : Page, INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies that the property has changed.
        /// </summary>
        /// <param name="propertyName">The property name. Optional.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
