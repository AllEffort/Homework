using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzHomework
{
    class Developer : Worker
    {
        public delegate void MessageSendHandler(object source, MessageArgs args);
        public event MessageSendHandler MessageSend;
        string Keyword = "feature";
        public Developer(float _TO_PAY, string _F_NAME, string _L_NAME) : base(_TO_PAY, _F_NAME, _L_NAME)
        {
        }
        public override String ReturnTitle()
        {
            return "feature";
        }
        //Jer samo developeri mogu da salju poruke zato je definisana samo ovde
        public override void OnMessageSend(String _TO_SEND)
        {
            if (MessageSend != null)
            {
                MessageSend(this as object, new MessageArgs { Message = _TO_SEND });
            }
        }
        public override String GiveJob()
        {
            return "Developer";
        }


    }
}
