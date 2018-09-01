using System;
using System.Collections.Generic;
using System.Text;
using iKCoderSDK;
using System.Data;
using System.Xml;
using System.Threading;

namespace CoreM.Modules
{
    public class M_CheckStore
    {
        class_Data_SqlConnectionHelper db_objectConnectionHelper = new class_Data_SqlConnectionHelper();
        Data_dbSqlHelper db_objectSqlHelper = new Data_dbSqlHelper();
        Dictionary<String, class_Data_SqlSPEntry> Map_SPS = new Dictionary<string, class_Data_SqlSPEntry>();
        Thread currentThread;

        public void Init(Basic_Config refConfigObject)
        {
            XmlNode sessionNode = refConfigObject.GetSessionNode("coreMDB");
            string server = refConfigObject.GetAttrValue(sessionNode, "server");
            string uid = refConfigObject.GetAttrValue(sessionNode, "uid");
            string pwd = refConfigObject.GetAttrValue(sessionNode, "pwd");
            string db = refConfigObject.GetAttrValue(sessionNode, "db");
            ConsoleMessageItem newMessageItem = new ConsoleMessageItem();
            newMessageItem.Message = "[Connect to store database]";
            newMessageItem.Result = true;
            Program.obj_message.set_newMessage(newMessageItem);
            db_objectConnectionHelper.Set_NewConnectionItem(Program.key_db_ikcoder_store, server, uid, pwd, db, enum_DatabaseType.MySql);
            newMessageItem.Message = "[Loading SPS for store]";
            newMessageItem.Result = true;
            Program.obj_message.set_newMessage(newMessageItem);
            Map_SPS = db_objectSqlHelper.ActionAutoLoadingAllSPS(db_objectConnectionHelper.Get_ActiveConnection(Program.key_db_ikcoder_store), "");
            
        }


        public void Start()
        {
            currentThread = new Thread(new ParameterizedThreadStart(Processor_CheckStatus));
            currentThread.Start();
        }

        public void Stop()
        {
            currentThread.Suspend();
        }

        public void Processor_CheckStatus(Object param)
        {
            class_Data_SqlSPEntry activeSP = Map_SPS["spa_operation_db_reg"];
            while(true)
            {
                DataTable regTable = db_objectSqlHelper.ExecuteSelectSPForDT(activeSP, db_objectConnectionHelper, Program.key_db_ikcoder_store);
                foreach(DataRow activeDR in regTable.Rows)
                {
                    string id = string.Empty;
                    string dbkey = string.Empty;
                    string dbserver = string.Empty;
                    string dbuid = string.Empty;
                    string dbpwd = string.Empty;
                    string dbdatabase = string.Empty;
                    string tindex = string.Empty;
                    string storetable = string.Empty;
                    string limitedrows = string.Empty;
                    string rows = string.Empty;
                    string online = string.Empty;
                    Data_dbDataHelper.GetColumnData(activeDR, "id", out id);
                    Data_dbDataHelper.GetColumnData(activeDR, "dbkey", out dbkey);
                    Data_dbDataHelper.GetColumnData(activeDR, "dbserver", out dbserver);
                    Data_dbDataHelper.GetColumnData(activeDR, "dbuid", out dbuid);
                    Data_dbDataHelper.GetColumnData(activeDR, "dbpwd", out dbpwd);
                    Data_dbDataHelper.GetColumnData(activeDR, "dbdatabase", out dbdatabase);
                    Data_dbDataHelper.GetColumnData(activeDR, "storetable", out storetable);
                    Data_dbDataHelper.GetColumnData(activeDR, "limitedrows", out limitedrows);
                    Data_dbDataHelper.GetColumnData(activeDR, "rows", out rows);
                    Data_dbDataHelper.GetColumnData(activeDR, "online", out online);
                    if (db_objectConnectionHelper.Get_ExistedConnection(dbkey))
                        db_objectConnectionHelper.Set_RemoveExistedConnection(dbkey);
                    ConsoleMessageItem CMI = new ConsoleMessageItem();
                    CMI.Message = "Try:@->Connect to server:" + dbserver;
                    Program.obj_message.set_newMessage(CMI);
                    if (db_objectConnectionHelper.Set_NewConnectionItem(dbkey, dbserver, dbuid, dbpwd, dbdatabase, enum_DatabaseType.MySql))
                    {
                        CMI.Message = "Result:@->Faild to connect to;" + dbserver;
                        Program.obj_message.set_newMessage(CMI);
                        online = "0";

                    }
                    else
                    {
                        CMI.Message = "Result:@->Connected to:" + dbserver;
                        Program.obj_message.set_newMessage(CMI);
                        online = "1";
                        string tmpSql = "select count(*) as rows from " + storetable;
                        DataTable dtCount = null;
                        Data_dbDataHelper.ActionExecuteSQLForDT(db_objectConnectionHelper.Get_ActiveConnection(dbkey), tmpSql, out dtCount);
                        if (dtCount != null && dtCount.Rows.Count > 0)
                        {
                            string tmpRows = string.Empty;
                            Data_dbDataHelper.GetColumnData(dtCount.Rows[0], "rows", out tmpRows);
                            if (tmpRows != rows)
                            {
                                rows = tmpRows;
                            }
                        }                        
                        activeSP.ModifyParameterValue("id", id);
                        activeSP.ModifyParameterValue("rows", rows);
                        activeSP.ModifyParameterValue("online", online);
                        db_objectSqlHelper.ExecuteUpdateSP(activeSP, db_objectConnectionHelper, dbkey);
                        CMI.Message = "Result:@->Update reg information:" + dbserver;
                        Program.obj_message.set_newMessage(CMI);
                    }
                }
                Thread.Sleep(1000 * 30);
            }
        }



    }
}
