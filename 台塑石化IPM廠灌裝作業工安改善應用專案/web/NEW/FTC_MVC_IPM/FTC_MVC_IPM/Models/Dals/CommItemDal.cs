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
    public class CommItemDal : CommDalBase
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
        public CommItemDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
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
        public CommItemDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng, ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
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

        public List<COMM_ItemList> GetCommItem(string p_sListName, string p_sKeyword)
        {
            //建立物件
            List<COMM_ItemList> mCommItemList = new List<COMM_ItemList>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                //資料庫查詢語法
                sbSql.Append(@"SELECT TOP 1000 c.* FROM COMM_ITEMLIST c
                LEFT JOIN COMM_USER u
                ON(c.ModifyUser = u.USERID) 
                WHERE ListName =@ListName");

                //使用關鍵字搜尋異常頁面
                if (!String.IsNullOrEmpty(p_sKeyword) && p_sListName == "EventReason")
                {
                    sbSql.Append(@" AND (c.ItemIndex LIKE '%' + @Keyword + 
                    '%' OR c.ItemName LIKE '%' + @Keyword + '%' 
                    OR u.USER_NAME LIKE '%' + @Keyword + '%')");
                }
                
                if (!String.IsNullOrEmpty(p_sKeyword) && p_sListName == "EventCritical")
                {
                    sbSql.Append(@" AND c.ItemName = @Keyword");
                }
                sbSql.Append(@" ORDER BY ItemIndex");
                p.Add("@ListName", p_sListName);
                p.Add("@Keyword", p_sKeyword);

                mCommItemList = cn.Query<COMM_ItemList>(sbSql.ToString(), p).ToList();
                return mCommItemList;
            }
            //異常處理
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                WriteLog_Error(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            return null;
        }

        //新增CommItem DAL
        public bool AddCommItem(COMM_ItemList p_oAdd)
        {
            List<COMM_ItemList> mCommItemList = new List<COMM_ItemList>();
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
             

                mCommItemList = this.GetCommItem(p_oAdd.ListName, p_oAdd.ItemName);
                if (mCommItemList.Any(x => x.ItemName == p_oAdd.ItemName && x.SysID != p_oAdd.SysID))
                {
                    if (p_oAdd.ListName == "EventReason")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("001", "異常原因不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                    if (p_oAdd.ListName == "EventCritical")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("002", "嚴重度說明不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                    if (p_oAdd.ListName == "NotifyMethod")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("003", "通知方式不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                }
                   
         
                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                p_oAdd.TargetType = "Web";
                p_oAdd.Target = "事件紀錄資料";
                p_oAdd.ModifyUser = base.UserId;
                sSql = @"INSERT INTO dbo.COMM_ITEMLIST (ListName, ItemIndex, ItemName, ItemValue, 
                    ItemMemo, TargetType, Target, ModifyDate, ModifyUser)
                        VALUES (@ListName, @ItemIndex, @ItemName, @ItemValue, 
                    @ItemMemo, @TargetType, @Target, getdate(), @ModifyUser)";
             
                //紀錄資料庫異動的筆數
                int iAffectedRecs = base.cn.Execute(sSql, p_oAdd, oSqlTxn);
                //未異動
                if (iAffectedRecs <= 0)
                {
                    //還原資料庫狀態
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "新增資料失敗!", _dictRetMsg);
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

        //刪除CommItem DAL
        public bool DelCommItem(COMM_ItemList p_oDel)
        {
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                

                //sql刪除資料語法
                sSql = @"DELETE FROM dbo.COMM_ITEMLIST
                WHERE SysID = @SysID";

                //紀錄資料庫異動的筆數
                int iAffectedRecs = base.cn.Execute(sSql, p_oDel, oSqlTxn);
                //未異動
                if (iAffectedRecs <= 0)
                {
                    //還原資料庫狀態
                    if (oSqlTxn.Connection != null) oSqlTxn.Rollback();
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("101", "刪除資料失敗!", _dictRetMsg);
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

        //更新CommItem DAL
        public bool UpdCommItem(COMM_ItemList p_oUpd)
        {
            List<COMM_ItemList> mCommItemList = new List<COMM_ItemList>();
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                mCommItemList = this.GetCommItem(p_oUpd.ListName, p_oUpd.ItemName);
                if (mCommItemList.Any(x => x.ItemName == p_oUpd.ItemName && x.SysID != p_oUpd.SysID))
                {
                    if (p_oUpd.ListName == "EventReason")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("004", "異常原因不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                    if (p_oUpd.ListName == "EventCritical")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("005", "嚴重度說明不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                    if (p_oUpd.ListName == "NotifyMethod")
                    {
                        base.ReturnCode = DalReturnCode.Failed;
                        base.ReturnMessage = base.GetReturnMsg("006", "通知方式不可重複!", _dictRetMsg);
                        return bRetFlag;
                    }
                }


                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                p_oUpd.ModifyUser = base.UserId;
                //sql更新資料語法
                sSql = @"UPDATE dbo.COMM_ITEMLIST
                    SET ItemIndex = @ItemIndex ,ItemName = @ItemName,ItemValue = @ItemValue,
	                ModifyDate = getDate() ,ModifyUser = @ModifyUser
                    WHERE SysID = @SysID";

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