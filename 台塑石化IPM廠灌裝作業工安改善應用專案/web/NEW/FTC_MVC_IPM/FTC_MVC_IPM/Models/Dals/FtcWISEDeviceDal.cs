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
    public class FtcWISEDeviceDal : CommDalBase
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
        public FtcWISEDeviceDal(string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng = "zh-TW")
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
        public FtcWISEDeviceDal (string p_sUserId, string p_sCompanyId, string p_sFactoryId, string p_sLng, ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
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

        //查詢DAL
        public List<FtcWISEDevice>GetFtcWISEDevice(string p_sKeyword)
        {
            //建立物件
            List<FtcWISEDevice> mFtcWISEDeviceList = new List<FtcWISEDevice>();
            StringBuilder sbSql = new StringBuilder();
            //DynamicParameters以動態方式添加參數
            //p儲存所有查詢參數
            DynamicParameters p = new DynamicParameters();
            try
            {
                //資料庫查詢語法
                sbSql.Append(@"SELECT TOP 1000 f.* FROM FtcWISE_Device f
                LEFT JOIN COMM_USER u
                ON(f.ModifyUser = u.USERID)");

                //使用關鍵字搜尋異常頁面
                if (!String.IsNullOrEmpty(p_sKeyword) )
                {
                    sbSql.Append(@" WHERE f.DeviceId LIKE '%' + @Keyword + '%' 
                    OR f.DeviceName LIKE '%' + @Keyword + '%' 
                    OR f.DeviceDescribe LIKE '%' + @Keyword + '%' 
                    OR f.RTSP_URL LIKE '%' + @Keyword + '%' 
                    OR f.PictureDestination LIKE '%' + @Keyword + '%'
                    OR f.NVR_URL LIKE '%' + @Keyword + '%'");
                }               

                p.Add("@Keyword", p_sKeyword);

                mFtcWISEDeviceList = cn.Query<FtcWISEDevice>(sbSql.ToString(), p).ToList();
                return mFtcWISEDeviceList;
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

        //新增DAL
        public bool AddFtcWISEDevice(FtcWISEDevice p_oAdd)
        {
            List<FtcWISEDevice> mFtcWISEDevice = new List<FtcWISEDevice>();
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                //檢核設備編號是否重複
                mFtcWISEDevice = this.GetFtcWISEDevice(p_oAdd.DeviceId);
                if (mFtcWISEDevice.Any(x => x.DeviceId == p_oAdd.DeviceId && x.DeviceSysID != p_oAdd.DeviceSysID))
                {
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("001", "設備編號不可重複!", _dictRetMsg);
                    return bRetFlag;
                }
        
                //檢核設備名稱是否重複
                mFtcWISEDevice = this.GetFtcWISEDevice(p_oAdd.DeviceName);
                if (mFtcWISEDevice.Any(x => x.DeviceId == p_oAdd.DeviceId && x.DeviceSysID != p_oAdd.DeviceSysID))
                {
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("002", "設備名稱不可重複!", _dictRetMsg);
                    return bRetFlag;
                }
              
              
                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
                p_oAdd.CreateUser = base.UserId;
                p_oAdd.ModifyUser = base.UserId;
                sSql = @"INSERT INTO dbo.FtcWISE_Device(DeviceId,DeviceName,DeviceDescribe,
	            PictureDestination,RTSP_URL,NVR_URL,CreateUser,CreateTime,ModifyUser,ModifyDate)   
                VALUES (@deviceid,@devicename,@devicedescribe,@picturedestination,@rtsp_url,
	            @nvr_url,@modifyuser,getdate(),@modifyuser,getdate())";
          
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

        //刪除DAL
        public bool DelFtcWISEDevice(FtcWISEDevice p_oDel)
        {
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();
            
                //sql刪除資料語法
                sSql = @"DELETE FROM dbo.FtcWISE_Device
                WHERE DeviceSysID = @DeviceSysID";

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

        //更新DAL
        public bool UpdFtcWISEDevice(FtcWISEDevice p_oUpd)
        {
            List<FtcWISEDevice> mFtcWISEDevice = new List<FtcWISEDevice>();
            SqlTransaction oSqlTxn = null;
            bool bRetFlag = false;
            string sSql;
            try
            {
                //檢核設備編號是否重複
                mFtcWISEDevice = this.GetFtcWISEDevice(p_oUpd.DeviceId);
                if (mFtcWISEDevice.Any(x => x.DeviceId == p_oUpd.DeviceId && x.DeviceSysID != p_oUpd.DeviceSysID))
                {
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("003", "設備編號不可重複!", _dictRetMsg);
                    return bRetFlag;
                }

                //檢核設備名稱是否重複
                mFtcWISEDevice = this.GetFtcWISEDevice(p_oUpd.DeviceName);
                if (mFtcWISEDevice.Any(x => x.DeviceName == p_oUpd.DeviceName && x.DeviceSysID != p_oUpd.DeviceSysID))
                {
                    base.ReturnCode = DalReturnCode.Failed;
                    base.ReturnMessage = base.GetReturnMsg("004", "設備名稱不可重複!", _dictRetMsg);
                    return bRetFlag;
                }


                //取得資料庫交易物件(有跨類別交易時不另開立交易)
                oSqlTxn = base.IsInCrossClassTxn ? base.refTxn : base.cn.BeginTransaction();

                p_oUpd.ModifyUser = base.UserId;
                //sql更新資料語法
                sSql = @"UPDATE dbo.FtcWISE_Device SET DeviceId = @DeviceId
	                    ,DeviceName = @DeviceName, DeviceDescribe = @DeviceDescribe
	                    ,RTSP_URL = @RTSP_URL, PictureDestination = @PictureDestination
	                    ,NVR_URL = @NVR_URL, ModifyUser = @ModifyUser 
	                    ,ModifyDate = getdate() WHERE DeviceSysID = @DeviceSysID";
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