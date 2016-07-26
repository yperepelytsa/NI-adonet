using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace AdoNetTask2
{
    public class Task2
    {
        public static string dbloc = Directory.GetCurrentDirectory() + @"\northwind.db";
           public static void Main(string[] args)
            {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            SqliteConnection conn = new SqliteConnection("Data Source = " + dbloc);
            conn.Open();

            Console.WriteLine("Select all customers whose name (CompanyName) starts with letter “D”)");
            Console.WriteLine("------------------");
            SqliteCommand comm = conn.CreateCommand();
            comm.CommandText = @"SELECT CustomerID,CompanyName FROM customers Where CompanyName LIKE 'D%';";
            var reader = comm.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}|{1}", reader.GetString(0),
                    reader.GetString(1));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Convert names (CompanyNames) of all customers to Upper Case;");
            Console.WriteLine("------------------");
            SqliteCommand comm2 = conn.CreateCommand();
            comm2.CommandText = @"SELECT UPPER(CompanyName) FROM customers;";
            reader = comm2.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select distinct country from Customers");
            Console.WriteLine("------------------");
            SqliteCommand comm3 = conn.CreateCommand();
            comm3.CommandText = @"SELECT DISTINCT Country FROM customers;";
            reader = comm3.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select Contact name from Customers Table from London and title like 'Sales'");
            Console.WriteLine("------------------");
            SqliteCommand comm4 = conn.CreateCommand();
            comm4.CommandText = @"SELECT ContactName FROM customers Where City='London' AND ContactTitle LIKE '%Sales%';";
            reader = comm4.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select all orders id where was bought 'Tofu'");
            Console.WriteLine("------------------");
            SqliteCommand comm5 = conn.CreateCommand();
            comm5.CommandText = @"Select OrderID From 'Order Details' Where ProductID IN (SELECT ProductID FROM Products Where ProductName='Tofu');";
            reader = comm5.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select all product names that were shipped to Germany");
            Console.WriteLine("------------------");
            SqliteCommand comm6 = conn.CreateCommand();
            comm6.CommandText = @"SELECT DISTINCT ProductName FROM Products Where ProductID IN (
                                  SELECT ProductID FROM 'Order Details' Where OrderID IN (
                                  SELECT OrderID FROM Orders Where ShipCountry='Germany'));";
            reader = comm6.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select all customers that ordered 'Ikura'");
            Console.WriteLine("------------------");
            SqliteCommand comm7 = conn.CreateCommand();
            comm7.CommandText = @"Select CustomerID,CompanyName From Customers WHERE CustomerID IN (
                                  Select CustomerID From Orders WHERE OrderID IN (
                                  Select OrderID From 'Order Details' Where ProductID IN (
                                  Select ProductID FROM Products Where ProductName='Ikura' )));";
            reader = comm7.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0},{1}", reader.GetString(0),reader.GetString(1));
            }
            Console.WriteLine("------------------");

           

            Console.WriteLine("Select all phones from Shippers and Suppliers");
            Console.WriteLine("------------------");
            SqliteCommand comm10 = conn.CreateCommand();
            comm10.CommandText = @"SELECT Phone FROM Shippers
                                   UNION
                                   SELECT Phone FROM Suppliers;";
            reader = comm10.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Count all customers grouped by city");
            Console.WriteLine("------------------");
            SqliteCommand comm11 = conn.CreateCommand();
            comm11.CommandText = @"SELECT City,Count(CustomerID) FROM Customers GROUP By City;";
            reader = comm11.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0},{1}", reader.GetString(0), reader.GetString(1));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select all customers that placed more than 10 orders with average Unit Price less than 17");
            Console.WriteLine("------------------");
            SqliteCommand comm12 = conn.CreateCommand();
            comm12.CommandText = @"SELECT Customers.CompanyName, COUNT(selord.OrderID) AS NumberOfOrders FROM (
                                   SELECT AVG(UnitPrice) as AVR, Orders.OrderID,Customers.CustomerID AS CustomerID FROM 'Order Details'
                                   INNER JOIN Orders ON 'Order Details'.OrderID=Orders.OrderID
                                   INNER JOIN Customers ON Orders.CustomerID=Customers.CustomerID
                                   Group BY Orders.OrderID 
                                   HAVING AVR>17) AS selord
                                   INNER JOIN Customers
                                   ON selord.CustomerID=Customers.CustomerID
                                   GROUP BY CompanyName
                                   HAVING COUNT(selord.OrderID) > 10;";
            reader = comm12.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0},{1}", reader.GetString(0), reader.GetString(1));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select all customers with phone that has format (’NNNN-NNNN’)");
            Console.WriteLine("------------------");
            SqliteCommand comm13 = conn.CreateCommand();
            comm13.CommandText = @"SELECT ContactName,Phone FROM Customers WHERE Phone LIKE '____-____';";
            reader = comm13.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0},{1}", reader.GetString(0), reader.GetString(1));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select customer that ordered the greatest amount of goods (not price)");
            Console.WriteLine("------------------");
            SqliteCommand comm14 = conn.CreateCommand();
            comm14.CommandText = @"SELECT ContactName,MAX(sum1) as Quantity FROM(
                                   SELECT ContactName, Sum(Quantity) AS sum1 From Orders 
                                   INNER JOIN Customers ON Orders.CustomerID=Customers.CustomerID
                                   Inner JOIN 'Order Details' ON 'Order Details'.OrderID=Orders.OrderID
                                   GROUP BY ContactName);";
            reader = comm14.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0},{1}", reader.GetString(0), reader.GetString(1));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Select only these customers that ordered the absolutely the same products as customer “FAMIA”");
            Console.WriteLine("this returns the customer used as parameter as well, to remove this you can add AND CustomerID<>'FAMIA' at the end of query");
            Console.WriteLine("------------------");
            SqliteCommand comm15 = conn.CreateCommand();
            comm15.CommandText = @"SELECT CustomerID 
