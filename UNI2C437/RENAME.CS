using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace UNI2C437 {
    public static class RENAME {
        private static char[] CA = MAINCONV.CA;
        private static int P = 0;

        //procedure to test generated name for directories/files
        //FEXT - File EXTension - if blank, testing a directory, if with value, then it is a file
        public static string TestGenName(int idx, int level, string WPATH, string FEXT){
	        for (var f = 0; f < CA.Length; f++){
		        if (idx > level){
			        //expand level there
			        string E = TestGenName(idx, level+1, WPATH+""+CA[f], FEXT);
                    if (E != "")
                        return E;
		        }
		        else{
                    string DESTWDIR = WPATH+""+CA[f];
                    if (!Directory.Exists(DESTWDIR) && FEXT == "")
                        return DESTWDIR;
                    else if (!File.Exists(DESTWDIR+"."+FEXT) && FEXT != "")
                        return DESTWDIR+"."+FEXT;
		        }
	        }
            return "";
        }

        public static CONTEXT RENAM(this CONTEXT C, string PTH){  
            string BKPTHFRM = "";
            string BKPTHTO = "";
            bool BKPDODIR = false;
            int EXECMOVE = 1;
            try{
                if (C.OPERMODE == 0){
                    if (Directory.Exists(PTH)){
                         string MDPTH = C.MAINPTH.DFULLPTH();
                         string DPTH = PTH.DFULLPTH();
                
                         string DNAME = "";
                         string DESTDIR = "";
                         string ORIGDNAM = "";
                         string ORIGDPTH = "";
                         if (MDPTH != DPTH){
                            DirectoryInfo D = Directory.GetParent(DPTH);
                            PTH = D.ToString().DFULLPTH();
                            DPTH = DPTH.DFULLPTH();
                            ORIGDPTH = DPTH;
                            ORIGDNAM = new DirectoryInfo(DPTH).Name;
                            DNAME = ORIGDNAM.C2ASCII(C.CONVSPEC);

                            if (C.DONAMLN > 0) DNAME = DNAME.Substring(0, Math.Min(C.DONAMLN,DNAME.Length)).Trim();
                            DESTDIR = Path.Combine(PTH, DNAME);

                        }
                        else{
                            PTH = DPTH;
                            ORIGDPTH = DPTH;
                            DPTH = "";                        
                        }

                        if (DPTH != ""){
                            if (DNAME != ORIGDNAM){
                                DESTDIR = Path.Combine(PTH, DNAME);
                                string WRES = "";
                                bool ISGOOD = false;
                                int MINLEN = 1; //minimal unchanged length of name
                                if (Directory.Exists(DESTDIR)){
                                    string WNAME = DNAME;
                                    for (var q = 1; q < C.DONAMLN-(Math.Min(MINLEN, DNAME.Length)); q++){
	                                    var x = C.DONAMLN-q-MINLEN;
	                                    if (x < WNAME.Length)
		                                    WNAME = WNAME.Substring(0, x);
                                        string WPATH = Path.Combine(PTH, WNAME);
	                                    WRES = TestGenName(q, 1, WPATH+"#", "");
                                        if (WRES != "")
                                            break;
                                    }
                                    do{
                                        if (WRES == ""){
                                            Console.WriteLine(">> DIRECTORY {0} ALREADY EXISTS! PLEASE, INPUT A NEW DIRECTORY NAME:", DNAME);
                                            string REPLY = Console.ReadLine();
                                            REPLY = REPLY.DSTR();
                                            if (REPLY.PATHCHCK() && REPLY != ""){
                                                if (Directory.Exists(Path.Combine(PTH, REPLY))){
                                                    Console.WriteLine(">> DIRECTORY {0} ALREADY EXISTS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>? <y/n>", REPLY);
                                                    REPLY = Console.ReadLine();
                                                    if (REPLY.ToUpper() == "Y"){
                                                        C.STATE = 8;
                                                        return C;
                                                    }
                                                }
                                                else
                                                    ISGOOD = true;
                                            }
                                            else{
                                                Console.WriteLine("> DIRECTORY NAME {0} CONTAINS ILLEGAL CHARACTERS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>", REPLY);
                                                REPLY = Console.ReadLine();
                                                if (REPLY.ToUpper() == "Y"){
                                                    C.STATE = 8;
                                                    return C;
                                                }
                                            }
                                        }
                                        else
                                            ISGOOD = true;
                                    } while (!ISGOOD);
                                    if (ISGOOD){
                                        DESTDIR = WRES;
                                        PTH = DESTDIR;
                                    }
                                }
                            }
                            else
                                PTH = ORIGDPTH;

                            BKPTHFRM = DPTH;
                            BKPTHTO = DESTDIR;
                            BKPDODIR = false;

                            EXECMOVE = 1;
                            foreach (string s in C.RESTRDIR){
                                if (DPTH.ToUpper().Contains(s) || DESTDIR.ToUpper().Contains(s)){
                                    EXECMOVE = 8;
                                    break;
                                }
                            }

                            if (DPTH != DESTDIR){
                                if (C.DOBAK == 1)
                                    BKPDODIR = true;
                                if (EXECMOVE == 1){
                                    DirectoryInfo RD = new DirectoryInfo(DPTH);
                                    P++;
                                    RD.MoveTo(DESTDIR);
                                    int EXPWAITT = 1;
                                    int MAXEWAIT = 30;
                                    while (EXECMOVE == 1){
                                        DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                        bool BRKEXPRD = false;
                                        while (Directory.Exists(DPTH) && !Directory.Exists(DESTDIR) && !BRKEXPRD ) { 
                                            if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                            else System.Threading.Thread.Sleep(EXPWAITT);
                                        }
                                        if (BRKEXPRD) {
                                            EXECMOVE = 0;
                                            while(BRKEXPRD){
                                                BRKEXPRD = false;
                                                Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE DIRECTORY PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", DPTH, DESTDIR, MAXEWAIT);
                                                string REPLY = Console.ReadLine();
                                                if (REPLY.ToUpper() == "X"){
                                                    C.STATE = 8;
                                                    return C;
                                                }
                                                else if (REPLY.ToUpper() == "W")
                                                    EXECMOVE = 1;
                                                else if (REPLY.ToUpper() == "S")
                                                    break;
                                                else{
                                                    Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                    BRKEXPRD = true;
                                                }
                                            }
                                        }
                                        else
                                            EXECMOVE = 2;
                                    };
                                }
                                if (EXECMOVE == 2)
                                    Console.WriteLine("[{0}]: DIRECTORY PROCESSED: {1} -> {2}", P, DPTH, DESTDIR);
                                else if (EXECMOVE == 1)
                                    Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, DPTH, DESTDIR);
                            }
                            PTH = DESTDIR;
                            if (EXECMOVE == 8)
                                Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, DPTH, DESTDIR);
                        }

                        if (EXECMOVE == 1 || EXECMOVE == 2){
                        
                            List<string> FILES = new List<string>(Directory.GetFiles(PTH));
                            List<string> DIRS = new List<string>(Directory.GetDirectories(PTH));

                            foreach(var WRKFILE in FILES){
                                string FPTH = WRKFILE.FFULLPTH();
                                string FNAMEOLD = Path.GetFileName(FPTH);
                                string FEXT = FNAMEOLD.GETFEXT();
                                string FNAME = Path.GetFileNameWithoutExtension(FNAMEOLD).C2ASCII(C.CONVSPEC).Trim();
                                if (C.DONAMLN > 0) FNAME = FNAME.Substring(0, Math.Min(C.DONAMLN,FNAME.Length)).Trim();
                                string DESTFILE = Path.Combine(PTH, FNAME+"."+FEXT);
                                if (FNAME != Path.GetFileNameWithoutExtension(FPTH)){
                                    DESTFILE = Path.Combine(PTH, FNAME+"."+FEXT);
                                    string FRES = "";
                                    bool ISGOOD = false;
                                    int MINLEN = 1; //minimal unchanged length of name
                                    if (File.Exists(DESTFILE)){
                                        string WNAME = FNAME;
                                        for (var q = 1; q < C.DONAMLN-(Math.Min(MINLEN, FNAME.Length)); q++){
	                                        var x = C.DONAMLN-q-MINLEN;
	                                        if (x < WNAME.Length)
		                                        WNAME = WNAME.Substring(0, x);
                                            string WPATH = Path.Combine(PTH, WNAME);
	                                        FRES = TestGenName(q, 1, WPATH+"#", FEXT);
                                            if (FRES != "")
                                                 break;
                                        }
                                        do{
                                            if (FRES == ""){
                                                Console.WriteLine(">> FILE {0} ALREADY EXISTS! PLEASE, INPUT A NEW FILE NAME:", FNAME+""+FEXT);
                                                string REPLY = Console.ReadLine();
                                                if (REPLY.PATHCHCK() && REPLY != ""){
                                                    if (File.Exists(Path.Combine(PTH, REPLY))){
                                                        Console.WriteLine(">> FILE {0} ALREADY EXISTS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>", REPLY);
                                                        REPLY = Console.ReadLine();
                                                        if (REPLY.ToUpper() == "Y"){
                                                            C.STATE = 8;
                                                            return C;
                                                        }
                                                    }
                                                    else{
                                                        FRES = Path.Combine(PTH, REPLY);
                                                        ISGOOD = true;
                                                    }
                                                }
                                                else{
                                                    Console.WriteLine(">> FILE NAME {0} CONTAINS ILLEGAL CHARACTERS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>", REPLY);
                                                    REPLY = Console.ReadLine();
                                                    if (REPLY.ToUpper() == "Y"){
                                                        C.STATE = 8;
                                                        return C;
                                                    }
                                                }
                                            }
                                            else
                                                ISGOOD = true;
                                        } while (!ISGOOD);
                                        if (ISGOOD)
                                            DESTFILE = FRES;
                                    }                          
                                }

                                if (FPTH != DESTFILE){
                                    if (C.DOBAK == 1){
                                        StreamWriter WB = File.AppendText(C.BAKFPATH);
                                        WB.WriteLine("F:#:@:#:{0}:#:@:#:{1}", FPTH, DESTFILE);
                                        WB.Close();
                                    }
                                    P++;
                                    EXECMOVE = 1;
                                    foreach (string s in C.RESTRDIR){
                                        if (FPTH.ToUpper().Contains(s) || DESTFILE.ToUpper().Contains(s)){
                                            EXECMOVE = 8;
                                            break;
                                        }
                                    }
                                    if (EXECMOVE == 1){
                                        File.Move(FPTH, DESTFILE);
                                        int EXPWAITT = 1;
                                        int MAXEWAIT = 30;
                                        while (EXECMOVE == 1){
                                            DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                            bool BRKEXPRD = false;
                                            while (File.Exists(FPTH) && !File.Exists(DESTFILE) && !BRKEXPRD ) { 
                                                if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                                else System.Threading.Thread.Sleep(EXPWAITT);
                                            }
                                            if (BRKEXPRD) {
                                                EXECMOVE = 0;
                                                while(BRKEXPRD){
                                                    BRKEXPRD = false;
                                                    Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", FPTH, DESTFILE, MAXEWAIT);
                                                    string REPLY = Console.ReadLine();
                                                    if (REPLY.ToUpper() == "X"){
                                                        C.STATE = 8;
                                                        return C;
                                                    }
                                                    else if (REPLY.ToUpper() == "W")
                                                        EXECMOVE = 1;
                                                    else if (REPLY.ToUpper() == "S")
                                                        break;
                                                    else{
                                                        Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                        BRKEXPRD = true;
                                                    }
                                                }
                                            }
                                            else
                                                EXECMOVE = 2;
                                        };
                                    }
                                    if (EXECMOVE == 2)
                                        Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, FPTH, DESTFILE);
                                    else
                                        Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, FPTH, DESTFILE);
                                }
                            }

                            if (C.DOSUBDIR == 1){
                                foreach(var WRKDIR in DIRS){
                                    string WDPTH = WRKDIR.DFULLPTH();

                                    int TITER = 0;
                                    int TRUN = 0;
                                    int TSIZ = 262144; //maxstacksize for a subprocess...maximum of concurently running processes: C.THCNTLMX...256k
                                    for (; TITER < 40; TITER++){
                                        int TEXPWAIT = 1;
                                        int TMXEWAIT = 30;
                                        while (TEXPWAIT < TMXEWAIT){
                                            long CSIZ = GC.GetTotalMemory(false);
                                            if (C.THCNTL < C.THCNTLMX && CSIZ < C.THCNTLMX*TSIZ && CSIZ < 167772160000){//167772160000 is maxsize of 400MB per program
                                                C.THCNTL++;
                                                var T = new Thread(() => {
                                                    C = RENAM(C, WDPTH); 
                                                }, TSIZ);
                                                T.Start();
                                                T.Join();
                                                
                                                if (C.STATE == 8){
                                                    Console.WriteLine("");
                                                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                                                    Console.WriteLine(">>> PROGRAM PROCESSING TERMINATION!");
                                                    Console.WriteLine(">>>    CONTEXT: RENAME > RENAM()");
                                                    Console.WriteLine(">>>    C_STATE: {0}", C.STATE);
                                                    Console.WriteLine(">>>    WATCHDOG: {0}", TITER);
                                                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                                                    Console.WriteLine("");
                                                    Console.WriteLine("> PROGRAM WILL BE TERMINATED!");
                                                    Console.WriteLine("");
                                                    Console.WriteLine("PRESS ANY KEY TO CONTINUE ...");
                                                    Console.ReadKey();
                                                    C.STATE = 8;
                                                    return C;
                                                }
                                                C.THCNTL--;
                                                TRUN = 2;
                                                break;
                                            }
                                            else{
                                                System.Threading.Thread.Sleep(TEXPWAIT);
                                                TEXPWAIT++;
                                            }
                                        }
                                        if (TRUN == 0){
                                            Console.WriteLine("");
                                            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                                            Console.WriteLine(">>> PROGRAM PROCESSING STACK SIZE WARNING!");
                                            Console.WriteLine(">>>    CONTEXT: RENAME > RENAM()");
                                            Console.WriteLine(">>>    C_STATE: {0}", C.STATE);
                                            Console.WriteLine(">>>    WATCHDOG: {0}", TITER);
                                            Console.WriteLine("");
                                            Console.WriteLine(">>>    TOTAL PROCESSES: {0}", C.THCNTL);
                                            Console.WriteLine(">>>    SOFTCAP: {0}", C.THCNTL);
                                            Console.WriteLine(">>>    HEAPSIZE: {0}", GC.GetTotalMemory(false));
                                            Console.WriteLine(">>>    HEAPCOMIT: {0}", C.THCNTL*TSIZ);
                                            Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                                            Console.WriteLine("");
                                            Console.WriteLine("DO YOU WANT TO WAIT ADDITIONAL {0} SECONDS TO SEE, IF THE CONCURENT PROCESSES WILL FINISH? <Y/N>", TMXEWAIT);
                                            string REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() != "Y")
                                                break;
                                        }
                                        else
                                            break;
                                    }
                                    if (TITER > 3999 || TRUN == 0){
                                        Console.WriteLine("");
                                        Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                                        Console.WriteLine(">>> PROGRAM PROCESSING ERROR!");
                                        Console.WriteLine(">>>    CONTEXT: RENAME > RENAM()");
                                        Console.WriteLine(">>>    C_STATE: {0}", C.STATE);
                                        Console.WriteLine(">>>    WATCHDOG: {0}", TITER);
                                        Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                                        Console.WriteLine("");
                                        Console.WriteLine("> PROGRAM WILL BE TERMINATED!");
                                        Console.WriteLine("");
                                        Console.WriteLine("PRESS ANY KEY TO CONTINUE ...");
                                        Console.ReadKey();
                                        C.STATE = 8;
                                        return C;
                                    }
                                   
                                    if (C.STATE != 0){
                                        C.STATE = 8;
                                        return C;
                                    }
                                }
                            } 

                            if (BKPDODIR){
                                StreamWriter WB2 = File.AppendText(C.BAKFPATH);
                                WB2.WriteLine("D:#:@:#:{0}:#:@:#:{1}", BKPTHFRM, BKPTHTO);
                                WB2.Close();
                            }

                        }
                    }
                    else{
                        Console.WriteLine("");
                        Console.WriteLine(">> PATH {0} IS NOT A DIRECTORY! PROGRAM WILL BE TERMINATED!", PTH);
                        Console.WriteLine("");
                        Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
                        Console.ReadKey();
                        C.STATE = 8;
                        return C;
                    } 
                }
                else {
                    if (File.Exists(PTH)){
                        string FPTH = PTH.FFULLPTH();
                        string FNAMEOLD = Path.GetFileName(FPTH);
                        string FEXT = FNAMEOLD.GETFEXT();
                        string FNAME = Path.GetFileNameWithoutExtension(FNAMEOLD).C2ASCII(C.CONVSPEC).Trim();
                        if (C.DONAMLN > 0) FNAME = FNAME.Substring(0, Math.Min(C.DONAMLN,FNAME.Length)).Trim();
                        PTH = PTH.GETDIRPT();
                        string DESTFILE = Path.Combine(PTH, FNAME+"."+FEXT);
                        int FSAVE = 0;
                        if (FNAME != Path.GetFileNameWithoutExtension(FPTH)){
                            DESTFILE = Path.Combine(PTH, FNAME+"."+FEXT);
                            string FRES = "";
                            bool ISGOOD = false;
                            int MINLEN = 1; //minimal unchanged length of name
                            if (File.Exists(DESTFILE)){
                                string WNAME = FNAME;
                                for (var q = 1; q < C.DONAMLN-(Math.Min(MINLEN, FNAME.Length)); q++){
	                                var x = C.DONAMLN-q-MINLEN;
	                                if (x < WNAME.Length)
		                                WNAME = WNAME.Substring(0, x);
                                    string WPATH = Path.Combine(PTH, WNAME);
	                                FRES = TestGenName(q, 1, WPATH+"#", FEXT);
                                    if (FRES != "")
                                        break;
                                }
                                do{
                                    if (FRES == ""){
                                        Console.WriteLine(">> FILE {0} ALREADY EXISTS! PLEASE, INPUT A NEW FILE NAME:", FNAME+""+FEXT);
                                        string REPLY = Console.ReadLine();
                                        //  REPLY = REPLY.DFORMAT();
                                        if (REPLY.PATHCHCK() && REPLY != ""){
                                            if (File.Exists(Path.Combine(PTH, REPLY))){
                                                Console.WriteLine(">> FILE {0} ALREADY EXISTS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>", REPLY);
                                                REPLY = Console.ReadLine();
                                                if (REPLY.ToUpper() == "Y"){
                                                    C.STATE = 8;
                                                    return C;
                                                }
                                            }
                                            else{
                                                FRES = Path.Combine(PTH, REPLY);
                                                ISGOOD = true;
                                            }
                                        }
                                        else{
                                            Console.WriteLine(">> FILE NAME {0} CONTAINS ILLEGAL CHARACTERS! DO YOU WANT TO EXIT THE PROGRAM EXECUTION? <Y/N>", REPLY);
                                            REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() == "Y"){
                                                C.STATE = 8;
                                                return C;
                                            }
                                        }
                                    }
                                    else
                                        ISGOOD = true;
                                } while (!ISGOOD);
                                if (ISGOOD)
                                    DESTFILE = FRES;
                            }
                        }

                        if (C.DOBAK == 1){
                            StreamWriter WB = File.AppendText(C.BAKFPATH);
                            WB.WriteLine("C:#:@:#:{0}:#:@:#:{1}", "FLOAD", FSAVE);
                            WB.WriteLine("F:#:@:#:{0}:#:@:#:{1}", FPTH, DESTFILE);
                            WB.Close();
                            FSAVE++;
                        }

                        int EXECMOV3 = 1;
                        foreach (string s in C.RESTRDIR){
                            if (FPTH.ToUpper().Contains(s) || DESTFILE.ToUpper().Contains(s)){
                                EXECMOV3 = 8;
                                break;
                            }
                        }

                        if (FPTH != DESTFILE){
                            P++;
                            if (EXECMOV3 == 1){
                                File.Move(FPTH, DESTFILE);
                                int EXPWAITT = 1;
                                int MAXEWAIT = 30;
                                while (EXECMOV3 == 1){
                                    DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                    bool BRKEXPRD = false;
                                    while (File.Exists(FPTH) && !File.Exists(DESTFILE) && !BRKEXPRD ) { 
                                        if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                        else System.Threading.Thread.Sleep(EXPWAITT);
                                    }
                                    if (BRKEXPRD) {
                                        EXECMOV3 = 0;
                                        while(BRKEXPRD){
                                            BRKEXPRD = false;
                                            Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", FPTH, DESTFILE, MAXEWAIT);
                                            string REPLY = Console.ReadLine();
                                            if (REPLY.ToUpper() == "X"){
                                                C.STATE = 8;
                                                return C;
                                            }
                                            else if (REPLY.ToUpper() == "W")
                                                EXECMOV3 = 1;
                                            else if (REPLY.ToUpper() == "S")
                                                break;
                                            else{
                                                Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                                BRKEXPRD = true;
                                            }
                                        }
                                    }
                                    else
                                        EXECMOV3 = 2;
                                };
                            }
                            if (EXECMOV3 == 2)
                                Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, FPTH, DESTFILE);
                            else if (EXECMOV3 == 1)
                                Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, FPTH, DESTFILE);
                        }

                        if (EXECMOV3 == 8)
                            Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, FPTH, DESTFILE);

                        string GENF = System.IO.Path.GetTempFileName();
                        StreamWriter GW = new StreamWriter(File.Open(GENF, FileMode.OpenOrCreate), Encoding.GetEncoding("UTF-8"));
                        StreamReader FW = new StreamReader(File.Open(DESTFILE, FileMode.Open), Encoding.GetEncoding("UTF-8")); 

                        string LINE = null;
                        int Li = 0;
                        while((LINE = FW.ReadLine()) != null){
                            string DESTLINE = LINE.C2ASCII(C.CONVSPEC).Trim();
                            GW.WriteLine(DESTLINE);
                            Li++;
                            if (C.DOBAK == 1){
                                StreamWriter WB = File.AppendText(C.BAKFPATH);
                                WB.WriteLine("L:#:@:#:{0}:#:@:#:{1}", LINE, Li);
                                WB.Close();
                            }
                        }

                        GW.Close();
                        FW.Close();

                        P++;
                        int EXECMOV2 = 1;
                        foreach (string s in C.RESTRDIR){
                            if (GENF.ToUpper().Contains(s) || DESTFILE.ToUpper().Contains(s)){
                                EXECMOV2 = 8;
                                break;
                            }
                        }
                        if (EXECMOV2 == 1){
                            File.Delete(DESTFILE);
                            File.Move(GENF, DESTFILE);
                            int EXPWAIT2 = 1;
                            int MAXEWAI2 = 30;
                            while (EXECMOV2 == 1){
                                DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAI2);
                                bool BRKEXPRD = false;
                                while (File.Exists(GENF) && !File.Exists(DESTFILE) && !BRKEXPRD ) { 
                                    if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                    else System.Threading.Thread.Sleep(EXPWAIT2);
                                }
                                if (BRKEXPRD) {
                                    EXECMOV2 = 0;
                                    while(BRKEXPRD){
                                        BRKEXPRD = false;
                                        Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", GENF, DESTFILE, MAXEWAI2);
                                        string REPLY = Console.ReadLine();
                                        if (REPLY.ToUpper() == "X"){
                                            C.STATE = 8;
                                            return C;
                                        }
                                        else if (REPLY.ToUpper() == "W")
                                            EXECMOV2 = 1;
                                        else if (REPLY.ToUpper() == "S")
                                            break;
                                        else{
                                            Console.WriteLine("> INVALID REPLY. PLEASE, TRY IT AGAIN.");
                                            BRKEXPRD = true;
                                        }
                                    }
                                }
                                else
                                    EXECMOV2 = 2;
                            };
                        }
                        if (EXECMOV2 == 2)
                            Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, GENF, DESTFILE);
                        else
                            Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, GENF, DESTFILE);
                    }
                    else{
                        Console.WriteLine("");
                        Console.WriteLine(">> PATH {0} IS NOT A FILE! PROGRAM WILL BE TERMINATED!", PTH);
                        Console.WriteLine("");
                        Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
                        Console.ReadKey();   
                        C.STATE = 8;
                        return C;
                    }
                }
                C.STATE = 0;
                return C;
            } catch(Exception Ex){
                Console.WriteLine("");
                Console.WriteLine(">> ERROR {0} HAS OCCURED! PROGRAM WILL BE TERMINATED!", Ex);
                Console.WriteLine("");
                Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
                Console.ReadKey();
                C.STATE = 8;
                return C;
            }
        }
    }
}
