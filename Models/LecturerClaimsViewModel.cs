using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace part_1.Models
{                   /// ViewModel to load and store approved lecturer claims from the database
    public class LecturerClaimViewModel
    {
        public List<int> ClaimIDs { get; set; }
        public List<string> LecturerIDs { get; set; }
        public List<string> FullNames { get; set; }
        public List<string> Emails { get; set; }
        public List<int> NumberOfSessions { get; set; }
        public List<int> NumberOfHours { get; set; }
        public List<decimal> Rates { get; set; }
        public List<decimal> TotalAmounts { get; set; }
        public List<string> ModuleNames { get; set; }
        public List<string> FacultyNames { get; set; }
        public List<string> SupportingDocuments { get; set; }
        public List<string> ClaimStatuses { get; set; }
        public List<DateTime> CreatingDates { get; set; }

        public LecturerClaimViewModel()
        {
            ClaimIDs = new List<int>();
            LecturerIDs = new List<string>();
            FullNames = new List<string>();
            Emails = new List<string>();
            NumberOfSessions = new List<int>();
            NumberOfHours = new List<int>();
            Rates = new List<decimal>();
            TotalAmounts = new List<decimal>();
            ModuleNames = new List<string>();
            FacultyNames = new List<string>();
            SupportingDocuments = new List<string>();
            ClaimStatuses = new List<string>();
            CreatingDates = new List<DateTime>();

            LoadApprovedClaims();
        }

        private void LoadApprovedClaims()
        {
            string connection = @"Server=(localdb)\claim_system;Database=claims_database;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connection))
                {
                    conn.Open();

                    string query = @"
                        SELECT c.claimID,
                               c.lecturerID,
                               u.full_names + ' ' + u.surname AS FullName,
                               u.email,
                               c.number_of_sessions,
                               c.number_of_hours,
                               c.amount_of_rate,
                               (c.number_of_sessions * c.amount_of_rate) AS total_amount,
                               c.module_name,
                               c.faculty_name,
                               c.supporting_documents,
                               c.claim_status,
                               c.creating_date
                        FROM Claims c
                        INNER JOIN Users u ON c.lecturerID = u.userID
                        WHERE c.claim_status = 'Approved'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClaimIDs.Add(Convert.ToInt32(reader["claimID"]));
                            LecturerIDs.Add(reader["lecturerID"].ToString());
                            FullNames.Add(reader["FullName"].ToString());
                            Emails.Add(reader["email"].ToString());
                            NumberOfSessions.Add(Convert.ToInt32(reader["number_of_sessions"]));
                            NumberOfHours.Add(Convert.ToInt32(reader["number_of_hours"]));
                            Rates.Add(Convert.ToDecimal(reader["amount_of_rate"]));
                            TotalAmounts.Add(Convert.ToDecimal(reader["total_amount"]));
                            ModuleNames.Add(reader["module_name"].ToString());
                            FacultyNames.Add(reader["faculty_name"].ToString());
                            SupportingDocuments.Add(reader["supporting_documents"].ToString());
                            ClaimStatuses.Add(reader["claim_status"].ToString());
                            CreatingDates.Add(Convert.ToDateTime(reader["creating_date"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading claims: " + ex.Message);
            }
        }
    }



    public class ClaimItem
    {
        public int ClaimID { get; set; }
        public string ModuleName { get; set; }
        public string LecturerName { get; set; }

        public int NumberOfSessions { get; set; }
        public int NumberOfHours { get; set; }
        public decimal Rate { get; set; }
        public decimal TotalAmount { get; set; }
        public string FacultyName { get; set; }
        public string SupportingDocument { get; set; }
        public DateTime CreatingDate { get; set; }
    }
}


