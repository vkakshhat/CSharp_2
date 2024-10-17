create database CareerHub;

use CareerHub;

CREATE TABLE Company (
    CompanyID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing CompanyID
    CompanyName VARCHAR(255) NOT NULL,
    Location VARCHAR(255)
);

CREATE TABLE JobListing (
    JobID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing JobID
    CompanyID INT NOT NULL, -- Foreign key to Company
    JobTitle VARCHAR(255) NOT NULL,
    JobDescription TEXT NOT NULL,
    JobLocation VARCHAR(255),
    Salary DECIMAL(18, 2),
    JobType VARCHAR(50),
    PostedDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (CompanyID) REFERENCES Company(CompanyID) -- Reference to Company table
);

CREATE TABLE Applicant (
    ApplicantID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing ApplicantID
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    Phone VARCHAR(50),
    Resume VARCHAR(255) -- Reference to resume file path
);
CREATE TABLE JobApplication (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1), -- Auto-incrementing ApplicationID
    JobID INT NOT NULL, -- Foreign key to JobListing
    ApplicantID INT NOT NULL, -- Foreign key to Applicant
    ApplicationDate DATETIME DEFAULT GETDATE(),
    CoverLetter TEXT,
    FOREIGN KEY (JobID) REFERENCES JobListing(JobID) ON DELETE CASCADE, -- When a Job is deleted, remove the applications
    FOREIGN KEY (ApplicantID) REFERENCES Applicant(ApplicantID) ON DELETE CASCADE -- When an Applicant is deleted, remove the applications
);

-- Insert sample companies
INSERT INTO Company (CompanyName, Location)
VALUES 
('TechCorp', 'New York, NY'),
('Innovatech', 'San Francisco, CA'),
('HealthWorks', 'Chicago, IL');


-- Insert sample job listings
INSERT INTO JobListing (CompanyID, JobTitle, JobDescription, JobLocation, Salary, JobType, PostedDate)
VALUES
(1, 'Software Engineer', 'Develop and maintain web applications.', 'New York, NY', 95000, 'Full-time', GETDATE()),
(2, 'Data Scientist', 'Analyze data and create predictive models.', 'San Francisco, CA', 120000, 'Full-time', GETDATE()),
(3, 'Project Manager', 'Manage projects and coordinate with stakeholders.', 'Chicago, IL', 85000, 'Contract', GETDATE());


-- Insert sample applicants
INSERT INTO Applicant (FirstName, LastName, Email, Phone, Resume)
VALUES 
('John', 'Doe', 'john.doe@example.com', '123-456-7890', 'john_resume.pdf'),
('Jane', 'Smith', 'jane.smith@example.com', '098-765-4321', 'jane_resume.pdf'),
('Mark', 'Taylor', 'mark.taylor@example.com', '111-222-3333', 'mark_resume.pdf');


-- Insert sample job applications
INSERT INTO JobApplication (JobID, ApplicantID, ApplicationDate, CoverLetter)
VALUES
(1, 1, GETDATE(), 'I am passionate about software development and would love to join your team.'),
(2, 2, GETDATE(), 'I have a strong background in data science and believe I am a great fit for this role.'),
(3, 3, GETDATE(), 'With years of project management experience, I am confident in my ability to contribute.');

