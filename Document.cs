using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallel_Rabin_Karp_Application
{
    public class Document
    {
        private string name;
        private string path;
        private string content;
        private int sizeDocument;
        private static string filter = "txt files (*.txt)|*.txt";

        private string testDoc;
        private string trainDoc;

        public Document() { }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public static string Filter()
        {
            return filter;
        }

        public string Content(string pathFile)
        {
            try
            {
                using (StreamReader reader = new StreamReader(pathFile))
                {
                    // read all the content into single variable using ReadToEnd() Method
                    this.content = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return this.content;
        }

        public int CountWords(string file)
        {
            try
            {
                sizeDocument = System.Text.RegularExpressions.Regex.Matches(file, @"[A-Za-z0-9]+").Count;
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

            return sizeDocument;
        }

        public string TestDocVerified {
            get { return testDoc; }
            set { testDoc = value; }
        }

        public string TrainDocVerified
        {
            get { return trainDoc; }
            set { trainDoc = value; }
        }
    }
}
