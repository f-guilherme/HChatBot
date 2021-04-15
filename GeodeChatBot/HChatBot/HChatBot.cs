using System;
using System.Collections.Generic;
using static HChatBotUtil.HDialogue;

namespace HChatBotUtil
{
    class HChatBot
    {
        private bool isIterating;
        private int i, j;

        /// <summary>
        /// If someone says something that is not expected in 
        /// the next speech, the bot is resetted for a first interaction.
        /// Default value: true.
        /// </summary>
        public bool firstMissReset { get; set; } = true;
        /// <summary>
        /// Returns the last request made in string (can be used for some logic).
        /// </summary>
        public string lastRequest { get; private set; }

        public event EventHandler<object> OnMessageResponse;
        public List<HDialogue> chatList;

        public HChatBot() => chatList = new List<HDialogue>();

        public void AddDialogue(HDialogue diag) => chatList.Add(diag);
        public void Chatting(string text)
        {
            if (isIterating == false)
                FirstRequest(text);
            else
                Iterate(text);
        }
        public void Reset()
        {
            isIterating = false;
            i = 0;
            j = 0;
        }
        private void FirstRequest(string text)
        {
            for (int i = 0; i < chatList.Count; i++)
            {
                HDialogue dialogue = chatList[i];
                for (int j = 0; j < dialogue.diagList.Count; j++)
                {
                    var speech = dialogue.diagList[j];
                    if (speech is Request req)
                    {
                        for (int k = 0; k < req.request.Length; k++)
                            if (req.request[k] == text)
                            {
                                lastRequest = text;
                                isIterating = true;
                                this.i = i;
                                this.j = j + 1;

                                speech = dialogue.diagList[this.j];
                                if (speech is Response res)
                                {
                                    string res_string = res.response[k];
                                    OnMessageResponse?.Invoke(this, res_string);
                                }
                                else if (speech is ResponseAction resA)
                                {
                                    Action res_action = resA.response[k];
                                    OnMessageResponse?.Invoke(this, res_action);
                                }

                                if (this.j >= dialogue.diagList.Count - 1)
                                    Reset();
                                else
                                    this.j++;

                                return;
                            }
                        break;
                    }
                }
            }
        }
        private void Iterate(string text)
        {
            HDialogue dialogue = chatList[i];
            var speech = dialogue.diagList[j];

            if (speech is Request req)
            {
                for (int k = 0; k < req.request.Length; k++)
                {
                    if (req.request[k] == text)
                    {
                        lastRequest = text;
                        j++;

                        speech = dialogue.diagList[j];
                        if (speech is Response res)
                        {
                            string res_string = res.response[k];
                            OnMessageResponse?.Invoke(this, res_string);
                        }
                        else if (speech is ResponseAction resA)
                        {
                            Action res_action = resA.response[k];
                            OnMessageResponse?.Invoke(this, res_action);
                        }

                        if (j >= dialogue.diagList.Count - 1)
                            Reset();
                        else
                            j++;

                        return;
                    }
                }
                if (firstMissReset)
                    Reset();
            }
        }
    }
}
