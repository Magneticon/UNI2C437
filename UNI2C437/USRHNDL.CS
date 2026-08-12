using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UNI2C437 {
    public static class USRHNDL {
        private static CONTEXT C;
        private static string REPLY = "";
        private static string RSTR = "";
        private static int PSTATE = 0;
        private static int RSTATE = 0;
        private static int PSTATEDF = 0;
        private static int RSTATEDF = 0;
        private static int DOSTATE = 0;

        private static void INIT(){
            DOSTATE = 0;
            PSTATE = 0;
            RSTATE = 0;
            PSTATEDF = PSTATE;
            RSTATEDF = RSTATE; 
            REPLY = "";
            RSTR = "";
        }

        public static CONTEXT USRPROC(this CONTEXT CO){
            C = CO;
            INIT();
            int i = 0;
            for (; i < 40; i++){
                USRSTART();
                if (C.STATE != 4) break;
            }
            if (C.STATE == 4){
                Console.WriteLine("");
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine(">>> PROGRAM PROCESSING ERROR!");
                Console.WriteLine(">>>    CONTEXT: USRPROC > USRSTART()");
                Console.WriteLine(">>>    C_STATE: {0}", C.STATE);
                Console.WriteLine(">>>    WATCHDOG: {0}", i);
                Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                Console.WriteLine("");
                Console.WriteLine("> PROGRAM WILL BE TERMINATED!");
                Console.WriteLine("");
                Console.WriteLine("PRESS ANY KEY TO CONTINUE ...");
                Console.ReadKey();
                C.STATE = 8;
            }
            return C;
        }

        private static void USRSTART(){
            if (RSTATE > RSTATEDF) RSTATEDF = RSTATE;
            if (PSTATE > 0){
                Console.WriteLine("> OLD SETTINGS FROM LAST RUN WERE DETECTED. DO YOU WANT TO USE THEM? <Y/N>");
                REPLY = Console.ReadLine();
                if (REPLY.ToUpper() == "Y"){
                    Console.WriteLine("OLD SETTINGS WERE LOADED SUCCESSFULLY.");
                    PSTATEDF = PSTATE;
                    RSTATE = RSTATEDF;
                }
                else {
                    PSTATEDF = 0;
                    PSTATE = PSTATEDF;
                    C.INIT(); //clear context
                    INIT(); //clear user context
                    C.STATE = 4; //recycle
                    return;
                }
            }
            else{
                if (RSTATEDF < 1){
                    C.INIT();
                    INIT();
                }
                Console.WriteLine("UNC2C437 PROGRAM: UNICODE TO IBM-CP437 V1.00");
                Console.WriteLine("");
                C.DIDBAK = false;
                if (RSTATEDF < 1){
                    DOSTATE = 0;
                    Console.WriteLine("> DO YOU WANT TO EXECUTE DATA FROM BACKUP? <Y/N>");
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper().Trim() == "Y"){
                        DOSTATE = 0;
                        string BAKWPATH = C.BAKFPATH;
                        do {
                            BAKWPATH = BAKWPATH.FFULLPTH();
                            Console.WriteLine("> IS THIS PATH {0} CORRECT? <Y/N>", BAKWPATH);
                            REPLY = Console.ReadLine();
                            if (REPLY.ToUpper().Trim() == "Y"){
                                if (!File.Exists(BAKWPATH)){
                                    Console.WriteLine("> FILE {0} DOES NOT EXIST. DO YOU WANT TO TRY IT AGAIN OR TERMINATE PROGRAM?? <Y/N>", BAKWPATH);
                                    REPLY = Console.ReadLine();
                                    if (REPLY.ToUpper().Trim() != "Y"){
                                        C.STATE = 8;
                                        break;
                                    }
                                    Console.WriteLine("> INPUT A NEW FILE PATH:");
                                    BAKWPATH = Console.ReadLine();
                                    if (!BAKWPATH.PATHCHCK()){
                                        Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", BAKWPATH);
                                        BAKWPATH = "none";
                                    }
                                }
                                else{
                                    C.BAKFPATH = BAKWPATH;
                                    C.DIDBAK = true;
                                    C = C.LOADRENM();
                                    if (C.STATE != 0)
                                        C.STATE = 8;
                                    return;
                                }
                            }
                            else{
                                Console.WriteLine("> INPUT A NEW FILE PATH OR INPUT X TO END THE PROGRAM PROCESSING:");
                                BAKWPATH = Console.ReadLine();
                                if (BAKWPATH.ToUpper().Trim() == "X"){
                                    C.STATE = 8;
                                    return;
                                }
                                if (!BAKWPATH.PATHCHCK()){
                                    Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", BAKWPATH);
                                    BAKWPATH = "none";
                                }
                            }
                        } while (DOSTATE == 0);
                    } 
                }
                if (C.DIDBAK){
                    C.STATE = 8;
                    return;
                }
                DOSTATE = 0;
            }
            if (PSTATEDF < 3){
                if (!(RSTATEDF < 1)) RSTR = "/S> ("+((C.LOADFPTH) ? "yes" : "no")+")";
                else RSTR = ">";
                if (PSTATEDF < 1){
                    Console.WriteLine("> DO YOU WANT TO LOAD SETTINGS FROM SETTINGS FILE? <Y/N{0}", RSTR);
                    REPLY = Console.ReadLine();
                }
                if ((REPLY.ToUpper() == "S" && RSTR != ">") || !(PSTATEDF < 1)) REPLY = C.LOADFPTH ? "Y" : "N";
                C.LOADFPTH = (REPLY.ToUpper() == "Y") ? true : false;
                if (REPLY.ToUpper() == "Y"){
                    DOSTATE = 0;
                    PSTATE = 1;
                    if (!(RSTATEDF < 2)) RSTR = "/S>";
                    else RSTR = ">";
                    do {
                        DOSTATE = 0;
                        if (PSTATEDF < 2){
                            Console.WriteLine("> IS THIS PATH CORRECT: {0}? <Y/N{1}", C.SETFPATH, RSTR);
                            REPLY = Console.ReadLine();
                        }
                        if (REPLY.ToUpper() == "Y" || (REPLY.ToUpper() == "S" && RSTR != ">") || !(PSTATEDF < 2)){
                            C.SETFPATH = C.SETFPATH.FFULLPTH();
                            if (!C.SETFPATH.ISFILE())
                                Console.WriteLine(">> YOU HAVE INPUTED A DIRECTORY NAME {0}. PLEASE, TRY IT AGAIN.", C.SETFPATH);
                            else if (File.Exists(C.SETFPATH)){
                                DOSTATE = 1;
                                PSTATE = 2;
                                //READ & LOAD SETTING FROM SETTING FILE HERE
                                do {
                                    DOSTATE = 0;
                                    C = C.GETLOAD(C.SETFPATH);
                                    if (C.STATE != 0){
                                        Console.WriteLine(">> CANNOT LOAD THE FILE {0}. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>", C.SETFPATH);
                                        REPLY = Console.ReadLine();
                                        if (REPLY.ToUpper() == "Y"){
                                            C.STATE = 8;
                                            return;
                                        }
                                    }
                                    else DOSTATE = 1;
                                } while (DOSTATE == 0);
                                if (DOSTATE == 1){
                                    DOSTATE = 0;
                                    if (!(RSTATEDF < 2)) RSTR = "<S> ("+C.MAINPTH+")";
                                    else RSTR = "";
                                    do {
                                        Console.WriteLine("");
                                        if (C.OPERMODE == 0){
                                            Console.WriteLine("> INPUT A SOURCE DIRECTORY LOCATION: {0}", RSTR);
                                            REPLY = Console.ReadLine();
                                            if (REPLY != ""){
                                                if (REPLY.ToUpper() == "S" && RSTR != "") REPLY = C.MAINPTH;
                                                if (!REPLY.PATHCHCK()){
                                                    Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                                                    REPLY = "none";
                                                }
                                                else{
                                                    REPLY = REPLY.FFULLPTH();
                                                    if (REPLY.ISFILE()){
                                                        REPLY = REPLY.GETDIRPT();
                                                        Console.WriteLine(">> YOU HAVE INPUTED A FILE NAME. IT WAS CHANGED TO A DIRECTORY PATH {0} AS PER SELECTED SETTINGS.", REPLY);
                                                    }
                                                    REPLY = REPLY.DFULLPTH();
                                                }
                                                if (Directory.Exists(REPLY)){
                                                    C.MAINPTH = REPLY;
                                                    DOSTATE = 1;
                                                    break;
                                                }
                                            }
                                            Console.WriteLine(">> DIRECTORY DOES NOT EXIST. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                                            REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() == "Y"){
                                                C.STATE = 8;
                                                return;
                                            }
                                            RSTR = "";
                                        }
                                        else{
                                            Console.WriteLine("> INPUT A SOURCE FILE LOCATION: {0}", RSTR);
                                            REPLY = Console.ReadLine();
                                            if (REPLY != ""){
                                                if (REPLY == "S" && RSTR != "") REPLY = C.MAINPTH;
                                                if (!REPLY.PATHCHCK()){
                                                    Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                                                    REPLY = "none";
                                                }
                                                REPLY = REPLY.FFULLPTH();
                                                if (!REPLY.ISFILE())
                                                    Console.WriteLine(">> YOU HAVE INPUTED A DIRECTORY NAME {0}. PLEASE, TRY IT AGAIN.", REPLY);
                                                else if (File.Exists(REPLY)){
                                                    C.MAINPTH = REPLY;
                                                    DOSTATE = 1;
                                                    break;
                                                }
                                            }
                                            Console.WriteLine(">> FILE DOES NOT EXIST. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                                            REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() == "Y"){
                                                C.STATE = 8;
                                                return;
                                            }
                                            RSTR = "";
                                        }
                                    } while (DOSTATE == 0);
                                    if (C.DOBAK == 1){
                                        bool WASGOOD = true;
                                        DOSTATE = 1;
                                        do{
                                            if (!WASGOOD){
                                                Console.WriteLine("> INPUT A PATH TO THE BACKUP FILE OR SKIP TO USE DEFAULT LOCATION {0}: <S>", C.BAKFPATH);
                                                Console.WriteLine();
                                                REPLY = Console.ReadLine();
                                            }
                                            else REPLY = C.BAKFPATH;
                                            if (REPLY.ToUpper() == "S")
                                                REPLY = C.BAKFPATH;
                                            else
                                                REPLY = REPLY.FFULLPTH();
                                            string ISINDIR = REPLY.GETDIRPT();
                                            if (!Directory.Exists(ISINDIR)){
                                                Console.WriteLine(">> DIRECTORY {0} FOR BACKUP FILE DOES NOT EXIST. DO YOU WANT TO TERMINATE A PROGRAM OR TRY IT AGAIN? <Y/N>", ISINDIR);
                                                REPLY = Console.ReadLine();
                                                if (REPLY.ToUpper() == "Y"){
                                                    C.STATE = 8;
                                                    return;
                                                }
                                                else
                                                    DOSTATE = 0;
                                            }
                                            else{
                                                if (!REPLY.ISFILE()){
                                                    REPLY = ISINDIR+"UNI2C437.BAK";
                                                    Console.WriteLine(">> YOU HAVE INPUTED A DIRECTORY NAME ONLY. IT WAS CHANGED TO A FILE PATH {0}.", REPLY);
                                                }
                                                if (File.Exists(REPLY)){
                                                    Console.WriteLine(">> BACKUP FILE {0} ALREADY EXISTS. DO YOU WANT TO DELETE IT OR TRY IT AGAIN? <Y/N>", REPLY);
                                                    string REPLY2 = Console.ReadLine();
                                                    if (REPLY2.ToUpper() == "Y"){
                                                        File.Delete(REPLY);
                                                        DOSTATE = 1;
                                                    }
                                                    else
                                                        DOSTATE = 0;
                                                }
                                                else DOSTATE = 1;
                                            }
                                            if (DOSTATE == 1)
                                                C.BAKFPATH = REPLY;
                                        } while (DOSTATE == 0);
                                    }
                                    PSTATE = 3;
                                    RSTATE = PSTATE;
                                    USRPARMV();
                                    return;
                                }
                            }
                            else{
                                Console.WriteLine("FILE {0} DOES NOT EXIST! DO YOU WANT TO REPEAT IT AGAIN OR RESTART THE PROGRAM? <Y/N>", C.SETFPATH);
                                REPLY = Console.ReadLine();
                                if (REPLY.ToUpper() != "Y"){
                                    C.STATE = 4;
                                    return; 
                                }
                                RSTR = ">";
                            }
                        }
                        else{
                            Console.WriteLine(">> INPUT A NEW FILE PATH OR INPUT X TO EXIT THE PROGRAM:");
                            REPLY = Console.ReadLine();
                            if (!REPLY.PATHCHCK()){
                                Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                                REPLY = "none";
                            }
                            if (REPLY.ToUpper() == "X"){
                                C.STATE = 8;
                                return;
                            }
                            else C.SETFPATH = REPLY;
                        }
                    } while (DOSTATE == 0);
                }
                PSTATE = 3;
                RSTATE = PSTATE;
            }
            if (PSTATEDF < 4){
                DOSTATE = 0;
                if (!(RSTATEDF < 4)) RSTR = "/S> (FROM: "+(C.CONVMODE == 0 ? "UNICODE" : "IBM-437")+")";
                else RSTR = ">";
                do {
                    Console.WriteLine("");
                    Console.WriteLine("> DO YOU WANT TO DO CONVERSION FROM UNICODE OR FROM IBM-437 CODE PAGE? <1/2{0}", RSTR);
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                        if (REPLY == "1"){
                            C.CONVMODE = 0;
                            DOSTATE = 1;
                            break;
                        }
                        if (REPLY == "2"){
                            C.CONVMODE = 1;
                            DOSTATE = 1;
                            break;                
                        }
                        Console.WriteLine(">> SELECTION WAS NOT RIGHT. ALLOWED VALUES ARE 1 OR 2. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                    } else DOSTATE = 1;
                } while (DOSTATE == 0);
                PSTATE = 4;
            }
            if (PSTATEDF < 5){
                DOSTATE = 0;
                if (!(RSTATEDF < 5)) RSTR = "/S> ("+(C.OPERMODE == 0 ? "RENAME FILES" : "CONVERT A TEXT FILE")+")";
                else RSTR = ">";
                do {
                    Console.WriteLine("");
                    Console.WriteLine("> DO YOU WANT TO RENAME FILES OR CONVERT A TEXT FILE? <1/2{0}", RSTR);
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                        if (REPLY == "1"){
                            C.OPERMODE = 0;
                            DOSTATE = 1;
                            break;
                        }
                        if (REPLY == "2"){
                            C.OPERMODE = 1;
                            DOSTATE = 1;
                            break;                
                        }
                        Console.WriteLine(">> SELECTION WAS NOT RIGHT. ALLOWED VALUES ARE 1 OR 2. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                    } else DOSTATE = 1;
                } while (DOSTATE == 0);
                PSTATE = 5;
            }
            if (PSTATEDF < 7){
                DOSTATE = 0;
                bool DOAGAIN = false;
                if (!(RSTATEDF < 6)) RSTR = "<S> ("+C.MAINPTH+")";
                else RSTR = "";
                do {
                    Console.WriteLine("");
                    if (C.OPERMODE == 0){
                        if (PSTATEDF < 6 || (DOAGAIN)){
                            Console.WriteLine("> INPUT A SOURCE DIRECTORY LOCATION: {0}", RSTR);
                            REPLY = Console.ReadLine();
                            if (!REPLY.PATHCHCK()){
                                Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                                REPLY = "none";
                            }
                        }
                        if ((REPLY.ToUpper() == "S" || RSTR != "" || (!(PSTATEDF < 6))) && (!DOAGAIN)) REPLY = C.MAINPTH;
                        if (REPLY != ""){
                            REPLY = REPLY.FFULLPTH();
                            if (REPLY.ISFILE()){
                                REPLY = REPLY.GETDIRPT();
                                Console.WriteLine(">> YOU HAVE INPUTED A FILE NAME. IT WAS CHANGED TO A DIRECTORY PATH {0} AS PER SELECTED SETTINGS.", REPLY);
                            }
                            REPLY = REPLY.DFULLPTH();
                            if (Directory.Exists(REPLY)){
                                C.MAINPTH = REPLY;
                                DOSTATE = 0;
                                PSTATE = 6;
                                if (!(RSTATEDF < 7)) RSTR = "/S> ("+(C.DOSUBDIR == 0 ? "NO" : "YES")+")";
                                else RSTR = ">";
                                do {
                                    Console.WriteLine("");
                                    Console.WriteLine("> DO YOU WANT TO ALSO PROCESS SUBDIRECTORIES? <Y/N{0}", RSTR);
                                    REPLY = Console.ReadLine();
                                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                                        if (REPLY.ToUpper() == "Y"){
                                            C.DOSUBDIR = 1;
                                            break;
                                        }
                                        if (REPLY.ToUpper() == "N"){
                                            C.DOSUBDIR = 0;
                                            break;                
                                        }
                                        Console.WriteLine(">> SELECTION WAS NOT RIGHT. ALLOWED VALUES ARE Y OR N. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                                        REPLY = Console.ReadLine();
                                        if (REPLY.ToUpper() == "Y"){
                                            C.STATE = 8;
                                            return;
                                        }
                                    } else DOSTATE = 1;
                                } while (DOSTATE == 0);
                                DOSTATE = 1;
                                break;
                            }
                        }
                        Console.WriteLine(">> DIRECTORY DOES NOT EXIST. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                        DOAGAIN = true;
                    }
                    else{
                        if (PSTATEDF < 6 || (DOAGAIN)){
                            Console.WriteLine("> INPUT A SOURCE FILE LOCATION: {0}", RSTR);
                            REPLY = Console.ReadLine();
                            if (!REPLY.PATHCHCK()){
                                Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                                REPLY = "none";
                            }
                        }
                        if ((REPLY.ToUpper() == "S" || RSTR != "" || (!(PSTATEDF < 6))) && (!DOAGAIN)) REPLY = C.MAINPTH;
                        if (REPLY != ""){
                            REPLY = REPLY.FFULLPTH();
                            if (!REPLY.ISFILE())
                                Console.WriteLine(">> YOU HAVE INPUTED A DIRECTORY NAME {0}. PLEASE, TRY IT AGAIN.", REPLY);
                            else if (File.Exists(REPLY)){
                                C.MAINPTH = REPLY;
                                DOSTATE = 1;
                                break;
                            }
                        }
                        Console.WriteLine(">> FILE DOES NOT EXIST. DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                        DOAGAIN = true;
                    }
                } while (DOSTATE == 0);
                PSTATE = 7;
            }
            if (PSTATEDF < 8){
                DOSTATE = 0;
                if (!(RSTATEDF < 8)) RSTR = "/S> ("+(C.DOBAK == 0 ? "NO" : "YES")+")";
                else RSTR = ">";
                do {
                    Console.WriteLine("");
                    Console.WriteLine("> DO YOU WANT TO DO A BACKUP OF THE {0}? <Y/N{1}", C.OPERMODE == 0 ? "FILE NAMES" : "ORIGINAL FILE", RSTR);
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                        if (REPLY.ToUpper() == "Y"){
                            C.DOBAK = 1;
                            DOSTATE = 1;
                            break;   
                        }
                        if (REPLY.ToUpper() == "N"){
                            C.DOBAK = 0;
                            DOSTATE = 1;
                            break;                
                        }
                        Console.WriteLine(">> SELECTION WAS NOT RIGHT. ALLOWED VALUES ARE Y OR N. DO YOU TERMINATE PROGRAM? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                    } else DOSTATE = 1;
                } while (DOSTATE == 0);
                PSTATE = 8;
            }
            if (PSTATEDF < 9){
                if (C.DOBAK == 1){
                    DOSTATE = 0;
                    do{
                        Console.WriteLine("> INPUT A PATH TO THE BACKUP FILE OR SKIP TO USE DEFAULT LOCATION {0}: <S>", C.BAKFPATH);
                        Console.WriteLine();
                        REPLY = Console.ReadLine();
                        if (REPLY.Trim() == "" || REPLY.ToUpper() == "S")
                            REPLY = C.BAKFPATH;
                        else REPLY = REPLY.FFULLPTH();
                        string ISINDIR = REPLY.GETDIRPT();
                        if (!Directory.Exists(ISINDIR)){
                            Console.WriteLine(">> DIRECTORY {0} FOR BACKUP FILE DOES NOT EXIST. DO YOU WANT TO TERMINATE A PROGRAM OR TRY IT AGAIN? <Y/N>", ISINDIR);
                            REPLY = Console.ReadLine();
                            if (REPLY.ToUpper() == "Y"){
                                C.STATE = 8;
                                return;
                            }
                        }
                        else{
                            if (!REPLY.ISFILE()){
                                REPLY = ISINDIR+"UNI2C437.BAK";
                                Console.WriteLine(">> YOU HAVE INPUTED A DIRECTORY NAME ONLY. IT WAS CHANGED TO A FILE PATH {0}.", REPLY);
                            }
                            if (File.Exists(REPLY)){
                                Console.WriteLine(">> BACKUP FILE {0} ALREADY EXISTS. DO YOU WANT TO DELETE IT OR TRY IT AGAIN? <Y/N>", REPLY);
                                string REPLY2 = Console.ReadLine();
                                if (REPLY2.ToUpper() == "Y"){
                                    File.Delete(REPLY);
                                    DOSTATE = 1;
                                }
                            }
                            else DOSTATE = 1;
                        }
                        if (DOSTATE == 1)
                            C.BAKFPATH = REPLY;
                    } while (DOSTATE == 0);
                }
                PSTATE = 9;
            }
            if (PSTATEDF < 10){
                DOSTATE = 0;
                if (!(RSTATEDF < 10)) RSTR = "<S> ("+(C.DONAMLN > 0 ? C.DONAMLN+"" : "UNLIMITED")+")";
                else RSTR = "";
                do {
                    Console.WriteLine("");
                    Console.WriteLine("> INPUT A MAXIMAL ALLOWED LENGTH OF A RESULT WORD (E.G. 5 FOR DOS NAMES): {0}", RSTR);
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                        if (REPLY.ISDIGIT()){
                            C.DONAMLN = Convert.ToInt32(REPLY);
                            if (C.DONAMLN < 3){
                                Console.WriteLine(">> INPUTED VALUE IS TOO SMALL. DO YOU WANT TO USE MINIMAL ALLOWED LENGTH (3)? <Y/N>");
                                REPLY = Console.ReadLine();
                                if (REPLY.ToUpper() == "Y"){
                                    C.DONAMLN = 3;
                                    DOSTATE = 1;
                                    break;
                                }
                            }
                            else if (C.DONAMLN > 64){
                                Console.WriteLine(">> INPUTED VALUE IS TOO BIG (MAXIMUM IS 64). DO YOU WANT TO WORK WITH UNLIMITED LENGTH NAMES? <Y/N>");
                                REPLY = Console.ReadLine();
                                if (REPLY.ToUpper() == "Y"){
                                    C.DONAMLN = 0;
                                    DOSTATE = 1;
                                    break;
                                }
                            }
                            else{
                                DOSTATE = 1;
                                break;
                            }
                        }
                        else{
                            Console.WriteLine(">> INPUTED VALUE WAS NON-NUMERICAL. DO YOU WANT TO WORK WITH UNLIMITED LENGTH NAMES? <Y/N>");
                            REPLY = Console.ReadLine();
                            if (REPLY.ToUpper() == "Y"){
                                C.DONAMLN = 0;
                                DOSTATE = 1;
                                break;
                            }
                        }
                        Console.WriteLine(">> DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                        if (REPLY.ToUpper() == "Y"){
                            C.STATE = 8;
                            return;
                        }
                    } else DOSTATE = 1;
                } while (DOSTATE == 0);
                PSTATE = 10;
            }
            if (PSTATEDF < 11){
                DOSTATE = 0;
                if (!(RSTATEDF < 11)) RSTR = "<N/S> ("+(C.CONVSPEC == '¾' ? "DO NOT CONVERT UNKNOWN" : C.CONVSPEC+"")+")";
                else RSTR = "";
                do {
                    Console.WriteLine("");
                    Console.WriteLine("> INPUT A CHARACTER TO BE USED AS REPLACEMENT FOR UNKNOWN LITERALS - REPLY N IF YOU WANT TO PRESERVE UNKNOWN LITERALS IN THE NAMES: {0}", RSTR);
                    REPLY = Console.ReadLine();
                    if (REPLY.ToUpper() != "S" || RSTR == ">"){
                        if (REPLY.ToUpper() == "N")
                            C.CONVSPEC = '¾';
                        else if (REPLY.Trim() != "")
                            C.CONVSPEC = REPLY[0];
                        DOSTATE = 1;
                        break;
                    } else DOSTATE = 1;
                } while (DOSTATE == 0);
                PSTATE = 11;
            }
            USRPARMV(); 
            return;
        }

        private static void USRPARMV(){
            Console.WriteLine("");
            Console.WriteLine("---");
            Console.WriteLine("PROGRAM PARAMETERS SUMMARY:");
            Console.WriteLine("");
            Console.WriteLine("CONVERSION MODE: {0}", C.CONVMODE == 0 ? "UNICODE > IBM-437" : "IBM-437 > UNICODE");
            if (C.OPERMODE == 0){
                Console.WriteLine("PROCESS TYPE: RENAME OF FILES");
                Console.WriteLine("PROCESSING OF DIRECTORY: {0}", C.MAINPTH);
                Console.WriteLine("PROCESSING OF SUBDIRECTORIES: {0}", C.DOSUBDIR == 0 ? "NO" : "YES");
            }
            else {
                Console.WriteLine("PROCESS TYPE: CONVERSION OF FILE");
                Console.WriteLine("PROCESSING OF FILE: {0}", C.MAINPTH);
            }
            C.BAKFPATH = Directory.Exists(C.SETFPATH.DFORMAT()) ? Path.Combine(C.SETFPATH.DFORMAT(), "UNI2C437.BAK") : C.BAKFPATH;
            Console.WriteLine("MAKING A BACKUP: {0}", C.DOBAK == 0 ? "NO" : "YES");
            Console.WriteLine("PATH FOR A BACKUP FILE: {0}", C.BAKFPATH);
            Console.WriteLine("MAXIMAL ALLOWED LENGTH: {0}", C.DONAMLN > 0 ? C.DONAMLN+"" : "UNLIMITED");
            Console.WriteLine("DEFAULT CHARACTER FOR UNKNOWN LITERALS: {0}", C.CONVSPEC == '¾' ? "DO NOT CONVERT UNKNOWN" : C.CONVSPEC+"");
            Console.WriteLine("");
            Console.WriteLine("---");
            Console.WriteLine("> DO YOU WANT TO CONTINUE WITH THESE SETTINGS? <Y/N>");
            REPLY = Console.ReadLine();
            if (REPLY.ToUpper().Trim() != "Y") {
                Console.WriteLine(">> DO YOU WANT TO TERMINATE THE PROGRAM? <Y/N>");
                REPLY = Console.ReadLine();
                if (REPLY.ToUpper() == "Y"){
                    C.STATE = 8;
                    return;
                }
                else{
                    PSTATE = 0;
                    C.STATE = 4;
                    return;
                }
            }
            Console.WriteLine("> DO YOU WANT TO SAVE THESE SETTINGS? <Y/N>");
            REPLY = Console.ReadLine();
            if (REPLY.ToUpper() == "Y"){
                DOSTATE = 0;
                do {
                    bool CANSAVE = true;
                    Console.WriteLine("> INPUT A NEW SETTINGS FILE DESTINATION OR INPUT S TO USE DEFAULT: <S> {0}", C.SETFPATH);
                    REPLY = Console.ReadLine();
                    if (REPLY != ""){
                        REPLY = REPLY.ToUpper().Trim();
                        if (REPLY == "S") REPLY = C.SETFPATH;
                        if (!REPLY.PATHCHCK()){
                            Console.WriteLine("> PATH {0} CONTAINS ILLEGAL CHARACTERS! INPUT THE PATH AGAIN.", REPLY);
                            REPLY = "none";
                            CANSAVE = false;
                        }
                        REPLY = Path.GetFullPath(REPLY);
                        if (File.Exists(REPLY)){
                            CANSAVE = false;
                            Console.WriteLine("> DO YOU WANT TO OVERWRITE EXISTING FILE? <Y/N> {0}", REPLY);
                            string REPLY2 = Console.ReadLine();;
                            if (REPLY2.ToUpper().Trim() == "Y"){
                                File.Delete(REPLY);
                                CANSAVE = true;
                            }
                        }
                        if (CANSAVE){
                            C.SETFPATH = REPLY;
                            CANSAVE = C.SETLOAD(C.SETFPATH);
                            if (CANSAVE){
                                Console.WriteLine(">> SETTINGS FILE {0} WAS SAVED SUCCESSFULLY!", C.SETFPATH);
                                DOSTATE = 1;
                            }
                            else{
                                Console.WriteLine(">> AN ERROR HAS OCCURED WITH SAVING THE SETTINGS FILE {0}. DO YOU WANT TO TRY IT AGAIN? <Y/N>", C.SETFPATH);
                                REPLY = Console.ReadLine();
                                if (REPLY.ToUpper() != "Y")
                                    DOSTATE = 2;
                            }
                        }
                    }
                    else{
                        Console.WriteLine(">> NO FILE PATH WAS INPUT. DO YOU WANT TO RETURN TO PARAMETERS OVERVIEW? <Y/N>");
                        REPLY = Console.ReadLine();
                        if (REPLY.ToUpper() == "Y")
                            DOSTATE = 2;
                    }
                } while(DOSTATE == 0);
                if (DOSTATE == 2){
                    USRPARMV();
                    return;
                }
            }
            Console.WriteLine("");
            Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
            Console.ReadKey();
            Console.WriteLine("");
            Console.WriteLine("PROGRAM IS PROCESSING THE REQUEST ...");   
            Console.WriteLine("");
            Console.WriteLine("----------");
            Console.WriteLine("");

            C.STATE = 0;
            return;
        }
    }
}