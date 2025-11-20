using System.Data.SqlClient;

namespace part_1.Models
{
    public class all_method
    {

        private string connection = @"Server=(localdb)\claim_system;Database=claims_database;";




        public string login_user(string email , string role , string password)
        {
            string message = "no";
            string query = @"select * from Users where email='"+email+"' and role='"+role+"' and password='"+password+"';";


            try
            {

                using (SqlConnection connect = new SqlConnection(connection))
                {


                    connect.Open();


                    using (SqlCommand finds = new SqlCommand(query ,connect))
                    {

                        using (SqlDataReader found = finds.ExecuteReader() )
                        {

                            while (found.Read())
                            {

                                message = found["userID"] + "," + found["role"];

                            }

                        }

                    }

                        connect.Close();

                }


            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }






            return message;
        }






        public string claims_submit(claims claim)
        {
            string message = "no";

            string query = @"
        INSERT INTO Claims 
        (number_of_sessions, number_of_hours, amount_of_rate, module_name, faculty_name, supporting_documents, claim_status, creating_date, lecturerID)
        VALUES 
        (@sessions, @hours, @rate, @module, @faculty, @document, @status, @date, @lecturerID);
    ";

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    using (SqlCommand insert = new SqlCommand(query, connect))
                    {
                        // Bind parameters safely to prevent SQL injection
                        insert.Parameters.AddWithValue("@sessions", claim.Sessions);
                        insert.Parameters.AddWithValue("@hours", claim.Hours);
                        insert.Parameters.AddWithValue("@rate", claim.Rate);
                        insert.Parameters.AddWithValue("@module", claim.NameModule);
                        insert.Parameters.AddWithValue("@faculty", claim.Name);
                        insert.Parameters.AddWithValue("@document", string.IsNullOrEmpty(claim.SupportingDocumentPath) ? DBNull.Value : claim.SupportingDocumentPath);
                        insert.Parameters.AddWithValue("@status", "Pending");
                        insert.Parameters.AddWithValue("@date", DateTime.Now);
                        insert.Parameters.AddWithValue("@lecturerID", claim.LecturerID);

                        int rowsAffected = insert.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            message = "Claim submitted successfully";
                        }
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                message = "Error: " + error.Message;
            }

            return message;
        }




