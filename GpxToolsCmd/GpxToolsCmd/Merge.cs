using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using CSharpLib;




namespace GpxToolsCmd {
    public class Merge {
        private List<wptType> uniqueWptList = new List<wptType>();
        private List<wptType> duplicateWptList = new List<wptType>();
        private GpxHelper gpxHelper = new GpxHelper();



        public Merge() { }

        /// <summary>
        /// Merge all files from a directory to a single file
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="outputFile"></param>
        public void Process(string directory, string outputFile, out string message) {
            if (!Directory.Exists(directory)) {
                throw new Exception("The directory path does not exist");
            }

            if (string.IsNullOrEmpty(outputFile)) {
                outputFile = string.Format("{0}.{1}", directory, "gpx");
            }


            FileInfo outFile = new FileInfo(outputFile);



            List<string> files = FileSystemHelper.GetFilesSorted(directory, "gpx");


            foreach (string file in files) {
                Console.WriteLine("Processing {0}", file);
                wptType[] wayPoints = gpxHelper.ReadWayOrRoutePoints(file);

                foreach (wptType wpt in wayPoints) {
                    AddToUniqueList(wpt);
                }

            }


            //Write output
            gpxType obj = new gpxType();

            obj.wpt = uniqueWptList.ToArray();
            gpxHelper.XmlWriteGpx(outputFile, obj);

            gpxType obj2 = new gpxType();
            obj.wpt = duplicateWptList.ToArray();
            gpxHelper.XmlWriteGpx(string.Format("{0}.duplicate", outFile.FullName), obj);

            message = string.Format("Done! Compiled {0} file", outputFile);

        }


        private void AddToUniqueList(wptType wpt) {
            //The list empty add the first one
            if (uniqueWptList.Count == 0) {
                uniqueWptList.Add(wpt);
                return;
            }

            DistanceHelper.Haversine dHelper = new DistanceHelper.Haversine();
            DistanceHelper.Position position1 = new DistanceHelper.Position();
            position1.Latitude = (double)wpt.lat;
            position1.Longitude = (double)wpt.lon;

            foreach (wptType uWpt in uniqueWptList) {
                DistanceHelper.Position position2 = new DistanceHelper.Position();
                position2.Latitude = (double)uWpt.lat;
                position2.Longitude = (double)uWpt.lon;


                DistanceHelper.Haversine haversine = new DistanceHelper.Haversine();
                double result = haversine.Distance(position1, position2, DistanceHelper.DistanceType.Kilometers);

                Console.WriteLine("Distance {0}", result);

                if (result == (double)0) {
                    //if (wpt.name.Length > uWpt.name.Length) {

                    //    Console.WriteLine("The name appears to be longer. Replace with the new one");
                    //    Console.WriteLine("uWpt.Name = {0}", uWpt.name);
                    //    Console.WriteLine("wpt.Name = {0}", wpt.name);
                    //    uniqueWptList.Remove(uWpt);
                    //    uniqueWptList.Add(wpt);
                    //    Console.WriteLine("Adding to the duplicate list. Old value");
                    //    duplicateWptList.Add(uWpt);
                    //}
                    //else {

                    //    Console.WriteLine("Adding to the duplicate list. New value");
                    //    duplicateWptList.Add(wpt);

                    //}

                    Console.WriteLine("Adding to the duplicate list");
                    duplicateWptList.Add(wpt);
                    return;
                }


            }

            Console.WriteLine("Adding to the unique list");
            uniqueWptList.Add(wpt);
            return;



        }



    }
}
