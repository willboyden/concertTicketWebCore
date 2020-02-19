using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace concertTicketWebCoreMVC.App_Code
{   
    public class dbConnector
    {
        private string constr = ConfigurationManager.ConnectionStrings["StubHubCityContext"].ConnectionString;


        public string getQueryResultAsJsonString(string sqlstring)
        {
            string resultStr="";
            StringBuilder sb = new StringBuilder();
            using (var con = new SqlConnection(constr))
            {
                con.Open();
                using (var cmd = new SqlCommand(sqlstring, con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        //remove single quotes from returned json "string" and add comma between objects in the array array 
                        sb.Append(reader[0].ToString());
                        
                    }
                }
                resultStr = sb.ToString();//comment out and directly return sb.ToString()
                return resultStr;
            }
        }



    }
}
