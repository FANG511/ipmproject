using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//自訂參考
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FTC_MES_MVC.Models
{
    public class ReturnMessageModel
    {
        [Key]
        [Column(Order = 1)]
        //訊息編號(Class.Method.流水號3碼)
        public string MESSAGEID { get; set; }
        [Key]
        [Column(Order = 2)]
        //訊息語言(三碼, CHT:繁體, CHS:簡體)
        public string LANGUAGE { get; set; }
        //類別名稱
        public string CLASSNAME { get; set; }
        //方法名稱
        public string METHODNAME { get; set; }
        //回覆代碼
        public string RETURNCODE { get; set; }
        //回傳訊息內容
        public string RETURNMESSAGE { get; set; }
        //紀錄新增使用者
        public string CREATEUSER { get; set; }
        //紀錄新增時間
        public DateTime CREATETIME { get; set; }
        //紀錄異動使用者
        public string MODIFYUSER { get; set; }
        //紀錄異動時間
        public DateTime MODIFYTIME { get; set; }
    }
}