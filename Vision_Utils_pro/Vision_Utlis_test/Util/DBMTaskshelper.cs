﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Threading;
using System.Reflection;


namespace GH_Utlis
{
    public class dbTaskHelper
    {
        DbHelper db;
        public  string constring = "Data Source=127.0.0.1;Initial Catalog=temprobot_datarecord;User ID=sa; Password=6923263;Enlist=true;Pooling=true;Max Pool Size=300;Min Pool Size=0;Connection Lifetime=300;packet size=1000";
        private static object obj = new object();
        public dbTaskHelper()
        {
            db = new DbHelper(constring);
        }
        
        public int MultithreadExecuteNonQuery(string sql)
        {
            lock (obj)
            {
                return db.ExecuteNonQuery(db.GetSqlStringCommond(sql));
            }
        }
        public DbDataReader MultithreadDataReader(string sql)
        {
            lock (obj)
            {
                return db.ExecuteReader(db.GetSqlStringCommond(sql));
            }
        }
        public DataTable MultithreadDataTable(string sql)
        {
            lock (obj)
            {
                return db.ExecuteDataTable(db.GetSqlStringCommond(sql));
            }
        }

        public DataTable MultithreadDataTable_Prc(string PrcName)
        {
            lock (obj)
            {
                return db.ExecuteDataTable(db.GetStoredProcCommond(PrcName));
            }
        }

        public object MultithreadGetTimeSpace()
        {
            lock (obj)
            {
                return db.ExecuteScalar(db.GetSqlStringCommond("SELECT top(1) [OPCDataAcquisition_UpdataRate]/1000 FROM [OPCAcquisitionConfig]"));
            }
        }


        public int updata_table (string tablename,string fieldname,string fielvalue,string keyname,string keyvalue)
        {
            lock (obj)
            {
                string sql = string.Format("UPDATE {0} SET {1}='{2}' WHERE {3}='{4}'", tablename, fieldname, fielvalue, keyname, keyvalue);
                return this.MultithreadExecuteNonQuery(sql);
            }
        }
        public string read_table_onefield(string fieldname,string tablename,string keyname, string keyvalue)
        {
            lock(obj)
            {

            
                string sql = string.Format("SELECT {0} from {1} WHERE {2}='{3}'",  fieldname,tablename,  keyname, keyvalue);
                DbDataReader dr = this.MultithreadDataReader(sql);
                string result="";
                while (dr.Read())
                {
                    if (dr[fieldname] != DBNull.Value)
                        result = Convert.ToString(dr[fieldname]);
                    else
                        result = "";
                }
                dr.Close();
                return result;
            }
        }
        public void inser_log(string tablename,string str)
        {
            lock(obj)
            {
                string sql = string.Format("INSERT INTO {0} (time,logtext) VALUES ('{1}','{2}')", tablename, DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss")), str);
                this.MultithreadExecuteNonQuery(sql);
            }
            
        }
        public void close_conn()
        {
            db.connection.Close();
        }
      

    }
}
/*
    string sql = string.Format("UPDATE TLabelContent SET REC_CREATE_TIME='{0}',IMP_FINISH={1} WHERE REC_ID={2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Program.MessageFlg, REC_ID);
    tm.MultithreadExecuteNonQuery(sql);
    string str = "收到PLC焊接完成信号"+Program.MessageFlg.ToString() + " " + merge_sn;
    sql = string.Format("INSERT INTO RECVLOG(REC_CREATE_TIME,CONTENT) VALUES ('{0}','{1}')", DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss")), str);
    tm.MultithreadExecuteNonQuery(sql);
    
    string sql = "SELECT ACQUISITIONCONFIG_ID,DATAACQUISITION_IP,DATAACQUISITION_PORTS,DATAACQUISITION_PORTR FROM ACQUISITIONCONFIG where ACQUISITIONCONFIG_ID=8 or ACQUISITIONCONFIG_ID=18";
    DbDataReader dr = null;
    dr = tm.MultithreadDataReader(sql);
    while (dr.Read())
    {
        if (Convert.ToInt16(dr["ACQUISITIONCONFIG_ID"]) == 8)
        {
            MESIP = Convert.ToString(dr["DATAACQUISITION_IP"]);
            mesportr = Convert.ToInt32(dr["DATAACQUISITION_PORTR"]);
        }
        if (Convert.ToInt16(dr["ACQUISITIONCONFIG_ID"]) == 18)
        {
            localip = Convert.ToString(dr["DATAACQUISITION_IP"]);
            localportr = Convert.ToInt32(dr["DATAACQUISITION_PORTS"]);
        }
    }
    dr.Close();

    //////断开式读取表数据
    sql = string.Format("select top 1 ID_LOT_PROD,ID_PART_LOT,NUM_BDL,SEQ_LEN,SEQ_OPR,DES_FIPRO_SECTION,NAME_PROD,BAR_CODE from TLabelContent WHERE REC_ID>{0} AND IMP_FINISH=0 order by REC_ID ASC", MAXRECID);
    string ID_LOT_PROD = "";    
    Int16 ID_PART_LOT = 0;   
    Int16 NUM_BDL = 0;
    Int16 SEQ_LEN= 0;
    Int16 SEQ_OPR = 0;
    string DES_FIPRO_SECTION = "";
    string BAR_CODE = "", NAME_PROD = "";
    DataTable dt = tm.MultithreadDataTable(sql);
    for (int i = 0; i<dt.Rows.Count; i++)
    {
        ID_LOT_PROD = dt.Rows[i]["ID_LOT_PROD"].ToString();
        ID_PART_LOT = Int16.Parse(dt.Rows[i]["ID_PART_LOT"].ToString());
        NUM_BDL = Int16.Parse(dt.Rows[i]["NUM_BDL"].ToString());
        SEQ_LEN = Int16.Parse(dt.Rows[i]["SEQ_LEN"].ToString());
        SEQ_OPR = Int16.Parse(dt.Rows[i]["SEQ_OPR"].ToString());
        DES_FIPRO_SECTION = dt.Rows[i]["DES_FIPRO_SECTION"].ToString();
        NAME_PROD= dt.Rows[i]["NAME_PROD"].ToString();
        BAR_CODE = dt.Rows[i]["BAR_CODE"].ToString();
                      
    }
    string str = "发送到PLC信号:" + logindexstr + Program.MessageFlg.ToString() + " " + PLClable.merge_sinbar;
    sql = string.Format("INSERT INTO SENDLOG(REC_CREATE_TIME,CONTENT) VALUES ('{0}','{1}')", DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss")), str);
    tm.MultithreadExecuteNonQuery(sql);
 */
