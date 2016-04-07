using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.UI.Core;

namespace MixMusic.Helpers
{
    public class CommonHelper
    {
        public static Func<DispatchedHandler, Task> CallOnUiThreadAsync = async (handler) => await
        CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);

        //CheckInternet Access

        public static bool CheckInternetAccess()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile == null) return false;

            var connectivityLevel = profile.GetNetworkConnectivityLevel();
            return connectivityLevel.HasFlag(NetworkConnectivityLevel.InternetAccess);
        }
        public static bool IsInternetAccess()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

    }
}
