using Dapper;
using FTC_MES_MVC.Base;
using FTC_MES_MVC.Models.IPMModels;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.Dals
{
    public class IPMSettingDal : CommDalBase
    {
        private string _apDbConnStr = "";
        private string _className = "";  //類別名稱變數
        private Dictionary<string, ReturnMessageModel> _dictRetMsg = null;  //保存此類別所有語系的回覆訊息

        /// <summary>
        ///BaseSysParamsDal建構式(for Controller呼叫)
        ///ps.使用後一定要Dispose(),否則連線會超過
        /// </summary>
        /// <param name="p_sUserId">使用者代碼</param>
        /// <param name="p_sCompanyId">公司代碼</param>
        /// <param name="p_sFactoryId">廠別(含應用程式代碼)</param>
        /// <param name="p_sLng">語系(預設繁體中文)</param>
        public IPMSettingDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
        {
            ReturnMessageDal oRetMsgSvc = null;
            try
            {
                base.UserId = p_sUserId;
                base.CompanyId = p_sCompanyId;
                base.FactoryId = p_sFactoryId;
                base.LanguageId = p_sLng;

                //清除錯誤狀態
                base.resetErrState();
                //取得物件名稱
                _className = this.GetType().Name;
                //取得ApDbConnString
                _apDbConnStr = base.GetApDbConnStr(base.CompanyId, base.FactoryId, base.UserId);
                //取得與開啟連線(後續透過base.cn取得連線物件)
                base._cn(_apDbConnStr, base.UserId);
                if (base.ReturnCode != DalReturnCode.Success) { throw new Exception(base.ReturnMessage); }
                //取得此類別的所有回覆訊息
                oRetMsgSvc = new ReturnMessageDal(base.UserId, base.CompanyId, base.FactoryId, base.LanguageId, ref base.refConn, ref base.refTxn);
                var oRetMsgList = oRetMsgSvc.GetAllByClass(_className);
                _dictRetMsg = oRetMsgList.Where(c => c.LANGUAGE == base.LanguageId).ToDictionary(p => p.MESSAGEID);
                oRetMsgSvc.Dispose();
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.ConectionError;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(CompanyId={1}, FactoryId={2}, UserId={3}), ErrMsg:{4}",
                    base.UserId, p_sCompanyId, p_sFactoryId, p_sUserId, ex.ToString()));
                throw;
            }
            finally
            {
                if (oRetMsgSvc != null) { oRetMsgSvc.Dispose(); }
            }
        }

        //internal bool DeleteEventStatus(Test_ItemList o)
        //{
        //    throw new NotImplementedException();
        //}

        //internal bool DeleteEventStatus(Comm_ItemList o)
        //{
        //    throw new NotImplementedException();
        //}


        /// <summary>
        ///BaseSysParamsDal建構式2(for 其他DAL呼叫)
        ///ps.使用後一定要Dispose(),否則連線會超過
        /// </summary>
        /// <param name="p_sUserId">使用者代碼</param>
        /// <param name="p_sCompanyId">公司代碼</param>
        /// <param name="p_sFactoryId">廠別(含應用程式代碼)</param>
        /// <param name="p_sLng">語系</param>
        /// <param name="p_oRefConn">呼叫者傳入的連線物件</param>
        /// <param name="p_oRefTxn">呼叫者傳入的交易物件(用於資料庫交易期間)</param>
        public IPMSettingDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng, ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
        {
            ReturnMessageDal oRetMsgSvc = null;
            try
            {
                base.UserId = p_sUserId;
                base.CompanyId = p_sCompanyId;
                base.FactoryId = p_sFactoryId;
                base.LanguageId = string.IsNullOrEmpty(p_sLng) ? "zh-TW" : p_sLng;

                //清除錯誤狀態
                base.resetErrState();
                //取得物件名稱
                _className = this.GetType().Name;
                //取得ApDbConnString
                _apDbConnStr = base.GetApDbConnStr(base.CompanyId, base.FactoryId, base.UserId);

                //取得與開啟連線(後續透過base.cn取得連線物件)
                if (p_oRefTxn != null)
                    //處於資料庫交易狀態下
                    base.ShareDbTxn(ref p_oRefConn, ref p_oRefTxn);
                else
                {
                    base._cn(_apDbConnStr, base.UserId);
                    if (base.ReturnCode != DalReturnCode.Success) { throw new Exception(base.ReturnMessage); }
                }
                //取得此類別的所有回覆訊息
                oRetMsgSvc = new ReturnMessageDal(base.UserId, base.CompanyId, base.FactoryId, base.LanguageId, ref base.refConn, ref base.refTxn);
                var oRetMsgList = oRetMsgSvc.GetAllByClass(_className);
                _dictRetMsg = oRetMsgList.Where(c => c.LANGUAGE == base.LanguageId).ToDictionary(p => p.MESSAGEID);
                oRetMsgSvc.Dispose();
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.ConectionError;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(CompanyId={1}, FactoryId={2}, UserId={3}, RefConn={4}, RefTxn={5}), ErrMsg:{6}",
                    base.UserId, p_sCompanyId, p_sFactoryId, p_sUserId, p_oRefConn.ToString(), p_oRefTxn.ToString(), ex.ToString()));
                throw;
            }
            finally
            {
                if (oRetMsgSvc != null) { oRetMsgSvc.Dispose(); }
            }
        }

        protected void AllDispose()
        {
            Dispose();
        }


        /// <summary>
		/// 事件狀態:查詢
		/// </summary>
		/// <param name="sItemName"></param>
		/// <returns></returns>
        public List<COMM_ItemList> GetEventStatus(string sItemName)
        {
            List<COMM_ItemList> oRetList;
            try
            {
                // 重置錯誤狀態
                base.resetErrState();


                string sSql = @"SELECT
                            SysID,
                            ItemIndex,
                            ItemName,
                            ModifyDate,
                            ModifyUser
                        FROM
                            [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                        WHERE
                            ListName IN ('EventStatus')";

                DynamicParameters oMyDp = new DynamicParameters();

                if (sItemName != null)
                {
                    oMyDp.Add("@ItemName", "%" + sItemName + "%");
                    sSql += " AND ItemName LIKE @ItemName";
                }

                // 按 `ItemIndex` 排序
                sSql += " ORDER BY ItemIndex";

                oRetList = base.cn.Query<COMM_ItemList>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }


        /// <summary>
        /// 事件狀態:刪除
        /// </summary>
        /// <param name="oComm_temList o"></param>
        /// <returns></returns>
        //public bool DeleteEventStatus(oComm_temList o)
        //{
        //    bool bRetFlag = false;
        //    SqlTransaction oSqlTxn = null;

        //    try
        //    {
        //        if (o.SysID == null)
        //        {
        //            base.ReturnCode = DalReturnCode.InputParameterInvalid;
        //            base.ReturnMessage = base.GetReturnMsg("001", "SysID為空值", _dictRetMsg);
        //            return bRetFlag;
        //        }
        //        base.resetErrState();
        //        oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
        //        DynamicParameters oMyDp = new DynamicParameters();
        //        oMyDp.Add("@SysID", o.SysID);

        //        string sSql = @"DELETE FROM [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
        //                        WHERE [SysID] = @SysID";

        //        int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);

        //        if (iAffectedRecs <= 0)
        //        {
        //            if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
        //            base.ReturnCode = DalReturnCode.Failed;
        //            base.ReturnMessage = base.GetReturnMsg("101", "ODOD_Detail失敗(不明原因刪除不到資料)!" + this.ReturnMessage, _dictRetMsg);
        //            return bRetFlag;
        //        }
        //        if (!base.IsInCrossClassTxn)
        //            oSqlTxn.Commit();
        //        bRetFlag = true;
        //        WriteLog_Info("完成刪除DeleteEventStatus");

        //    }
        //    catch (Exception ex)
        //    {
        //        base.ReturnCode = DalReturnCode.Failed;
        //        base.ReturnMessage = ex.Message;
        //        base.WriteLog_Error(string.Format(
        //            "User:{0},Parameters:(ErrMsg:{1})",
        //            base.UserId, ex.ToString()));
        //        throw;
        //    }
        //    return bRetFlag;

        //}


        /// <summary>
        /// 事件狀態:新增
        /// </summary>
        /// <param name="iItemIndex "></param>
        /// <param name="sItemName "></param>
        /// <param name="sModifyUser "></param>
        /// <returns></returns>
        public bool CreateEventStatus(int iItemIndex, string sItemName, string sModifyUser)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (iItemIndex == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "項目索引未設置", _dictRetMsg);
                    return bRetFlag;
                }
                if (sItemName == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("003", "項目名稱未設置", _dictRetMsg);
                    return bRetFlag;
                }
                //if (sModifyDate == null)
                //{
                //    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                //    base.ReturnMessage = base.GetReturnMsg("004", "修改日期未設置", _dictRetMsg);
                //    return bRetFlag;
                //}
                if (sModifyUser == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("005", "修改人員未設置", _dictRetMsg);
                    return bRetFlag;
                }
                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();

                oMyDp.Add("@ItemIndex", iItemIndex);
                oMyDp.Add("@ItemName", sItemName);
                oMyDp.Add("@ModifyUser", sModifyUser);

                string sSql = @"INSERT INTO [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                  (
                                  [ListName]
                                  ,[ItemIndex]
                                  ,[ItemName]
                                  ,[ItemValue]
                                  ,[ItemMemo]
                                  ,[TargetType]
                                  ,[Target]
                                  ,[ModifyDate]
                                  ,[ModifyUser]
                                   )
                                   VALUES
                                    ('EventStatus'
                                    ,@ItemIndex
                                    ,@ItemName
                                    ,'處理中'
                                    ,'狀態'
                                    ,'Web'
                                    ,'事件紀錄資料'
                                    ,GETDATE()
                                    ,@ModifyUser)";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventStatus失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }

                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成新增CreateEventStatus");
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }

        /// <summary>
        /// 事件狀態:修改
        /// </summary>
        /// <param name="sSysID "></param>
        /// <param name="iItemIndex "></param>
        /// <param name="sItemName "></param>
        /// <param name="sModifyUser "></param>
        /// <returns></returns>
        public bool UpdateEventStatus(string sSysID, int iItemIndex, string sItemName)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (sSysID == null)
                {
                    {
                        base.ReturnCode = DalReturnCode.InputParameterInvalid;
                        base.ReturnMessage = base.GetReturnMsg("001", "SysID為空值", _dictRetMsg);
                        return bRetFlag;
                    }
                }
                if (iItemIndex == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "ItemIndex為0", _dictRetMsg);
                    return bRetFlag;
                }

                if (sItemName == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "ItemIndex為空值", _dictRetMsg);
                    return bRetFlag;
                }




                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@SysID", sSysID);
                oMyDp.Add("@ItemIndex", iItemIndex);
                oMyDp.Add("@ItemName", sItemName);


                string sSql = @"UPDATE [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                SET [ItemIndex] = @ItemIndex
                                ,[ItemName] = @ItemName
                                ,[ModifyDate] = GETDATE()    
                                WHERE [SysID]=@SysID";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    //Rollback方法取消所有更改。
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "ODOD_Detail失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                //跨類別交易時先不commit(交易源頭才commit)
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成更新EventStatus");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }

        /// <summary>
        /// 事件對策:查詢
        /// </summary>
        /// <param name="sItemName"></param>
        /// <returns></returns>
        public List<COMM_ItemList> GetEventStrategy(string sItemName)
        {
            List<COMM_ItemList> oRetList;
            try
            {
                // 重置錯誤狀態
                base.resetErrState();


                string sSql = @"SELECT
                            SysID,
                            ItemIndex,
                            ItemName,
                            ModifyDate,
                            ModifyUser
                        FROM
                            [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                        WHERE
                            ListName IN ('EventStrategy')";

                DynamicParameters oMyDp = new DynamicParameters();

                if (sItemName != null)
                {
                    oMyDp.Add("@ItemName", "%" + sItemName + "%");
                    sSql += " AND ItemName LIKE @ItemName";
                }

                // 按 `ItemIndex` 排序
                sSql += " ORDER BY ItemIndex";

                oRetList = base.cn.Query<COMM_ItemList>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }


        /// <summary>
        /// 事件對策:刪除
        /// </summary>
        /// <param name="oComm_temList o"></param>
        /// <returns></returns>
        //public bool DeleteEventStrategy(oComm_temList o)
        //{
        //    bool bRetFlag = false;
        //    SqlTransaction oSqlTxn = null;

        //    try
        //    {
        //        if (o.SysID == null)
        //        {
        //            base.ReturnCode = DalReturnCode.InputParameterInvalid;
        //            base.ReturnMessage = base.GetReturnMsg("001", "SysID為空值", _dictRetMsg);
        //            return bRetFlag;
        //        }
        //        base.resetErrState();
        //        oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
        //        DynamicParameters oMyDp = new DynamicParameters();
        //        oMyDp.Add("@SysID", o.SysID);

        //        string sSql = @"DELETE FROM [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
        //                        WHERE [SysID] = @SysID";

        //        int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);

        //        if (iAffectedRecs <= 0)
        //        {
        //            if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
        //            base.ReturnCode = DalReturnCode.Failed;
        //            base.ReturnMessage = base.GetReturnMsg("101", "EventStrategy失敗(不明原因刪除不到資料)!" + this.ReturnMessage, _dictRetMsg);
        //            return bRetFlag;
        //        }
        //        if (!base.IsInCrossClassTxn)
        //            oSqlTxn.Commit();
        //        bRetFlag = true;
        //        WriteLog_Info("完成刪除DeleteEventStrategy");

        //    }
        //    catch (Exception ex)
        //    {
        //        base.ReturnCode = DalReturnCode.Failed;
        //        base.ReturnMessage = ex.Message;
        //        base.WriteLog_Error(string.Format(
        //            "User:{0},Parameters:(ErrMsg:{1})",
        //            base.UserId, ex.ToString()));
        //        throw;
        //    }
        //    return bRetFlag;

        //}

        /// <summary>
        /// 事件對策:新增
        /// </summary>
        /// <param name="iItemIndex "></param>
        /// <param name="sItemName "></param>
        /// <param name="sModifyUser "></param>
        /// <returns></returns>
        public bool CreateEventStrategy(int iItemIndex, string sItemName, string sModifyUser)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (iItemIndex == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "項目索引未設置", _dictRetMsg);
                    return bRetFlag;
                }
                if (sItemName == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("003", "項目名稱未設置", _dictRetMsg);
                    return bRetFlag;
                }
                //if (sModifyDate == null)
                //{
                //    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                //    base.ReturnMessage = base.GetReturnMsg("004", "修改日期未設置", _dictRetMsg);
                //    return bRetFlag;
                //}
                if (sModifyUser == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("005", "修改人員未設置", _dictRetMsg);
                    return bRetFlag;
                }
                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();

                oMyDp.Add("@ItemIndex", iItemIndex);
                oMyDp.Add("@ItemName", sItemName);
                oMyDp.Add("@ModifyUser", sModifyUser);

                string sSql = @"INSERT INTO [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                  (
                                  [ListName]
                                  ,[ItemIndex]
                                  ,[ItemName]
                                  ,[ItemValue]
                                  ,[ItemMemo]
                                  ,[TargetType]
                                  ,[Target]
                                  ,[ModifyDate]
                                  ,[ModifyUser]
                                   )
                                   VALUES
                                    ('EventStrategy'
                                    ,@ItemIndex
                                    ,@ItemName
                                    ,'處理中'
                                    ,'對策'
                                    ,'Web'
                                    ,'事件紀錄資料'
                                    ,GETDATE()
                                    ,@ModifyUser)";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventStrategy失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }

                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成新增CreateEventStrategy");
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }


        /// <summary>
        /// 事件狀態:修改
        /// </summary>
        /// <param name="sSysID "></param>
        /// <param name="iItemIndex "></param>
        /// <param name="sItemName "></param>
        /// <param name="sModifyUser "></param>
        /// <returns></returns>
        public bool UpdateEventStrategy(string sSysID, int iItemIndex, string sItemName)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (sSysID == null)
                {
                    {
                        base.ReturnCode = DalReturnCode.InputParameterInvalid;
                        base.ReturnMessage = base.GetReturnMsg("001", "SysID為空值", _dictRetMsg);
                        return bRetFlag;
                    }
                }
                if (iItemIndex == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "ItemIndex為0", _dictRetMsg);
                    return bRetFlag;
                }

                if (sItemName == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "ItemIndex為空值", _dictRetMsg);
                    return bRetFlag;
                }

                //if(sModifyDate == null)
                //{
                //    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                //    base.ReturnMessage = base.GetReturnMsg("003", "sModifyDate為空值", _dictRetMsg);
                //    return bRetFlag;
                //}

                //if (sModifyUser == null)
                //{
                //    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                //    base.ReturnMessage = base.GetReturnMsg("004", "sModifyUser為空值", _dictRetMsg);
                //    return bRetFlag;
                //}

                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@SysID", sSysID);
                oMyDp.Add("@ItemIndex", iItemIndex);
                oMyDp.Add("@ItemName", sItemName);
                //oMyDp.Add("@ModifyDate", sModifyDate);
                //oMyDp.Add("@ModifyUser", sModifyUser);

                string sSql = @"UPDATE [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                SET [ItemIndex] = @ItemIndex
                                ,[ItemName] = @ItemName
                               
                                WHERE [SysID]=@SysID";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    //Rollback方法取消所有更改。
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventStrategy失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                //跨類別交易時先不commit(交易源頭才commit)
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成更新EventStrategy");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }




        /// <summary>
        /// 群組成員:查詢
        /// </summary>
        /// <param name="sGroupName"></param>
        /// <param name="sMemberId"></param>
        /// <returns></returns>
        public List<oGroupMembers> GetGroupMembers(string sGroupName, string sMemberId)
        {
            List<oGroupMembers> oRetList;
            try
            {
                // 重置錯誤狀態
                base.resetErrState();

                // 建構基本 SQL 查询
                string sSql = @"SELECT
                            gm.GroupId,
                            g.GroupName,
                            STUFF((SELECT ', ' + CONCAT(c.USERID, '-', c.USER_NAME)
                                   FROM [FtcWISE_IPM].[dbo].[GroupMembers] sub_gm
                                   LEFT JOIN [FtcWISE_IPM].[dbo].[COMM_USER] c ON sub_gm.MemberId = c.USERID
                                   WHERE sub_gm.GroupId = gm.GroupId
                                   FOR XML PATH('')), 1, 2, '') AS PersonnelName,
                            gm.ModifyUser,
                            gm.ModifyDate
                        FROM 
                            [FtcWISE_IPM].[dbo].[GroupMembers] gm
                        LEFT JOIN 
                            [FtcWISE_IPM].[dbo].[Groups] g ON gm.GroupId = g.GroupId
                        WHERE 1=1";


                DynamicParameters oMyDp = new DynamicParameters();


                if (sGroupName != null)
                {
                    oMyDp.Add("@GroupName", sGroupName);
                    sSql += " AND g.GroupName = @GroupName";  // 确保使用正确的字段名
                }

                if (sMemberId != null)
                {
                    oMyDp.Add("@MemberId", sMemberId);
                    sSql += " AND c.USERID = @MemberId";  // 确保使用正确的字段名
                }

                sSql += @" GROUP BY
                    gm.GroupId,
                    g.GroupName,
                    gm.ModifyUser,
                    gm.ModifyDate";


                // 执行查询并返回结果
                oRetList = base.cn.Query<oGroupMembers>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "读取数据");
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0}, Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }


        /// <summary>
        /// 群組成員:刪除
        /// </summary>
        /// <param name="oGroupMembers o"></param>
        /// <returns></returns>
        public bool DeleteGroupMembers(oGroupMembers o)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (o.GroupId == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "GroupId為空值", _dictRetMsg);
                    return bRetFlag;
                }
                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@GroupId", o.GroupId); // 假設需要傳遞GroupId
                oMyDp.Add("@MemberId", o.MemberId); // 添加MemberId參數

                string sSqlGroupMembers = @"DELETE FROM  [FtcWISE_IPM].[dbo].[GroupMembers]
                                WHERE [GroupId] = @GroupId";

                //Groups表
                DynamicParameters oMyDp2 = new DynamicParameters();
                oMyDp2.Add("@GroupId", o.GroupId);
                string sSqlGroups = @"DELETE FROM [FtcWISE_IPM].[dbo].[GroupMembers]
                                WHERE [GroupId] = @GroupId";


                int iAffectedRecs1 = base.cn.Execute(sSqlGroupMembers, oMyDp, oSqlTxn);
                int iAffectedRecs2 = base.cn.Execute(sSqlGroups, oMyDp2, oSqlTxn);

                if (iAffectedRecs1 <= 0 && iAffectedRecs2 <= 0)
                {
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "GroupId失敗(不明原因刪除不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成刪除DeleteGroupMembers");

            }

            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }




        public List<oGroupMembers> GET_DDL_GroupName()
        {
            List<oGroupMembers> oRetList;
            try
            {
                base.resetErrState();

                string sSql = @"SELECT DISTINCT GroupName
                                FROM  [FtcWISE_IPM].[dbo].[Groups]
                                WHERE 1 = 1";
                DynamicParameters oMyDp = new DynamicParameters();
                oRetList = base.cn.Query<oGroupMembers>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }

        public List<oGroupMembers> GET_DDL_USER_NAME()
        {
            List<oGroupMembers> oRetList;
            try
            {
                base.resetErrState();

                string sSql = @"SELECT DISTINCT USER_NAME
                                FROM  [FtcWISE_IPM].[dbo].[COMM_USER]
                                WHERE 1 = 1";
                DynamicParameters oMyDp = new DynamicParameters();
                oRetList = base.cn.Query<oGroupMembers>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }

        /// <summary>
        /// 群組成員設定:修改
        /// </summary>
        /// <param name="sMemberId "></param>
        /// <returns></returns>
        public bool UpdateGroupMembers(string sMemberId, string sGroupId)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {


                if (sMemberId == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "MemberId為空值", _dictRetMsg);
                    return bRetFlag;
                }
                if (sGroupId == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "sGroupId為空值", _dictRetMsg);
                    return bRetFlag;
                }


                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();

                oMyDp.Add("@MemberId", sMemberId);
                oMyDp.Add("@GroupId", sGroupId);

                string sSql = @"UPDATE [GroupMembers]
                        SET [MemberId] = @MemberId
                        WHERE [GroupId] = @GroupId AND [MemberId] = @MemberId;";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    //Rollback方法取消所有更改。
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventStrategy失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                //跨類別交易時先不commit(交易源頭才commit)
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成更新EventStrategy");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }

        public List<oGroupMembers> getUser()
        {
            List<oGroupMembers> qData = new List<oGroupMembers>();
            try
            {

                return cn.Query<oGroupMembers>("SELECT * FROM GroupMembers").ToList();
            }
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            finally
            {
                AllDispose();
            }
            return qData;
        }

        /// <summary>
        /// 事件基礎檔設定:查詢
        /// </summary>
        /// <param name="sEventBaseName"></param> 
        /// <param name="sEventBaseDescribe"></param>
        /// <param name="sNotifyContent"></param>
        /// <returns></returns>
        public List<oEventBase> GetEventBase(string skeyword)
        {
            List<oEventBase> oRetList;
            try
            {
                // 重置錯誤狀態
                base.resetErrState();


                string sSql = @"SELECT
                            EventBaseSysId,
                            EventBaseName,
                            EventBaseDescribe,
                            EventReason,
                            Critical,
                            NotifyContent,     
                            ModifyDate,
                            ModifyUser
                        FROM
                            [FtcWISE_IPM].[dbo].[EventBase]
                        WHERE
                            1=1";



                DynamicParameters oMyDp = new DynamicParameters();

                if (skeyword != null)
                {
                    oMyDp.Add("@Keyword", "%" + skeyword + "%");
                    sSql += " AND (EventBaseName LIKE @Keyword OR EventBaseDescribe LIKE @Keyword OR NotifyContent LIKE @Keyword)";
                }

                //if (sEventBaseName != null)
                //{
                //    oMyDp.Add("@EventBaseName", "%" + sEventBaseName + "%");
                //    sSql += " AND EventBaseName LIKE @EventBaseName";

                //}
                //if (sEventBaseDescribe != null)
                //{
                //    oMyDp.Add("@EventBaseDescribe", "%" + sEventBaseDescribe + "%");
                //    sSql += " AND EventBaseDescribe LIKE @EventBaseDescribe";
                //}

                //if (sNotifyContent != null)
                //{
                //    oMyDp.Add("@NotifyContent", "%" + sNotifyContent + "%");
                //    sSql += " AND NotifyContent LIKE @NotifyContent";
                //}



                oRetList = base.cn.Query<oEventBase>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }

        /// <summary>
        /// 事件基礎檔設定:刪除
        /// </summary>
        /// <param name="oComm_temList o"></param>
        /// <returns></returns>
        public bool DeleteEventBase(oEventBase o)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (o.EventBaseSysId == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "EventBaseId為空值", _dictRetMsg);
                    return bRetFlag;
                }
                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@EventBaseSysId", o.EventBaseSysId);

                string sSql = @"DELETE FROM  [FtcWISE_IPM].[dbo].[EventBase]
                                WHERE [EventBaseSysId] = @EventBaseSysId";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);

                if (iAffectedRecs <= 0)
                {
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventBase失敗(不明原因刪除不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成刪除DeleteEventBase");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;

        }


        /// <summary>
        /// 事件基礎:新增
        /// </summary>
        /// <param name="sEventBaseName "></param>
        /// <param name="sEventBaseDescribe "></param>
        /// <param name="sEventReason "></param>
        /// <param name="dCritical "></param>
        /// <param name="sNotifyContent "></param>
        /// <param name="sModifyUser "></param>
        /// <returns></returns>
        public bool CreateEventBase(string sEventBaseName, string sEventBaseDescribe,string sEventReason, decimal dCritical, string sNotifyContent, string sModifyUser)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (sEventBaseName == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("001", "事件名稱未設置", _dictRetMsg);
                    return bRetFlag;
                }
                if (sEventBaseDescribe == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "事件描述未設置", _dictRetMsg);
                    return bRetFlag;
                }
                if (sEventReason == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("003", "異常原因未設置", _dictRetMsg);
                    return bRetFlag;
                }
                if (dCritical == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("004", "嚴重度未設置", _dictRetMsg);
                    return bRetFlag;
                }
                
                if (sModifyUser == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("006", "修改人員未設置", _dictRetMsg);
                    return bRetFlag;
                }

                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();

                oMyDp.Add("@EventBaseName", sEventBaseName);
                oMyDp.Add("@EventBaseDescribe", sEventBaseDescribe);
                oMyDp.Add("@EventReason", sEventReason);
                oMyDp.Add("@Critical", dCritical);
                oMyDp.Add("@NotifyContent", sNotifyContent);
                oMyDp.Add("@ModifyUser", sModifyUser);

                string sSql = @"INSERT INTO  [FtcWISE_IPM].[dbo].[EventBase]
                                  (
                                    EventBaseName,
                                    EventBaseDescribe,
                                    EventReason,
                                    Critical,
                                    NotifyContent, 
                                    CreateUser,
                                    CreateDate,
                                    ModifyUser,
                                    ModifyDate
                                    
                                   )
                                   VALUES
                                    (@EventBaseName
                                    ,@EventBaseDescribe
                                    ,@EventReason
                                    ,@Critical
                                    ,@NotifyContent
                                    ,@MODIFYUSER
                                    ,GETDATE()
                                    ,@ModifyUser
                                    ,GETDATE())";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventBase失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }

                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成新增CreateEventBase");
            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }


        /// <summary>
        /// 事件基礎:修改
        /// </summary>
        /// <param name="sEventBaseName "></param>
        /// <param name="sEventBaseDescribe "></param>
        /// <param name="sEventReason "></param>
        /// <param name="dCritical "></param>
        /// <param name="sNotifyContent "></param>
        /// <returns></returns>
        public bool UpdateEventBase(string sEventBaseName, string sEventBaseDescribe, string sEventReason, decimal dCritical, string sNotifyContent, string sEventBaseSysId)
        {
            bool bRetFlag = false;
            SqlTransaction oSqlTxn = null;

            try
            {
                if (sEventBaseName == null)
                {
                    {
                        base.ReturnCode = DalReturnCode.InputParameterInvalid;
                        base.ReturnMessage = base.GetReturnMsg("001", "EventBaseName為空值", _dictRetMsg);
                        return bRetFlag;
                    }
                }
                if (sEventBaseDescribe == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("002", "EventBaseDescribe為空值", _dictRetMsg);
                    return bRetFlag;
                }

                if (sEventReason == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("003", "EventReason為空值", _dictRetMsg);
                    return bRetFlag;
                }
                if (dCritical == 0)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("004", "Critical", _dictRetMsg);
                    return bRetFlag;
                }

                if (sEventBaseSysId == null)
                {
                    base.ReturnCode = DalReturnCode.InputParameterInvalid;
                    base.ReturnMessage = base.GetReturnMsg("005", "EventBaseSysId為空值", _dictRetMsg);
                    return bRetFlag;
                }


                base.resetErrState();
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@EventBaseSysId", sEventBaseSysId);
                oMyDp.Add("@EventBaseName", sEventBaseName);
                oMyDp.Add("@EventBaseDescribe", sEventBaseDescribe);
                oMyDp.Add("@EventReason", sEventReason);
                oMyDp.Add("@Critical", dCritical);
                oMyDp.Add("@NotifyContent", sNotifyContent);
              

                string sSql = @"UPDATE  [FtcWISE_IPM].[dbo].[EventBase]
                                SET [EventBaseName] = @EventBaseName
                                ,[EventBaseDescribe] = @EventBaseDescribe   
                                ,[EventReason] = @EventReason
                                ,[Critical] = @Critical 
                                ,[NotifyContent] = @NotifyContent  
                                WHERE [EventBaseSysId]=@EventBaseSysId";

                int iAffectedRecs = base.cn.Execute(sSql, oMyDp, oSqlTxn);
                if (iAffectedRecs <= 0)
                {
                    //Rollback方法取消所有更改。
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "EventBase失敗(不明原因更新不到資料)!" + this.ReturnMessage, _dictRetMsg);
                    return bRetFlag;
                }
                //跨類別交易時先不commit(交易源頭才commit)
                if (!base.IsInCrossClassTxn)
                    oSqlTxn.Commit();
                bRetFlag = true;
                WriteLog_Info("完成更新EventBase");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return bRetFlag;
        }


        /// <summary>
        /// 事件基礎之異常原因:下拉式選單
        /// </summary>
        /// <returns></returns>
        public List<oComm_temList> GetEventReasonDrop()
        {
            List<oComm_temList> oRetList;
            try
            {
                base.resetErrState();
                string sSql = @"SELECT ItemName
                                FROM  [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                WHERE ListName = 'EventReason'";
                DynamicParameters oMyDp = new DynamicParameters();
                oRetList = base.cn.Query<oComm_temList>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }

        /// <summary>
        /// 事件基礎之嚴重度:下拉式選單
        /// </summary>
        /// <returns></returns>
        public List<oComm_temList> GetEventCriticalDrop()
        {
            List<oComm_temList> oRetList;
            try
            {
                base.resetErrState();
                string sSql = @"SELECT ItemValue
                                FROM  [FtcWISE_IPM].[dbo].[COMM_ITEMLIST]
                                WHERE ListName = 'EventCritical'
                                ORDER BY ItemIndex ASC;";
                DynamicParameters oMyDp = new DynamicParameters();
                oRetList = base.cn.Query<oComm_temList>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料");

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ErrMsg:{1})",
                    base.UserId, ex.ToString()));
                throw;
            }
            return oRetList;
        }





    }
}