using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MySqlConnector;

namespace testGameSlayer
{
    public class xUnitTest
    {
        [Fact]
        public void thisTestShouldPass()
        {
            Assert.False(false, "This should pass");
        }
        [Fact]
        public void thistestShouldFail()
        {
            Assert.False(true, "This should fail");
        }
        [Fact]
        public void testAnIntegerIs42()
        {
            /*
             Arrange
                Setup test enviroment
            */
            int expected = 42;

            /*
             Act
                Perform the act to be tested
                Gather the actual answer
            */
            int actual = 6 * 7;

            /*
             Assert
                check that expected is actual
             */
            Assert.Equal(expected, actual);
        }
        [Fact(Skip = "This should be skipped")]
        public void thisTestShouldBeSkipped()
        {
            Assert.False(false, "This should not appear");
        }
    }
    public class dBConnectionTest
    {

        private const string connectionString = "Server=videogamegrade.mysql.database.azure.com;Uid=gamegradeadmin;Pwd=capstone2024!;SslMode=Required;";
        private readonly IDbConnection connection;

        public dBConnectionTest()
        {
            connection = new MySqlConnection(connectionString);
        }

        [Fact]
        public void doesItConnect()
        {
            connection.Open();

            var results = connection.Query("SELECT * FROM videogamegrade_db.test_table");

            //Somthing for a later time
            //foreach (var row in results) 
            //{
            //    Console.WriteLine(row[0]);
            //}

            connection.Dispose();
        }
    }
}