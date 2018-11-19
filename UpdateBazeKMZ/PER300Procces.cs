using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBConnectionLib;
using System.IO;

namespace UpdateBazeKMZ
{
    public class PER300Procces : FileProcces
    {
        public event ProgressChanged progressChanged;
        public event ProgressNotify progressNotify;
        public event ProgressCompleted progressCompleted;

        public override void ReadFile(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
