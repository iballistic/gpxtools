using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace GpxToolsCmd
{
    public class ImageHelper
    {
        public ImageHelper() { }

        public bool ThumbnailCallback()
        {
            return true;
        }



        /// <summary>
        /// Create thumbnail for an image
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="imageHeight"></param>
        /// <param name="imageWidth"></param>
        public void CreateThumbnail(string filePath, int imageHeight, int imageWidth )
        {

            Image image = new Bitmap(filePath);
            //Image pThumbnail = image.GetThumbnailImage(imageHeight, imageWidth, null, new IntPtr());
            int srcImageWidth =  image.Width;
            int srcImageHeight = image.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)imageWidth / (float)(srcImageWidth));
            nPercentH = ((float)imageHeight / (float)(srcImageHeight));


 

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;


            int imageNewWidth = (int)(srcImageWidth * nPercent);
            int imageNewHeight = (int)(srcImageHeight * nPercent);

            Image pThumbnail = (Image)(new Bitmap(image, new Size(imageNewWidth, imageNewHeight)));
            FileInfo imageFile = new FileInfo(filePath);
            
            
            DirectoryInfo thumbFolder = new DirectoryInfo(Path.Combine(imageFile.DirectoryName,"Thumbnails"));
            

            thumbFolder.Create();

            string thumbFile = Path.Combine(thumbFolder.FullName,imageFile.Name);
                //Path.GetFileNameWithoutExtension(imageFile.FullName);
                //Path.GetExtension(imageFile.FullName);
            pThumbnail.Save(thumbFile);

            

        }

        /// <summary>
        /// Get image metadata
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetImageMetaData(string pathToFile)
        {
            Dictionary<string, object> imageExif = new Dictionary<string, object>();
            Console.WriteLine("Processing {0}", pathToFile);

            if (!File.Exists(pathToFile))
            {
                throw new Exception(String.Format("The image file {0} does not exist", pathToFile));
            }

            Image imageObj = new Bitmap(pathToFile);

            try
            {
                //http://msdn.microsoft.com/en-us/library/windows/desktop/ms534418(v=vs.85).aspx
                //0x0002
                PropertyItem propertyTagGpsLatitudeRef = imageObj.GetPropertyItem(0x0001);
                PropertyItem propertyTagGpsLatitude = imageObj.GetPropertyItem(0x0002);
                double coordinatesLat = ExifGpsToDouble(propertyTagGpsLatitudeRef, propertyTagGpsLatitude);
                Console.WriteLine("coordinatesLat: {0}", coordinatesLat);
                imageExif.Add("lat", coordinatesLat);



                PropertyItem propertyTagGpsLongitudeRef = imageObj.GetPropertyItem(0x0003);
                PropertyItem propertyTagGpsLongitude = imageObj.GetPropertyItem(4);
                double coordinatesLong = ExifGpsToDouble(propertyTagGpsLongitudeRef, propertyTagGpsLongitude);
                Console.WriteLine("coordinatesLong: {0}", coordinatesLong);

                imageExif.Add("lon", coordinatesLong);



                //PropertyItem[] propItems = imageObj.PropertyItems;
                //foreach (PropertyItem item in propItems)
                //{
                //    //conver item.Id to hex so you can compare result to 
                //    //http://msdn.microsoft.com/en-us/library/windows/desktop/ms534418(v=vs.85).aspx
                //    Console.WriteLine("Item id: {0} and type: {1} and value: {2}", item.Id.ToString("X"), item.Type, item.Value);

                //}

                //PropertyTagImageDescription
                PropertyItem PropertyTagImageDescription = imageObj.GetPropertyItem(0x10E);
                ASCIIEncoding enc = new ASCIIEncoding();     
                string title = enc.GetString(PropertyTagImageDescription.Value);
                imageExif.Add("title", title.TrimEnd('\0'));
                //Console.WriteLine("Title: {0}", enc.GetString(PropertyTagImageDescription.Value));

            }
            catch (System.ArgumentException err)
            {
                
                return imageExif;
            }


            return imageExif;

        }

        /// <summary>
        /// Get Gps location
        /// </summary>
        /// <param name="itemRef"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private double ExifGpsToDouble(PropertyItem itemRef, PropertyItem item)
        {

            //degrees
            uint degreesNumerator = BitConverter.ToUInt32(item.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(item.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;
            Console.WriteLine("degreesNumerator: {0}", degreesNumerator);
            Console.WriteLine("degreesDenominator: {0}", degreesDenominator);
            Console.WriteLine("degrees: {0}", degrees);

            //minutes
            uint minutesNumerator = BitConverter.ToUInt32(item.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(item.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;
            Console.WriteLine("minutesNumerator: {0}", minutesNumerator);
            Console.WriteLine("minutesDenominator: {0}", minutesDenominator);
            Console.WriteLine("minutes: {0}", minutes);



            //seconds
            uint secondsNumerator = BitConverter.ToUInt32(item.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(item.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;
            Console.WriteLine("secondsNumerator: {0}", secondsNumerator);
            Console.WriteLine("secondsDenominator: {0}", secondsDenominator);
            Console.WriteLine("seconds: {0}", seconds);

            double coordinates = degrees + (minutes / 60d) + (seconds / 3600d);

            string gpsRef = Encoding.ASCII.GetString(new byte[1] { itemRef.Value[0] });
            if (gpsRef.ToUpper() == "S" || gpsRef.ToUpper() == "W")
                coordinates = 0 - coordinates;

            return coordinates;
        }

    }
}
