using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using FTC_MES_MVC.Models.ViewModels;
using Newtonsoft.Json;
using NLog;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Oracle.ManagedDataAccess.Client;
using FTC_MES_MVC.Models;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Digests;
using System.Web.Configuration;
using System.Collections;

namespace FTC_MES_MVC.Base
{
    public class CommDalBase : IDisposable
    {
        //Log相關變數
        protected Logger logger = NLog.LogManager.GetCurrentClassLogger();
        protected LogMessage oLogMessage = new LogMessage();
        //資料庫相關變數
        //protected SqlConnection cn { get; set; }
        protected SqlConnection cn = null;
        protected SqlConnection refConn = null; //外部傳入的連線
        protected SqlTransaction refTxn = null; //外部傳入的交易

        //查詢相關變數
        protected DynamicParameters dp = new DynamicParameters();
        protected string sql { get; set; }

        //DAL使用的屬性:使用者代碼
        protected string UserId { get; set; }
        //DAL使用的屬性:公司代碼
        protected string CompanyId { get; set; }
        //DAL使用的屬性:廠別(+應用系統)代碼
        protected string FactoryId { get; set; }
        //DAL使用的屬性:語系代碼
        protected string LanguageId { get; set; }
        //DAL使用的屬性:是否處於交易中
        protected bool IsInCrossClassTxn => refTxn != null;

        /// <summary>
        /// 回傳錯誤代碼
        /// </summary>
        public DalReturnCode ReturnCode { get; set; }
        /// <summary>
        /// 回傳錯誤訊息
        /// </summary>
        public string ReturnMessage { get; set; }

        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        /// <summary>
        /// 開啟SqlDb連線
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        protected SqlConnection _cn(string p_sConnStr, string p_sUserId)
        {
            try
            {
                resetErrState();

                if (String.IsNullOrEmpty(p_sConnStr))
                    return cn;

                if (cn == null)
                {
                    cn = new SqlConnection(AesDecrypt(p_sConnStr));
                }
                else
                {
                    cn.Close();
                    cn.ConnectionString = new SqlConnectionStringBuilder(p_sConnStr).ConnectionString;
                }
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }
            }
            catch (Exception ex)
            {
                this.ReturnCode = DalReturnCode.ConectionError;
                this.ReturnMessage = ex.Message;
                WriteLog_Error(string.Format(
                    "Parameters:(ConnStr={0}, UserId={1}), ErrMsg:{2}",
                    p_sConnStr, p_sUserId, ex.ToString()));
            }
            return cn;
        }

        /// <summary>
        /// GetNewSqlConn:取得新連線(非property)
        /// </summary>
        /// <param name="connectionString">連線字串</param>
        /// <returns></returns>
        protected SqlConnection GetNewSqlConn(string p_sConnStr, string p_sUserId)
        {
            SqlConnection oRetConn = null;
            try
            {
                resetErrState();

                if (String.IsNullOrEmpty(p_sConnStr))
                    return oRetConn;

                if (oRetConn == null)
                {
                    oRetConn = new SqlConnection(AesDecrypt(p_sConnStr));
                }
                else
                {
                    oRetConn.Close();
                    oRetConn.ConnectionString = p_sConnStr;
                }
                if (oRetConn.State == ConnectionState.Closed)
                {
                    oRetConn.Open();
                }
            }
            catch (Exception ex)
            {
                this.ReturnCode = DalReturnCode.ConectionError;
                this.ReturnMessage = ex.Message;
                oLogMessage.Error = ex.ToString();
                WriteLog_Error(string.Format(
                    "Parameters:(ConnStr={0}, UserId={1}), ErrMsg:{2}",
                    p_sConnStr, p_sUserId, ex.ToString()));
            }
            return oRetConn;
        }

