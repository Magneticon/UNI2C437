using System;
using System.Collections.Generic;
using System.Text;

namespace UNI2C437 {
    public static class MAINCONV {
        //CONV TABLE

        //ALLOWED ASCII CHARACTERS

        public static char[] CA = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

        //PASS WITHOUT MODIFICATION
        private static char[] C0 = {'=', '#', '$', '%', '&', '*', '+', '\'', '"', ':', '!', '?', ';', ')', '(', ',', '.', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '-', '_'};
        //SPECIAL CHARACTER CONVERSION
        private static char[] C1 = {'[', ']', '{', '}', '<', '>', '〈', '〉', '«', '»', '【', '】', '・'};
        private static string[] D1 = {"(", ")", "(", ")", "(", ")", "(", ")", "(", ")", "(", ")", "-"};
        //EXTENDED ASCII CHARACTER CONVERSION
        private static char[] C2 = {'Ě', 'Š', 'Č', 'Ř', 'Ž', 'Ý', 'Á', 'Í', 'É', 'Ú', 'Ů', 'Ü', 'Ö', 'Ó', 'Ä', 'Ë', 'Ď', 'Ť', 'Ľ', 'Ň', 'À', 'Ï', 'Å', 'Æ', 'Ç', 'È', 'Ê', 'Ì', 'Î', 'Ð', 'Ñ', 'Ò', 'Õ', 'Ø', 'Ù', 'Û', 'Þ', 'ß', 'Ā', 'Ă', 'Ą', 'Ć', 'Ċ', 'Đ', 'Ē', 'Ĕ', 'Ė', 'Ę', 'Ĝ', 'Ğ', 'Ġ', 'Ģ', 'Ĥ', 'Ħ', 'Ĩ', 'Ī', 'Ĭ', 'Į', 'İ', 'Ã', 'Ű', 'Ő'};
        private static string[] D2 = {"e", "s", "c", "r", "z", "y", "a", "i", "e", "u", "u", "u", "o", "o", "a", "e", "d", "t", "l", "n", "a", "i", "a", "ae", "c", "e", "e", "i", "i", "d", "n", "o", "o", "o", "u", "u", "d", "ss", "a", "a", "a", "c", "c", "d", "e", "e", "e", "e", "g", "g", "g", "g", "h", "h", "i", "i", "i", "i", "i", "a", "u", "o"};
        //CYRILIC CHARACTER CONVERSION
        private static char[] C3 = {'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ь', 'Э', 'Ю', 'Я', 'Е', 'Ө', 'Ъ', 'Ы', 'Ә', 'Ғ', 'Қ', 'Ң', 'Ұ', 'Һ', 'І'};
        private static string[] D3 = {"a", "b", "v", "g", "d", "e", "zh", "z", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shcha", "", "e", "yu", "ya", "e", "o", "", "y", "a", "gh", "q", "ng", "u", "h", "i"};
        //HIRAGANA CHARACTER CONVERSION
        private static char[] C4 = {'あ', 'い', 'う', 'え', 'お', 'か', 'が', 'き', 'ぎ', 'く', 'ぐ', 'け', 'げ', 'こ', 'ご', 'さ', 'ざ', 'し', 'じ', 'す', 'ず', 'せ', 'ぜ', 'そ', 'ぞ', 'た', 'だ', 'ち', 'ぢ', 'つ', 'づ', 'て', 'で', 'と', 'ど', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ば', 'ぱ', 'ひ', 'ぴ', 'び', 'ふ', 'ぶ', 'ぷ', 'へ', 'べ', 'ぺ', 'ほ', 'ぼ', 'ぽ', 'ま', 'み', 'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'り', 'る', 'れ', 'ろ', 'わ', 'ゐ', 'ゑ', 'を', 'ん', 'ゔ'};
        private static string[] D4 = {"a", "i", "u", "e", "o", "ka", "ga", "ki", "gi", "ku", "gu", "ke", "ge", "ko", "go", "sa", "za", "si", "zi", "su", "zu", "se", "ze", "so", "zo", "ta", "da", "ti", "di", "tu", "du", "te", "de", "to", "do", "na", "ni", "nu", "ne", "no", "ha", "ba", "pa", "hi", "pi", "bi", "hu", "bu", "pu", "he", "be", "pe", "ho", "bo", "po", "ma", "mi", "mu", "me", "mo", "ya", "yu", "yo", "ra", "ri", "ru", "re", "ro", "wa", "wi", "we", "wo", "n", "vu"};
        //KATAKANA CHARACTER CONVERSION
        private static char[] C5 = {'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'ガ', 'キ', 'ギ', 'ク', 'グ', 'ケ', 'ゲ', 'コ', 'ゴ', 'サ', 'ザ', 'シ', 'ジ', 'ス', 'ズ', 'セ', 'ゼ', 'ソ', 'ゾ', 'タ', 'ダ', 'チ', 'ヂ', 'ツ', 'ヅ', 'テ', 'デ', 'ト', 'ド', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'バ', 'パ', 'ヒ', 'ビ', 'ピ', 'フ', 'ブ', 'ブ', 'プ', 'ヘ', 'ベ', 'ペ', 'ホ', 'ボ', 'ポ', 'マ', 'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ヰ', 'ヱ', 'ヲ', 'ン', 'ヴ', 'ヷ', 'ヸ', 'ヹ', 'ヺ'};
        private static string[] D5 = {"a", "i", "u", "e", "o", "ka", "ga", "ki", "gi", "ku", "gu", "ke", "ge", "ko", "go", "sa", "za", "si", "zi", "su", "zu", "se", "ze", "so", "zo", "ta", "da", "ti", "di", "tu", "du", "te", "de", "to", "do", "na", "ni", "nu", "ne", "no", "ha", "ba", "pa", "hi", "pi", "bi", "hu", "bu", "pu", "he", "be", "pe", "ho", "bo", "po", "ma", "mi", "mu", "me", "mo", "ya", "yu", "yo", "ra", "ri", "ru", "re", "ro", "wa", "wi", "we", "wo", "n", "vu", "va", "vi", "ve", "vo"};
        
        // CONVERSION TO ASCII PROCEDURE
        //convspec == '¾' - unknown  characters will be preserved, otherwise converted to CONVSPEC character.
        public static string C2ASCII(this string UNICODE, char CONVSPEC){
            string Res = "";
            UNICODE = UNICODE.Hangul2String();
            string WorkStr = UNICODE.ToUpper();
            int WasLastDot = 1;
            for (int a1 = 0; a1 < UNICODE.Length; a1++){
		        if (WasLastDot == 2) WasLastDot = 0;
		        if (UNICODE[a1] == '.' || UNICODE[a1] == ',' || UNICODE[a1] == '!' || UNICODE[a1] == '?' || UNICODE[a1] == '。'){
			        WasLastDot = 1;
			        Res += UNICODE[a1]+"";
			        continue;
		        }
		        else if (UNICODE[a1] == ' ' || UNICODE[a1] == 'ﾠ'){
                    Res += ' ';
                    continue;
                }
                else if (WasLastDot == 1) WasLastDot = 2;  
                bool WasSet = false;
                for (int b1 = 0; b1 < C0.Length; b1++){
                    if (WorkStr[a1] == C0[b1]){
                        Res += UNICODE[a1];
                        WasSet = true;
                        break;
                    }
                }   
                if (WasSet) continue;
                for (int b2 = 0; b2 < C1.Length; b2++){
                    if (WorkStr[a1] == C1[b2]){
                        if (UNICODE[a1] == Char.ToUpper(UNICODE[a1])) Res += D1[b2].ToUpper();
                        else Res += D1[b2];
                        WasSet = true;
                        break;
                    }
                }
                if (WasSet) continue;
                for (int b3 = 0; b3 < C2.Length; b3++){
                    if (WorkStr[a1] == C2[b3]){
                        if (UNICODE[a1] == Char.ToUpper(UNICODE[a1])) Res += D2[b3].ToUpper();
                        else Res += D2[b3];
                        WasSet = true;
                        break;
                    }
                }
                if (WasSet) continue;
                for (int b4 = 0; b4 < C3.Length; b4++){
                    if (WorkStr[a1] == C3[b4]){
                        if (UNICODE[a1] == Char.ToUpper(UNICODE[a1])) Res += D3[b4].ToUpper();
                        else Res += D3[b4];
                        WasSet = true;
                        break;
                    }
                }
                if (WasSet) continue;
                for (int b5 = 0; b5 < C4.Length; b5++){
                    if (WorkStr[a1] == C4[b5]){
                        if (WasLastDot == 2) Res += Char.ToUpper(D4[b5][0]);
                        else Res += D4[b5][0];
                        Res += D4[b5].Substring(1);
                        WasSet = true;
                        break;
                    }
                }
                if (WasSet) continue;
                for (int b6 = 0; b6 < C5.Length; b6++){
                    if (WorkStr[a1] == C5[b6]){
                        if (WasLastDot == 2) Res += Char.ToUpper(D5[b6][0]);
                        else Res += D5[b6][0];
                        Res += D5[b6].Substring(1);
                        WasSet = true;
                        break;
                    }
                }
                if (WasSet) continue;
                if (CONVSPEC == '¾')
                    Res += UNICODE[a1];
                else
                    Res += CONVSPEC;
            }
            return Res;
        }
    }
}
