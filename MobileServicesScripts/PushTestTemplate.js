exports.get = function(request, response) {
    var azure = require('azure');
    var notificationHubService = azure.createNotificationHubService('HubName', 
                        'FullConnectionString');
             
    var payload = '{ "message" : "Template push", "collapse_key" : "Message" }';
    notificationHubService.send('iOSUser || AndroidUser', payload, 
     function(error, outcome) {
         console.log('issue sending push');
         console.log('error: ', error);
         console.log('outcome: ',outcome);
     });  

    
    response.send(statusCodes.OK, { message : 'Notification Sent' });
};