using System;
using System.Collections.Generic;

namespace HChatBotUtil
{
    public class HDialogue
    {
        public interface IReqRes { }
        public List<IReqRes> diagList;
        public class Request : IReqRes
        {
            public string[] request { get; private set; }
            public Request(string[] request)
            {
                bool hasRepeat = false;
                var hs = new HashSet<string>();
                foreach (string s in request)
                {
                    if (!hs.Add(s))
                    {
                        hasRepeat = true;
                        break;
                    }
                }
                if (hasRepeat == false)
                    this.request = request;
                else
                    throw new Exception("Can't repeat requests.");
            }
        }
        public class Response : IReqRes
        {
            public string[] response { get; private set; }
            public Response(string[] response) => this.response = response;
        }
        public class ResponseAction : IReqRes
        {
            public Action[] response { get; private set; }
            public ResponseAction(Action[] response) => this.response = response;
        }

        public HDialogue() => diagList = new List<IReqRes>();

        public void AddRequest(params string[] request) => diagList.Add(new Request(request));
        public void AddResponse(params string[] response) => diagList.Add(new Response(response));
        public void AddResponse(params Action[] response) => diagList.Add(new ResponseAction(response));
    }
}
