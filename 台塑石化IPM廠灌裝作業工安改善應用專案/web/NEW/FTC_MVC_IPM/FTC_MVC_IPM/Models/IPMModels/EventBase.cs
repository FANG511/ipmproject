using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class EventBase
    {
        [Key]
        public string EventBaseSysId { get; set; }

        public string EventBaseName { get; set; }    
        public string EventBaseDescribe { get; set; }
        public string EventReason { get; set; }
        public decimal Critical { get; set; }
        public string NotifyContent { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifyUser { get; set; }
        public DateTime ModifyDate { get; set; }


    }
}