using System;
using System.Linq;

namespace Parallel_Rabin_Karp_Application
{
    public class Preprocess
    {
        protected static CRUD database;

        private static char[] delimeters = new char[] { 
            '{', '}', '(', ')', '[', ']', '>', '<', '-', '_', '=', '+',
            '|', '\\', ':', ';', ' ', ',', '.', '/', '?', '~', '!',
            '@', '#', '$', '%', '^', '&', '*', '\r', '\n', '\t',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public Preprocess() {
            
        }

        public static string[] Tokenize(string text) {
            string[] tokens = text.Split(delimeters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++) {
                string tempToken = tokens[i];

                // Change token only when it starts and/or ends with "'" and the 
                // token has at least 2 characters.
                if (tempToken.Length > 1) { 
                    if(tempToken.StartsWith("'") && tempToken.EndsWith("'")){
                        tokens[i] = tempToken.Substring(1, tempToken.Length - 2); // remove starting and ending "'"
                    }
                    else if (tempToken.StartsWith("'")) {
                        tokens[i] = tempToken.Substring(1); // remove starting "'"
                    }
                    else if(tempToken.EndsWith("'")) {
                        tokens[i] = tempToken.Substring(0, tempToken.Length - 1); // remove ending "'"
                    }
                    else if(tempToken.Contains("'")) {
                        int index = tempToken.IndexOf("'");
                        
                        if (index != -1) {
                            tokens[i] = tempToken.Remove(index, 1);  // remove string contains "'" in center word
                        }

                    }
                }
            }

            return tokens;
        }

        public static string[] Casefolding(string[] text) {
            string[] content = Array.ConvertAll<string, string>(text, delegate(string contents){
                return contents.ToLower();
                });

            return content;
        }

        public static string[] StopwordRemove(string[] text) {
            string query = "select stoplist from tb_stoplist";

            database = new CRUD();

            // save return values to var stopword
            string[] stopword = database.Select(query, "tb_stoplist", "stoplist");
            
            var stopwordExist = text.Intersect(stopword);
    
            var delStopWord = text.Except(stopwordExist);

            // define size of string
            string[] result = new string[delStopWord.Count()];

            int i = 0;
            foreach (string str in delStopWord) {
                result[i] = str;
                i += 1;
            }

            return result;
        }
    }
}

