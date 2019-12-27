using ClubScansub.App_Data.ScansubModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace ClubScansub.App_Data
{
    public class OdbcRepository
    {
        private OdbcConnection connection;
        public OdbcRepository()
        {
            var dbq = "~/App_Data/scanmdl.mdb";
            //Driver={SQL Server Native Client 11.0};Server=myServerAddress; Database = myDataBase; Uid = myUsername; Pwd = myPassword;
            var connectionString = $"Driver={{SQL Server Native Client 11.0}};Server=myServerAddress; Database = myDataBase;";

            connection = new OdbcConnection(connectionString);
        }

        public List<medlemsdata> Get()
        {

            //Construct the query
            //Usually you use @parameter as the syntax for querying SQL Server
            //But for querying to Microsoft Access you must use ?
            var queryText = "SELECT * FROM medlemsdata";

            //Use dapper to query with parameter
            //It's also a good idea if you use a string as a parameter that you use DbString instead of sending the variable directly
            //You will also need to specify the Length exactly as the length of the column in the Microsoft Access table
            var data = connection.Query<medlemsdata>(queryText);

            return data.ToList();
        }

        public List<saldooplysning> Get(string id)
        {
            var queryText = "SELECT * from saldooplysning";
            var data = connection.Query<saldooplysning>(queryText).ToList();

            return data.Where(x => x.dsfnr == id).ToList();
        }

        public List<saldooplysning>Get(string id, string password)
        {
            //Construct the query
            //Usually you use @parameter as the syntax for querying SQL Server
            //But for querying to Microsoft Access you must use ?
            var queryText = $"SELECT * FROM medlemsdata where id='{id}'";

            //Use dapper to query with parameter
            //It's also a good idea if you use a string as a parameter that you use DbString instead of sending the variable directly
            //You will also need to specify the Length exactly as the length of the column in the Microsoft Access table
            var data = connection.Query<medlemsdata>(queryText).FirstOrDefault();
            if (data != null)
            {
                if (data.password == password)
                {
                    var saldooplysningQueryText = $"SELECT * from saldooplysning where dsfnr = {id}";
                    var saldoData = connection.Query<saldooplysning>(saldooplysningQueryText);

                    return saldoData.ToList();
                }
            }

            return null;
            
        }
    }
}