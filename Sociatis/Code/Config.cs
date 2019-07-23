using Common.Configs;
using Sociatis.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sociatis.Code
{
    public static class Config
    {
        public static readonly int CongressVotingCommentsPerPage = ConfigManager.GetAppSetting("CongressVotingCommentsPerPage").Integer.Value;

        public static readonly int CongressVotingsPerPage  = ConfigManager.GetAppSetting("CongressVotingsPerPage").Integer.Value;

        public static readonly string MapUrl = ConfigManager.GetAppSetting("MapURL").Value;

        public static readonly string WebsiteURL = ConfigManager.GetAppSetting("WebsiteURL");


        public static readonly string SmtpAddress = ConfigManager.GetAppSetting("smtp").Value.Split(':')[0];
        public static readonly int SmtpPort = int.Parse(ConfigManager.GetAppSetting("smtp").Value.Split(':')[1]);

        public static readonly string ImapAddress = ConfigManager.GetAppSetting("imap").Value.Split(':')[0];
        public static readonly int ImapPort = int.Parse(ConfigManager.GetAppSetting("imap").Value.Split(':')[1]);
        
        public static readonly string EmailUsername = ConfigManager.GetAppSetting("emailUsername").Value;
        public static readonly string EmailPassword = ConfigManager.GetAppSetting("emailPassword").Value;
    }
}