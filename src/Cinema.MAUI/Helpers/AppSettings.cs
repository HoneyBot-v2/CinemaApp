using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.MAUI.Helpers;

internal static class AppSettings
{
#if ANDROID
    public const string HttpsApiUrl = "https://10.0.2.2:5001/api/";
    public const string HttpApiUrl = "http://10.0.2.2:5000/api/";
#elif IOS
    public const string HttpsApiUrl = "https://localhost:5001/api/";
    public const string HttpApiUrl = "http://localhost:5000/api/";
#else
    public const string HttpsApiUrl = "https://localhost:5001/api/"; // Windows/macOS
    public const string HttpApiUrl = "http://localhost:5000/api/"; // Windows/macOS
#endif
}
