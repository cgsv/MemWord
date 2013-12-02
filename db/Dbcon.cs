using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;


namespace MemWord.db
{
    class Dbcon
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader reader;
        public OleDbConnection getCon(){
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=d:\\myfiles\\document\\visual studio 2010\\Projects\\MemWord\\MemWord\\data\\vocabu.mdb;";
            con = new OleDbConnection(strConn);
            con.Open();
            return con;
        }

        public voca getVocaByID(int id){
            voca vo = new voca();
            string sql="SELECT ID, words, meaning, lx FROM cetsix where ID="+id;
            this.getCon();
            cmd = new OleDbCommand(sql, con);
  //          con.Open();
            if (cmd.ExecuteScalar()==null)
            {
                vo.id = 0; vo.meaning = ""; vo.example = ""; vo.word = "";
                return vo;
            }
            reader = cmd.ExecuteReader();
            
            reader.Read();


                vo.id = reader.GetDouble(0);
                vo.word = reader.GetString(1);

                vo.meaning = reader.GetString(2);
            
            if (reader.IsDBNull(3))
            {
                vo.example = "";
            }
            else
            {
                vo.example = reader.GetString(3);
            }
   //         this.closeAll();
   //         reader.Close();
            return vo;
        }

        public double getIDByFirstLetter(string letter){
            this.getCon();
            string sq = "select ID from cetsix where words like '" + letter + "%' ORDER BY words";
            cmd = new OleDbCommand(sq, con);
            if (cmd.ExecuteScalar()==null)
            {
                return 0;
            }
            reader = cmd.ExecuteReader();
            reader.Read();

            return reader.GetDouble(0);
        }

        public void closeAll(){
            if (!reader.IsClosed)
            {
                reader.Close();
            }
            if (con.State==ConnectionState.Open)
            {
                con.Close();
            }
        }

        
    }
}
