using Dapper;
using FTC_MES_MVC.Base;
using FTC_MES_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace FTC_MES_MVC.Models.Dals
{
    public class EventNotifyTransDal : CommDalBase
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
        public EventNotifyTransDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
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
        public EventNotifyTransDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng, ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
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

        //查詢EventNotifyTransDAL
        public List<EventNotifyTransactions> GetEventNotifyTrans(List<string> p_sEventBaseSysId, DateTime p_sStartDate, DateTime p_sEndDate)
        {
            //建立物件
            List<EventNotifyTransactions> mEventNotifyTransList = new List<EventNotifyTransactions>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                //資料庫查詢語法
                sbSql.Append(@"SELECT TOP 10000 n.EventSysId, n.Notified, n.NotifyGroup, n.NotifyMethod,      
                    n.ModifyDate, n.CreateDate, b.EventBaseName FROM EventNotifyTransactions n
                    LEFT JOIN EventTransactions e ON n.EventSysId = e.EventSysId
                    LEFT JOIN EventBase b ON e.EventBaseSysId = b.EventBaseSysId
                    WHERE 1 = 1");

                //篩選起訖時間
                if (p_sStartDate!= DateTime.MinValue)
                {
                    sbSql.Append(@" AND n.CreateDate >= @StartDate");
                    if(p_sEndDate!= DateTime.MinValue)
                    {
                        p_sEndDate= p_sEndDate.AddDays(1);
                        sbSql.Append(@" AND n.CreateDate < @EndDate");              
                        p.Add("@EndDate", p_sEndDate);
                    }
                    p.Add("@StartDate", p_sStartDate);
                }

                //篩選事件基礎名稱
                if (p_sEventBaseSysId != null && p_sEventBaseSysId.Any(s => !string.IsNullOrWhiteSpace(s)))
                {
                    sbSql.Append(" AND b.EventBaseSysId IN @EventBaseSysId");
                    p.Add("@EventBaseSysId", p_sEventBaseSysId);
                }                      
                mEventNotifyTransList = cn.Query<EventNotifyTransactions>(sbSql.ToString(), p).ToList();

                return mEventNotifyTransList;
            }
            //異常處理
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
          
        }

        //更新通知狀態DAL
        public bool UpdEventNotifyTrans(EventNotifyTransactions p_oUpd)
        {
            List<EventNotifyTransactions> mEventNotifyTransList = new List<EventNotifyTransactions>();
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                p_oUpd.ModifyUser = base.UserId;
                //sql更新資料語法
                sSql = @"UPDATE dbo.EventNotifyTransactions
                    SET Notified = 1,ModifyDate = getdate()
                    WHERE EventSysId = @EventSysId";

                //紀錄資料庫異動的筆數
                int iAffectedRecs = base.cn.Execute(sSql, p_oUpd, oSqlTxn);
                //未異動
                if (iAffectedRecs <= 0)
                {
                    //還原資料庫狀態
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "更新資料失敗!", _dictRetMsg);
                    return bRetFlag;
                }
                //跨類別交易時先不commit(交易源頭才commit)
                if (!base.IsInCrossClassTxn)
                {
                    oSqlTxn.Commit();
                }
                bRetFlag = true;
            }
            //異常處理
            catch (Exception ex)
            {
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.ToString();
            }
            //回傳資料給Controller
            return bRetFlag;
        }
    }
}