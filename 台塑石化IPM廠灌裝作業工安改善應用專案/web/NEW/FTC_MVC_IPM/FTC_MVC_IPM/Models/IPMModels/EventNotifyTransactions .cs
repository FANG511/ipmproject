using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class EventNotifyTransactions
    {
       
        public string EventSysId { get; set; }
        public bool Notified { get; set; }
        public string NotifiedStatus
        {
            get
            {
                return Notified ? "已通知" : "未通知";
            }
        }
        public string NotifyGroup { get; set; }
        public string NotifyMethod { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public DateTime ModifyDate { get; set; }

        public string EventBaseName { get; set; }

    }
}