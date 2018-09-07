using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Search_Engine
{
    public class searchClass
    {
        SqlConnection conn = new SqlConnection("Server = AYMAN-PC; Database = IRdb; Trusted_Connection = True; MultipleActiveResultSets = true");
        public searchClass() {
            conn.Open();
        }
        public void MultiKeywordSearch(List<Term> terms, string[] query, ListBox ResultsBox, Label DidUMeanLabelID, System.Web.UI.HtmlControls.HtmlGenericControl divLinks) {

            //1.Multi Keyword Search 
            int small = +999999999, smallposition = +999999999;//small->the smallest term that contain  no of DocID && smallPosition->smallest Distance between poitions
            int small_index = 0, check = 0;//small_index->the index of smallest term in the termList && check->to check the number of intersection docs
            List<int> doc_list = new List<int>();
            Dictionary<int, int> mydic = new Dictionary<int, int>();

            if (terms.Count() == query.Count())//to check that all search query found in the inverted index
            {
                //Get Smallest Term 
                for (int i = 0; i < terms.Count(); i++)
                {
                    if (small > terms[i].details.Count()) 
                    {
                        small = terms[i].details.Count();
                        small_index = i;
                    }
                }
                //Get Documents Contain Query Words
                for (int i = 0; i < terms[small_index].details.Count(); i++)
                {
                    check = 0;
                    for (int j = 0; j < terms.Count(); j++)
                    {
                        for (int k = 0; k < terms[j].details.Count(); k++)
                        {
                            if (terms[small_index].details[i].doc_id == terms[j].details[k].doc_id)
                            {
                                terms[small_index].details[i].positions.Sort();
                                terms[j].details[k].positions.Sort();
                                if (terms[small_index].name != terms[j].name)
                                {
                                    if (Math.Abs(terms[small_index].details[i].positions[0] - terms[small_index].details[i].positions[0]) > smallposition) {
                                        smallposition = Math.Abs(terms[small_index].details[i].positions[0] - terms[small_index].details[i].positions[0]);

                                    }

                                }

                                check++;//that mean it found intersection DocID
                                break;
                            }

                        }

                    }
                    if (check == terms.Count())
                    {

                        //  doc_list.Add(terms[small_index].details[i].doc_id);
                        if (!mydic.ContainsKey(terms[small_index].details[i].doc_id))
                            mydic.Add(terms[small_index].details[i].doc_id, smallposition);


                    }

                }
                //sort ascending according To Frequency 
                var items = from pair in mydic
                            orderby pair.Value ascending,
                                    pair.Key
                            select pair;
                List<string> links = new List<string>();
                //Print The Result
                if (mydic.Count() > 0)
                {

                    foreach (var i in items)
                    {
                        SqlCommand cmd = new SqlCommand("select [linkUrl]  from [IRdb].[dbo].[AllPages] where [id]= @doc_id ", conn);
                        cmd.Parameters.AddWithValue("@doc_id", i.Key);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String link = reader["linkUrl"].ToString();
                                // ResultsBox.Items.Add(link);


                                links.Add("<a href=" + link + ">" + link + "'</a><p></p> <br />");
                            }

                        }
                    }
                    List<string> newlinks = links.Distinct().ToList();
                    // links = links.Distinct().ToList();
                    divLinks.InnerHtml = "";
                    for (int i = 0; i < newlinks.Count(); i++)
                    {
                        divLinks.InnerHtml += newlinks[i].ToString();


                    }
                    DidUMeanLabelID.Text = "";

                }
                else
                {
                    divLinks.InnerHtml = "";
                    DidUMeanLabelID.Text = "No Matching Results Found";
                }


            }
            else
            {
                divLinks.InnerHtml = "";
                DidUMeanLabelID.Text = "No Matching Results Found";
            }

        }

        public void ExactSearch(List<Term> terms, string[] query, ListBox ResultsBox, Label DidUMeanLabelID, System.Web.UI.HtmlControls.HtmlGenericControl divLinks) {
            int check = 0, check_pos = 0;
            Boolean pos = false;
            List<int> doc_list = new List<int>();
            List<int> exact_doc_list = new List<int>();
            Dictionary<int, int> mydic = new Dictionary<int, int>();
            if (terms.Count() == query.Count())
            {
                //Get Documents Contain Query Words
                for (int i = 0; i < terms[0].details.Count(); i++)
                {
                    check = 0;
                    check_pos = 0;
                    for (int j = 0; j < terms.Count(); j++)
                    {
                        for (int k = 0; k < terms[j].details.Count(); k++)
                        {
                            if (terms[0].details[i].doc_id == terms[j].details[k].doc_id)
                            {
                                pos = false;
                                for (int z = 0; z < terms[0].details[i].positions.Count(); z++)
                                {
                                    for (int x = 0; x < terms[j].details[k].positions.Count(); x++)
                                    {
                                        if (terms[0].details[i].positions[z] - terms[j].details[k].positions[x] == -j)
                                        {
                                            pos = true;
                                            break;

                                        }

                                    }

                                }
                                if (pos == true)
                                {
                                    check_pos++;
                                }

                                check++;
                                break;
                            }


                        }

                    }
                    if (check == terms.Count() && check_pos == terms.Count())
                    {
                        mydic.Add(terms[0].details[i].doc_id, terms[0].details[i].frequency);
                    }

                }
                //sort Descending according To Frequency 
                var items = from pair in mydic
                            orderby pair.Value descending,
                                    pair.Key
                            select pair;
                List<string> links = new List<string>();
                //Print The Result
                if (mydic.Count() > 0)
                {

                    foreach (var i in items)
                    {
                        SqlCommand cmd = new SqlCommand("select [linkUrl] FROM [IRdb].[dbo].[AllPages] where [id]= @doc_id ", conn);
                        cmd.Parameters.AddWithValue("@doc_id", i.Key);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                String link = reader["linkUrl"].ToString();
                                //   ResultsBox.Items.Add(link );

                                links.Add("<a href=" + link + ">" + link + "'</a><p></p> <br />");


                                links.Add("<a href=" + link + ">" + link + "'</a><p></p> <br />");
                            }

                        }
                    }
                    List<string> newlinks = links.Distinct().ToList();
                    divLinks.InnerHtml = "";

                    for (int i = 0; i < newlinks.Count(); i++)
                    {
                        divLinks.InnerHtml += newlinks[i].ToString();


                    }
                    DidUMeanLabelID.Text = "";
                }
                else
                {
                    divLinks.InnerHtml = "";

                    DidUMeanLabelID.Text = "No Matching Results Found";
                }


            }
            else
            {
                divLinks.InnerHtml = "";

                DidUMeanLabelID.Text = "No Matching Results Found";
            }

        }

        public static string GetWordSoundx(string input)
        {
            List<char> find;
            Dictionary<int, List<char>> dr = new Dictionary<int, List<char>>();
            List<char> arr = new List<char> { 'a', 'e', 'i', 'o', 'u', 'h', 'w', 'y' };
            List<char> arr1 = new List<char> { 'b', 'f', 'p', 'v' };
            List<char> arr2 = new List<char> { 'c', 'g', 'j', 'k', 'q', 's', 'x', 'z' };
            List<char> arr3 = new List<char> { 'd', 't' };
            List<char> arr4 = new List<char> { 'l' };
            List<char> arr5 = new List<char> { 'm', 'n' };
            List<char> arr6 = new List<char> { 'r' };
            dr.Add(0, arr);
            dr.Add(1, arr1);
            dr.Add(2, arr2);
            dr.Add(3, arr3);
            dr.Add(4, arr4);
            dr.Add(5, arr5);
            dr.Add(6, arr6);
            string template = "";
            template = input[0].ToString();
            for (int i = 1; i < input.Length; i++)
            {
                for (int j = 0; j < dr.Count(); j++)
                {
                    find = dr[j];
                    if (find.Contains(input[i]))
                    {
                        template = template + j.ToString();
                    }


                }

            }
            string output = "";
            for (int i = 1; i < template.Length - 1; i++)
            {
                if (!(template[i] == template[i + 1]))
                {
                    output = output + template[i];

                }
            }
            if (input.Length > 1)
            {
                output = template[0] + output + template[template.Length - 1];
            }
            else
            {

                output = template[0] + output;
            }
            string o = "";
            for (int i = 0; i < output.Length; i++)
                if (!(output[i] == '0'))
                {

                    o = o + output[i];

                }
            while (o.Length < 4)
            {

                o = o + "0";
            }

            return o;
        }

        public void singleWordSearch(List<Term> terms, string[] query, ListBox ResultsBox, Label DidUMeanLabelID, System.Web.UI.HtmlControls.HtmlGenericControl divLinks) {
            Dictionary<int, int> mydic = new Dictionary<int, int>();
            for (int i = 0; i < terms.Count(); i++) {
                for (int j = 0; j < terms[i].details.Count; j++) {
                    if(!mydic.ContainsKey(terms[i].details[j].doc_id))
                        mydic.Add(terms[i].details[j].doc_id,terms[i].details[j].frequency);
                }
            }
            //sort Descending according To Frequency 
            var items = from pair in mydic
                        orderby pair.Value descending,
                                pair.Key
                        select pair;

            List<string> links = new List<string>();
            //Print The Result
            if (mydic.Count() > 0)
            {

                foreach (var i in items)
                {
                    SqlCommand cmd = new SqlCommand("select [linkUrl] FROM [IRdb].[dbo].[AllPages] where [id]= @doc_id ", conn);
                    cmd.Parameters.AddWithValue("@doc_id", i.Key);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String link = reader["linkUrl"].ToString();
                            //   ResultsBox.Items.Add(link );

                            links.Add("<a href=" + link + ">" + link + "'</a><p></p> <br />");


                            links.Add("<a href=" + link + ">" + link + "'</a><p></p> <br />");
                        }

                    }
                }
                List<string> newlinks = links.Distinct().ToList();
                divLinks.InnerHtml = "";
                //To Fill Div With Links 
                for (int i = 0; i < newlinks.Count(); i++)
                {
                    divLinks.InnerHtml += newlinks[i].ToString();


                }
                DidUMeanLabelID.Text = "";
            }
            else
            {
                divLinks.InnerHtml = "";

                DidUMeanLabelID.Text = "No Matching Results Found";
            }

        }
    }
     
}