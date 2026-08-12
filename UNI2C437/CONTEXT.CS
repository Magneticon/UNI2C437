using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace UNI2C437 {
    public class CONTEXT {
        public int DONAMLN = 0;
        public int OPERMODE = 0;
        public int CONVMODE = 0;
        public int DOBAK = 0;

        public string MAINPTH = "";
        public int DOSUBDIR = 0;

        public string SETFPATH = "";
        public string BAKFPATH = "";

        public bool LOADFPTH = false;

        public int STATE = 0;

        public bool DIDBAK = false;

        public char CONVSPEC = '_';

        public int THCNTL = 0;
        public int THCNTLMX = 100;//maximal thread anmount to prevent stack overflow

        public string[] RESTRDIR;

        public CONTEXT(){
            STATE = 0;      //0 - continue processing, 4 - recycle in current context, 8 - terminate program  
            INIT();
        }

        public void INIT(){
            DONAMLN = 0;
            OPERMODE = 0;
            CONVMODE = 0;
            DOBAK = 0;
            MAINPTH = "";
            DOSUBDIR = 0;
            LOADFPTH = false;
            SETFPATH = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString().DFULLPTH(), "UNI2C437.SET");
            BAKFPATH =  Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString().DFULLPTH(), "UNI2C437.BAK");
            DIDBAK = false;
            RESTRDIR = new string[]{"SYSTEM VOLUME INFORMATION"}; //NAMES OF DIRECTORIES TO NOT BE EXECUTED TO PREVENT SYSTEM I/O AUTHORIZATION PROBLEMS WHEN ACCESSING THE ROOT DRIVES.
            CONVSPEC = '_';
            THCNTL = 0;
            THCNTLMX = 127;
        }
    }
}
