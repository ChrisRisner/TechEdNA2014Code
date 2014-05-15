exports.get = function(request, response) {
    var azure = require('azure');
    var notificationHubService = azure.createNotificationHubService('HubName', 
                        'FullConnectionString');
             
    notificationHubService.apns.send(null,
            {
            "alert": "Push to everyone on iOS!"
            }
        ,
        function (error)
        {
            if (!error) {
                console.warn("Notification successful");
            }
        }
    );
             
    
    
    response.send(statusCodes.OK, { message : 'Notification Sent' });
};