using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UNI2C437 {
    public static class SETLDR {
        public static CONTEXT GETLOAD(this CONTEXT C, string PTH){
            C.STATE = 0;
            try{
                using (var r = new StreamReader(File.Open(PTH, FileMode.Open), Encoding.GetEncoding("UTF-8"))){
                    string SET = null;
                    while((SET = r.ReadLine()) != null){
                        SET = SET.ToString().Trim().ToUpper();
                        string[] SETGET = SET.Split(":$:");
                        if (SETGET.Length > 1){ 
                            SETGET[1] = SETGET[1].Trim();
                            switch(SETGET[0].Trim()){
                                case "CONVMODE":
                                    C.CONVMODE = SETGET[1].ISDIGIT() ? Int32.Parse(SETGET[1]) : C.CONVMODE;
                                    break;
                                case "OPERMODE":
                                    C.OPERMODE = SETGET[1].ISDIGIT() ? Int32.Parse(SETGET[1].Trim()) : C.OPERMODE;
                                    break;
                                case "DOSUBDIR":
                                    C.DOSUBDIR = SETGET[1].ISDIGIT() ? Int32.Parse(SETGET[1].Trim()): C.DOSUBDIR;
                                    break;
                                case "DOBAK":
                                    C.DOBAK = SETGET[1].ISDIGIT() ? Int32.Parse(SETGET[1].Trim()) : C.DOBAK;
                                    break;
                                case "BAKFPATH":
                                    C.BAKFPATH = SETGET[1].Trim();
                                    break;
                                case "DONAMLN":
                                    C.DONAMLN = SETGET[1].ISDIGIT() ? Int32.Parse(SETGET[1].Trim()) : C.DONAMLN;
                                    break;
                                case "CONVSPEC":
                                    C.CONVSPEC = SETGET[1][0];
                                    break;
                            }
                        }
                    }
                }
            } catch(Exception Ex) {
                Console.WriteLine(">> ERROR WHEN READING A SETTING FILE!");
                C.STATE = 4;
            }
            return C;
        }

        public static bool SETLOAD(this CONTEXT C, string PTH){
            bool res = true;
            try{
                if (File.Exists(PTH)) File.Delete(PTH);
                using (var w = new StreamWriter(File.Open(PTH, FileMode.CreateNew), Encoding.GetEncoding("UTF-8")))
                {
                    w.WriteLine("UNI2C437 PROGRAM SETTING FILE");
                    w.WriteLine("");
                    w.WriteLine("CREATED IN {0}/{1}/{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    w.WriteLine("");
                    w.WriteLine("----------------------------------------------");
                    w.WriteLine("");
                    w.WriteLine("CONVMODE:$: "+C.CONVMODE);
                    w.WriteLine("OPERMODE:$: "+C.OPERMODE);
                    w.WriteLine("DOSUBDIR:$: "+C.DOSUBDIR);
                    w.WriteLine("DOBAK:$: "+C.DOBAK);
                    w.WriteLine("BAKFPATH:$: "+C.BAKFPATH);
                    w.WriteLine("DONAMLN:$: "+C.DONAMLN);
                    w.WriteLine("CONVSPEC:$: "+C.CONVSPEC);
                }
            } catch(Exception Ex) {
                res = false;
            }
            return res;
        }
    }
}
