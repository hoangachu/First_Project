using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
namespace Hoang_Project_one.Hubs
{
    public class ChatHub : Hub
    {
        //FileController fileController = new FileController();
        const int CheckUser = 2;
        public async Task SendMessage(string userIDReceive, string username, string message,string imgURL)
        {
            if (!string.IsNullOrEmpty(message))
            {
                //FileController.CreatXMLFile(userIDReceive, message);
                await Clients.All.SendAsync("ReceiveMessage", userIDReceive, username, message, CheckUser, imgURL);
            }

        }
        public async Task TestConnect(string userID)
        {
           
                //FileController.CreatXMLFile(userIDReceive, message);
                await Clients.All.SendAsync("CheckStatus", userID);
            

        }
    }
}
