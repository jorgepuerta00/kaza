using System;
using System.Collections.Generic;
using System.Text;

namespace App.Common.Email
{
    public class SendEmailSettings
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
    }
}
