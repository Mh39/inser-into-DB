using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Data;

namespace LastEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            //Done 7/12/2022
            //Declare Variable and set values
            //provide input folder
           // string paths = @"E:\tit\person.txt";
            string SourceFolder = @"E:\tit\";
            //provide file name
            string SourceFileName = @"MK_07122022.g";
            //provide the table name in which you would like to load data
            //string TableName = "dbo.PersonData";
            //provide the file delimiter such as comma,pipe
            //string filedelimiter = ",";
            //provide the Archive folder where you would like to move file
            //string ArchiveFodler = @"F:\";
            static string CalculateMD5(string SourceFileName)
            {
                using (var md5 = MD5.Create())
                {
                    string SourceFolder = @"E:\tit\";
                    using (var stream = File.OpenRead(SourceFolder+ SourceFileName))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            var md5Hash = CalculateMD5(SourceFileName);

            try
            {
                //Create Connection to SQL Server
                SqlConnection conn = new SqlConnection(@"Data Source =LAPTOP-JFIAGS6G\SQLEXPRESS; Initial Catalog =TiT; Integrated Security=true;");

                StreamReader SourceFile = new StreamReader(SourceFolder + SourceFileName);

                string line = "";
                Int32 counter = 0;


                //// CalculateMD5(SourceFileName);
                // string hash = "select md5Hash from PersonData where 1=1 ";
                //SqlDataAdapter sa = new SqlDataAdapter() ; 
                //SqlCommand cmd = new SqlCommand("select md5Hash from PersonData where 1=1 ", conn);
                //SqlDataReader dr= cmd.ExecuteReader(); 
                //while (dr.Read())
                //{

                //}
                SqlDataAdapter da = new SqlDataAdapter("select md5Hash from PersonData where 1=1 ", conn);
                DataTable dt= new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0 )
                {
                    Console.WriteLine("File is exist");
                }
                else
                {
                    conn.Open();
                    while ((line = SourceFile.ReadLine()) != null)
                    {
                        //skip the header row
                        if (counter > 0)
                        {
                            //string hash = "select md5Hash from PersonData where 1=1 ";
                            // SqlCommand myCommand = new SqlCommand(hash, conn);


                            //prepare insert query
                            string query = "Insert into PersonData (FileName, FileData,md5Hash) Values (@sourcefilename,@line,@md5Hash)";

                            //execute sqlcommand to insert record
                            SqlCommand myCommand = new SqlCommand(query, conn);
                            myCommand.Parameters.AddWithValue("@sourcefilename", SourceFileName);
                            myCommand.Parameters.AddWithValue("@line", line);
                            myCommand.Parameters.AddWithValue("@md5Hash", md5Hash);

                            myCommand.ExecuteNonQuery();
                        }
                        counter++;
                    }

                    SourceFile.Close();
                    conn.Close();
                }

                //Move the file to Archive folder
                //File.Move(SourceFolder + SourceFileName, ArchiveFodler + SourceFileName);

            }
            catch (IOException Exception)
            {
                Console.Write(Exception);
            }

        }

    }
}
