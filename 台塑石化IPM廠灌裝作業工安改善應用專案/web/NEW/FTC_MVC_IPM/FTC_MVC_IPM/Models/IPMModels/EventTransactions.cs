using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class EventTransactions
    {
       
        public string EventSysId { get; set; }
        public string EventBaseSysId { get; set; }    
        public int Critical { get; set; }
        public string EventReason { get; set; }
        public string EventStatus { get; set; }
        public string EventDescription { get; set; }
        public string DeviceSysId { get; set; }
        public string PictureDestination { get; set; }
        public DateTime PreFinishTime { get; set; }
        public DateTime FinishTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public DateTime ModifyDate { get; set; }

    }
}