using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ExceptionLibrary;
using EntityLibrary;


namespace DAOLibrary
{
    public class DatabaseManager
    {
        private string connectionString;

        public DatabaseManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //new Job Listing
        public void InsertJobListing(JobListing job, Company company)
        {
            try
            {
                // Check if the company exists
                int companyID = GetCompanyID(company.CompanyName);
                if (companyID == 0)
                {
                    InsertCompany(company);
                    companyID = GetCompanyID(company.CompanyName); // Get the CompanyID after insertion
                }
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Jobs (CompanyID, JobTitle, JobDescription, JobLocation, Salary, JobType, PostedDate) " +
                                   "VALUES (@CompanyID, @JobTitle, @JobDescription, @JobLocation, @Salary, @JobType, @PostedDate)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyID", companyID);
                        command.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                        command.Parameters.AddWithValue("@JobDescription", job.JobDescription);
                        command.Parameters.AddWithValue("@JobLocation", job.JobLocation);
                        command.Parameters.AddWithValue("@Salary", job.Salary);
                        command.Parameters.AddWithValue("@JobType", job.JobType);
                        command.Parameters.AddWithValue("@PostedDate", job.PostedDate);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Job listing inserted.");
            }
            catch (SqlException ex)
            {
                throw new DataInsertionException($"Error inserting job listing: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
        }
        // Helper method to check if the company exists in the database
        private int GetCompanyID(string companyName)
        {
            int companyID = 0;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT CompanyID FROM Companies WHERE CompanyName = @CompanyName";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyName", companyName);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        if (result != null)
                        {
                            companyID = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataRetrievalException($"Error retrieving company: {ex.Message}");
            }
            return companyID;
        }
        // new Company
        public void InsertCompany(Company company)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Companies (CompanyName, Location) VALUES (@CompanyName, @Location)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                        command.Parameters.AddWithValue("@Location", company.Location);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Company inserted.");
            }
            catch (SqlException ex)
            {
                throw new DataInsertionException($"Error inserting company: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
        }

        //new Applicant
        public void InsertApplicant(Applicant applicant)
        {
            try
            {
                if (!IsValidEmail(applicant.Email))
                {
                    throw new InvalidEmailFormatException("Email format is invalid.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Applicants (FirstName, LastName, Email, Phone, Resume) " +
                                   "VALUES (@FirstName, @LastName, @Email, @Phone, @Resume)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", applicant.FirstName);
                        command.Parameters.AddWithValue("@LastName", applicant.LastName);
                        command.Parameters.AddWithValue("@Email", applicant.Email);
                        command.Parameters.AddWithValue("@Phone", applicant.Phone);
                        command.Parameters.AddWithValue("@Resume", applicant.Resume);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Applicant inserted.");
            }
            catch (SqlException ex)
            {
                throw new DataInsertionException($"Error inserting applicant: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
        }

        //new Job Application
        public void InsertJobApplication(JobApplication application, DateTime applicationDeadline)
        {
            try
            {
                if (DateTime.Now > applicationDeadline)
                {
                    throw new ApplicationDeadlineException("Application submitted after the deadline.");
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Applications (JobID, ApplicantID, ApplicationDate, CoverLetter) " +
                                   "VALUES (@JobID, @ApplicantID, @ApplicationDate, @CoverLetter)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@JobID", application.JobID);
                        command.Parameters.AddWithValue("@ApplicantID", application.ApplicantID);
                        command.Parameters.AddWithValue("@ApplicationDate", application.ApplicationDate);
                        command.Parameters.AddWithValue("@CoverLetter", application.CoverLetter);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("Job application inserted.");
            }
            catch (SqlException ex)
            {
                throw new DataInsertionException($"Error inserting job application: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
        }

        //all Job Listings
        public List<JobListing> GetJobListings()
        {
            var jobListings = new List<JobListing>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Jobs";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var job = new JobListing
                                {
                                    JobID = (int)reader["JobID"],
                                    CompanyID = (int)reader["CompanyID"],
                                    JobTitle = (string)reader["JobTitle"],
                                    JobDescription = (string)reader["JobDescription"],
                                    JobLocation = (string)reader["JobLocation"],
                                    Salary = (decimal)reader["Salary"],
                                    JobType = (string)reader["JobType"],
                                    PostedDate = (DateTime)reader["PostedDate"]
                                };
                                jobListings.Add(job);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataRetrievalException($"Error retrieving job listings: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
            return jobListings;
        }

        //all Companies
        public List<Company> GetCompanies()
        {
            var companies = new List<Company>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Companies";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var company = new Company
                                {
                                    CompanyID = (int)reader["CompanyID"],
                                    CompanyName = (string)reader["CompanyName"],
                                    Location = (string)reader["Location"]
                                };
                                companies.Add(company);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataRetrievalException($"Error retrieving companies: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
            return companies;
        }

        //all Applicants
        public List<Applicant> GetApplicants()
        {
            var applicants = new List<Applicant>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Applicants";

                    using (var command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var applicant = new Applicant
                                {
                                    ApplicantID = (int)reader["ApplicantID"],
                                    FirstName = (string)reader["FirstName"],
                                    LastName = (string)reader["LastName"],
                                    Email = (string)reader["Email"],
                                    Phone = (string)reader["Phone"],
                                    Resume = (string)reader["Resume"]
                                };
                                applicants.Add(applicant);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DataRetrievalException($"Error retrieving applicants: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException($"Database connection error: {ex.Message}");
            }
            return applicants;
        }

        //validate email format
        private bool IsValidEmail(string email)
        {
            return email.Contains("@"); // Simplified validation for demonstration
        }
        public decimal CalculateAverageSalary()
        {
            var jobListings = GetJobListings();
            decimal totalSalary = 0;
            int count = 0;

            foreach (var job in jobListings)
            {
                if (job.Salary < 0)
                {
                    throw new InvalidDataException($"Invalid salary found for Job ID: {job.JobID}");
                }

                totalSalary += job.Salary;
                count++;
            }

            if (count == 0)
            {
                throw new InvalidDataException("No valid jobs available to calculate the average salary.");
            }

            return totalSalary / count; // Return average salary
        }
    }


}
