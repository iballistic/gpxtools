using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;


namespace GpxToolsCmd
{
    public class GpxHelper
    {
        //Todo get the following info from assembly
        private static readonly string GPX_NAMESPACE = "http://www.topografix.com/GPX/1/1";
        private static readonly string GPX_VERSION = "1.1";
        private static readonly string GPX_CREATOR = "Gpx Merge - Telman Rustam";
        private static readonly string GARMIN_EXTENSIONS_NAMESPACE = "http://www.garmin.com/xmlschemas/GpxExtensions/v3";
        private static readonly string GARMIN_EXTENSIONS_PREFIX = "gpxx";
        private XmlWriter writer;


        public GpxHelper() { }

        /// <summary>
        /// ReadWayOrRoutePoints
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public wptType[] ReadWayOrRoutePoints(string fileName)
        {

            XmlSerializer deserializer = new XmlSerializer(typeof(gpxType));
            TextReader textReader = new StreamReader(fileName);

            gpxType gpx = (gpxType)deserializer.Deserialize(textReader);

            textReader.Close();


            return gpx.wpt;

        }

        /// <summary>
        /// XmlWriteGpx
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public void XmlWriteGpx(string fileName, gpxType obj)
        {

            writer = XmlWriter.Create(fileName, new XmlWriterSettings { CloseOutput = true, Indent = true });
            writer.WriteStartDocument(false);
            writer.WriteStartElement("gpx", GPX_NAMESPACE);
            writer.WriteAttributeString("version", GPX_VERSION);
            writer.WriteAttributeString("creator", GPX_CREATOR);
            writer.WriteAttributeString("xmlns", GARMIN_EXTENSIONS_PREFIX, null, GARMIN_EXTENSIONS_NAMESPACE);

            //Write Metadata
            //writer.WriteStartElement("metadata");
            //writer.WriteEndElement();
            foreach (wptType wpt in obj.wpt)
            {
                writer.WriteStartElement("wpt");
                WriteWayOrRoutePoint(wpt);
                writer.WriteEndElement();



            }


            //close file
            writer.Close();


        }

        /// <summary>
        /// Write Point
        /// </summary>
        /// <param name="wayPoint"></param>
        private void WriteWayOrRoutePoint(wptType wayPoint)
        {
            writer.WriteAttributeString("lat", wayPoint.lat.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lon", wayPoint.lon.ToString(CultureInfo.InvariantCulture));
            if (wayPoint.ele != default(decimal))
                writer.WriteElementString("ele", wayPoint.ele.ToString(CultureInfo.InvariantCulture));
            //https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings
            if (wayPoint.time != default(DateTime))
                writer.WriteElementString("time", wayPoint.time.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.CreateSpecificCulture("en-us")));
            if (!string.IsNullOrEmpty(wayPoint.name))
                writer.WriteElementString("name", wayPoint.name);
            if (!string.IsNullOrEmpty(wayPoint.cmt))
                writer.WriteElementString("cmt", wayPoint.cmt);
            if (!string.IsNullOrEmpty(wayPoint.desc))
                writer.WriteElementString("desc", wayPoint.desc);
            if (!string.IsNullOrEmpty(wayPoint.src))
                writer.WriteElementString("src", wayPoint.src);

            if (wayPoint.link != null)
            {

                foreach (linkType link in wayPoint.link)
                {
                    WriteLink("link", link);
                }
            }


            if (!string.IsNullOrEmpty(wayPoint.sym))
                writer.WriteElementString("sym", wayPoint.sym);
            if (!string.IsNullOrEmpty(wayPoint.type))
                writer.WriteElementString("type", wayPoint.type);

        }

        /// <summary>
        /// Write Link
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="link"></param>
        private void WriteLink(string elementName, linkType link)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("href", link.href.ToString());
            if (!string.IsNullOrEmpty(link.text))
                writer.WriteElementString("text", link.text);

            if (!string.IsNullOrEmpty(link.type))
                writer.WriteElementString("type", link.type);
            writer.WriteEndElement();
        }

    }
}
