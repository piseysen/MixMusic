using Microsoft.Practices.ServiceLocation;
using MixMusic.ComponentModel;
using MixMusic.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.System.Profile;

namespace MixMusic.Models
{
    /// <summary>
    /// Contains global access to specific data.
    /// </summary>
    public class AppEnvironment : ObservableObjectBase, IAppEnvironment
    {
        public static readonly int DefaultMinimumCropDimension = 200;
        public static readonly double FloatingComparisonTolerance = 0.001;
        private User _currentUser;

        /// <summary>
        /// Gets or sets the number of category thumbnails that are requested
        /// from the service.
        /// </summary>
        public int CategoryThumbnailsCount { get; private set; }

        /// <summary>
        /// Stores the current user that is logged in.
        /// </summary>
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (value != _currentUser)
                {
                    _currentUser = value;
                    NotifyPropertyChanged(nameof(CurrentUser));

                    CurrentUserChanged?.Invoke(this, CurrentUser);
                }
            }
        }

        /// <summary>
        /// Gets the device family.
        /// </summary>
        private DeviceFamily DeviceFamily { get; } = AnalyticsInfo.VersionInfo.DeviceFamily.ToDeviceFamily();

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static AppEnvironment Instance
        {
            get { return ServiceLocator.Current.GetInstance<IAppEnvironment>() as AppEnvironment; }
        }

        /// <summary>
        /// Determines if app runs on a desktop device.
        /// </summary>
        public bool IsDesktopDeviceFamily
        {
            get { return DeviceFamily == DeviceFamily.Desktop; }
        }

        /// <summary>
        /// Determines if app runs on a mobile device.
        /// </summary>
        public bool IsMobileDeviceFamily
        {
            get { return DeviceFamily == DeviceFamily.Mobile; }
        }

        /// <summary>
        /// Invoked when the current user has changed.
        /// </summary>
        public event EventHandler<User> CurrentUserChanged;

        /// <summary>
        /// Applies the given configuration.
        /// </summary>
        /// <param name="config">The configuration to apply.</param>
        public void SetConfig(Config config)
        {
            if (IsMobileDeviceFamily)
            {
                CategoryThumbnailsCount = config.CategoryThumbnailsSmallFormFactor;
            }
            else
            {
                CategoryThumbnailsCount = config.CategoryThumbnailsLargeFormFactor;
            }
        }
    }
}
