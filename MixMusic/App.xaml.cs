using BackgroundAudioShared.Services;
using MixMusic.Common;
using MixMusic.Facades;
using MixMusic.Lifecycle;
using MixMusic.Unity;
using ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MixMusic
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static ItemListArgs iMixMusicData = new ItemListArgs();
        private const string sessionStateFilename = "_sessionState.xml";
        private RootFrameNavigationHelper rootFrameNavigationHelper;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Initialize the App launch.
        /// </summary>
        /// <returns>The AppShell of the app.</returns>
        private Shell Initialize()
        {
            var shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                //UnityBootstrapper.Init();
                //UnityBootstrapper.ConfigureRegistries();


                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new Shell();

                // Set the default language
                shell.Language = ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;
            }

            return shell;
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            var shell = Window.Current.Content as Shell;

            // Check if the application needs to be initialized.
            if (shell == null)
            {
                shell = Initialize();
            }

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            //GoldReceivedNotificationClickedHandler(args);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            /*
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            Shell shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a AppShell to act as the navigation context and navigate to the first page
                shell = new Shell();
                SuspensionManager.RegisterFrame(shell.AppFrame, "AppFrame");
                // Use the RootFrameNavigationHelper to respond to keyboard and mouse shortcuts.
                this.rootFrameNavigationHelper = new RootFrameNavigationHelper(shell.AppFrame);

                // Set the default language
                shell.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                shell.AppFrame.NavigationFailed += OnNavigationFailed;
                // If this is not the first time the app is run, then restore from the previous session.
                StorageFile file;
                try
                {
                    file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                }
                catch (Exception)
                {
                    file = null;
                }

                if (file != null)
                {
                    //Load state from previously suspended application
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (MixMusic.Common.SuspensionManagerException)
                    {

                    }
                }


                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                //{
                //    //TODO: Load state from previously suspended application
                //}

                // Place our app shell in the current Window
                Window.Current.Content = shell;
            }

            if (shell.AppFrame.Content == null)
            {
                // When the navigation stack isn't restored, navigate to the first page
                // suppressing the initial entrance animation.
                shell.AppFrame.Navigate(typeof(MainPage), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }

            // Ensure the current window is active
            Window.Current.Activate();

            */

            var shell = Initialize();

            // Place our app shell in the current Window
            Window.Current.Content = shell;

            if (shell.AppFrame.Content == null)
            {
                shell.AppFrame.Navigate(typeof(MainPage), e.Arguments, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());

                //var facade = ServiceLocator.Current.GetInstance<INavigationFacade>();

                //if (AppLaunchCounter.IsFirstLaunch())
                //{
                //    facade.NavigateToMainView();
                //}
                //else
                //{
                //    //facade.NavigateToCategoriesView();
                //}
            }

            // Refresh launch counter, needs to be done
            // after AppLaunchCounter.IsFirstLaunch() is being checked.
            //AppLaunchCounter.RegisterLaunch();

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            //await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var resourceLoader = ResourceLoader.GetForCurrentView();
            var dialog = new MessageDialog(resourceLoader.GetString("UnexpectedError_Message"),
                resourceLoader.GetString("UnexpectedError_Title"));

            await dialog.ShowAsync();
        }

    }
}
