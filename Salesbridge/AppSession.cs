using System;

namespace Salesbridge
{
    public static class AppSession
    {
        public static string CurrentUsername { get; set; } = "Staff";
        public static string CurrentUserEmail { get; set; } = "";

        public static event Action<string> NotificationAdded; //fired whenever a notification is added so open NOTIFICATION forms can refresh

        public static void RaiseNotification(string message)
        {
            DatabaseHelper.AddNotification(message);
            NotificationAdded?.Invoke(message);
        }
    }
}
