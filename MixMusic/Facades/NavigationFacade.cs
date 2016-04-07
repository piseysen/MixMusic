using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Reflection;
using MixMusic.Serialization;
using MixMusic.ViewModels;

namespace MixMusic.Facades
{
    /// <summary>
    /// Encapsulates page navigation.
    /// </summary>
    /// 

    public class NavigationFacade : INavigationFacade
    {
        /// <summary>
        /// The mappings between views and their view models
        /// </summary>
        private static readonly Dictionary<Type, Type> ViewViewModelDictionary = new Dictionary<Type, Type>();

        /// <summary>
        /// The current frame.
        /// </summary>
        private Frame _frame;

        /// <summary>
        /// Determines if back navigation
        /// </summary>
        public bool CanGoBack
        {
            get { return _frame.CanGoBack; }
        }

        /// <summary>
        /// Adds the specified types to the association list.
        /// </summary>
        /// <param name="view">The view type.</param>
        /// <param name="viewModel">The ViewModel type.</param>
        /// <exception cref="System.ArgumentException">The ViewModel has already been added and is only allowed once.</exception>
        public static void AddType(Type view, Type viewModel)
        {
            if (ViewViewModelDictionary.ContainsKey(viewModel))
            {
                throw new ArgumentException("The ViewModel has already been added and is only allowed once.");
            }

            ViewViewModelDictionary.Add(viewModel, view);
        }

        /// <summary>
        /// Makes sure a frame is available that can be used
        /// for navigation.
        /// </summary>
        private void EnsureNavigationFrameIsAvailable()
        {
            var content = Window.Current.Content;

            // The default state is that we expect to have the
            // AppShell as a hosting view for content
            if (content is Shell)
            {
                var appShell = content as Shell;
                _frame = appShell.AppFrame;
            }

            // We can also have a simple frame when the user
            // chooses to use the share target contract to share
            // photos from the Windows photos app.
            else if (content is Frame)
            {
                var frameShell = content as Frame;
                _frame = frameShell;
            }
            else
            {
                throw new ArgumentException("Window.Current.Content");
            }
        }

        /// <summary>
        /// Goes back in the navigation stack for the specified
        /// number of steps.
        /// </summary>
        /// <param name="steps">The steps. By default: 1.</param>
        /// 
        public void GoBack(int steps = 1)
        {
            EnsureNavigationFrameIsAvailable();

            if (steps > 1)
            {
                RemoveBackStackFrames(steps - 1);
            }

            if (_frame.CanGoBack)
            {
                _frame.GoBack();
            }
        }

        /// <summary>
        /// Navigates to the specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameter">The parameter. Optional.</param>
        /// <param name="serializeParameter">The serialized parameter. Optional.</param>
        private void Navigate(Type viewModelType, object parameter = null, bool serializeParameter = true)
        {
            var view = ViewViewModelDictionary[viewModelType];

            if (view == null)
            {
                throw new ArgumentException("The specified ViewModel could not be found.");
            }

            // Navigation has to be different if the view is a SettingsFlyout 
            // so this is checked here using reflection
            if (view.GetTypeInfo().IsSubclassOf(typeof(SettingsFlyout)))
            {
                // Create instance and show SettingsFlyout
                var flyout = (SettingsFlyout)Activator.CreateInstance(view);
                flyout.ShowIndependent();
            }
            else
            {
                // This is the navigation logic for views that are not
                // inherited from SettingsFlyout
                EnsureNavigationFrameIsAvailable();

                if (parameter == null)
                {
                    _frame.Navigate(view);
                }
                else
                {
                    if (serializeParameter)
                    {
                        var serialized = SerializationHelper.Serialize(parameter);
                        _frame.Navigate(view, serialized);
                    }
                    else
                    {
                        _frame.Navigate(view, parameter);
                    }
                }
            }
        }

        public void NavigateToProductionView()
        {
            throw new NotImplementedException();
        }

        public void NavigateToSettingView()
        {
            throw new NotImplementedException();
        }

        public void NavigateToSingerView()
        {
            Navigate(typeof(ProductionViewModel));
        }

        /// <summary>
        /// Displays a dialog that lets the user pick
        /// a category.
        /// Removes the specified number of frames from the back stack.
        /// </summary>
        /// <param name="numberOfFrames">The number of frames.</param>
        public void RemoveBackStackFrames(int numberOfFrames)
        {
            EnsureNavigationFrameIsAvailable();

            var framesToRemove = numberOfFrames;
            framesToRemove = Math.Min(framesToRemove, _frame.BackStackDepth);

            while (framesToRemove > 0)
            {
                _frame.BackStack.RemoveAt(_frame.BackStackDepth - 1);
                framesToRemove--;
            }
        }

        public void NavigateToMainView()
        {
            Navigate(typeof(MainViewModel));
        }
    }
}
