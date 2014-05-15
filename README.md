TechEdNA2014Code
================
This repository holds all of the code for the TechEd North America 2014 session I did on Notification Hubs.  
You can view the recording of the session once posted, [here](http://channel9.msdn.com/Events/TechEd/NorthAmerica/2014/DEV-B345).  

There are a couple considerations for this code including:
* GeoPush contains the Windows Store app sample that gets the user's location and then handles registering to hubs with the city as a tag as well as registering for a template.  In order to get it to work, you'll need to enter your Bing Maps API key, Notification Hub Name, and Notification Hub Listen Connection String in the MainPage.xaml.cs file.  You'll also need to associate the app with an app you have created in the Windows Store admin site.  You can sign up for and create a Bing Maps API key [here](https://www.bingmapsportal.com/application).
* MobileServicesScripts contain the scripts used to tell the Notification Hub to send out push notifications.  In order to use these, you'll need to drop them into your own Mobile Service (I used custom APIs so I could trigger them by visiting the URL corresponding to the API).  You'll also need to enter your Notficiation Hub Name and Notification Hub Full Connection String.
* push contains an iOS app and an Android app.  To get either to work you'll need to enter the your Notficiation Hub Name and Notification Hub Listen Connection String.  You'll also need to set the iOS app's bundle identifer to whatever ID you've created in the Apple Dev Portal and set the GoogleSenderId property in the Android app to the Project ID you've created in Googles Code Console.
* Lastly, for the Notification Hub, you'll need to set the Package SID and Client Secret for Windows Store apps, the GCM API key to push to Android, and upload the certificate you obtain from the Apple Dev Portal for APNS and iOS.

=============
As always, if you have questions, please reach out to me on [twitter](http://twitter.com/chrisrisner) or send me an [email](mailto:chrisner@microsoft.com).
