using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Geode.Habbo;
using Geode.Network;
using Geode.Extension;
using HChatBotUtil;

namespace GChatBot
{
    [Module("GChatBot", "Arckmed", "Simple chatbot interface.")]
    public class GChatBot : GService
    {
        public MainWindow MainWindowParent;
        bool isDancing;
        public string ownerName;
        List<LocalHEntity> UsersData = new List<LocalHEntity>();

        HChatBot chat;
        HChatBot chat2;
        public GChatBot(MainWindow MainWindowParent)
        {
            this.MainWindowParent = MainWindowParent;

            chat = new HChatBot();
            chat2 = new HChatBot();

            chat.firstMissReset = true;
            chat.OnMessageResponse += On_MessageResponse;

            chat2.firstMissReset = false;
            chat2.OnMessageResponse += On_MessageResponse;

            #region Chat examples
            HDialogue diag1 = new HDialogue();
            diag1.AddRequest("hello", "hi", "whats up");
            diag1.AddResponse("hello!", "hi!", "hey sup!");
            chat.AddDialogue(diag1);

            HDialogue diag2 = new HDialogue();
            diag2.AddRequest("ping");
            diag2.AddResponse("pong");
            chat.AddDialogue(diag2);

            HDialogue diag3 = new HDialogue();
            diag3.AddRequest("!commands");
            diag3.AddResponse("commands avaiable: !sleep, !time, !dance, !rps, !apresentation, !soda");
            chat.AddDialogue(diag3);

            HDialogue diag4 = new HDialogue();
            diag4.AddRequest("!sleep", "!time", "!dance", "!apresentation");
            diag4.AddResponse(Sleep, Time, Dance, Apresentation);
            chat.AddDialogue(diag4);

            HDialogue diag5 = new HDialogue();
            diag5.AddRequest("!soda");
            diag5.AddResponse("do you like soda?");
            diag5.AddRequest("yes", "no", "maybe");
            diag5.AddResponse("cool!", "oh :(", "hmm");
            chat.AddDialogue(diag5);

            HDialogue rockpaperscissors = new HDialogue();
            rockpaperscissors.AddRequest("!rps");
            rockpaperscissors.AddResponse("say !rock, !paper, or !scissors");
            rockpaperscissors.AddRequest("!rock", "!paper", "!scissors");
            rockpaperscissors.AddResponse(RockPaperScissors, RockPaperScissors, RockPaperScissors);
            rockpaperscissors.AddRequest("gg");
            rockpaperscissors.AddResponse("thanks!");

            chat.AddDialogue(rockpaperscissors);

            //

            HDialogue diag_chat2 = new HDialogue();
            diag_chat2.AddRequest("hello");
            diag_chat2.AddResponse("hi stranger!");
            chat2.AddDialogue(diag_chat2);
            #endregion
        }

        [InDataCapture("Chat")]
        public void OnChat(DataInterceptedEventArgs e)
        {
            //searching for the user name using its index
            int userIndex = e.Packet.ReadInt32();
            string text = e.Packet.ReadUTF8();
            if (UsersData.Exists(any => any.Index == userIndex))
            {
                int UserIndexToGetId = UsersData.FindIndex(any => any.Index == userIndex);

                if (UsersData[UserIndexToGetId].Name == ownerName)
                    chat.Chatting(text);
                else
                    chat2.Chatting(text); // if it's not "ownerName", then use another chat
            }
        }

        //event handler for HChatBot class to send the responses (void methods or strings)
        public void On_MessageResponse(object sender, object e)
        {
            if (e is Action act)
                act.Invoke();

            else if (e is string str)
                Talk(str);
        }

        #region Get, remove n' clear user data
        //ontrolling the users list, by adding, removing and resetting
        [InDataCapture("Users")]
        public void OnUsers(DataInterceptedEventArgs obj)
        {
            foreach (var entity in HEntity.Parse(obj.Packet))
            {
                UsersData.Add(new LocalHEntity
                {
                    Id = entity.Id,
                    Index = entity.Index,
                    Name = entity.Name
                });
            }
        }

        [InDataCapture("UserRemove")]
        public void OnUserRemove(DataInterceptedEventArgs e)
        {
            //user index is a string for some reason lol
            int UserIndexToRemove = Int32.Parse(e.Packet.ReadUTF8());

            if (UsersData.Exists(any => any.Index == UserIndexToRemove))
            {
                int UserIndex = UsersData.FindIndex(any => any.Index == UserIndexToRemove);
                UsersData.RemoveAt(UserIndex);
            }
        }

        [InDataCapture("HeightMap")]
        public void OnHeightMap(DataInterceptedEventArgs e)
        {
            UsersData.Clear();
        }
        #endregion

        #region Void methods
        public void Talk(string text) => SendToServerAsync(Out.Chat, text, 0, 0);
        public void Sleep() => SendToServerAsync(Out.AvatarExpression, 5);
        public void Dance()
        {
            int dance = (isDancing = !isDancing) ? 1 : 0;
            SendToServerAsync(Out.Dance, dance);
        }
        public async void Time()
        {
            await SendToServerAsync(Out.Chat, DateTime.Now.ToLongTimeString() + ", " + DateTime.Now.ToLongDateString(), 0, 0);
            await Task.Delay(1000);
            await SendToServerAsync(Out.Chat, ":)", 0, 0);
        }
        public async void RockPaperScissors()
        {
            string p = chat.lastRequest;
            string[] rps = { "rock!", "paper!", "scissors!" };

            int _random = new Random().Next(0, 3);
            string answer = rps[_random];
            await SendToServerAsync(Out.Chat, answer, 0, 0);
            await Task.Delay(1000);

            #region draw
            if (p == "!rock" && answer == "rock!" ||
                p == "!paper" && answer == "paper!" ||
                p == "!scissors" && answer == "scissors!")
                await SendToServerAsync(Out.Chat, "draw!", 0, 0);

            #endregion

            #region win
            else if (p == "!rock" && answer == "paper!" ||
                     p == "!paper" && answer == "scissors!" ||
                     p == "!scissors" && answer == "rock!")
            {
                await SendToServerAsync(Out.Chat, "Wohoo! I Won!", 0, 0);
                await SendToServerAsync(Out.AvatarExpression, 1);
            }
            #endregion

            #region lose
            else if (p == "!rock" && answer == "scissors!" ||
                p == "!paper" && answer == "rock!" ||
                p == "!scissors" && answer == "paper!")
                await SendToServerAsync(Out.Chat, "Damn! lost it :(", 0, 0);
            #endregion
        }

        public async void Apresentation()
        {
            await SendToServerAsync(Out.AvatarExpression, 1);
            await SendToServerAsync(Out.Chat, "Hello, I'm a fully customizable bot.", 0, 0);
            await Task.Delay(4000);
            await SendToServerAsync(Out.Dance, 1);
        }
        #endregion

        #region LocalHEntity class
        class LocalHEntity
        {
            public int Id { get; set; }
            public int Index { get; set; }
            public string Name { get; set; }
        }
        #endregion
    }
}
