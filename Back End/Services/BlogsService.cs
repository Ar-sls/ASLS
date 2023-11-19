
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WebApi.Helpers;
using RestSharp;
using Newtonsoft.Json;
using Snowball;
using WebApi.Entities;


namespace WebApi.Services
{
    public interface IBlogsService
    {               
       
        BlogList GetBlogs(string word);
        BlogSubList GetCleanBlogs(string guid);
        BlogSubList GetStemBlogs(string guid);
        

    }   
    public class BlogsService : IBlogsService
    {      
        /////////////////////GetBlogs Function///////////////////////////////////////////////

        public BlogList GetBlogs(string word)
        {
            //return a list of blogs found in blog DB which contains the input word
            DataTable BlogTable = new DataTable();
            BlogList blogList = new BlogList();
                       
            using (MySqlConnection con = new MySqlConnection(Connection.Blogs))
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "FindSearchWordinBlogs"; 
                cmd.CommandType = CommandType.StoredProcedure;
                MySqlParameter p_word = new MySqlParameter("@SearchWord", word);
                cmd.Parameters.Add(p_word);
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(BlogTable);
                con.Close();

                blogList.NoBlogs = BlogTable.Rows.Count;
                blogList.wordsearch = word;

                if (blogList.NoBlogs > 0)
                {
                    blogList.guid = Guid.NewGuid().ToString();
                    //filter data from Blogs Table
                    blogList.sentenceLists = Helper.WordProcess(BlogTable, word, blogList.guid);
                }                
            }
            return blogList;
        }

       
        /////////////////////GetCleanBlogs Function///////////////////////////////////////////////

        public BlogSubList GetCleanBlogs(string guid)
        {
            string fileContentEcho = "";
            BlogSubList blogList = new BlogSubList();
            string targetpath = Directory.GetCurrentDirectory() + "\\BlogsResult";
            string newPath = Path.Combine(targetpath, guid);
            if (!Directory.Exists(newPath))
            {
                blogList.NoBlogs = 0;                
                return blogList;
            }
            string myPageString = File.ReadAllText(newPath+ "\\OrginalSentences.txt"); ;//read file
            string[] ListOfSentens = myPageString.Split("\r\n");
            blogList.NoBlogs = ListOfSentens.Length;
            if (blogList.NoBlogs > 0)
            {
                blogList.guid= guid;
                blogList.sentenceLists = new List<string>();
               
                for(int i = 0; i < ListOfSentens.Length; i++) {
                    string cleanSentence = ListOfSentens[i];
                    cleanSentence = Helper.RemoveDiac(cleanSentence);                   
                    cleanSentence = Helper.RemoveUnArabicChar(cleanSentence);
                    cleanSentence = Helper.RemoveDoubleSpaces(cleanSentence);
                    cleanSentence = cleanSentence.Trim();
                   
                                    
                    blogList.sentenceLists.Add(cleanSentence);
                    fileContentEcho += cleanSentence;
                    if (i < ListOfSentens.Length-1)
                        fileContentEcho += "\r\n";
                }
                System.IO.File.WriteAllText(Path.Combine(newPath, "CleanSentences.txt"), fileContentEcho, Encoding.UTF8);
            }
            return blogList;
        }

       
        /////////////////////GetStemBlogs Function///////////////////////////////////////////////

        public BlogSubList GetStemBlogs( string guid)
        {
            string fileContentEcho = "";
            BlogSubList blogList = new BlogSubList();
            string targetpath = Directory.GetCurrentDirectory() + "\\BlogsResult";
            string newPath = Path.Combine(targetpath, guid);
            if (!Directory.Exists(newPath))
            {
                blogList.NoBlogs = 0;                
                return blogList;
            }
            string myPageString = File.ReadAllText(newPath + "\\CleanSentences.txt"); ;//read file
            string[] ListOfSentens = myPageString.Split("\r\n");
            blogList.NoBlogs = ListOfSentens.Length;
            if (blogList.NoBlogs > 0)
            {
                blogList.guid = guid;
                blogList.sentenceLists = new List<string>();
                
                for (int i = 0; i < ListOfSentens.Length; i++)
                {
                    string StemSentence = ListOfSentens[i];

                    StemSentence = Helper.convertSentencetoStems(StemSentence);
                                       
             
                    blogList.sentenceLists.Add(StemSentence);
                    fileContentEcho += StemSentence;
                    if (i < ListOfSentens.Length - 1)
                        fileContentEcho += "\r\n";
                }
                System.IO.File.WriteAllText(Path.Combine(newPath, "StemSentences.txt"), fileContentEcho, Encoding.UTF8);
            }
            return blogList;
        }

        /////////////////////CallAlriyadhDictioanryAPI Function///////////////////////////////////////////////

        private string CallAlriyadhDictioanryAPI(string InputStr)
        {
            try
            {
                var url = "https://siwar.ksaa.gov.sa/api/alriyadh/search?query=" + InputStr;
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.Get);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Accept", "application/json");
                request.AddHeader("apikey", "da5fbfb6-dca1-47b6-bedc-f389990f6248");
                RestResponse response = client.Execute(request);
                var Output = response.Content;
                var obj = JsonConvert.DeserializeObject<dynamic>(response.Content);
                foreach (var item in obj)
                {
                    if (item.nonDiacriticsLemma == InputStr)
                        return InputStr;
                }
                return InputStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
