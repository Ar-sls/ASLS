using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using WebApi.Entities;
using MySql.Data.MySqlClient;
using Snowball;

namespace WebApi.Helpers
{
    public class Helper
    {
        /********************Helper Function for GetBlogs******************************************************/
        private static bool IsSepratorchar(char c)
        {
            string AllArabicChar = "دجحخهعغفقثصضطكمنتالبيسشظزوةىرؤءئأإ";
            if (AllArabicChar.IndexOf(c) > 0)
                return false;

            return true;
        }
        private static int ExactWordfromSentence(string sentence, string Word)
        {
            int Index = 0;
            Index = sentence.IndexOf(Word, Index);
            if (Index == 0 && Word.Length == sentence.Length)
                return Index;

            if (Index == 0 && Word.Length < sentence.Length && IsSepratorchar(sentence[Word.Length]))
                return Index;

            while (Index > 0)
            {

                if (IsSepratorchar(sentence[Index - 1]) && Index + Word.Length < sentence.Length && IsSepratorchar(sentence[Index + Word.Length]))
                    return Index;

                if (IsSepratorchar(sentence[Index - 1]) && Index + Word.Length == sentence.Length)
                    return Index;
                Index = sentence.IndexOf(Word, Index + 1);
            }
            return -1;

        }
        private static string SafeReplace(string input, string find, string replace, bool matchWholeWord)
        {
            string textToFind = matchWholeWord ? string.Format(@"\b{0}\b", find) : find;
            return Regex.Replace(input, textToFind, replace);
        }

        public static List<SentenceList> WordProcess(DataTable DT, string searchWord, string guid)
        {

            SentenceList sentenceList = new SentenceList();
            List<SentenceList> sentenceListOut = new List<SentenceList>();

            string targetpath = Directory.GetCurrentDirectory() + "\\BlogsResult";
            string newPath = Path.Combine(targetpath, guid);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            string filenameDynamic = "OrginalSentences.txt";
            string OrginalSentencefile = "";
            int linecount = 0;
            foreach (DataRow row in DT.Rows)
            {

                int rowIndex = DT.Rows.IndexOf(row);
                string description = row["description"].ToString();
                string Sentence = "";
                string sentenceWithHTML = "";
                int WordCount = 0;
                description = description.Replace("\r\n", " ");
                description = description.Replace("...", ". ");
                description = description.Replace("..", ". ");
                description = description.Replace(".", ". ");

                description = description.Replace(" .", ".");

                description = description.Replace("،،،", "، ");
                description = description.Replace("،،", "، ");
                description = description.Replace("،", "، ");

                description = description.Replace("\t", " ");

                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                description = regex.Replace(description, " ");

                int WordPlace = ExactWordfromSentence(description, searchWord);

                if (WordPlace > -1)
                {
                    for (int i = WordPlace; i < description.Length; i++)
                    {
                        Sentence += description[i];
                        if (description[i] == ' ')
                            WordCount++;
                        if (WordCount > 7)
                            break;
                    }
                    for (int i = WordPlace - 1; i >= 0; i--)
                    {
                        Sentence = description[i] + Sentence;
                        if (description[i] == ' ')
                            WordCount++;
                        if (WordCount == 16)
                            break;
                    }
                }
                sentenceWithHTML = SafeReplace(Sentence, searchWord, "<span style=\"color:red;font-weight:700;position:relative\")\" >" + searchWord + "</span>", true);

                sentenceList = new SentenceList();
                sentenceList.sentence = Sentence;
                sentenceList.sentenceHTML = sentenceWithHTML;
                sentenceListOut.Add(sentenceList);
                linecount++;
                OrginalSentencefile += sentenceList.sentence.Replace("\r\n", " ").Replace("\n", " ");
                if (linecount < DT.Rows.Count)
                    OrginalSentencefile += "\r\n";
            }

            System.IO.File.WriteAllText(Path.Combine(newPath, filenameDynamic), OrginalSentencefile, Encoding.UTF8);

            return sentenceListOut;
        }

        /********************Helper Function for GetCleanBlogs*************************************************/

