using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using FacebookClone.Models.Data;
using Newtonsoft.Json;

namespace FacebookClone
{
    [HubName("echo")]
    public class EchoHub : Hub
    {
        public void Hello(string message)
        {
            //    Clients.All.hello();
            //set clients
            var clients = Clients.All;
            //call js functions
            clients.test("this is a test");
        }

        public void Notify(string friend)
        {
            //init db
            Db db = new Db();
            //get friends id
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(friend));
            int friendId = userDto.Id;

            //get fr count
            var frCount = db.Friends.Count(x => x.User2 == friendId && x.Active == false);
            //set clients
            var clients = Clients.Others;

            //call js function

            clients.frnotify(friend, frCount);
        }

        public void GetFrcount()
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(Context.User.Identity.Name));
            int userId = userDto.Id;
            var friendReqCount = db.Friends.Count(x => x.User2 == userId && x.Active == false);
            var clients = Clients.Caller;
            clients.frcount(Context.User.Identity.Name, friendReqCount);
        }

        public void GetFcount(int friendId)
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(Context.User.Identity.Name));
            int userId = userDto.Id;
            var friendCount1 = db.Friends.Count(x => (x.User1 == userId || x.User2 == userId) && x.Active);


            UserDTO userDto2 = db.Users.FirstOrDefault(x => x.Id == friendId);
            string username = userDto2.Username;

            var friendCount2 = db.Friends.Count(x => (x.User1 == friendId || x.User2 == friendId) && x.Active);

            var clients = Clients.All;
            clients.fcount(Context.User.Identity.Name,username,friendCount1,friendCount2);
        }

        public void notifyOfMessage(string friend)
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(friend));
            int userId = userDto.Id;

            var messageCoount = db.Messages.Count(x => x.To == userId && x.Read == false);


            var client = Clients.Others;
            client.msgcount(friend,messageCoount);
        }

        public void NotifyOfMessageOwner()
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(Context.User.Identity.Name));
            int userId = userDto.Id;

            var messageCoount = db.Messages.Count(x => x.To == userId && x.Read == false);


            var client = Clients.Caller;
            client.msgcount(Context.User.Identity.Name, messageCoount);
        }

        public override Task OnConnected()
        {
            Db db = new Db();
            UserDTO userDto = db.Users.FirstOrDefault(x => x.Username.Equals(Context.User.Identity.Name));
            int userId = userDto.Id;

            string connId = Context.ConnectionId;
            if (!db.Online.Any(x => x.Id == userId))
            {


                OnlineDTO onlineDto = new OnlineDTO();
                onlineDto.Id = userId;
                onlineDto.ConnId = connId;

                db.Online.Add(onlineDto);
                db.SaveChanges();
            }

            List<int> onlineIds = db.Online.ToArray().Select(x => x.Id).ToList();

            List<int> friendsId1 =
    db.Friends.Where(x => x.User1 == userId && x.Active == true).ToArray().Select(x => x.User2).ToList();
            List<int> friendsId2 =
    db.Friends.Where(x => x.User2 == userId && x.Active == true).ToArray().Select(x => x.User1).ToList();
            List<int> allFriendsIds = friendsId1.Concat(friendsId2).ToList();

            List<int> resultList = onlineIds.Where((i) => allFriendsIds.Contains(i)).ToList();

            Dictionary<int,string> dictFriends=new Dictionary<int, string>();
            foreach (var id in resultList)
            {
                var users = db.Users.Find(id);
                string friend = users.Username;

                if (!dictFriends.ContainsKey(id))
                {
                    dictFriends.Add(id,friend);
                }
            }


            var transformed = from key in dictFriends.Keys
                select new {id = key, friend = dictFriends[key]};


            string json = JsonConvert.SerializeObject(transformed);
            var clients = Clients.Caller;
            clients.getonlinefriends(json, Context.User.Identity.Name);
            //Log User Conn
            //Trace.WriteLine
            return base.OnConnected();
        }
    }
}