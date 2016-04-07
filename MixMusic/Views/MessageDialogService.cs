using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MixMusic.Views
{
    /// <summary>
    /// A dialog service that uses <see cref="MessageDialog"/> for showing
    /// messages to the user.
    /// </summary>
    /// 
    public class MessageDialogService : IDialogService
    {
        private readonly CoreDispatcher _dispatcher;

        public MessageDialogService()
        {
            _dispatcher = Window.Current.Dispatcher;
        }
        public async Task ShowGenericServiceErrorNotification()
        {
            await ShowNotification("ServiceError_Message", "GenericError_Title");

        }

        /// <summary>
        /// Shows the notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="useResourceLoader">
        /// if set to <c>true</c> the message and title
        /// parameters specify the resource id.
        /// </param>
        public async Task ShowNotification(string message, string title, bool useResourceLoader = true)
        {
            Action action = async () =>
            {
                // Load from resource loader if necessary
                if (useResourceLoader)
                {
                    var resourceLoader = ResourceLoader.GetForCurrentView();
                    message = resourceLoader.GetString(message);
                    title = resourceLoader.GetString(title);
                }

                var dialog = new MessageDialog(message, title);
                await dialog.ShowAsync();
            };

            // Some parts of the app like IncrementalLoadingCollection show error messages 
            // on a different thread than the UI thread, so we need to have them dispatched.
            if (_dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => action());
            }
        }


        /// <summary>
        /// Shows a notification that lets the user choose between yes and no.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <param name="useResourceLoader">
        /// if set to <c>true</c> the message and title
        /// parameters specify the resource id.
        /// </param>
        /// <returns>True, if user has chosen yes. Otherwise, false.</returns>
        public async Task<bool> ShowYesNoNotification(string message, string title, bool useResourceLoader = true)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();

            // Load from resource loader if necessary
            if (useResourceLoader)
            {
                message = resourceLoader.GetString(message);
                title = resourceLoader.GetString(title);
            }

            var yesCommand = new UICommand(resourceLoader.GetString("MessageDialog_Yes"));
            var noCommand = new UICommand(resourceLoader.GetString("MessageDialog_No"));

            var dialog = new MessageDialog(message, title);
            dialog.Commands.Add(yesCommand);
            dialog.Commands.Add(noCommand);

            var result = await dialog.ShowAsync();

            return result == yesCommand;
        }
    }
}
