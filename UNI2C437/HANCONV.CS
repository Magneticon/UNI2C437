using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UNI2C437 {
    public static class HANCONV {

        private static string[] H1 = {"g", "kk", "n", "d", "tt", "r", "m", "b", "pp", "s", "ss", "￭", "j", "jj", "ch", "kh", "th", "ph", "h"};
        private static string[] H2 = {"a", "ae", "ya", "yae", "eo", "e", "yeo", "ye", "o", "wa", "wae", "oe", "yo", "u", "weo", "we", "wi", "yu", "eu", "yi", "i"};
        private static string[] H3 = {"￭", "g", "kk", "gs", "n", "nj", "lh", "d", "l", "lg", "lm", "lb", "ls", "lth", "lph", "lh", "m", "b", "bs", "s", "ss", "ng", "j", "ch", "kh", "th", "ph", "h"};
        private static char[] L1 = {'ﾠ', 'ﾡ', 'ﾢ', 'ﾣ', 'ﾤ', 'ﾥ', 'ﾦ', 'ﾧ', 'ﾨ', 'ﾩ', 'ﾪ', 'ﾫ', 'ﾬ', 'ﾭ', 'ﾮ', 'ﾯ', 'ﾰ', 'ﾱ', 'ﾲ', 'ﾳ', 'ﾴ', 'ﾵ', 'ﾶ', 'ﾷ', 'ﾸ', 'ﾹ', 'ﾺ', 'ﾻ', 'ﾼ', 'ﾽ', 'ﾾ', 'ￂ', 'ￃ', 'ￄ', 'ￅ', 'ￆ', 'ￇ', 'ￊ', 'ￋ', 'ￌ', 'ￍ', 'ￎ', 'ￏ', 'ￒ', 'ￓ', 'ￔ', 'ￕ', 'ￖ', 'ￗ', 'ￚ', 'ￛ', 'ￜ'};
        private static string[] L2 = {" ", "g", "kk", "ks", "n", "nj", "nh", "d", "tt", "l", "lg", "lm", "lp", "ls", "lth", "lph", "lh", "m", "b", "pp", "bs", "s", "ss", "ng", "j", "jj", "ch", "kh", "th", "ph", "h", "a", "ae", "ya", "yae", "eo", "e", "yeo", "ye", "o", "wa", "wae", "oe", "yo", "u", "weo", "we", "wi", "yu", "eu", "yi", "i"};

        private static int H1ST = 44032;
        private static int H2ST = 0;
        private static int H3ST = 0;
        
        //CONVERT TO HANGUL:

        public static string String2Hangul(this string ASCIIStr){
	        ASCIIStr = ASCIIStr.Replace("￭", string.Empty);
	        string WorkStr = ASCIIStr.ToLower();
	        string Res = "";
            for (int a1 = 0; a1 < WorkStr.Length; a1++){
                int i1 = a1;
                int[] Indexes = {0, 0, 0};
                int m1 = 0;
                int n1 = 0;
                for (int b1 = 0; b1 < H1.Length; b1++){
                    if (WorkStr.Length >= i1+H1[b1].Length && WorkStr.Substring(i1, H1[b1].Length) == H1[b1] && m1 < H1[b1].Length){
                        m1 = H1[b1].Length;
                        n1 = b1;
                    }
                }
                if (m1 == 0) Indexes[0] = 11;
                else Indexes[0] = n1;
                i1 += m1;
                m1 = 0;
                n1 = 0;
                for (int b1 = 0; b1 < H2.Length; b1++){
                    if (WorkStr.Length >= i1+H2[b1].Length && WorkStr.Substring(i1, H2[b1].Length) == H2[b1] && m1 < H2[b1].Length){
                        m1 = H2[b1].Length;
                        n1 = b1;
                    }
                }
                if (m1 == 0){
                    Res += ASCIIStr[a1];
                    continue;
                }
                else{
                    Indexes[1] = n1;
                    i1 += m1;
                    m1 = 0;
                    n1 = 0;
                    for (int b1 = 0; b1 < H3.Length; b1++){
                        if (WorkStr.Length >= i1+H3[b1].Length && WorkStr.Substring(i1, H3[b1].Length) == H3[b1] && m1 < H3[b1].Length){
                            m1 = H3[b1].Length;
                            n1 = b1;
                        }
                    }                 
                    if (m1 > 0){
                        Indexes[2] = n1;
                        i1 += m1;
                    }
                }
                int HanIndex = H1ST+H2ST+H3ST+(Indexes[0]*H2.Length*H3.Length)+(Indexes[1]*H3.Length)+(Indexes[2]);
                string HHex = HanIndex.ToString("X");
                byte[] HByteArr = Enumerable.Range(0, HHex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(HHex.Substring(x, 2), 16))
                    .ToArray();
                Res += System.Text.Encoding.BigEndianUnicode.GetString(HByteArr);
                a1 = i1-1;
            }
            return Res;
        }

        // CONVERT FROM HANGUL

        public static string Hangul2String(this string HangulStr){
	        string Res = "";
	        int WasLastDot = 1;
	        foreach (char HanChar in HangulStr){
		        if (WasLastDot == 2) WasLastDot = 0;
		        if (HanChar == '.' || HanChar == ',' || HanChar == '!' || HanChar == '?' || HanChar == '。'){
			        WasLastDot = 1;
			        Res += HanChar+"";
			        continue;
		        }
		        else if (HanChar == ' ' || HanChar == 'ﾠ'){
                    Res += ' ';
                    continue;
                }
                else if (WasLastDot == 1) WasLastDot = 2;               
		        int HanIndex = HanChar;
 		        if (HanIndex > H1ST-1 && HanIndex < (H1ST+H2ST+H3ST+(H1.Length*H2.Length*H3.Length))){
			        HanIndex -= H1ST;
                    int I1 = 0;
                    while (HanIndex/(H2.Length*H3.Length) > 0){
                        I1 += 1;
                        HanIndex -= (H2.Length*H3.Length);
                    }
                    int I2 = 0;
                    while (HanIndex/H3.Length > 0){
                        I2 += 1;
                        HanIndex -= H3.Length;
                    }
			        int I3 = -1;
			        if (HanIndex > 0) I3 = HanIndex;
			        string AddStr = "";
			        if (I1 > -1 && I1 < H1.Length) AddStr += H1[I1];
			        if (I2 > -1 && I2 < H2.Length) AddStr += H2[I2];
			        if (I3 > -1 && I3 < H3.Length) AddStr += H3[I3];
			        if (AddStr != ""){
				        if (WasLastDot == 2){
                            Res += (""+AddStr[0]).ToUpper();
                            if (I1 == 11){
                                Res += (""+AddStr[1]).ToUpper();
                                Res += AddStr.Substring(2);
                            }
                            else Res += AddStr.Substring(1);
				        }
				        else Res += AddStr;
			        }
			        continue;
		        }
		        else if (HanIndex < H1ST){
			        Res += HanChar+"";
                    WasLastDot = 0;
			        continue;
		        }
		        else{
                    int Idx = Array.IndexOf(L1, HanChar);
			        if (Idx > -1) Res += L2[Idx];
			        else Res += HanChar+"";
			        continue;	
		        }
	        }
	        Res = Res.Replace("￭", string.Empty);
	        return Res;
        }
    }
}
