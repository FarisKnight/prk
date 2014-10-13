using System.Linq;

using System.Text.RegularExpressions;

namespace Parallel_Rabin_Karp_Application
{
    public class ECSStemm : Preprocess
    {
        public static string Stemm(string words)
        {
            string word = words;

            if (isInDictionary(words))
            {
                return words;
            }

            words = removeInflectionSuffixes(words);

            words = removeDerivationSuffixes(words);

            words = removeDerivationPrefixes(words);

            return words;
        }

        /* checking the word on database, done if its exists */
        private static bool isInDictionary(string word)
        {
            string query = "select * from tb_katadasar where katadasar = '" + word + "' limit 1";
            string[] result = database.Select(query, "tb_katadasar", "katadasar");

            if (result.Count() == 1)
            {
                return true;
            }

            return false;
        }

        /* remove Inflection Suffixes, 
         * 1. Particle "-lah" "-kah" "-tah" and "-pun" 
         * 2. Possesive Pronoun "-ku" "-mu" "-nya"
         */
        private static string removeInflectionSuffixes(string word)
        {
            string baseWord = word;

            if (Regex.IsMatch(word, @"([klt]ah|pun|[km]u|nya)$"))
            {
                string infSuf = Regex.Replace(word, @"([klt]ah|pun|[km]u|nya)$", "");

                if (Regex.IsMatch(word, @"([klt]ah|pun)$"))
                {
                    if (Regex.IsMatch(infSuf, @"([km]u|nya)$"))
                    {
                        string posPron = Regex.Replace(infSuf, @"([km]u|nya)$", "");
                        return posPron;
                    }
                }
                return infSuf;
            }

            return baseWord;
        }

        /* check the Rule Precedence
         * combination of Prefix and Suffix
         * "be-lah" "be-an" "me-i" "di-i" "pe-i" or "te-i"
         */
        private static bool isRulePrecedence(string word)
        {
            if (Regex.IsMatch(word, @"^(be)([A-Za-z\-]+)(lah|an)$")) { return true; }    /* "be-lah|an" */

            if (Regex.IsMatch(word, @"^(di|[mpt]e)([A-Za-z\-]+)(i)$")) { return true; }    /* "di|me|pe|te-i" */

            return false;
        }

        /* check Disallowed Prefix-Suffix Combinations
         * "be-i" . "di-an" . "ke-i|kan" . "me-an" . "se-i|kan" or "te-an"
         * **/
        private static bool isDisallowedPrefixSuffixes(string word)
        {
            if (Regex.IsMatch(word, @"^(be)([A-Za-z\-]+)(i)$")) { return true; } /* "be-i" */

            if (Regex.IsMatch(word, @"^(di)([A-Za-z\-]+)(an)$")) { return true; } /* "di-an" */

            if (Regex.IsMatch(word, @"^(ke)([A-Za-z\-]+)(i|kan)$")) { return true; } /* "ke-i|kan" */

            if (Regex.IsMatch(word, @"^(me)([A-Za-z\-]+)(an)$")) { return true; } /* "me-an" */

            if (Regex.IsMatch(word, @"^(se)([A-Za-z\-]+)(i|kan)$")) { return true; } /* "se-i|kan" */

            if (Regex.IsMatch(word, @"^(te)([A-Za-z\-]+)(an)$")) { return true; } /* "te-an" */

            return false;
        }

        /* remove Dervation Suffixes
         * "-i" . "-kan" . "-an"
         * **/
        private static string removeDerivationSuffixes(string word)
        {
            string baseWord = "";

            if (Regex.IsMatch(word, @"(kan)$"))
            {
                baseWord = Regex.Replace(word, @"(kan)$", "");
                if (isInDictionary(baseWord))
                {
                    return baseWord;
                }
            }

            if (Regex.IsMatch(word, @"(an|i)$"))
            {
                baseWord = Regex.Replace(word, @"(an|i)$", "");
                if (isInDictionary(baseWord))
                {
                    return baseWord;
                }
            }

            if (isDisallowedPrefixSuffixes(baseWord))
            {
                return word;
            }
            return word;
        }

