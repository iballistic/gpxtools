using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GpxToolsCmd {
    public class ProgramClass {

        private string _sourceDir = string.Empty;
        private string _fileOutput = string.Empty;
        private string _type = string.Empty;


        public ProgramClass() { }

        public int Run(string[] args) {

            if (args.Length < 4) {
                
                ShowUsage();
                return 1;
            }

            try {
                for (int i = 0; i < args.Length; i++) {
                    string arg = args[i];
                    switch (arg.TrimStart('-').ToLower()) {
                        case "dir":
                            this._sourceDir = TextArgument(args, ref i).ToLower();
                            break;
                        case "type":
                            this._type = TextArgument(args, ref i).ToLower();
                            break;
                        case "out":
                            this._fileOutput = TextArgument(args, ref i);
                            break;
                        case "h":
                            ShowUsage();
                            return 1;
                    }
                }
            }
            catch (ArgumentException) {
                ShowUsage();
                return 1;
            }

            try {
                string message = string.Empty;
                switch (_type) {
                    case "image":
                        ImageToGpx image = new ImageToGpx();
                        image.Process(this._sourceDir, this._fileOutput, out message);
                        Console.WriteLine(message);
                        break;
                    case "gpx":
                        Merge merge = new Merge();
                        merge.Process(this._sourceDir, this._fileOutput, out message);
                        Console.WriteLine(message);
                        break;
                    default:
                        ShowUsage();
                        break;
                }
                return 0;
            }
            catch (SystemException e) {

                Console.WriteLine(String.Format("Error {0}", e.ToString()));
                Console.ReadLine();
                return 1;
            }


        }

        /// <summary>
        /// Show program usage
        /// </summary>
        private void ShowUsage() {
            Console.WriteLine();
            Console.WriteLine("ARGUMENTS:");
            Console.WriteLine("-type \"<Type>\". This the innput file type,  for example \"image\" or \"gpx\"");
            Console.WriteLine("-dir \"<Directory>\": Input directory containing required files");
            Console.WriteLine("-out <File>: This paramerer is optional, if it is not provided the the source directory name is used to generae output file");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("To Export GPS location from images if available:");
            Console.WriteLine("GpxToolsCmd.exe -type image -dir c:\\temp\\mydirectory -out mytrip.gpx");
            Console.WriteLine();
            Console.WriteLine("To Merge GPX files into a singlge file. \n This is useful to have an amalgamated file that can be opened in Google Earth");
            Console.WriteLine("GpxToolsCmd.exe -type gpx -dir c:\\temp\\mydirectory -out mylocations.gpx");

        }


        /// <summary>
        /// Read prorgram input
        /// </summary>
        /// <param name="args"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string TextArgument(string[] args, ref int i) {
            if (args.Length < i + 2) throw new ArgumentException();
            string value = args[++i];
            if (value[0] == '-' || value[0] == '/') throw new ArgumentException();
            return value;
        }
    }

}

