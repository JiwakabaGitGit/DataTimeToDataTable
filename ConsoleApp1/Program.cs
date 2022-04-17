using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ConsoleApp1
{
    class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine( "Hello World!" );
            string dir = @"どこかの適当なフォルダ";

            DataSet ds = CreateDataSource();
            string file = "withschema.xml";
            ds.WriteXml( dir + file, XmlWriteMode.WriteSchema );

            file = "WithoutSchema.xml";
            ds.WriteXml( dir + file, XmlWriteMode.IgnoreSchema );

            DataSet dataset1 = new DataSet();
            DataSet dataset2 = new DataSet();

            dataset1.ReadXml( dir + "WithSchema.xml" );
            dataset2.ReadXml( dir + "WithoutSchema.xml" );

            Type type1 = dataset1.Tables[ 0 ].Columns[ "Date" ].DataType;
            Type type2 = dataset2.Tables[ 0 ].Columns[ "Date" ].DataType;

            Console.WriteLine( $"Type: {type1}" );
            Console.WriteLine( $"Type: {type2}" );

            // 結果は：
            // Type: System.DateTime
            // Type: System.String

            //DataRow[] datarows = ( from row in dataset1.Tables[ 0 ].AsEnumerable()
            //                       let date = row.Field<string>( "Date" ) let name = row.Field<string>("Name")
            //                       where DateTime.Parse( date ) <= DateTime.Now && DateTime.Parse( date ) >= DateTime.Now.AddDays( -7 ) && name == "Product Name_13"
            //                       select row ).ToArray();


            DataRow[] datarows = ( from row in dataset1.Tables[ 0 ].AsEnumerable()
                                   let date = DateTime.Parse(  row.Field<string>( "Date" ) )
                                   let name = row.Field<string>( "Name" )
                                   where  date <= DateTime.Now &&date >= DateTime.Now.AddDays( -8 )// && name == "Product Name_13"
                                   select row ).ToArray();
            
            List<LogDataSample> logList = new();

            foreach( var row in datarows )
            {
                LogDataSample log = new()
                {
                    ID = row.Field<int>( "ID" ),
                    Name = row.Field<string>( "Name" ),
                    Price = row.Field<decimal>( "Price" ),
                    Discontinued = row.Field<bool>( "Discontinued" ),
                    Date = DateTime.Parse( row.Field<string>( "Date" ) )
                };

                logList.Add( log );
            }


            //foreach ( DataRow row in datarows )
            //{
            //    Console.WriteLine( $"ID: {row[ 0 ]}, Name: {row[ 1 ]}, Price: {row[ 2 ]}, Discontinued: {row[ 3 ]}, Date: {row[ 4 ]}" );
            //}
            foreach( var log in logList )
            {
                Console.WriteLine( $"ID: {log.ID.ToString() }, Name: {log.Name}, Price: {log.Price.ToString()}, Discontinued: {log.Discontinued.ToString()}, Date: {log.Date.ToString()}" );
            }

            // 結果は：
            // ID: 9, Name: Product Name_9, Price: 1230000, Discontinued: False, Date: 2018 / 10 / 21 15:27:08
            // ID: 10, Name: Product Name_10, Price: 1353000, Discontinued: True, Date: 2018 / 10 / 22 15:27:08
            // ID: 11, Name: Product Name_11, Price: 1476000, Discontinued: False, Date: 2018 / 10 / 23 15:27:08
            // ID: 12, Name: Product Name_12, Price: 1599000, Discontinued: True, Date: 2018 / 10 / 24 15:27:08
            // ID: 13, Name: Product Name_13, Price: 1722000, Discontinued: False, Date: 2018 / 10 / 25 15:27:08
            // ID: 14, Name: Product Name_14, Price: 1845000, Discontinued: True, Date: 2018 / 10 / 26 15:27:08
        }

        // データソース用の DataSet を作成
        protected static DataSet CreateDataSource()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add( new DataColumn( "ID", typeof( Int32 ) ) );
            dt.Columns.Add( new DataColumn( "Name", typeof( string ) ) );
            dt.Columns.Add( new DataColumn( "Price", typeof( decimal ) ) );
            dt.Columns.Add( new DataColumn( "Discontinued", typeof( bool ) ) );
            //dt.Columns.Add( new DataColumn( "Date", typeof( DateTime ) ) );
            dt.Columns.Add( new DataColumn( "Date", typeof( string ) ) );

            for ( int i = 0; i < 15; i++ )
            {
                dr = dt.NewRow();
                dr[ "ID" ] = i;
                dr[ "Name" ] = "Product Name_" + i.ToString();
                dr[ "Price" ] = 123000 * ( i + 1 );
                dr[ "Discontinued" ] = ( i % 2 == 0 ) ? true : false;
                dr[ "Date" ] = DateTime.Now.AddDays( i - 15 ).ToString();
                dt.Rows.Add( dr );
            }

            DataSet ds = new DataSet();
            ds.Tables.Add( dt );
            return ds;
        }

        public class LogDataSample
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }

            public bool Discontinued { get; set; }

            public DateTime Date { get; set; }
        }
    }
}
