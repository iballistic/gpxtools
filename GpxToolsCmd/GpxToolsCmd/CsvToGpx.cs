using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System.Data;

using CSharpLib;


namespace GpxToolsCmd
{
    public class CsvToGpx
    {
        public CsvToGpx() { }

        public void Process(string inFile, out string message)
        {
            message = "";
            FileInfo aFile = new FileInfo(inFile);
            string outFile = string.Format("{0}\\{1}.gpx",
                aFile.DirectoryName,
                Path.GetFileNameWithoutExtension(aFile.FullName));


            GpxHelper gpxHelper = new GpxHelper();
            DataTable wptTable = null;
            wptTable = CSVFileManager.CSVToDataTable(inFile, "waypoint");

            rteType route = new rteType();


            List<wptType> wptList = new List<wptType>();


            foreach (DataRow row in wptTable.Rows)
            {
                wptType wtpObj = new wptType();
                wtpObj.lon = Decimal.Parse(row["lon"].ToString());
                wtpObj.lat = Decimal.Parse(row["lat"].ToString());

                if (wptTable.Columns.Contains("name"))
                    wtpObj.name = row["name"].ToString();
                else
                    throw new Exception("Name column value is empty");
                if (wptTable.Columns.Contains("cmt"))
                    wtpObj.cmt = row["cmt"].ToString();
                if (wptTable.Columns.Contains("desc"))
                    wtpObj.desc = row["desc"].ToString();
                if (wptTable.Columns.Contains("sym"))
                    wtpObj.sym = row["sym"].ToString();
                wptList.Add(wtpObj);
            }
            //Write output
            gpxType obj = new gpxType();
            obj.wpt = wptList.ToArray();
            gpxHelper.XmlWriteGpx(outFile, obj);

            message = string.Format("Done! Compiled {0} file", outFile);

         }

    }
}
