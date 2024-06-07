using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models
{
    public class FtcWISEDevice
    {
        [Key]
        public string DeviceSysID { get; set; }
        public string DeviceId { get; set; }
      
        public string DeviceName { get; set; }
        public string DeviceDescribe { get; set; }
        public string RTSP_URL { get; set; }
        public string PictureDestination { get; set; }
        public string NVR_URL { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string ModifyUser { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}