using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;

namespace AdoNetTask1
{
    public class Task1
    {

        public static string dbloc = Directory.GetCurrentDirectory() + @"\test.sqlite";
        public static void Main(string[] args)
        {
            if (!File.Exists(dbloc)) {
                FileStream fs = File.Create(dbloc);
                fs.Dispose();
            }
            SqliteConnection conn = new SqliteConnection("Data Source = " + dbloc);

            SqliteCommand comm = conn.CreateCommand();
            conn.Open();
            comm.CommandText = @"CREATE TABLE IF NOT EXISTS [companies] (
                    [id] integer PRIMARY KEY AUTOINCREMENT NOT NULL,
                    [Title] string NOT NULL,
                    [Country] string NOT NULL,
                    [AddedDate] Date NOT NULL                  
                    );
                  
                    ";
            comm.ExecuteNonQuery();

           //
            SqliteTransaction st2 = conn.BeginTransaction();
            try
            {
                SqliteCommand comm2 = conn.CreateCommand();
                comm2.CommandText = @"INSERT INTO companies(Title,Country,AddedDate) VALUES 
                           (@title, @country, @date);";
                List<Company> companies = new List<Company>();
                companies.Add(new Company("Roshen", "Ukraine", "10.10.2010"));
                companies.Add(new Company("Sandora", "Ukraine", "11.09.2011"));
                companies.Add(new Company("Svitoch","Ukraine","12.08.2012"));
                companies.Add(new Company("Rosinka","Ukraine","13.07.2013"));
                companies.Add(new Company("Korona","Ukraine","14.06.2014"));
                companies.Add(new Company("Mircrosoft","USA","10.10.2009"));
                companies.Add(new Company("Google","USA","10.10.2008"));
                companies.Add(new Company("Facebook","USA","10.10.2007"));
                companies.Add(new Company("Air France","France","10.10.2006"));
                companies.Add(new Company("Koenisegg","Sweden","10.10.2005"));

                comm2.Parameters.Add(new SqliteParameter("@title", SqliteType.Text));
                comm2.Parameters.Add(new SqliteParameter("@country", SqliteType.Text));
                comm2.Parameters.Add(new SqliteParameter("@date", SqliteType.Text));
                
                    foreach (Company comp in companies)
                    {
                        comm2.Parameters[0].Value = comp.Title;
                        comm2.Parameters[1].Value = comp.Country;
                        comm2.Parameters[2].Value = comp.AddedDate;
                        if (comm2.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }
                    }
                    st2.Commit();
                }
                catch (Exception ex)
                {
                    st2.Rollback();
                }

            //
                SqliteCommand comm3 = conn.CreateCommand();
                comm3.CommandText = @"SELECT MAX(id),Title FROM companies";
                var reader = comm3.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("{0}|{1}", reader.GetInt32(0),
                        reader.GetString(1));
                }
                Console.WriteLine("------------------");

            //
                SqliteCommand comm4 = conn.CreateCommand();
                comm4.CommandText = @"
            UPDATE companies
            SET Country= @country1
            WHERE Country= @country2;
            ";
                var country1Param = new SqliteParameter();
                country1Param.ParameterName = "@country1";
                country1Param.Value = "USA";
                var country2Param = new SqliteParameter();
                country2Param.ParameterName = "@country2";
                country2Param.Value = "Ukraine";
                comm4.Parameters.Add(country1Param);
                comm4.Parameters.Add(country2Param);
                comm4.ExecuteNonQuery();
            //
                SqliteCommand comm5 = conn.CreateCommand();
                comm5.CommandText = @"
            DELETE FROM companies
            WHERE Country<>@country1;
            ";
            comm5.Parameters.Add(country1Param);
            comm5.ExecuteNonQuery();
            //    


                SqliteCommand comm6 = conn.CreateCommand();
                comm6.CommandText = @"
            SELECT COUNT(*) FROM companies
            ";
                Console.WriteLine(comm6.ExecuteScalar());
                Console.WriteLine("------------------");

            //
                SqliteCommand comm7 = conn.CreateCommand();
                comm7.CommandText = @"SELECT * FROM companies";
                reader = comm7.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("{0}|{1}|{2}|{3}", reader.GetInt32(0),
                        reader.GetString(1), reader.GetString(2), reader.GetString(3));
                }
                Console.WriteLine("------------------");

            //
                Console.WriteLine(@"input example:{'Title':'title','Country':'country','AddedDate':'20.10.2015'}");
                Console.WriteLine("------------------");
            List<Company> companiesToAdd = new List<Company>();
            string str;
                while ((str = Console.ReadLine()) != "q")
                {
                    if (str.StartsWith("{"))
                    {
                    Company obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Company>(str);
                    companiesToAdd.Add(obj);
                    }
                }

            if (companiesToAdd.Count > 0)
            {
                SqliteTransaction st = conn.BeginTransaction();
                try
                {

                    SqliteCommand comm8 = conn.CreateCommand();
                    comm8.CommandText = @"INSERT INTO companies(Title,Country,AddedDate) VALUES 
                           (@title, @country, @date);";
                    comm8.Parameters.Add(new SqliteParameter("@title", SqliteType.Text));
                    comm8.Parameters.Add(new SqliteParameter("@country", SqliteType.Text));
                    comm8.Parameters.Add(new SqliteParameter("@date", SqliteType.Text));

                    foreach (Company comp in companiesToAdd)
                    {
                        comm8.Parameters[0].Value = comp.Title;
                        comm8.Parameters[1].Value = comp.Country;
                        comm8.Parameters[2].Value = comp.AddedDate;
                        if (comm8.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }
                    }
                    st.Commit();
                }
                catch (Exception ex)
                {
                    st.Rollback();
                }
            }

            conn.Close();

            }
    }

    class Company
    {
        public string Title;
        public string Country;
        public string AddedDate;


        public Company(string v1, string v2, string v3)
        {
            this.Title = v1;
            this.Country = v2;
            this.AddedDate = v3;
        }
    }

}
    

