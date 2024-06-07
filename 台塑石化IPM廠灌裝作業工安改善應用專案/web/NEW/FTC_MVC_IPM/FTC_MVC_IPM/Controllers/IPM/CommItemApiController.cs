using System;
using System.Collections.Generic;
using System.Web.Http;
using FTC_MES_MVC.Models;
using FTC_MES_MVC.Models.Dals;
using FTC_MES_MVC.Models.IPMModels;
using FTC_MES_MVC.Models.ViewModels;

namespace FTC_MES_MVC.Controllers.CommItem.Api
{
    public class CommItemApiController : BaseApiController
    {

        public CommItemApiController()
        {
            oApiReturnMessage = new ApiReturnMessage();
        }

    
        [HttpPost]

        //查詢CommItem api
        public List<dynamic> GetCommItemDetail(oCommItemDetail o)
        {
            List<dynamic> mCommItemList = new List<dynamic>();
            CommItemDal oDal = null;
            try
            {
                oDal = new CommItemDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                mCommItemList.Add(oDal.GetCommItem(o.sListName, o.sKeyword));

            }
            catch (Exception ex)
            {
                WriteLog_Error("GetCommItemDetail Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCommItemList.Add(oApiReturnMessage);
            }
            return mCommItemList;
        }

        [HttpPost]
        //新增CommItem api
        public List<dynamic> AddCommItemDetail(oCOMM_ItemListModel o)
        {
            List<dynamic> mCommItemList = new List<dynamic>();
            CommItemDal oDal = null;
            COMM_ItemList oComm = o.RULEs;
            try
            {
                oDal = new CommItemDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                if (!oDal.AddCommItem(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddCommItemDetail Error" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCommItemList.Add(oApiReturnMessage);
            }
            return mCommItemList;
        }

        [HttpPost]
        //刪除CommItem api
        public List<dynamic> DelCommItemDetail(oCOMM_ItemListModel o)
        {
            List<dynamic> mCommItemList = new List<dynamic>();
            CommItemDal oDal = null;
            COMM_ItemList oComm = o.RULEs;
            try
            {
                oDal = new CommItemDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                if (!oDal.DelCommItem(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("DeleteCommItemDetail Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCommItemList.Add(oApiReturnMessage);
            }
            return mCommItemList;
        }

        [HttpPost]
        //更新CommItem api
        public List<dynamic> UpdateCommItemDetail(oCOMM_ItemListModel o)
        {
            List<dynamic> mCommItemList = new List<dynamic>();
            CommItemDal oDal = null;
            COMM_ItemList oComm = o.RULEs;
            try
            {
                oDal = new CommItemDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                if (!oDal.UpdCommItem(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdateCommItemDetail Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCommItemList.Add(oApiReturnMessage);
            }
            return mCommItemList;
        }

        [HttpPost]
        //查詢攝影機設備api
        public List<dynamic> GetFtcWISEDeviceDetail(oCommItemDetail o)
        {
    
            List<dynamic> mCameraInfoList = new List<dynamic>();
            FtcWISEDeviceDal oDal = null;
            try
            {
                oDal = new FtcWISEDeviceDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                mCameraInfoList.Add(oDal.GetFtcWISEDevice(o.sKeyword));

            }
            catch (Exception ex)
            {
                WriteLog_Error("GetFtcWISEDeviceDetail Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCameraInfoList.Add(oApiReturnMessage);
            }
            return mCameraInfoList;
        }

        [HttpPost]
        //新增攝影機設備api
        public List<dynamic> AddFtcWISEDeviceDetail(oFtcWISEDeviceModel o)
        {
            List<dynamic> mCameraInfoList = new List<dynamic>();
            FtcWISEDeviceDal oDal = null;
            FtcWISEDevice oComm = o.RULEs;
            try
            {
                oDal = new FtcWISEDeviceDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                if (!oDal.AddFtcWISEDevice(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("AddFtcWISEDeviceDetail Error" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCameraInfoList.Add(oApiReturnMessage);
            }
            return mCameraInfoList;
        }

        [HttpPost]
        //刪除攝影機設備api
        public List<dynamic> DelFtcWISEDeviceDetail(oFtcWISEDeviceModel o)
        {
            List<dynamic> mCameraInfoList = new List<dynamic>();
            FtcWISEDeviceDal oDal = null;
            FtcWISEDevice oComm = o.RULEs;
            try
            {
                oDal = new FtcWISEDeviceDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                if (!oDal.DelFtcWISEDevice(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("DelFtcWISEDeviceDetail Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCameraInfoList.Add(oApiReturnMessage);
            }
            return mCameraInfoList;
        }

        [HttpPost]
        //更新攝影機設備api
        public List<dynamic> UpdFtcWISEDeviceDetail(oFtcWISEDeviceModel o)
        {
            List<dynamic> mCameraInfoList = new List<dynamic>();
            FtcWISEDeviceDal oDal = null;
            FtcWISEDevice oComm = o.RULEs;
            try
            {
                oDal = new FtcWISEDeviceDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                //回傳false
                if (!oDal.UpdFtcWISEDevice(oComm))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }
            }
            catch (Exception ex)
            {
                WriteLog_Error("UpdFtcWISEDeviceDetail Error = " + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mCameraInfoList.Add(oApiReturnMessage);
            }
            return mCameraInfoList;
        }

        [HttpPost]
        //查詢異常事件通知紀錄api
        public List<dynamic> GetEventNotifyTransDetail(oEventNotifyTransModel o)
        {
            List<dynamic> mEventNotifyTransList = new List<dynamic>();
            EventNotifyTransDal oDal = null;
            try
            {
                oDal = new EventNotifyTransDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                
                mEventNotifyTransList.Add(oDal.GetEventNotifyTrans(o.sEventBaseSysIdList, o.sStartDate, o.sEndDate));

            }
            catch (Exception ex)
            {
                WriteLog_Error("GetEventNotifyTransDetail Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mEventNotifyTransList.Add(oApiReturnMessage);
            }
            return mEventNotifyTransList;
        }

        [HttpPost]
        //異常事件通知紀錄發送通知變更api
        public List<dynamic> UpdEventNotifyTransDetail(oEventNotifyTransModel o)
        {
            List<dynamic> mEventNotifyTransList = new List<dynamic>();
            EventNotifyTransDal oDal = null;
            EventNotifyTransactions oENT = o.RULEs;
            try
            {
                oDal = new EventNotifyTransDal(o.MODIFYUSER, o.CompanyID, o.FactoryID);
                //回傳false
                if (!oDal.UpdEventNotifyTrans(oENT))
                {
                    oApiReturnMessage.ReturnCode = (int)oDal.ReturnCode;
                    oApiReturnMessage.ReturnMessage = oDal.ReturnMessage;
                }            

            }
            catch (Exception ex)
            {
                WriteLog_Error("GetEventNotifyTransDetail Error=" + ex.ToString());
                oApiReturnMessage.ReturnCode = (int)ReturnCode.Other;
                oApiReturnMessage.ReturnMessage = ex.ToString();
            }
            finally
            {
                if (oDal != null) { oDal.Dispose(); }
                mEventNotifyTransList.Add(oApiReturnMessage);
            }
            return mEventNotifyTransList;
        }
    }
}