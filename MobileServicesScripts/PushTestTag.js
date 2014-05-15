exports.get = function(request, response) {
    var azure = require('azure');
    var notificationHubService = azure.createNotificationHubService('HubName', 
                        'FullConnectionString');
             
    notificationHubService.apns.send('Houston',        
        {
        "alert": "Hi houston"
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