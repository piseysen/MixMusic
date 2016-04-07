using Windows.System.Profile;
using Windows.UI.Xaml;

namespace MixMusic.StateTriggers
{
    /// <summary>
    /// Custom trigger for DeviceFamily UI states.
    /// </summary>
    public class DeviceFamilyTrigger : StateTriggerBase
    {
        private string _currentDeviceFamily;
        private string _queriedDeviceFamily;

        /// <summary>
        /// The target device family.
        /// </summary>
        public string DeviceFamily
        {
            get { return _queriedDeviceFamily; }
            set
            {
                _queriedDeviceFamily = value;

                // Get the current device family.
                _currentDeviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;

                // The trigger will be activated if the current device family
                // matches the device family value in XAML.
                SetActive(_queriedDeviceFamily == _currentDeviceFamily);
            }
        }
    }
}
