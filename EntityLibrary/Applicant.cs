using ExceptionLibrary;
using System;
using System.Collections.Generic;

namespace EntityLibrary
{
    public class Applicant
    {
        public int ApplicantID { get; private set; } // Make it private set for encapsulation
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public string Resume { get; private set; }

        private List<JobApplication> jobApplications = new List<JobApplication>();

        // Constructor to initialize ApplicantID and other properties if needed
        public Applicant(int applicantID)
        {
            ApplicantID = applicantID;
        }

        // Create the applicant's profile
        public void CreateProfile(string email, string firstName, string lastName, string phone)
        {
            if (!IsValidEmail(email))
            {
                throw new InvalidEmailFormatException("Email format is invalid");
            }

            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Phone = phone;

            Console.WriteLine($"Profile created for {firstName} {lastName}.");
        }

        // Apply for a specific job
        public void ApplyForJob(JobListing job, string coverLetter)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job), "Job cannot be null");
            }

            job.Apply(this.ApplicantID, coverLetter); // Ensure Apply method exists in JobListing class

            var jobApplication = new JobApplication
            {
                JobID = job.JobID,
                ApplicantID = this.ApplicantID,
                CoverLetter = coverLetter,
                ApplicationDate = DateTime.Now
            };

            jobApplications.Add(jobApplication);
            Console.WriteLine($"{FirstName} {LastName} applied for job '{job.JobTitle}' with cover letter: {coverLetter}");
        }

        // Validate email format
        private bool IsValidEmail(string email)
        {
            return email.Contains("@");  // Simplified email validation
        }
    }
}