        public static string RemoveUnArabicChar(string SentenceWords)
        {
            string outputstr = "";
            string Arabic = "شؤيثبلاهتنمةىخحضقسفعرصءغئأآإجدظزوكطذ ";
            for (int i = 0; i < SentenceWords.Length; i++)
            {
                if (Arabic.IndexOf(SentenceWords[i]) >= 0)
                    outputstr += SentenceWords[i];
                else
                    outputstr += " ";
            }
            return outputstr;

        }
        public static string RemoveDoubleSpaces(string SentenceWords)
        {
            string oldstr = SentenceWords;
            SentenceWords = SentenceWords.Replace("  ", " ");

            while (oldstr != SentenceWords)
            {
                oldstr = SentenceWords;
                SentenceWords = SentenceWords.Replace("  ", " ");
            }
            return SentenceWords;
        }
        public static string RemoveDiac(string ModelSearchWord)
        {
            string searchword = ModelSearchWord;
            searchword = searchword.Replace("ّ", "");
            searchword = searchword.Replace("َ", "");
            searchword = searchword.Replace("ً", "");
            searchword = searchword.Replace("ُ", "");
            searchword = searchword.Replace("ٌ", "");
            searchword = searchword.Replace("ِ", "");
            searchword = searchword.Replace("ٍ", "");
            searchword = searchword.Replace("ْ", "");
            searchword = searchword.Replace("ـ", "");

            return searchword;
        }

        /********************Helper Function for GetStemBlogs***************************************************/

