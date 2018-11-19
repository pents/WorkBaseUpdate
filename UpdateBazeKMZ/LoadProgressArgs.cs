using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateBazeKMZ
{
    public class LoadProgressArgs  // struct ?
    {
        public int currentProgress;
        public int fullProgress;
        public byte percentage;
        public string message;

        public LoadProgressArgs(int Current, int Full, string Msg)
        {

            currentProgress = Current;
            fullProgress = Full;
            percentage = (byte)(((float)currentProgress / (float)fullProgress) * 100);
            message = Msg;
        }

        public LoadProgressArgs(int Current, int Full) : this(Current, Full, "") { }

    }
}
