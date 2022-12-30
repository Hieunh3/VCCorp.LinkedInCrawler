using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCCorp.LinkedInCrawler.Model;

namespace VCCorp.LinkedInCrawler.DAO
{
    public class LinkedInCommentDAO
    {
        private readonly MySqlConnection _conn;
        public LinkedInCommentDAO(string connection)
        {
            _conn = new MySqlConnection(connection);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    _conn.Close();
                    _conn.Dispose();
                }
                else
                {
                    _conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Insert Content to si_demand_resource_post table (bảng chính)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> InsertCommentContent(LinkedInCommentDTO content)
        {
            int res = 0;

            try
            {
                await _conn.OpenAsync();

                string query = "insert ignore crawler_linkedin.content_comment " +
                    "(comment,author_id,author_name,job) " +
                    "values (@comment,@author_id,@author_name,@job)";

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@comment", content.comment);
                cmd.Parameters.AddWithValue("@author_id", content.author_id);
                cmd.Parameters.AddWithValue("@author_name", content.author_name);
                cmd.Parameters.AddWithValue("@job", content.job);
                await cmd.ExecuteNonQueryAsync();

                res = 1;

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa

                    // ghi lỗi xuống fil
                }
            }

            return res;
        }

        /// <summary>
        /// Select URL from tiktok_link table để bóc
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<LinkedInCommentDTO> GetLinkByDomain()
        {
            List<LinkedInCommentDTO> data = new List<LinkedInCommentDTO>();
            string query = $"Select * from crawler_linkedin.content_comment ";
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, _conn))
                {
                    _conn.Open();
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            data.Add(new LinkedInCommentDTO
                            {
                                author_id = reader["author_id"].ToString(),
                                status = (int)reader["status"],
                            }
                            );
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _conn.Close();

            return data;
        }

        /// <summary>
        /// Insert Content to si_demand_resource_post table (bảng chính)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<int> InsertCommentProfile(LinkedInPorfileCommentDTO content)
        {
            int res = 0;

            try
            {
                await _conn.OpenAsync();

                string query = "insert ignore crawler_linkedin.profile_comment " +
                    "(author_name,profile_pic,followers,country,experiences) " +
                    "values (@author_name,@profile_pic,@followers,@country,@experiences)";

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;

                cmd.Parameters.AddWithValue("@author_name", content.author_name);
                cmd.Parameters.AddWithValue("@profile_pic", content.profile_pic);
                cmd.Parameters.AddWithValue("@followers", content.followers);
                cmd.Parameters.AddWithValue("@country", content.country);
                cmd.Parameters.AddWithValue("@experiences", content.experiences);

                await cmd.ExecuteNonQueryAsync();

                res = 1;

            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa

                    // ghi lỗi xuống fil
                }
            }

            return res;
        }
        public async Task<int> UpdateStatus(string author_id)
        {
            LinkedInCommentDTO content = new LinkedInCommentDTO();
            int res = 0;
            try
            {
                await _conn.OpenAsync();

                string query = $"UPDATE crawler_linkedin.content_comment SET status = 1 WHERE author_id = '{author_id}'";

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = _conn;
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@status", content.status);
                await cmd.ExecuteNonQueryAsync();

                res = 1;


            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("duplicate entry"))
                {
                    res = -2; // trùng link
                }
                else
                {
                    res = -1; // lỗi, bắt lỗi trả ra để sửa

                    // ghi lỗi xuống fil
                }
            }
            return res;
        }

    }
}
