using Microsoft.ServiceBus.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://mynotificationhub2-ns.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=milyEy21w7aEs483j7VJtQMTalJkRINIx0EJz/m+CJw=", "location");

            var toast = "<?xml version=\"1.0\" encoding=\"utf-8\"?><wp:Notification xmlns:wp=\"WPNotification\">  <wp:Toast>    <wp:Text1>Hello</wp:Text1>    <wp:Text2>World!</wp:Text2>    <wp:Param></wp:Param>  </wp:Toast></wp:Notification>";
            hub.SendMpnsNativeNotificationAsync(toast, "PostalCode:98112").Wait();
        }
    }
}
