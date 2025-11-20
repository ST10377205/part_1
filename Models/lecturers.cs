using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;

namespace part_1.Models
{
    public class lecturers
    {
        private string connection = @"Server=(localdb)\claim_system;Database=claims_database;";

        public List<UserModel> all_workers()
        {
            List<UserModel> lecturerList = new List<UserModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    string query = @"SELECT userID, full_names, surname, email, role, gender, date
                                     FROM Users
                                     WHERE role = 'lecturer'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lecturerList.Add(new UserModel
                                {
                                    UserID = Convert.ToInt32(reader["userID"]),
                                    FullNames = reader["full_names"].ToString(),
                                    Surname = reader["surname"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Role = reader["role"].ToString(),
                                    Gender = reader["gender"].ToString(),
                                    Date = Convert.ToDateTime(reader["date"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting lecturers: " + ex.Message);
            }

            return lecturerList;
        }

        public void delete_worker(int id)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "DELETE FROM Users WHERE userID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public UserModel get_worker(int id)
        {
            UserModel user = null;

            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = "SELECT * FROM Users WHERE userID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserModel
                            {
                                UserID = Convert.ToInt32(reader["userID"]),
                                FullNames = reader["full_names"].ToString(),
                                Surname = reader["surname"].ToString(),
                                Email = reader["email"].ToString(),
                                Role = reader["role"].ToString(),
                                Gender = reader["gender"].ToString(),
                                Date = Convert.ToDateTime(reader["date"])
                            };
                        }
                    }
                }
            }

            return user;
        }

        public void update_worker(UserModel u)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                string query = @"UPDATE Users
                         SET full_names = @fn,
                             surname = @sn,
                             email = @em,
                             gender = @g
                         WHERE userID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fn", u.FullNames);
                    cmd.Parameters.AddWithValue("@sn", u.Surname);
                    cmd.Parameters.AddWithValue("@em", u.Email);
                    cmd.Parameters.AddWithValue("@g", u.Gender);
                    cmd.Parameters.AddWithValue("@id", u.UserID);

                    cmd.ExecuteNonQuery();
                }
            }
        }






        public class UserModel
        {
            public int UserID { get; set; }
            public string FullNames { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Gender { get; set; }
            public DateTime Date { get; set; }
        }






    }
}