FROM Customers c1
WHERE NOT EXISTS
(
SELECT  od1.ProductID 
FROM 'Order Details' od1
Inner Join Orders ord1 ON od1.OrderID=ord1.OrderID
WHERE ord1.CustomerID='FAMIA' AND od1.ProductID NOT IN (

SELECT  ProductID 
FROM 'Order Details' od2
Inner Join Orders ord2 ON od2.OrderID=ord2.OrderID
Inner Join Customers c2 ON ord2.CustomerID=c2.CustomerID
WHERE c2.CustomerID=c1.CustomerID
)
)
AND NOT EXISTS(

	SELECT od3.ProductID
    FROM 'Order Details' od3
Inner Join Orders ord3 ON od3.OrderID=ord3.OrderID
Inner Join Customers c3 ON ord3.CustomerID=c3.CustomerID	

	WHERE   c3.CustomerID=c1.CustomerID AND od3.ProductID NOT IN (
							SELECT  od4.ProductID 
							FROM 'Order Details' od4
							Inner Join Orders ord4 ON od4.OrderID=ord4.OrderID
							WHERE ord4.CustomerID='FAMIA'
							   )
)";
            reader = comm15.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("{0}", reader.GetString(0));
            }
            Console.WriteLine("------------------");

            Console.WriteLine("Outputs from tasks 8 and 9 are very long, enter 8 or 9 to view (q to quit)");
            Console.WriteLine("Task 9 implements outer join (not available in sqlite) by union");
            string str;
            while ((str = Console.ReadLine()) != "q")
            {
                if (str.StartsWith("8"))
                {
                    Console.WriteLine("Select all employees and any orders they might have");
                    Console.WriteLine("------------------");
                    SqliteCommand comm8 = conn.CreateCommand();
                    comm8.CommandText = @"SELECT Employees.EmployeeID,Employees.LastName,Orders.OrderID
                                  FROM Employees
                                  LEFT JOIN Orders
                                  ON Employees.EmployeeID=Orders.EmployeeID
                                  ORDER BY LastName;";
                    reader = comm8.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("{0},{1},{2}", reader.GetString(0), reader.GetString(1), reader.GetString(2));
                    }
                    Console.WriteLine("------------------");
                }
                else if (str.StartsWith("9"))
                {
                    Console.WriteLine("Selects all employees, and all orders");
                    Console.WriteLine("------------------");
                    SqliteCommand comm9 = conn.CreateCommand();
                    comm9.CommandText = @"SELECT Employees.EmployeeID,Employees.LastName,Orders.OrderID
                                  FROM   Employees
                                  LEFT JOIN Orders
                                  ON Employees.EmployeeID = Orders.EmployeeID
                                  UNION ALL
                                  SELECT Employees.EmployeeID,Employees.LastName,Orders.OrderID
                                  FROM   Orders
                                  LEFT JOIN Employees
                                  ON Employees.EmployeeID = Orders.EmployeeID
                                  WHERE  Employees.EmployeeID IS NULL";
                    reader = comm9.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("{0},{1},{2}", reader.GetString(0), reader.GetString(1), reader.GetString(2));
                    }
                    Console.WriteLine("------------------");
                }
            }
            conn.Close();


        }
    }
}
