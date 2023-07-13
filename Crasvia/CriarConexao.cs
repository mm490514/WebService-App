using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Crasvia
{
    public class CriarConexao
    {
        public static SqlConnection Conexao(String baseName)
        {
            SqlConnection conexao = new SqlConnection();

            String strBanco = "Data Source=http://monitoracao.database.windows.net;Initial Catalog=" + baseName + ";Persist Security Info=True;" +
                "User ID=prj;Password=monit@123;MultipleActiveResultSets=true;Min Pool Size=10;Max Pool Size=250; Connect Timeout=30";

            conexao.ConnectionString = strBanco.ToString();
            conexao.Open();

            return conexao;
        }
    }
}