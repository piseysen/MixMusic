using MixMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMusic.Extensions
{
    /// <summary>
    /// Provides string extensions for <see cref="DeviceFamily" />.
    /// </summary>
    public static class DeviceFamilyStringExtensions
    {
        /// <summary>
        /// Parses the input value to <see cref="DeviceFamily" />.
        /// </summary>
        /// <param name="value">The value to parse to DeviceFamily.</param>
        /// <returns>The DeviceFamily.</returns>
        public static DeviceFamily ToDeviceFamily(this string value)
        {
            switch (value)
            {
                case "Windows.Desktop":
                    return DeviceFamily.Desktop;
                case "Windows.Mobile":
                    return DeviceFamily.Mobile;
                default:
                    return DeviceFamily.Other;
            }
        }
    }
}