        /* remove Derivation Prefixes
         * "di-" . "ke-" . "se-" . "me-" . "be-" . "pe-" or "te-"
         * */
        private static string removeDerivationPrefixes(string word)
        {
            string baseWord = word;
            string strRemovedDerivSuff = "";
            string strRemovedStdPref = "";
            string strRemovedCmplxPref = "";

            /************** check Standard Prefix "di-" . "-ke" . "-se" **************/
            if (Regex.IsMatch(word, @"^(di|[ks]e)\S{1,}"))
            {
                strRemovedStdPref = Regex.Replace(word, @"^(di|[ks]e)", "");
                if (isInDictionary(strRemovedStdPref))
                {
                    return strRemovedStdPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedStdPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }


            if (Regex.IsMatch(word, @"^([^aiueo])e\1[aiueo]\S{1,}"))    /* rule 35 */
            {
                string prefixes = Regex.Replace(word, @"^([^aiueo])e", "");
                if (isInDictionary(prefixes))
                {
                    return prefixes;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(prefixes);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

            }

            /************** check Complex Prefixes "te-"."me-"."be-"."pe-" **************/
            if (Regex.IsMatch(word, @"^([tmbp]e)\S{1,}"))
            {
                /************** Prefix "be-" **************/
                if (Regex.IsMatch(word, @"^(be)\S{1,}"))
                {  /* if prefix "be-" */
                    if (Regex.IsMatch(word, @"^(ber)[aiueo]\S{1,}"))
                    {  /* rule 1 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ber)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(ber)", "r");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(ber)[^aiueor]([A-Za-z\-]+)(?!er)\S{1,}"))
                    {   /* rule 2 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ber)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(ber)[^aiueor]([A-Za-z\-]+)er[aiueo]\S{1,}"))
                    {    /* rule 3 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ber)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^belajar\S{0,}"))
                    {   /* rule 4 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(bel)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(be)[^aiueolr]er[^aiueo]\S{1,}"))
                    { /* rule 5 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(be)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }
                }
                /************** end Prefix "be-" **************/

                /************** Prefix "te-" **************/
                if (Regex.IsMatch(word, @"^(te)\S{1,}"))
                {
                    if (Regex.IsMatch(word, @"^(terr)\S{1,}"))
                    {
                        return word;
                    }

                    if (Regex.IsMatch(word, @"^(ter)[aiueo]\S{1,}"))
                    {  /* rule 6 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ter)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(ter)", "r");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(ter)[^aiueor]er[aiueo]\S{1,}"))
                    {   /* rule 7 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ter)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(ter)[^aiueor](?!er)\S{1,}"))
                    {  /* rule 8 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ter)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(te)[^aiueor]er[aiueo]\S{1,}"))  /* rule 9 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(te)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(ter)[^aiueor]er[^aiueo]\S{1,}"))
                    {  /* rule 35 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(ter)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }
                }
                /************** end Prefix "te-" **************/

                /************** Prefix "me-" **************/
                if (Regex.IsMatch(word, @"^(me)\S{1,}"))
                {

                    if (Regex.IsMatch(word, @"^(me)[lrwyv][aiueo]"))
                    {  /* rule 10 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(me)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(mem)[bfvp]\S{1,}"))
                    { /* rule 11 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(mem)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(mem)((r[aiueo])|[aiueo])\S{1,}"))
                    { /* rule 13 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(mem)", "m");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(mem)", "p");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(men)[cdjszt]\S{1,}"))
                    { /* rule 14 */
                        strRemovedCmplxPref = Regex.Replace(word, @"^(men)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(men)[aiueo]\S{1,}"))   /* rule 15 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(men)", "n");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;

                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(men)", "t");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;

                        }

                    }

                    if (Regex.IsMatch(word, @"^(meng)[ghqk]\S{1,}"))   /* rule 16 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(meng)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(meng)[aiueo]\S{1,}"))   /* rule 17 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(meng)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(meng)", "k");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(menge)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(meny)[aiueo]\S{1,}"))   /* rule 18 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(meny)", "s");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(me)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }
                }
                /************** end Prefix "me-" **************/

                /************** Prefix "pe-" **************/
                if (Regex.IsMatch(word, @"^(pe)\S{1,}"))
                {
                    if (Regex.IsMatch(word, @"^(pe)[wy]\S{1,}"))   /* rule 20 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pe)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(per)[aiueo]\S{1,}"))   /* rule 21 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(per)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(per)", "r");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(per)[^aiueor]([A-Za-z\-]+)(?!er)\S{1,}"))   /* rule 23 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(per)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(per)[^aiueo]([A-Za-z\-]+)(er)[aiueo]\S{1,}"))   /* rule 24 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(per)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pem)[bfv]\S{1,}"))   /* rule 25 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pem)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pem)(r[aiueo]|[aiueo])\S{1,}"))   /* rule 26 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pem)", "m");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(pem)", "p");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pen)[cdjzt]\S{1,}"))   /* rule 27 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pen)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pen)[aiueo]\S{1,}"))   /* rule 28 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pen)", "n");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(pen)", "t");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(peng)[^aiueo]\S{1,}"))   /* rule 29 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(peng)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(peng)[aiueo]\S{1,}"))   /* rule 30 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(peng)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(peng)", "k");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(penge)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(peny)[aiueo]\S{1,}"))   /* rule 31 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(peny)", "s");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }

                        strRemovedCmplxPref = Regex.Replace(word, @"^(pe)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pel)[aiueo]\S{1,}"))   /* rule 32 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pel)", "l");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pelajar)\S{0,}"))
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pel)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pe)[^rwylmn]er[aiueo]\S{1,}"))   /* rule 33 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pe)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pe)[^rwylmn](?!er)\S{1,}"))   /* rule 34 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pe)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }

                    if (Regex.IsMatch(word, @"^(pe)[^aiueor]er[^aiueo]\S{1,}"))   /* rule 36 */
                    {
                        strRemovedCmplxPref = Regex.Replace(word, @"^(pe)", "");
                        if (isInDictionary(strRemovedCmplxPref))
                        {
                            return strRemovedCmplxPref;
                        }

                        strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                        if (isInDictionary(strRemovedDerivSuff))
                        {
                            return strRemovedDerivSuff;
                        }
                    }
                }

                /************** end Prefix "pe-" **************/
            }

            /************** Prefix "memper-" **************/
            if (Regex.IsMatch(word, @"^(memper)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(memper)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                /* check luluh -r */
                strRemovedCmplxPref = Regex.Replace(word, @"^(memper)", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "memper-" **************/

            /************** Prefix "mempel-" **************/
            if (Regex.IsMatch(word, @"^(mempel)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(mempel)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(mempel)", "l");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "mempel-" **************/

            /************** Prefix "menter-" **************/

            if (Regex.IsMatch(word, @"^(menter)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(menter)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(menter)", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }
            /************** end Prefix "menter-" **************/

            /************** Prefix "member-" **************/

            if (Regex.IsMatch(word, @"^(member)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(member)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(member)", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "member-" **************/

            /************** Prefix "diper-" **************/

            if (Regex.IsMatch(word, @"^(diper)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(diper)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(diper)", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "diper-" **************/

            /************** Prefix "diter-" **************/

            if (Regex.IsMatch(word, @"^(diter)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(diter)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(diter)", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "diter-" **************/

            /************** Prefix "dipel-" **************/

            if (Regex.IsMatch(word, @"^(dipel)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"^(dipel)", "l");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"^(dipel)", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end Prefix "dipel-" **************/

            /************** word "terpelajar" (special case) **************/

            if (Regex.IsMatch(word, @"terpelajar"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"terpel", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end word "terpelajar" (special case) **************/

            /************** word "seseorang" (special case) **************/

            if (Regex.IsMatch(word, @"seseorang"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"sese", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }
            }

            /************** end word "seseorang" (special case) **************/

            /************** prefix "diber-" **************/

            if (Regex.IsMatch(word, @"^(diber)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"diber", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"diber", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end prefix "diber-" **************/

            /************** prefix "keber-" **************/

            if (Regex.IsMatch(word, @"^(keber)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"keber", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"keber", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end prefix "diber-" **************/

            /************** prefix "keter-" **************/

            if (Regex.IsMatch(word, @"^(keter)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"keter", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

                strRemovedCmplxPref = Regex.Replace(word, @"keter", "r");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }
            }

            /************** end prefix "keter-" **************/

            /************** prefix "berke-" **************/

            if (Regex.IsMatch(word, @"^(berke)\S{1,}"))
            {
                strRemovedCmplxPref = Regex.Replace(word, @"berke", "");
                if (isInDictionary(strRemovedCmplxPref))
                {
                    return strRemovedCmplxPref;
                }

                strRemovedDerivSuff = removeDerivationSuffixes(strRemovedCmplxPref);
                if (isInDictionary(strRemovedDerivSuff))
                {
                    return strRemovedDerivSuff;
                }

            }

            /************** end prefix "berke-" **************/

            /* check for there is no Prefixes 
             "di-", "ke-", "se-", "te-", "be-", "me-", or "pe-"
             */
            if (!Regex.IsMatch(word, @"^(di|[kstbmp]e)\S{1,}"))
            {
                return baseWord;
            }

            return baseWord;
        }
    }
}
