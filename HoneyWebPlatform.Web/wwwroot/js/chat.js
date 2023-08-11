var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//disable send button until connection is established
document.getElementById("sendButton").disable = true;

connection.on("ReceiveMessage",
    function (user, message) {
        var li = document.createElement("li");
        li.textContent = `${user} says ${message}`;
        document.getElementById("messagesList").appendChild(li);
    });

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function(err) {
         return console.error(err.toString());
    });
});