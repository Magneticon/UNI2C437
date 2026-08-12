using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UNI2C437 {
    public static class BAKDATA {
        public static CONTEXT SETBKFIL(this CONTEXT C){
            try{
                StreamWriter WB = File.CreateText(C.BAKFPATH);
                WB.WriteLine("UNI2C437 PROGRAM BACKUP FILE");
                WB.WriteLine("");
                WB.WriteLine("CREATED IN {0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                WB.WriteLine("");
                WB.WriteLine("----------------------------------------------");
                WB.WriteLine("");
                WB.Close();
                C.STATE = 0;
            }catch(Exception Ex){
                Console.WriteLine(">> BACKUP FILE CANNOT BE CREATED IN LOCATION {0}. PROGRAM WILL BE TERMINATED.", C.BAKFPATH);
                C.STATE = 8;
            }
            return C;
        }

        //procedure  for renaming files/text content from the backup
        //backup path is storeed in this variable: BAKFPATH
        public static CONTEXT LOADRENM(this CONTEXT C){
            string REPLY = "";
            try{
                if (File.Exists(C.BAKFPATH)){
                    string FIRSTFS = "";
                    string FIRSTFD = "";
                    int DOALWAYS = 0;
                    int SDOALWAY = 0;
                    int FILEINIT = 0;
                    bool ISOK = true;

                    int P = 0;

                    try{
                        using (var r = new StreamReader(File.Open(C.BAKFPATH, FileMode.Open), Encoding.GetEncoding("UTF-8"))){
                            string LINE = null;
                            while((LINE = r.ReadLine()) != null){
                                string[] PLINE = LINE.Split(":#:@:#:");
                                if (PLINE.Length < 3) continue;
                                PLINE[0] = PLINE[0].Trim().ToUpper();
                                if (PLINE[0] == "D" || PLINE[0] == "F"){
                                    PLINE[1] = PLINE[1].Trim();
                                    PLINE[2] = PLINE[2].Trim();
                                }
                                else for (int I = 3; I < PLINE.Length; I++) PLINE[2] += ":#:@:#:"+PLINE[I];

                                ISOK = true;

                                if (PLINE[1] == "FLOAD")
                                    FILEINIT = 1;
                                else if (PLINE[0] == "D"){
                                    try{
                                        int EXECMOVE = 1;
                                        if (!Directory.Exists(PLINE[2])) Directory.CreateDirectory(PLINE[1]);
                                        else{
                                            foreach (string s in C.RESTRDIR){
                                                if (PLINE[1].ToUpper().Contains(s) || PLINE[2].ToUpper().Contains(s)){
                                                    EXECMOVE = 8;
                                                    break;
                                                }
                                            }
                                            if (EXECMOVE == 1){
                                                DirectoryInfo RD = new DirectoryInfo(PLINE[2]);
                                                RD.MoveTo(PLINE[1]);
                                                int EXPWAITT = 1;
                                                int MAXEWAIT = 30;
                                                while (EXECMOVE == 1){
                                                    DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                                    bool BRKEXPRD = false;
                                                    while (Directory.Exists(PLINE[2]) && !Directory.Exists(PLINE[1]) && !BRKEXPRD ) { 
                                                        if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                                        else System.Threading.Thread.Sleep(EXPWAITT);
                                                    }
                                                    if (BRKEXPRD) {
                                                        EXECMOVE = 0;
                                                        while(BRKEXPRD){
                                                            BRKEXPRD = false;
                                                            Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE DIRECTORY PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", PLINE[2], PLINE[1], MAXEWAIT);
                                                            REPLY = Console.ReadLine();
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
                                        }
                                        if (EXECMOVE == 2)
                                            Console.WriteLine("[{0}]: DIRECTORY PROCESSED: {1} -> {2}", P, PLINE[2], PLINE[1]);  
                                        else throw new Exception();
                                    } catch(Exception Ex){
                                        Console.WriteLine("[{0}]: >> DIRECTORY {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PLINE[2], PLINE[1]);
                                    }
                                }
                                else if (PLINE[0] == "F"){
                                    while (!File.Exists(PLINE[2]) && FILEINIT == 0){
                                        ISOK = true;
                                        if (SDOALWAY == 1){
                                            ISOK = false;
                                            break;
                                        }
                                        Console.WriteLine("");
                                        Console.WriteLine("> SOURCE FILE {0} DOES NOT EXIST! DO YOU WANT TO SKIP IT? (YES/NO/YES ALWAYS/NO ALWAYS/EXIT) <Y/N/YA/NA/X>", PLINE[2]);     
                                        REPLY = Console.ReadLine();
                                        switch(REPLY.Trim().ToUpper()){
                                            case "Y":
                                                ISOK = false;
                                                break;
                                            case "YA":
                                                ISOK = false;
                                                SDOALWAY = 1;
                                                break;
                                            case "X":
                                                C.STATE = 8;
                                                return C;
                                            default:
                                                Console.WriteLine(">> PUT THE FILE TO THE REQUIRED LOCATION ({0}) AND PRESS ANY KEY TO CONTINUE ...", PLINE[2]);
                                                Console.ReadKey();
                                                break;
                                        } 
                                        if (!ISOK) break;
                                    }

                                    if (!ISOK) continue;

                                    while(File.Exists(PLINE[1])){
                                        ISOK = true;
                                        if (DOALWAYS == 1 || FILEINIT != 0){
                                            File.Delete(PLINE[1]);
                                            break;
                                        }
                                        if (DOALWAYS == 2){
                                            ISOK = false;
                                            break;
                                        }
                                        Console.WriteLine("");
                                        Console.WriteLine("> DESTINATION FILE {0} ALREADY EXIST! DO YOU WANT TO OVERWRITE IT? (YES/NO/YES ALWAYS/NO ALWAYS/EXIT) <Y/N/YA/NA/X>", PLINE[1]);
                                        REPLY = Console.ReadLine();
                                        switch(REPLY.Trim().ToUpper()){
                                            case "Y":
                                                File.Delete(PLINE[1]);
                                                break;
                                            case "YA":
                                                File.Delete(PLINE[1]);
                                                DOALWAYS = 1;
                                                break;
                                            case "NA":
                                                DOALWAYS = 2;
                                                ISOK = false;
                                                break;
                                            case "X":
                                                C.STATE = 8;
                                                return C;
                                            default:
                                                ISOK = false;
                                                break;
                                        } 
                                        if (!ISOK) break;
                                    }

                                    int EXECMOVE = 1;

                                    if (!ISOK) continue;
                                    if (FILEINIT != 0 && !File.Exists(PLINE[1])){
                                        StreamWriter FW = new StreamWriter(File.Open(PLINE[1], FileMode.OpenOrCreate), Encoding.GetEncoding("UTF-8"));
                                        FW.Close();
                                        EXECMOVE = 2;
                                    }
                                    try{
                                        if (EXECMOVE == 1){
                                            foreach (string s in C.RESTRDIR){
                                                if (PLINE[1].ToUpper().Contains(s) || PLINE[2].ToUpper().Contains(s)){
                                                    EXECMOVE = 8;
                                                    break;
                                                }
                                            }
                                        }
                                        if (EXECMOVE == 1){
                                            File.Move(PLINE[2], PLINE[1]);
                                            int EXPWAITT = 1;
                                            int MAXEWAIT = 30;
                                            while (EXECMOVE == 1){
                                                DateTime EXPBREAK = DateTime.Now.AddSeconds(MAXEWAIT);
                                                bool BRKEXPRD = false;
                                                while (File.Exists(PLINE[2]) && !File.Exists(PLINE[1]) && !BRKEXPRD ) { 
                                                    if (DateTime.Now.CompareTo(EXPBREAK) > 0) BRKEXPRD = true;
                                                    else System.Threading.Thread.Sleep(EXPWAITT);
                                                }
                                                if (BRKEXPRD) {
                                                    EXECMOVE = 0;
                                                    while(BRKEXPRD){
                                                        BRKEXPRD = false;
                                                        Console.WriteLine(">> LIMIT FOR MOVING {0} TO {1} HAS REACHED DEFINED THRESHOLD ({2}). DO YOU WANT TO SKIP THE FILE PROCESSING / WAIT ADDITIONAL {2} SECONDS OR TERMINATE THE PROGRAM PROCESSING? <S/W/X>", PLINE[2], PLINE[1], MAXEWAIT);
                                                        REPLY = Console.ReadLine();
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
                                        if (EXECMOVE == 2){
                                            P++;
                                            Console.WriteLine("[{0}]: FILE PROCESSED: {1} -> {2}", P, PLINE[2], PLINE[1]);
                                            FIRSTFS = PLINE[1];
                                        }
                                        else throw new Exception();
                                    } catch(Exception Ex){
                                        Console.WriteLine("[{0}]: >> FILE {1} -> {2} CANNOT BE PROCESSED. SKIPPING.", P, PLINE[2], PLINE[1]);
                                    }
                                }
                                else if (PLINE[0] == "L"){
                                     //   if (FIRSTFS == "" || FIRSTFD == ""){
                                        if (FIRSTFS == ""){
                                            ISOK = false;
                                            Console.WriteLine("");
                                            Console.WriteLine(">> BACKUP FILE IS CORRUPTED! SOURCE/TARGET FILE DEFINITION STATEMENT IS MISSING! PROGRAM WILL BE TERMINATED!");
                                            Console.WriteLine("");
                                            Console.WriteLine("> Press any key to continue ...");
                                            Console.ReadKey();
                                            C.STATE = 8;
                                            return C;
                                        }
                                 //       if (ISOK)
                                    //        FILEINIT = 2;

                                    if (ISOK){
                                        StreamWriter FW = new StreamWriter(File.Open(FIRSTFS, FileMode.Append), Encoding.GetEncoding("UTF-8"));
                                        FW.WriteLine(PLINE[1]);
                                        FW.Close();
                                    }
                                }
                            };
                        }
                    }
                    catch(Exception Ex){
                        Console.WriteLine(">> ERROR WHEN READING A BACKUP FILE! {0}", Ex);
                        C.STATE = 4;   
                        return C;
                    }

                    Console.WriteLine("");
                    Console.WriteLine(">> BACKUP PROCESSING WAS FINISHED.");
                    Console.WriteLine("");
                    Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
                    Console.ReadKey();
                    C.STATE = 0;
                    return C;
                }
                else{
                    Console.WriteLine("");
                    Console.WriteLine(">> PATH {0} IS NOT A FILE! PROGRAM WILL BE TERMINATED!", C.BAKFPATH);
                    Console.WriteLine("");
                    Console.WriteLine("> PRESS ANY KEY TO CONTINUE ...");
                    Console.ReadKey();
                    C.STATE = 8;
                    return C;
                }
            } catch (Exception Ex){
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
