using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using EntityLibrary;
using DAOLibrary;
using UtilityLibrary;
using ExceptionLibrary;

namespace CareerHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string connectionString = DBPropertyUtil.GetConnectionString("dbConfig.properties");
            var dbManager = new DatabaseManager(connectionString);

            while (true)
            {
                Console.WriteLine("\nWelcome to CareerHub Job Board");
                Console.WriteLine("1. Post a Job");
                Console.WriteLine("2. Apply for a Job");
                Console.WriteLine("3. View Job Listings");
                Console.WriteLine("4. View Companies");
                Console.WriteLine("5. Create Applicant Profile");
                Console.WriteLine("6. Calculate Average Salary");
                Console.WriteLine("7. Exit");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            PostJob(dbManager);
                            break;
                        case 2:
                            ApplyForJob(dbManager);
                            break;
                        case 3:
                            ViewJobListings(dbManager);
                            break;
                        case 4:
                            ViewCompanies(dbManager);
                            break;
                        case 5:
                            CreateApplicantProfile(dbManager);
                            break;
                        case 6:
                            CalculateAverageSalary(dbManager);
                            break;
                        case 7:
                            Console.WriteLine("Exiting the system...");
                            return; // Exit the application
                        default:
                            Console.WriteLine("Invalid option. Please select a valid choice.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }
        }

        // post a job
        public static void PostJob(DatabaseManager dbManager)
        {
            try
            {
                // Create or retrieve Company
                Console.WriteLine("Enter Company Name: ");
                string companyName = Console.ReadLine();

                Console.WriteLine("Enter Company Location: ");
                string companyLocation = Console.ReadLine();

                // Create a new Company object
                Company company = new Company
                {
                    CompanyName = companyName,
                    Location = companyLocation
                };

                // Insert the company into the database
                dbManager.InsertCompany(company);

                Console.WriteLine("Enter Job Title: ");
                string jobTitle = Console.ReadLine();

                Console.WriteLine("Enter Job Description: ");
                string jobDescription = Console.ReadLine();

                Console.WriteLine("Enter Job Location: ");
                string jobLocation = Console.ReadLine();

                Console.WriteLine("Enter Salary: ");
                decimal salary = Convert.ToDecimal(Console.ReadLine());

                Console.WriteLine("Enter Job Type (Full-time, Part-time, Contract): ");
                string jobType = Console.ReadLine();

                JobListing job = new JobListing
                {
                    CompanyID = company.CompanyID, // Assuming CompanyID is set after inserting
                    JobTitle = jobTitle,
                    JobDescription = jobDescription,
                    JobLocation = jobLocation,
                    Salary = salary,
                    JobType = jobType,
                    PostedDate = DateTime.Now
                };

                dbManager.InsertJobListing(job, company); // Pass both job and company
                Console.WriteLine($"Job '{jobTitle}' posted successfully!");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while posting the job: {ex.Message}");
            }
        }

        // Method for an applicant to apply for a job
        public static void ApplyForJob(DatabaseManager dbManager)
        {
            try
            {
                Console.WriteLine("Enter application deadline (yyyy-mm-dd): ");
                DateTime deadline = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Enter Applicant ID: ");
                int applicantID = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter Job ID: ");
                int jobID = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter Cover Letter: ");
                string coverLetter = Console.ReadLine();

                if (DateTime.Now > deadline)
                {
                    throw new ApplicationDeadlineException("Application submitted after the deadline.");
                }

                JobApplication application = new JobApplication
                {
                    ApplicantID = applicantID,
                    JobID = jobID,
                    ApplicationDate = DateTime.Now,
                    CoverLetter = coverLetter
                };

                dbManager.InsertJobApplication(application, deadline);
                Console.WriteLine("Job application inserted successfully!");
            }
            catch (ApplicationDeadlineException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying for job: {ex.Message}");
            }
        }

        // Method to view all job listings
        public static void ViewJobListings(DatabaseManager dbManager)
        {
            try
            {
                var jobListings = dbManager.GetJobListings();
                Console.WriteLine("\nJob Listings:");
                foreach (var job in jobListings)
                {
                    Console.WriteLine($"Job ID: {job.JobID}, Title: {job.JobTitle}, Location: {job.JobLocation}, Salary: {job.Salary:C}, Type: {job.JobType}");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving job listings: {ex.Message}");
            }
        }

        // Method to view all companies
        public static void ViewCompanies(DatabaseManager dbManager)
        {
            try
            {
                var companies = dbManager.GetCompanies();
                Console.WriteLine("\nCompanies:");
                foreach (var company in companies)
                {
                    Console.WriteLine($"Company ID: {company.CompanyID}, Name: {company.CompanyName}, Location: {company.Location}");
                }
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving companies: {ex.Message}");
            }
        }

        // Method to create an applicant profile
        public static void CreateApplicantProfile(DatabaseManager dbManager)
        {
            try
            {
                Console.WriteLine("Enter First Name: ");
                string firstName = Console.ReadLine();

                Console.WriteLine("Enter Last Name: ");
                string lastName = Console.ReadLine();

                string email;
                while (true)
                {
                    Console.WriteLine("Enter Email: ");
                    email = Console.ReadLine();
                    try
                    {
                        if (!IsValidEmail(email))
                        {
                            throw new InvalidEmailFormatException("Email format is invalid.");
                        }
                        break; // Break if valid
                    }
                    catch (InvalidEmailFormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Console.WriteLine("Enter Phone: ");
                string phone = Console.ReadLine();

                Console.WriteLine("Enter Resume File Path: ");
                string resumeFilePath = Console.ReadLine();
                try
                {
                    UploadFile(resumeFilePath);
                }
                catch (FileUploadException ex)
                {
                    Console.WriteLine(ex.Message);
                    return; // Exit if file upload failed
                }

                Applicant applicant = new Applicant
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Resume = resumeFilePath
                };

                dbManager.InsertApplicant(applicant);
                Console.WriteLine($"Profile created for {firstName} {lastName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the applicant profile: {ex.Message}");
            }
        }

        // Method to calculate average salary
        public static void CalculateAverageSalary(DatabaseManager dbManager)
        {
            try
            {
                decimal averageSalary = dbManager.CalculateAverageSalary();
                Console.WriteLine($"Average Salary: {averageSalary:C}");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DatabaseConnectionException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while calculating the average salary: {ex.Message}");
            }
        }

        // Method to validate email format
        private static bool IsValidEmail(string email)
        {
            return email.Contains("@"); // Simplified validation for demonstration
        }

        // Simulate file upload handling
        private static void UploadFile(string filePath)
        {
            // Simulating different scenarios
            if (!File.Exists(filePath))
            {
                throw new FileUploadException("File not found.");
            }

            long fileSize = new FileInfo(filePath).Length;
            if (fileSize > 2 * 1024 * 1024) // Example: 2 MB limit
            {
                throw new FileUploadException("File size exceeds the allowed limit.");
            }

            // Check for file format if necessary
            string fileExtension = Path.GetExtension(filePath).ToLower();
            if (fileExtension != ".pdf" && fileExtension != ".doc" && fileExtension != ".docx")
            {
                throw new FileUploadException("Unsupported file format. Only PDF and Word documents are allowed.");
            }

            Console.WriteLine("File uploaded successfully.");
        }
    }

}
