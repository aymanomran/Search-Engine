using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Search_Engine
{
    public partial class SearchForm : System.Web.UI.Page
    {
        static string[] SplitWords(string s)
        {
            return Regex.Matches(s, "\\w+('(s|d|t|ve|m))?")
        .Cast<Match>().Select(x => x.Value).ToArray();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SearchBtn_Click(object sender, EventArgs e)
        {
            Stemmer stemmer = new Stemmer();
            ListBox DidUMean = ((ListBox)form1.FindControl("DidUMeanLBID"));
            ListBox ResultsBox = ((ListBox)form1.FindControl("REsultsBoxID"));
            string inputText = ((TextBox)form1.FindControl("SearchTxt")).Text.ToLower();
            inputText = inputText.ToLower();
            string DocID, frequency, Positions;
            Boolean exact_query=false;

            List<Term> terms = new List<Term>();
            List<singelTerm> SingleTermsList = new List<singelTerm>();
            Term T;
            singelTerm singleterm;
            Details D;
            SqlConnection conn = new SqlConnection("Server = AYMAN-PC; Database = IRdb; Trusted_Connection = True; MultipleActiveResultSets = true");
            conn.Open();

            if (inputText[0] == '"')
            {
                inputText = inputText.Replace(@"""", "");
               exact_query = true;
            }
            inputText= StopWords.RemoveStopwords(inputText);
            DidUMean.Items.Clear();
            ResultsBox.Items.Clear();
            string[] query = SplitWords(inputText);

            for (int i = 0; i < query.Count(); i++)
            {

                query[i] = stemmer.stem(query[i]);
                SqlCommand cmd = new SqlCommand("select [term],[doc_id],[frequency],[positions] from  [IRdb].[dbo].[Inverted_Index] where [term]= @term ", conn);
                cmd.Parameters.AddWithValue("@term", query[i]);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        singleterm = new singelTerm();
                        singleterm.name = (reader["term"].ToString());//Term
                        frequency = reader["frequency"].ToString();
                        DocID = reader["doc_id"].ToString();
                        Positions = reader["positions"].ToString();
                        string[] Pos_terms = Positions.Split('@');
                        string[] freqs = frequency.Split(',');
                        string[] _DocIDs = DocID.Split(',');
                        List<string> positionsList = new List<string>();
                        foreach (string p in Pos_terms)
                        {
                            positionsList = p.Split(',').ToList();
                        }

                        foreach (string doc_id in _DocIDs)
                        {
                            singleterm.doc_id.Add(int.Parse(doc_id));
                        }


                        //frequency
                        foreach (string f in freqs)
                        {
                            singleterm.frequency.Add(int.Parse(f));
                        }
                        //postions
                        foreach (string p in Pos_terms)   
                        {

                            singleterm.position.Add(p);
                        }

                        SingleTermsList.Add(singleterm);
               
                    }
                }

            }
            ////////////////another Form ///////////
            for (int i = 0; i < SingleTermsList.Count(); i++) {
                T = new Term();
                T.name = SingleTermsList[i].name;
                for (int j = 0; j < SingleTermsList[i].doc_id.Count(); j++) {
                    Details details = new Details();
                    details.doc_id = SingleTermsList[i].doc_id[j];
                    details.frequency = SingleTermsList[i].frequency[j];
                    string []positions = SingleTermsList[i].position[j].Split(',');
                    foreach (string pos in positions) {
                        details.positions.Add(int.Parse(pos));
                      
                    }
                    T.details.Add(details);
                }
                terms.Add(T);
            }
            /////////////////////////////////////////////////

            HtmlGenericControl div = ((HtmlGenericControl)form1.FindControl("SearchingResultsDiv"));//Div That Will Contain Links
            
            searchClass searchObj = new searchClass();
            if (terms.Count == 1)
            {
                searchObj.singleWordSearch(terms, query, ResultsBox, DidUMeanLabelID, div);
            }
            else
            {
                if (exact_query == false)
                {
                    //1.Multi Keyword Search 

                    searchObj.MultiKeywordSearch(terms, query, ResultsBox, DidUMeanLabelID, div);

                }
                else
                {

                    searchObj.ExactSearch(terms, query, ResultsBox, DidUMeanLabelID, div);

                }
            }
        }

        //Soundx Check Box Click Envent

        protected void SoundxID_CheckedChanged(object sender, EventArgs e)
        {
            DidUMeanLabelID.Text = "Did You Mean : ";
            ListBox Did_You_Mean = ((ListBox)form1.FindControl("DidUMeanLBID"));
            CheckBox checkBox2 = ((CheckBox)form1.FindControl("SoundxID"));
            string inputText = ((TextBox)form1.FindControl("SearchTxt")).Text.ToLower();
            Did_You_Mean.Items.Clear();
            SqlConnection conn = new SqlConnection(" Server=AYMAN-PC;Database=IRdb;Trusted_Connection=True;MultipleActiveResultSets=true");
            conn.Open();
            //Soundex
            if (checkBox2.Checked == true)
            {
                string s = inputText;
                string[] query = s.Split(' ');
                for (int i = 0; i < query.Count(); i++)
                {
                    string query_soundex = searchClass.GetWordSoundx(query[i]);
                    SqlCommand cmd = new SqlCommand("select [terms] from [IRdb].[dbo].[soundxIndex] where [soundx]= @soundex ", conn);
                    cmd.Parameters.AddWithValue("@soundex", query_soundex);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string terms = reader["terms"].ToString();
                            string[] soundexs = terms.Split(',');
                            for (int j = 0; j < soundexs.Count(); j++)
                            {
                                Did_You_Mean.Items.Add(soundexs[j]);
                            }
                        }

                    }


                }
            }

        }
        //Did You Mean LisBox Click Event
        protected void DidUMeanLBID_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label DidUmean = ((Label)form1.FindControl("DidUMeanLabelID"));
            DidUmean.Text = "Did You Mean : ";
            ListBox soundxListBox = ((ListBox)form1.FindControl("DidUMeanLBID"));
            TextBox searchtxt = ((TextBox)form1.FindControl("SearchTxt"));
            searchtxt.Text = soundxListBox.SelectedItem.ToString();
        }

        protected void ResultsBoxID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Spelling Correction
        protected void SpellCorrectionCB_CheckedChanged(object sender, EventArgs e)
        {
            ListBox DidUMean = ((ListBox)form1.FindControl("DidUMeanLBID"));
            ListBox ResultsBox = ((ListBox)form1.FindControl("REsultsBoxID"));
            string inputText = ((TextBox)form1.FindControl("SearchTxt")).Text.ToLower();
            CheckBox SpellCorrectionCB = ((CheckBox)form1.FindControl("SpellCorrectionCB"));
            DidUMean.Items.Clear();
            SqlConnection conn = new SqlConnection(" Server=AYMAN-PC;Database=IRdb;Trusted_Connection=True;MultipleActiveResultSets=true");
            conn.Open();
            //K Grams
            if (SpellCorrectionCB.Checked == true)
            {

                List<String> query_kgram = new List<string>();
                List<String> term_kgram = new List<string>();
                List<String> correction_terms = new List<string>();
                String[] query = SplitWords(inputText);
                for (int i = 0; i < query.Count(); i++)
                {
                    //Get Query Bigrams
                    //*************************************************
                    String q = query[i];
                    if (q.Length == 1)
                    {
                        query_kgram.Add(q);
                    }
                    else
                    {
                        //string f = q[0].ToString();
                        //query_kgram.Add(f);

                        //string l = q[q.Length - 1].ToString();
                        //query_kgram.Add(l);

                        for (int j = 0; j < q.Length - 1; j++)
                        {
                            String temp = q[j].ToString();
                            temp += q[j + 1];
                            query_kgram.Add(temp);
                        }

                    }
                    //*************************************************
                    //Get Related Terms
                    for (int ii = 0; ii < query_kgram.Count(); ii++)
                    {
                        String ts = "";
                        SqlCommand cmd = new SqlCommand("select [terms]  FROM [IRdb].[dbo].[Bi_gram] where [k_gram]= @k_gram ", conn);
                        cmd.Parameters.AddWithValue("@k_gram", query_kgram[ii]);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ts = reader["terms"].ToString();
                            }

                        }

                        String[] terms = ts.Split(',');
                        //Get Terms Bigrams
                        for (int j = 0; j < terms.Count(); j++)
                        {
                            Double query_bigrams = 0.0, term_bigrams = 0.0, common_bigrams = 0.0, jc = 0.0, preset_threshold = 0.45;
                            term_kgram.Clear();
                            String t = terms[j];
                            if (t.Length == 1)
                            {
                                term_kgram.Add(t);
                            }
                            else
                            {
                                //string f = t[0].ToString();
                                //term_kgram.Add(f);

                                //string l = t[t.Length - 1].ToString();
                                //term_kgram.Add(l);

                                for (int jj = 0; jj < t.Length - 1; jj++)
                                {
                                    String temp = t[jj].ToString();
                                    temp += t[jj + 1];
                                    term_kgram.Add(temp);
                                }

                            }

                            //Calculate bigrams of query
                            query_bigrams = query_kgram.Count();
                            //Calculate bigrams of dictionary term
                            term_bigrams = term_kgram.Count();
                            //Calculate common bigrams
                            for (int k = 0; k < query_kgram.Count(); k++)
                            {
                                for (int kk = 0; kk < term_kgram.Count(); kk++)
                                {
                                    if (query_kgram[k] == term_kgram[kk])
                                        common_bigrams++;
                                }

                            }
                            //Calculate JC
                            jc = (2 * common_bigrams) / (query_bigrams + term_bigrams);
                            //Add Match Term
                            if (jc > preset_threshold)
                            {
                                //Calculate Edit Distance Between Query & Term
                                int distance = EditDistance.Compute_Distance(query[i], terms[j]);
                                if (distance <= 1)
                                {
                                    correction_terms.Add(terms[j]);
                                }
                            }

                        }

                    }


                    query_kgram.Clear();
                    term_kgram.Clear();
                }

                correction_terms = correction_terms.Distinct().ToList();
                for (int c = 0; c < correction_terms.Count(); c++)
                    DidUMeanLBID.Items.Add(correction_terms[c]);

            }

        }

        /////////////////////////////




    }
    }