        /// <summary>
        /// 取得應用程式資料庫(ApDb)連線
        /// </summary>
        /// <returns></returns>
        public string GetApDbConnStr(string p_sCompanyId, string p_sFactoryId, string p_sUserId)
        {
            string sApDbConnStr = "Data Source=@ApHostIP@ApDbPort;Initial Catalog=@ApDbName;Persist Security Info=True;User ID=@ApDbUser;Password=@ApDbPswd";
            var DbAppName = "";
            try
            {
                resetErrState();

                using (var commDbConn = GetNewSqlConn(ConfigurationManager.ConnectionStrings["Base_DB_ConnectString"].ToString(), p_sUserId))
                {
                    DynamicParameters oMyDp = new DynamicParameters();
                    oMyDp.Add("@CompanyID", p_sCompanyId);
                    oMyDp.Add("@FactoryID", p_sFactoryId);

                    sql = @"SELECT TOP 1 * FROM MesApHostDb
                WHERE CompanyID=@CompanyID
                AND FactoryID=@FactoryID";

                    var qData = commDbConn.Query<MesApHostDb>(sql, oMyDp).FirstOrDefault();

                    if (qData == null)
                    {
                        throw new Exception("未設定預設應用程式資料(ApDb)庫連線");
                    }

                    sApDbConnStr = sApDbConnStr.Replace("@ApHostIP", qData.ApHostIP);
                    sApDbConnStr = sApDbConnStr.Replace("@ApDbPort", String.IsNullOrEmpty(qData.ApDbPort) ? "" : "," + qData.ApDbPort);
                    sApDbConnStr = sApDbConnStr.Replace("@ApDbName", qData.ApDbName);
                    sApDbConnStr = sApDbConnStr.Replace("@ApDbUser", qData.ApDbUser);
                    sApDbConnStr = sApDbConnStr.Replace("@ApDbPswd", qData.ApDbPswd);

                    DbAppName = WebConfigurationManager.AppSettings["DbAppName"];
                    if (DbAppName != "")
                    {
                        sApDbConnStr = sApDbConnStr + ";Application Name=" + DbAppName;
                    }
                }
            }
            catch (Exception ex)
            {
                this.ReturnCode = DalReturnCode.ConectionError;
                this.ReturnMessage = ex.Message;
                WriteLog_Error(string.Format(
                    "Parameters:(CompanyId={0}, FactoryId={1}, UserId={2}), ErrMsg:{3}",
                    p_sCompanyId, p_sFactoryId, p_sUserId, ex.ToString()));
                throw;
            }
            return sApDbConnStr;
        }

        protected void ShareDbTxn(ref SqlConnection p_oRefConn, ref SqlTransaction p_oRefTxn)
        {
            try
            {
                if (refTxn != null)
                    refTxn.Rollback();
                if (refConn != null)
                    refConn.Close();
                if (cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }

                refTxn = p_oRefTxn;
                refConn = p_oRefConn;
                cn = refConn;
            }
            catch (Exception ex)
            {
                ReturnCode = DalReturnCode.ConectionError;
                ReturnMessage = ex.Message;
                WriteLog_Error(string.Format(
                    "User:{0}, Parameters:(Conn={1}, Txn={2}), ErrMsg:{3}",
                    this.UserId, refConn.ToString(), refTxn.ToString(), ex.ToString()));
                throw;
            }
        }

