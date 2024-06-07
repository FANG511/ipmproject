using Dapper;
using FTC_MES_MVC.Base;
using FTC_MES_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FTC_MES_MVC.Models.Dals
{
    public class ReturnMessageDal : CommDalBase
    {
        private string _apDbConnStr = "";
        private string _className = "";  //類別名稱變數
        private Dictionary<string, ReturnMessageModel> _dictRetMsg = null;  //保存此類別所有語系的回覆訊息

        /// <summary>
        ///ReturnMessageDal建構式(for Controller呼叫)
        ///ps.使用後一定要Dispose(),否則連線會超過
        /// </summary>
        /// <param name="p_sUserId">使用者代碼</param>
        /// <param name="p_sCompanyId">公司代碼</param>
        /// <param name="p_sFactoryId">廠別(含應用程式代碼)</param>
        /// <param name="p_sLng">語系(預設繁體中文)</param>
        public ReturnMessageDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
        {
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
                base._cn(_apDbConnStr, base.UserId);
                if (base.ReturnCode != DalReturnCode.Success) { throw new Exception(base.ReturnMessage); }
                //取得此類別的所有回覆訊息
                var oRetMsgList = this.GetAllByClass(_className);
                _dictRetMsg = oRetMsgList.Where(c => c.LANGUAGE == base.LanguageId).ToDictionary(p => p.MESSAGEID);

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
        }

        /// <summary>
        ///ReturnMessageDal建構式2(for 其他DAL呼叫)
        ///ps.使用後一定要Dispose(),否則連線會超過
        /// </summary>
        /// <param name="p_sUserId">使用者代碼</param>
        /// <param name="p_sCompanyId">公司代碼</param>
        /// <param name="p_sFactoryId">廠別(含應用程式代碼)</param>
        /// <param name="p_sLng">語系</param>
        /// <param name="p_oRefConn">呼叫者傳入的連線物件</param>
        /// <param name="p_oRefTxn">呼叫者傳入的交易物件(用於資料庫交易期間)</param>
        public ReturnMessageDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng, ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
        {
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
                var oRetMsgList = this.GetAllByClass(_className);
                _dictRetMsg = oRetMsgList.Where(c => c.LANGUAGE == base.LanguageId).ToDictionary(p => p.MESSAGEID);
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
        }

        /// <summary>
        /// 查詢某一類別的所有回覆訊息(每個Dal Class建構時讀取到全域變數,供後續碰到需回覆訊息時使用)
        /// </summary>
        /// <param name="p_sClassName">類別名稱</param>
        /// <returns></returns>
        public List<ReturnMessageModel> GetAllByClass(string p_sClassName)
        {
            List<ReturnMessageModel> oRetList;
            try
            {
                base.resetErrState();

                DynamicParameters oMyDp = new DynamicParameters();
                oMyDp.Add("@CLASSNAME", p_sClassName);

                string sSql = "SELECT * FROM FTCRETURNMESSAGE \r\n" +
                              "WHERE CLASSNAME=@CLASSNAME ";

                oRetList = base.cn.Query<ReturnMessageModel>(sSql, oMyDp, base.refTxn).ToList();
                WriteLog_Info(base.UserId + "讀取資料：" + JsonConvert.SerializeObject(oRetList));

            }
            catch (Exception ex)
            {
                base.ReturnCode = DalReturnCode.Failed;
                base.ReturnMessage = ex.Message;
                base.WriteLog_Error(string.Format(
                    "User:{0},Parameters:(ClassName={1}), ErrMsg:{2}",
                    base.UserId, p_sClassName, ex.ToString()));
                throw;
            }
            return oRetList;
        }
    }
}