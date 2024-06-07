using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.IPMModels
{
    
    public class BasicInfo
    {
        public string CompanyID { get; set; }
        public string FactoryID { get; set; }
        public string MODIFYUSER { get; set; }
        public string Lng { get; set; }
    }
    public class IPMDBModels
    {
    }
    public class oCommItemDetail : BasicInfo
    {
        public string sListName { get; set; }
        public string sKeyword { get; set; }
    }
    public class oCOMM_ItemListModel : BasicInfo
    {
        public COMM_ItemList RULEs { get; set; }
    }
    public class oFtcWISEDeviceModel : BasicInfo
    {
        public FtcWISEDevice RULEs { get; set; }
    }

    public class oComm_temList : BasicInfo
    {
        public string SysID { get; set; }
        public string ListName { get; set; }
        public int ItemIndex { get; set; }
        public string ItemName { get; set; }
        public string ItemValue { get; set; }
        public string ItemMemo { get; set; }
        public string TargetType { get; set; }
        public string Target { get; set; }
        public DateTime ModifyDate { get; set; }
        public string USER_NAME { get; set; }
    }

    public class oGroupMembers : BasicInfo
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string USERID { get; set; }
        public string USER_NAME { get; set; }
        public string MemberId { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime ModifyDate { get; set; }
        public string GroupMembersName { get; set; }
        public string PersonnelName { get; set; }
    }
    public class oEventBase : BasicInfo
    {
        public string EventBaseSysId { get; set; }
        public string EventBaseName { get; set; }
        public string EventBaseDescribe { get; set; }
        public string EventReason { get; set; }
        public decimal Critical { get; set; } // decimal 型別在 C# 中不需要指定精度
        public string NotifyContent { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string keyword { get; set; }
    }

    public class oEventNotifyTransModel : BasicInfo
    {
        public List<string> sEventBaseSysIdList { get; set; }
        public DateTime sStartDate { get; set; }
        public DateTime sEndDate { get; set; }
        public EventNotifyTransactions RULEs { get; set; }

        

    }
}