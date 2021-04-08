"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("btntestconnection").disabled = true;

connection.on("CheckStatus", function (userID) {
    var urlPath = window.location.href.toString().substring(0, window.location.href.toString().lastIndexOf('/'));
    $.ajax({
        url: urlPath + "/CheckStatus",
        data: { userid: $('#userID').val() },
        type: 'POST',
        sync: true,
        success: function (data) {
            console.log(data.data);
            for (var i = 0; i <= data.data.length; i++) {
                if (data.data[i].status == 1 || data.data[i].status == 2) {
                    $('#user_' + data.data[i].friendId).find('span').css('background-color', 'limegreen')
                }
                else {
                    $('#user_' + data.data[i].friendId).find('span').css('background-color', '#bbb')
                }
               
            }
        }
    });
    $.ajax({
        url: urlPath + "/CheckUserStatus",
        data: { userid: $('#userID').val() },
        type: 'POST',
        sync: true
    });

});

connection.start().then(function () {
    document.getElementById("btntestconnection").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btntestconnection").addEventListener("click", function (event) {
    var userID = document.getElementById("userID").value;
    connection.invoke("TestConnect", userID).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});



$(document).ready(function () {

    setInterval(function () {
        $('#btntestconnection').click();

    }, 200);


});