        public List<claims> get_all_claims(string lecturerId)
        {
            List<claims> allClaims = new List<claims>();

            string query = @"
        SELECT 
            claimID,
            number_of_sessions,
            number_of_hours,
            amount_of_rate,
            module_name,
            faculty_name,
            supporting_documents,
            claim_status,
            creating_date,
            lecturerID
        FROM Claims
        WHERE lecturerID = @lecturerID
        ORDER BY creating_date DESC;
    ";

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@lecturerID", lecturerId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                claims claim = new claims
                                {
                                    ClaimID = Convert.ToInt32(reader["claimID"]),
                                    Sessions = Convert.ToInt32(reader["number_of_sessions"]),
                                    Hours = Convert.ToInt32(reader["number_of_hours"]),
                                    Rate = Convert.ToInt32(reader["amount_of_rate"]),
                                    NameModule = reader["module_name"].ToString(),
                                    Name = reader["faculty_name"].ToString(),
                                    SupportingDocumentPath = reader["supporting_documents"] == DBNull.Value ? null : reader["supporting_documents"].ToString(),
                                    ClaimStatus = reader["claim_status"].ToString(),
                                    CreatingDate = Convert.ToDateTime(reader["creating_date"]),
                                    LecturerID = Convert.ToInt32(reader["lecturerID"])
                                };

                                allClaims.Add(claim);
                            }
                        }
                    }

                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching claims: " + ex.Message);
            }

            return allClaims;
        }





        public List<claims> get_all_claim()
        {
            List<claims> allClaims = new List<claims>();

            string query = @"
        SELECT 
            claimID,
            number_of_sessions,
            number_of_hours,
            amount_of_rate,
            module_name,
            faculty_name,
            supporting_documents,
            claim_status,
            creating_date,
            lecturerID
        FROM Claims
        WHERE claim_status='pending'
        ORDER BY creating_date DESC;
    ";

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                claims claim = new claims
                                {
                                    ClaimID = Convert.ToInt32(reader["claimID"]),
                                    Sessions = Convert.ToInt32(reader["number_of_sessions"]),
                                    Hours = Convert.ToInt32(reader["number_of_hours"]),
                                    Rate = Convert.ToInt32(reader["amount_of_rate"]),
                                    NameModule = reader["module_name"].ToString(),
                                    Name = reader["faculty_name"].ToString(),
                                    SupportingDocumentPath = reader["supporting_documents"] == DBNull.Value ? null : reader["supporting_documents"].ToString(),
                                    ClaimStatus = reader["claim_status"].ToString(),
                                    CreatingDate = Convert.ToDateTime(reader["creating_date"]),
                                    LecturerID = Convert.ToInt32(reader["lecturerID"])
                                };

                                allClaims.Add(claim);
                            }
                        }
                    }

                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching claims: " + ex.Message);
            }

            return allClaims;
        }
        public List<claims> get_all_claims()
        {
            List<claims> allClaims = new List<claims>();

            string query = @"
        SELECT 
            claimID,
            number_of_sessions,
            number_of_hours,
            amount_of_rate,
            module_name,
            faculty_name,
            supporting_documents,
            claim_status,
            creating_date,
            lecturerID
        FROM Claims
        WHERE claim_status='Pre-Approved'
        ORDER BY creating_date DESC;
    ";

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                claims claim = new claims
                                {
                                    ClaimID = Convert.ToInt32(reader["claimID"]),
                                    Sessions = Convert.ToInt32(reader["number_of_sessions"]),
                                    Hours = Convert.ToInt32(reader["number_of_hours"]),
                                    Rate = Convert.ToInt32(reader["amount_of_rate"]),
                                    NameModule = reader["module_name"].ToString(),
                                    Name = reader["faculty_name"].ToString(),
                                    SupportingDocumentPath = reader["supporting_documents"] == DBNull.Value ? null : reader["supporting_documents"].ToString(),
                                    ClaimStatus = reader["claim_status"].ToString(),
                                    CreatingDate = Convert.ToDateTime(reader["creating_date"]),
                                    LecturerID = Convert.ToInt32(reader["lecturerID"])
                                };

                                allClaims.Add(claim);
                            }
                        }
                    }

                    connect.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching claims: " + ex.Message);
            }

            return allClaims;
        }


        public void delete_claim(int claimID)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = "DELETE FROM Claims WHERE claimID = @claimID AND claim_status != 'Approved'";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@claimID", claimID);

                        connect.Open();
                        cmd.ExecuteNonQuery();
                        connect.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting claim: " + ex.Message);
            }
        }


        public void update_claim_status(int claimID, string status)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    string query = "UPDATE Claims SET claim_status = @status WHERE claimID = @claimID";

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@claimID", claimID);

                        connect.Open();
                        cmd.ExecuteNonQuery();
                        connect.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating claim status: " + ex.Message);
            }
        }

        public string register_user(register_users user)
        {
            string message = "failed";
            if (user.email=="")
            {
                return message;
            }
            string query = @"INSERT INTO Users (full_names, surname, email, role, gender, password, date)
                     VALUES (@full_names, @surname, @mail, @role, @gender, @pass, @date)";

            try
            {
                using (SqlConnection connect = new SqlConnection(connection))
                {
                    connect.Open();

                    using (SqlCommand cmd = new SqlCommand(query, connect))
                    {
                        cmd.Parameters.AddWithValue("@full_names", user.name);
                        cmd.Parameters.AddWithValue("@surname", user.surname);
                        cmd.Parameters.AddWithValue("@mail", user.email);
                        cmd.Parameters.AddWithValue("@role", user.role);
                        cmd.Parameters.AddWithValue("@gender", "none");
                        cmd.Parameters.AddWithValue("@pass", user.password);
                        cmd.Parameters.AddWithValue("@date", DateTime.Now.Date);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            message = "success";
                        }
                    }

                    connect.Close();
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }

            return message;
        }

    }
}
