using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class SInput
    {
        public string searchWord { get; set; }

        public string GUID { get; set; }
        
    }
    public struct SentenceList
    {
        public string sentence { get; set; }
        public string sentenceHTML { get; set; }
    }
    public class BlogList
    {
        public int NoBlogs { get; set; }
        public string wordsearch { get; set; }
        public string guid { get; set; }
        public List<SentenceList> sentenceLists { get; set; }
    }
    public class BlogSubList
    {
        public int NoBlogs { get; set; }
        public string guid { get; set; }
        public List<string> sentenceLists { get; set; }
    }
}
