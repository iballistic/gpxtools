using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using CSharpLib;
using System.Drawing;
using System.Drawing.Imaging;



namespace GpxToolsCmd
{
    public class ImageToGpx
    {

        private List<wptType> wptList = new List<wptType>();
        private GpxHelper gpxHelper = new GpxHelper();

        public ImageToGpx() { }

        /// <summary>
        /// Get gps locations from images and put them into a gpx file
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="outputFile"></param>
        public void Process(string directory, string outputFile, out string message)
        {
            ImageHelper imageHelper = new ImageHelper();
               message = string.Empty;

            if (!Directory.Exists(directory))
            {
                throw new Exception("The directory path does not exist");
            }

            if (string.IsNullOrEmpty(outputFile))
            {
                outputFile = string.Format("{0}.{1}", directory, "gpx");
            }


            FileInfo outFile = new FileInfo(outputFile);



            List<string> files = FileSystemHelper.GetFilesSorted(directory, "jpg");


            foreach (string file in files)
            {
                Dictionary<string, object> imageExif = imageHelper.GetImageMetaData(file);

                if (imageExif.Count > 0)
                {
                    wptType wpt = new wptType();
                    FileInfo aFile = new FileInfo(file);
                    

                    if (imageExif.ContainsKey("lon") && imageExif.ContainsKey("lat"))
                    {
                        wpt.lon = Convert.ToDecimal(imageExif["lon"]);
                        wpt.lat = Convert.ToDecimal(imageExif["lat"]);
                        wpt.name = string.Format("{0}_{1}", aFile.Directory.Name, aFile.Name);
                        if (imageExif.ContainsKey("title"))
                        {
                            wpt.name = string.Format("{0}", imageExif["title"]);
                        }



                        //Porcess and image file
                        wptList.Add(wpt);
                    }

                }

            }

            //Write output
            gpxType obj = new gpxType();
            obj.wpt = wptList.ToArray();
            gpxHelper.XmlWriteGpx(outputFile, obj);


            message = string.Format("Done! Compiled {0} file", outputFile);

        }

      
      
    }
}



