using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSB.Activity.Mvc
{
    public class ConfigHelper
    {
        static ConfigHelper()
        {
            Properties.Settings settings = Properties.Settings.Default;
            BigRedpackets_Min = settings.BigRedpackets_Min;
            BigRedpackets_Max = settings.BigRedpackets_Max;
            BigRedpackets_Size = settings.BigRedpackets_Size;
            BigRedpackets_ExpireTime = settings.BigRedpackets_ExpireTime;
            SmallRedpackets_ReceiveCount = settings.SmallRedpackets_ReceiveCount;
            DeployType = settings.DeployType;
            HfyxWxUri = settings.HfyxWxUri;
            HfyxAppId = settings.HfyxAppId;
            TestWxUri = settings.TestWxUri;
            TestAppId = settings.TestAppId;
            JingKanUri = settings.JingKanUri;
        }

        public static int BigRedpackets_Min { get; set; }
        public static int BigRedpackets_Max { get; set; }
        public static int BigRedpackets_Size { get; set; }
        public static int BigRedpackets_ExpireTime { get; set; }
        public static int SmallRedpackets_ReceiveCount { get; set; }
        public static int DeployType { get; set; }
        public static string HfyxWxUri { get; set; }
        public static string HfyxAppId { get; set; }
        public static string TestWxUri { get; set; }
        public static string TestAppId { get; set; }
        public static string JingKanUri { get; set; }
    }
}