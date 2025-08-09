using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GpxToolsCmd {
    class Program {
        static int Main(string[] args) {
            ProgramClass prog = new ProgramClass();
            prog.Run(args);

            Console.WriteLine("Done!");
            return 0;
        }
    }
}