        /// <summary>
        ///  依傳入的參數動態產生sql 及 sql parameter
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sb"></param>
        /// <param name="p"></param>
        /// <param name="TimeRangeColName"></param>
        protected void AddQueryCondition(dynamic query, StringBuilder sb, DynamicParameters p, string TimeRangeColName = "")
        {
            try
            {
                DateTime StartTime = DateTime.Now;
                DateTime EndTime = DateTime.Now;
                if (string.IsNullOrEmpty(TimeRangeColName))
                {
                    TimeRangeColName = "TXNTIME";
                }
                if (query != null)
                {
                    foreach (var prop in query.GetType().GetProperties())
                    {
                        var sValue = prop.GetValue(query, null);
                        var sName = prop.Name;

                        if (Convert.ToString(sName) == "CompanyID" || Convert.ToString(sName) == "FactoryID" || Convert.ToString(sName) == "OrderBy")
                        {
                            continue;
                        }
                        if (sValue != null && !string.IsNullOrEmpty(sValue))
                        {
                            if (Convert.ToString(sName) == "StartTime")
                            {
                                sb.Append(" and " + TimeRangeColName + ">= @" + sName);
                                StartTime = CheckDate(Convert.ToString(sValue));
                                p.Add("@" + sName, StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (Convert.ToString(sName) == "EndTime")
                            {
                                sb.Append(" and " + TimeRangeColName + "< @" + sName);
                                EndTime = CheckDate(Convert.ToString(sValue));
                                if (EndTime.Hour == 0 && EndTime.Minute == 0 && EndTime.Second == 0)
                                {
                                    p.Add("@" + sName, EndTime.AddDays(1).ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    p.Add("@" + sName, EndTime.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                            else
                            {
                                string value = Convert.ToString(sValue);
                                if (value.Contains("^^"))//特殊註記
                                {
                                    sb.Append(" and " + sName + "!=@" + sName);
                                }
                                else
                                {
                                    sb.Append(" and " + sName + "=@" + sName);
                                }
                                value = value.Replace("^^", "");
                                p.Add("@" + sName, value);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oLogMessage.FuctionName = "AddQueryCondition";
                oLogMessage.Error = ex.ToString();
                WriteLog();
            }
        }

        protected DateTime CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// 取得回傳訊息
        /// </summary>
        /// <param name="s_MsgId">訊息編號的流水號(類別.方法.流水號)</param>
        /// <param name="p_sZhTwRetMsg">繁體中文的訊息(作為抓不到的預設回傳)</param>
        /// <param name="p_dictKeyValMap">訊息對照表(已先依照指定語系查出該類別的所有回傳訊息)</param>
        /// <returns></returns>
        public string GetReturnMsg(string p_sMsgSeq, string p_sZhTwRetMsg, Dictionary<string, ReturnMessageModel> p_dictKeyValMap)
        {
            try
            {
                var oMethodInfo = new StackTrace().GetFrame(1).GetMethod();

                string sClasName = oMethodInfo.ReflectedType.Name;
                string sMethodName = oMethodInfo.Name;
                string sMsgId = sClasName + "." + sMethodName + "." + p_sMsgSeq;

                if (!p_dictKeyValMap.ContainsKey(sMsgId))
                    return p_sZhTwRetMsg;
                else
                    return p_dictKeyValMap[sMsgId].RETURNMESSAGE;
            }
            catch (Exception ex)
            {
                ReturnCode = DalReturnCode.ConectionError;
                ReturnMessage = ex.Message;
                WriteLog_Error(string.Format(
                    "User:{0}, Parameters:(MsgSeq={1}, ZhTwRetMsg={2}, p_dictKeyValMap={3}), ErrMsg:{4}",
                    this.UserId, p_sMsgSeq, p_sZhTwRetMsg, JsonConvert.SerializeObject(p_dictKeyValMap), ex.ToString()));
                throw;
            }
        }

        /// <summary>
        /// 取得目前(呼叫此方法的那層)方法名稱
        /// </summary>
        /// <returns></returns>
        public string GetCurrentMethod()
        {
            return new StackTrace().GetFrame(1).GetMethod().Name;
        }

        /// <summary>
        /// 日誌記錄函數
        /// </summary>
        /// <param name="p_sFuncName">指定紀錄的函數名稱(預設為空,系統自行抓往上兩層的函數名稱)</param>
        protected void WriteLog(string p_sFuncName = "")
        {
            StackTrace stackTrace = new StackTrace();
            oLogMessage.RecTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            oLogMessage.Error = oLogMessage.Error == null ? "" : oLogMessage.Error;
            oLogMessage.Status = oLogMessage.Status == null ? "" : oLogMessage.Status;
            //此方法會先被WriteLog_Info/WriteLog_Error呼叫, 所以真正的FunctionName會再往上兩層
            oLogMessage.FuctionName = string.IsNullOrEmpty(p_sFuncName) ?
                 stackTrace.GetFrame(2).GetMethod().Name : p_sFuncName;

            if (oLogMessage.Error != "")
            {
                logger.Error(correctLogForging(JsonConvert.SerializeObject(oLogMessage)));
            }
            else
            {
                logger.Info(correctLogForging(JsonConvert.SerializeObject(oLogMessage)));
            }

            oLogMessage.Error = "";
            oLogMessage.Status = "";
        }
        /// <summary>
        /// 寫入log Info
        /// </summary>
        /// <param name="p_sInfo">訊息內容</param>
        /// <param name="p_sFuncName">指定紀錄的函數名稱(預設為空,系統自行判斷)</param>
        protected void WriteLog_Info(string p_sInfo, string p_sFuncName = "")
        {
            oLogMessage.Info = p_sInfo;
            oLogMessage.Status = "Run";
            WriteLog(p_sFuncName);
        }
        /// <summary>
        /// 寫入log Error
        /// </summary>
        /// <param name="p_sError">訊息內容</param>
        /// <param name="p_sFuncName">指定紀錄的函數名稱(預設為空,系統自行判斷)</param>
        protected void WriteLog_Error(string p_sError, string p_sFuncName = "")
        {
            oLogMessage.FuctionName = p_sFuncName;
            oLogMessage.Error = p_sError;
            oLogMessage.Status = "Run";
            WriteLog(p_sFuncName);
        }

        /// <summary>
        /// 回歸無錯誤狀態
        /// </summary>
        protected void resetErrState()
        {
            this.ReturnCode = DalReturnCode.Success;
            this.ReturnMessage = "";
        }

        /// <summary>
        /// 實作IDispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Base結束開閉資料庫連線
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();

                //沒有傳入參考連線時才關閉連線
                if (cn != null & !IsInCrossClassTxn)
                    cn.Close();

            }

            disposed = true;
        }
        static string Aeskey = "XD5300@bochi";
        private class AesKeyIV
        {
            public Byte[] Key = new Byte[16];
            public Byte[] IV = new Byte[16];
            public AesKeyIV(string strKey)
            {
                var sha = new Sha256Digest();
                var hash = new byte[sha.GetDigestSize()];
                var data = Encoding.ASCII.GetBytes(strKey);
                sha.BlockUpdate(data, 0, data.Length);
                sha.DoFinal(hash, 0);
                Array.Copy(hash, 0, Key, 0, 16);
                Array.Copy(hash, 16, IV, 0, 16);
            }
        }
        public static string AesEncrypt(string rawString)
        {
            try
            {
                var keyIv = new AesKeyIV(Aeskey);
                // Default - AES/GCM/NoPadding、System.Security.AES - AES/CBC/PKCS7
                var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
                cipher.Init(true, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var rawData = Encoding.UTF8.GetBytes(rawString);
                return Convert.ToBase64String(cipher.DoFinal(rawData));
            }
            catch (Exception ex)
            {
                return rawString;
            }
        }

        public static string AesDecrypt(string encString)
        {
            try
            {
                if (encString.ToLower().Contains("data source"))
                {
                    return new SqlConnectionStringBuilder(encString).ConnectionString;
                }
                var keyIv = new AesKeyIV(Aeskey);
                // Default - AES/GCM/NoPadding、System.Security.AES - AES/CBC/PKCS7
                var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
                cipher.Init(false, new ParametersWithIV(new KeyParameter(keyIv.Key), keyIv.IV));
                var encData = Convert.FromBase64String(encString);
                return new SqlConnectionStringBuilder(Encoding.UTF8.GetString(cipher.DoFinal(encData))).ConnectionString;
            }
            catch (Exception ex)
            {
                return encString;
            }
        }
        protected double ToNumeric(object value)
        {
            double v = 0;
            try
            {
                Double.TryParse(value.ToString().Trim(), out v);
            }
            catch (Exception ex)
            {
            }
            return v;
        }
        protected Boolean IsNumeric(object value)
        {
            double v = 0;
            Boolean result = true;
            try
            {
                Double.TryParse(value.ToString().Trim(), out v);
            }
            catch (Exception ex)
            {
                return false;
            }
            return result;
        }
        protected DateTime ToDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return dt;
            }
            catch
            {
                return DateTime.Now;
            }
        }
        protected string correctLogForging(string sMsgToWrite)
        {
            string retStr = sMsgToWrite.Normalize(NormalizationForm.FormKC); //正規化input string
            try
            {

                List<string> excludeList = new List<string>();                                            //應處理的換行符號
                excludeList.Add("%0a"); // \n
                excludeList.Add("%0A"); // \n
                excludeList.Add("%0d"); // \r
                excludeList.Add("%0D"); // \r
                excludeList.Add("\r");
                excludeList.Add("\n");
                excludeList.Add(@"\r");
                excludeList.Add(@"\n");
                foreach (string curItem in excludeList)
                    retStr = retStr.Replace(curItem, "");  //取代換行符號
            }
            catch (Exception ex)
            {

            }
            return retStr;
        }
        //移除危險字元
        public static string replaceSQLChar(string sValue, bool sReplace = true)
        {
            string sRetValue = String.Empty;
            if (string.IsNullOrEmpty(sValue)) return sRetValue;
            sRetValue = sValue.Normalize(NormalizationForm.FormKC);  //先正規化內容
            if (sReplace)
            {
                Dictionary<string, string> replaceList = new Dictionary<string, string>();
                replaceList.Add("'", "''"); replaceList.Add(";", "；");
                replaceList.Add(",", "，"); replaceList.Add("?", "？");
                replaceList.Add("<", "＜"); replaceList.Add(">", "＞");
                replaceList.Add("(", "（"); replaceList.Add(")", "）");
                replaceList.Add("@", "＠"); replaceList.Add("=", "＝");
                replaceList.Add("+", "＋"); replaceList.Add("*", "＊");
                replaceList.Add("&", "＆"); replaceList.Add("#", "＃");
                replaceList.Add("%", "％"); replaceList.Add("$", "￥");
                replaceList.Add("delete", "ｄｅｌｅｔｅ");
                replaceList.Add("Delete", "Ｄｅｌｅｔｅ");
                replaceList.Add("DELETE", "ＤＥＬＥＴＥ");
                replaceList.Add("update", "ｕｐｄａｔｅ");
                replaceList.Add("Update", "Ｕｐｄａｔｅ");
                replaceList.Add("UPDATE", "ＵＰＤＡＴＥ");
                replaceList.Add("insert", "ｉｎｓｅｒｔ");
                replaceList.Add("Insert", "Ｉｎｓｅｒｔ");
                replaceList.Add("INSERT", "ＩＮＳＥＲＴ");
                replaceList.Add("drop", "ｄｒｏｐ"); replaceList.Add("Drop", "Ｄｒｏｐ");
                replaceList.Add("DROP", "ＤＲＯＰ"); replaceList.Add("create", "ｃｒｅａｔｅ");
                replaceList.Add("Create", "Ｃｒｅａｔｅ");
                replaceList.Add("CREATE", "ＣＲＥＡＴＥ"); replaceList.Add("exec", "ｅｘｅｃ");
                replaceList.Add("Exec", "Ｅｘｅｃ"); replaceList.Add("EXEC", "ＥＸＥＣ");
                foreach (string curKey in replaceList.Keys)
                    sRetValue = sRetValue.Replace(curKey, replaceList[curKey]);
            }
            return sRetValue;
        }
        //20230809 N000170550新增
        //public static bool IsEmpty(object obj)
        //{
        //    // 在這裡判斷物件是否為空值，根據你的需求來處理
        //    // 例如，你可以檢查字串是否為空、集合是否為空等等
        //    if (obj is string str)
        //    {
        //        return string.IsNullOrEmpty(str);
        //    }
        //    else if (obj is ICollection collection)
        //    {
        //        return collection.Count == 0;
        //    }
        //    // 根據需要，可以加入其他型別的處理邏輯

        //    return false; // 預設假設物件不是空值
        //}
    }
}