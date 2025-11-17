using System;

namespace StudentEnrollmentApp.Domain
{
    /// <summary>
    /// Contract for an object that has a unique string Id.
    /// Demonstrates an interface as a "contract".
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public interface IIdentifiable
    {
        string Id { get; }
    }

    /// <summary>
    /// Contract for enrollable entities (e.g., Student) with basic operations.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public interface IEnrollable
    {
        /// <summary>Adds the entity to the system.</summary>
        void Enroll();
        /// <summary>Removes the entity from the system.</summary>
        void Withdraw();
    }

    /// <summary>
    /// Delegate type used by the enrollment domain to notify about significant
    /// changes (e.g., when a student is added).
    /// </summary>
    public delegate void EnrollmentChangedHandler(object sender, EnrollmentChangedEventArgs e);


    /// <summary>
    /// EventArgs specialization carrying domain data for enrollment changes.
    /// </summary>
    public sealed class EnrollmentChangedEventArgs : EventArgs
    {
        public string Message { get; }
        public DateTime OccurredAt { get; } = DateTime.Now;
        public Student Student { get; }
        public EnrollmentChangedEventArgs(string message, Student student)
        {
            Message = message;
            Student = student;
        }
    }
}
