using FTC_MES_MVC.Models.Dals;
using FTC_MES_MVC.Models.Dals.UserManage;
using FTC_MES_MVC.Models.IPMModels;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FTC_MES_MVC.Controllers.IPM
{
    public class IPMCommApiController : BaseApiController, IDisposable
    {

     
        /// <summary>
        /// GetEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        public List<dynamic> GetEventStatus(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            try 
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetEventStatus(o.ItemName));

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}),UID{3},DateFrom{4},DateTo{5}) ErrMsg:{6}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, o.ItemName, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }


        /// <summary>
        /// 刪除事件狀態
        /// DeleteEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> DeleteEventStatus(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try 
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                //result = dalIPMSetting.DeleteEventStatus(o);

                if (!result)
                {

                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch(Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                 o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));
                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 更新事件狀態
        /// UpdateEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> UpdateEventStatus(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.UpdateEventStatus(o.SysID, o.ItemIndex, o.ItemName);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                                     o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 新增事件狀態
        /// CreateEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> CreateEventStatus(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.CreateEventStatus(o.ItemIndex, o.ItemName, o.MODIFYUSER);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }


        /// <summary>
        /// GetEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        public List<dynamic> GetEventStrategy(oComm_temList o)
        
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetEventStrategy(o.ItemName));

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}),UID{3},DateFrom{4},DateTo{5}) ErrMsg:{6}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, o.ItemName, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }
        /// <summary>
        /// 刪除事件狀態
        /// DeleteEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> DeleteEventStrategy(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                //result = dalIPMSetting.DeleteEventStrategy(o);

                if (!result)
                {

                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                 o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));
                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 更新事件對策
        /// UpdateEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> UpdateEventStrategy(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.UpdateEventStrategy(o.SysID, o.ItemIndex, o.ItemName);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                                     o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }


        /// <summary>
        /// 新增事件狀態
        /// CreateEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> CreateEventStrategy(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.CreateEventStrategy(o.ItemIndex, o.ItemName, o.MODIFYUSER);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }







        /// <summary>
        /// GetGroupMembers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        public List<dynamic> GetGroupMembers(oGroupMembers o)

        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetGroupMembers(o.GroupName, o.MemberId));

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}),UID{3},DateFrom{4},DateTo{5}) ErrMsg:{6}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, o.GroupName, o.MemberId, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }


        /// <summary>
        /// 刪除群組成員
        /// DeleteGroupMembers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> DeleteGroupMembers(oGroupMembers o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.DeleteGroupMembers(o);

                if (!result)
                {

                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                 o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));
                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }


        /// <summary>
        /// 編輯群組成員
        /// UpdateGroupMembers
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> UpdateGroupMembers(oGroupMembers o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.UpdateGroupMembers(o.MemberId, o.GroupId);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                                     o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }




        [HttpPost]
        public List<dynamic> GET_DDL_GroupName(oGroupMembers o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GET_DDL_GroupName());

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), EventTime{3}, NotifySuccessTime{4}, EqpId{5}, Area{6}) ErrMsg:{7}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }




        [HttpPost]
        public List<dynamic> GET_DDL_USER_NAME(oGroupMembers o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GET_DDL_USER_NAME());

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), EventTime{3}, NotifySuccessTime{4}, EqpId{5}, Area{6}) ErrMsg:{7}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        //[HttpPost]
        //public List<oGroupMembers> getUser()
        //{
        //    try
        //    {
        //        return oDal.getUser().ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog_Error(ex.ToString());
        //    }
        //    return null;
        //}
        [HttpPost]
        public List<dynamic> getUser(oGroupMembers o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.getUser().ToList());

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), EventTime{3}, NotifySuccessTime{4}, EqpId{5}, Area{6}) ErrMsg:{7}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;

        }



        /// <summary>
        /// GetEventBase
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        public List<dynamic> GetEventBase(oEventBase o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetEventBase(o.keyword));

                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}),UID{3},DateFrom{4},DateTo{5}) ErrMsg:{6}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, o.keyword, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 刪除EventBase
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> DeleteEventBase(oEventBase o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.DeleteEventBase(o);

                if (!result)
                {

                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                 o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));
                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 新增事件狀態
        /// CreateEventStatus
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> CreateEventBase(oEventBase o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;
            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.CreateEventBase(o.EventBaseName, o.EventBaseDescribe, o.EventReason, o.Critical, o.NotifyContent, o.MODIFYUSER);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        /// <summary>
        /// 更新事件基礎
        /// UpdateEventBase
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public List<dynamic> UpdateEventBase(oEventBase o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;
            bool result = false;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                result = dalIPMSetting.UpdateEventBase(o.EventBaseName, o.EventBaseDescribe, o.EventReason, o.Critical, o.NotifyContent, o.EventBaseSysId);

                if (!result)
                {
                    // 操作失敗，返回 DAL 層設定的錯誤代碼和錯誤訊息
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2}), ErrMsg:{3}",
                                     o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;
        }

        [HttpPost]
        public List<dynamic> GetEventReasonDrop(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetEventReasonDrop());
                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2})) ErrMsg:{3}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;

        }



        [HttpPost]
        public List<dynamic> GetEventCriticalDrop(oComm_temList o)
        {
            List<dynamic> listReturn = new List<dynamic>();
            ApiReturnMessage oApiReturnMessage = new ApiReturnMessage();
            IPMSettingDal dalIPMSetting = null;

            try
            {
                dalIPMSetting = new IPMSettingDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                listReturn.Add(dalIPMSetting.GetEventCriticalDrop());
                if (listReturn[0] == null)
                {
                    oApiReturnMessage.ReturnCode = (int)dalIPMSetting.ReturnCode;
                    oApiReturnMessage.ReturnMessage = dalIPMSetting.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                WriteLog_Error(string.Format("User:{0},Parameters:(CompanyId={1}, FactoryId={2})) ErrMsg:{3}",
                    o.MODIFYUSER, o.CompanyID, o.FactoryID, ex.ToString()));

                oApiReturnMessage.ReturnCode = (int)DalReturnCode.Failed;
                oApiReturnMessage.ReturnMessage = ex.Message;
            }
            finally
            {
                if (dalIPMSetting != null) { dalIPMSetting.Dispose(); }
                listReturn.Add(oApiReturnMessage);
            }
            return listReturn;

        }



    }


}