        private static string[] StopWords = { "ا . ه", "اثنان", "اثنتان", "الألى", "الآن", "التي", "الذي", "الذين", "الر", "اللا", "اللاتي", "اللائي", "اللتان", "اللذان", "اللهم", "اللواتي", "الم", "المر", "المص", "ايم", "إ", "إبّان", "إذ", "إذا", "إذما", "إذن", "إزاء", "إلا", "إلام", "إلخ", "إلى", "إما", "إن", "إنما", "إي", "إياك", "إياكم", "إياكما", "إياكن", "إيانا", "إياه", "إياها", "إياهم", "إياهما", "إياهن", "إياي", "أ . ح", "أ . د", "أ . م", "أ هـ", "أبدا", "أجل", "أدنى", "أربع", "أربعة", "أربعمائة", "أربعمئة", "أربعون", "أربعينيات", "أسفل", "أعلى", "أفرو", "أل", "ألبتة", "ألف", "أم", "أمام", "أنا", "أنت", "أنتم", "أنتما", "أنتن", "أنجلو", "أنغلو", "أنى", "أو", "أوسط", "أولاء", "أولئك", "أولئكم", "أونا", "أي", "أيّان", "أيا", "أية", "أيتها", "أيضا", "أيما", "أين", "أينما", "أيها", "آنذاك", "آنئذ", "ب", "ب . م", "ب م", "بتة", "بترا‏", "بدون", "بعد", "بعدما", "بعدئذ", "بعيد", "بل", "بلا", "بلى", "بليار", "بليون", "بم", "بماذا", "بن", "بنا", "بيد", "بين", "بينا", "بينما", "ت", "ت غ", "تاسع", "تاسعة", "تحت", "ترليون", "تريليون", "تسع", "تسعة", "تسعمائة", "تسعمئة", "تسعون", "تسعينيات", "تغ", "تلقاء", "تلك", "تلكم", "تلكما", "تلكن", "توّا", "ث", "ثالث", "ثالثة", "ثالوث", "ثامن", "ثامنة", "ثان", "ثانية", "ثلاث", "ثلاثة", "ثلاثمائة", "ثلاثمئة", "ثلاثون", "ثلاثينيات", "ثلثمائة", "ثم", "ثمّ", "ثمانمائة", "ثمانمئة", "ثمانون", "ثمانية", "ثمانينيات", "ثمة", "ثنتان", "ثني", "ج", "ج . ع . م", "ج . م . ع", "ج ج", "جا", "جتا", "جد", "جم", "ح", "حادي", "حادية", "حاشا", "حالما", "حبذا", "حتام", "حتما", "حتى", "حسب", "حسبما", "حم", "حنانيك", "حوالي", "حول", "حيال", "حيث", "حيثما", "حين", "حينذاك", "حينما", "حينئذ", "خ", "خاصة", "خامس", "خامسة", "خصوصا", "خصيصا", "خصيصى", "خلا", "خلال", "خلف", "خماس", "خمس", "خمسة", "خمسمائة", "خمسمئة", "خمسون", "خمسينيات", "د", "د .", "د . أ", "د . ب", "د . ج", "د . جو", "د . ع", "د . ك", "د . ل", "د . م", "دسم", "دشليون", "دون", "دونما", "ديسم", "ديشيليون", "ذ", "ذ . م . م", "ذا", "ذاك", "ذاكم", "ذان", "ذانك", "ذلك", "ذلكم", "ذلكما", "ذلكن", "ذه", "ذو", "ذوا", "ذي", "ذينك", "ر", "ر . ع", "ر . ق", "رابع", "رابعة", "رب", "رباع", "ربما", "رض", "ريثما", "ز", "زمنئذ", "س", "س . ت", "س . ح . م", "س . ض", "س ت", "س ح م", "س و ج", "س.ح.م", "سابع", "سابعة", "سادس", "سادسة", "ساعتئذ", "سافل", "سانا‏", "سباع", "سبعة", "سبعمائة", "سبعمئة", "سبعون", "سبعينات", "سبعينيات", "ست", "ستة", "ستمائة", "ستمئة", "ستون", "ستينات", "ستينيات", "سعديك", "سم", "سم²", "سم³", "سنتم", "سوف", "سونا", "سوى", "ش", "ش . م . ب", "ش . م . ت", "ش . م . ج", "ش . م . ر", "ش . م . س", "ش . م . ع", "ش . م . ق", "ش . م . ك", "ش . م . ل", "ش . م . م", "شرقيّ", "شماليّ", "ص", "ص . ب", "ص ب", "صفر", "صلعم", "صلي", "ض", "ضج", "ط", "طالما", "طس", "طسم", "ظ", "ظا", "ع", "عاشر", "عاشرة", "عبر", "عدا", "عسق", "عشر", "عشرة", "عشرون", "عشرينيات", "عصرئذ", "عل", "علام", "على", "عما", "عمن", "عن", "عند", "عندما", "عندئذ", "غ", "غربيّ", "غضون", "غم", "غير", "ف", "فقط", "فور", "فوق", "في", "فيم", "فيما", "فيمن", "ق", "ق . خ", "ق . ع", "ق . م", "ق خ", "ق ع", "ق م", "ق.ج", "قبال", "قبالة", "قبل", "قبلما", "قبلئذ", "قبيل", "قثنا", "قد", "قدّام", "قرابة", "قط", "قلما", "قلى", "قنا", "ك", "كأن", "كأنما", "كأين", "كجم", "كذا", "كذلك", "كغم", "كلا", "كلتا", "كلجم", "كلغ", "كلغم", "كلم", "كلم - س", "كلم - سا", "كلم - ساعة", "كلم . س", "كلم . سا", "كلم . ساعة", "كلم / س", "كلم / سا", "كلم / ساعة", "كلم 2", "كلما", "كم", "كم - س", "كم . س", "كم / س", "كم 2", "كم²", "كم³", "كما", "كهذا", "كهذه", "كهيعص", "كي", "كيف", "كيفما", "كيلا", "كيلغ", "كيما", "ل", "ل . س", "ل . ل", "لا", "لات", "لبيك", "لحظتذاك", "لحظتئذ", "لدن", "لدى", "لذا", "لعل", "لعمرك", "لقد", "لكن", "لم", "لما", "لماذا", "لن", "لو", "لولا", "ليت", "لئلا", "لئن", "م", "م . غ . ح", "م . ن . ع", "م 2", "م 3", "م د", "م ط", "م/ث", "ما", "ماذا", "مائة", "مبروك", "متى", "مثلما", "مثنى", "مج", "مجم", "مذ", "مذّاك", "مرار", "مش", "مع", "مغم", "مل", "ملجم", "ملغ", "ملغم", "مللغم", "مللم", "ملم", "مليار", "مليون", "مم", "مما", "ممن", "من", "منذ", "منذئذ", "مهما", "مو", "مؤ", "مئة", "ن", "نا", "نت", "نحن", "نعم", "نق", "ننا", "هـ", "هـ . ط . ث", "هـ . ن . ع", "هـ ط ث", "ها", "هاتان", "هاته", "هاتيك", "هاك", "هاكم", "هاهنا", "هأنذا", "هذا", "هذان", "هذه", "هكذا", "هل", "هلا", "هم", "هما", "هن", "هنا", "هناك", "هنالك", "ههنا", "هو", "هؤلاء", "هي", "و", "وا", "واج‏", "واس‏", "وام", "وراء", "وسط", "وفا", "وقتذاك", "وقتما", "وقتئذ", "وما", "ونا", "ويح", "ويك", "ي", "يا", "يومذاك", "يومئذ" };
        private static string[] Prefix = { "أفب", "أفل", "أوب", "أوك", "أول", "ال", "أب", "أس", "أف", "أل", "أو", "فب", "فس", "فك", "فل", "فو", "لب", "وب", "وس", "وك", "ول", "أ", "آ", "ب", "ت", "س", "ف", "ك", "ل", "ن", "و", "ي" };
        private static string[] Suffix = { "كماها", "كماهم", "كموهم", "نيهما", "هموها", "هموهم", "كماه", "كموه", "كهما", "ناها", "نيها", "نيهم", "نيهن", "هموه", "اني", "تان", "تما", "تمو", "تين", "كما", "كها", "كهم", "كهن", "ناه", "نيه", "هما", "ات", "ان", "اه", "تا", "تم", "تن", "تي", "كم", "كن", "كه", "نا", "ني", "ها", "هم", "هن", "وا", "ون", "ين", "ا", "ة", "ت", "ك", "ن", "ه", "و", "ي" };
        
