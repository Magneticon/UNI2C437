using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace UNI2C437 {
    class MAIN {

        /* *******************************************************************************************
         * 
         * PROGRAM NAME: UNC2C437
         * PROGRAM VERSION: 1.00
         * 
         * LATEST RELEASE DATE: 2023/05/11
         * ORIGINAL RELEASE DATE: 2023/01/09
         * 
         * DESCRIPTION: PROGRAM FOR CONVERTING FROM UNICODE CHARSET TO IBM-437 CODE PAGE (ASCII)
         *                  SUPPORTED SOURCE CODE-PAGES:
         *                      - 437
         *                      - 770
         *                      - 852
         *                      - 855
         *                      - 860
         *                      - 1040
         * 
         * DEVELOPMENT NOTES:
         *                      2023/05/11 - ATOMIZATION OF PROGRAM INTO CLASSES - MORE OOP COMPLIANT
         * 
         * 
         * ******************************************************************************************* */

        private static CONTEXT C;

        public static void Main(string[] args){
            P_RUN();
            P_EXIT();
        }

        public static void P_RUN(){
            V_INIT();
            C = C.USRPROC(); //load user settings, load backup if required.
            if (C.STATE != 0) P_EXIT();
            if (C.DOBAK == 1) C = C.SETBKFIL(); //create backup file, if requested.
            if (C.STATE != 0 || C.DIDBAK) P_EXIT();
            C = C.RENAM(C.MAINPTH); //if all OK, do rename
            P_EXIT();
        }

        public static void V_INIT(){
            C = new CONTEXT();
        }

        public static void P_EXIT(){
            Console.WriteLine("");
            if (C.STATE == 0) Console.WriteLine("PROGRAM PROCESSING HAS FINISHED SUCCESSFULLY!");
            else Console.WriteLine("PROGRAM PROCESSING HAS FINISHED UNSUCCESSFULLY!");
            Console.WriteLine("");
            Console.WriteLine("PRESS ANY KEY TO CONTINUE ...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