        private static int searchwordInDB(string word)
        {
            using (MySqlConnection con = new MySqlConnection(Connection.Blogs))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "select count(id) from searchwords where word=N'" + word + "'"; //stored procedure getting the top 10 blogs entry by exact search word                              
                con.Open();
                int count = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (count != 0)
                    return 1;
            }
            return 0;
        }
        private static string getwordStemmer(string word)
        {
            Snowball.ArabicStemmer snowball = new ArabicStemmer();
            string stemWord = word;
            int count = searchwordInDB(word);
            if (count == 1)
                stemWord = word;
            else
            {
                string OrginalWord = word;
                foreach (string x in Prefix)
                {
                    int len = x.Length;
                    if (len >= word.Length)
                        continue;
                    string wordprfix = word.Substring(0, len);

                    string result = Array.Find(Prefix, element => element == wordprfix);

                    if (result != null)
                    {
                        stemWord = word.Substring(len);
                        count = searchwordInDB(stemWord);
                        if (count == 1)
                            break;
                        else
                        {
                            foreach (string ss in Suffix)
                            {
                                int lens = ss.Length;
                                if (lens >= stemWord.Length)
                                    continue;
                                string wordSuffix = stemWord.Substring(stemWord.Length - lens, lens);

                                string results = Array.Find(Suffix, element => element == wordSuffix);

                                if (results != null)
                                {
                                    stemWord = stemWord.Substring(0, stemWord.Length - lens);
                                    string wordwithPrefix = result + stemWord;
                                    count = searchwordInDB(wordwithPrefix);
                                    if (count == 1)
                                        return wordwithPrefix;
                                    count = searchwordInDB(stemWord);
                                    if (count == 1)
                                        return stemWord;
                                }
                            }
                            return word;
                        }
                    }
                }
                if (count == 0)
                {
                    string orginal_word = word;
                    foreach (string x in Suffix)
                    {
                        int len = x.Length;
                        if (len >= word.Length)
                            continue;
                        string wordSuffix = word.Substring(word.Length - len, len);

                        string result = Array.Find(Suffix, element => element == wordSuffix);

                        if (result != null)
                        {
                            stemWord = word.Substring(0, word.Length - len);
                            count = searchwordInDB(stemWord);
                            if (count == 1)
                                break;
                        }
                    }
                    if (count == 0)
                    {
                        stemWord = snowball.Stem(word);

                    }

                }
            }
            return stemWord;
        }
       
        public static string convertSentencetoStems(string sentencetoStems)
        {
            string StemList = "";
            string[] Words = sentencetoStems.Split(" ");
            for (int i = 0; i < Words.Length; i++)
            {
                string word = Words[i];
                string result = Array.Find(StopWords, element => element == word);
                if (result != null)
                    continue;
                result = Array.Find(StopWords, element => "و" + element == word);
                if (result != null)
                    continue;
                result = Array.Find(StopWords, element => "ل" + element == word);
                if (result != null)
                    continue;
                result = Array.Find(StopWords, element => "ف" + element == word);
                if (result != null)
                    continue;
                result = Array.Find(StopWords, element => "ك" + element == word);
                if (result != null)
                    continue;

                string ResultStemWord = getwordStemmer(word);

                //  word = snowball.Stem(word);

                StemList += ResultStemWord;
                if (i < Words.Length - 1)
                {
                    StemList += "-";
                }
            }
            return StemList.TrimEnd('-');
        }

    }

}
