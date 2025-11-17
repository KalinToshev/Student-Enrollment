using System;

namespace StudentEnrollmentApp.Domain
{
    /// <summary>
    /// Abstract base class Person demonstrates encapsulation (private fields
    /// + public properties), inheritance (base for Student) and virtual/override
    /// methods for polymorphism.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public abstract class Person : IIdentifiable
    {
        // Encapsulated backing fields
        private string _firstName;
        private string _lastName;


        /// <summary>Unique identifier (read-only) – interface implementation.</summary>
        public string Id { get; }


        /// <summary>First name – validated and raises computation if needed.</summary>
        public string FirstName
        {
            get => _firstName;
            set => _firstName = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("FirstName is required") : value.Trim();
        }


        /// <summary>Last name – validated.</summary>
        public string LastName
        {
            get => _lastName;
            set => _lastName = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("LastName is required") : value.Trim();
        }


        /// <summary>Date of birth.</summary>
        public DateTime BirthDate { get; set; }


        /// <summary>Computed full name property (read-only).</summary>
        public string FullName => $"{FirstName} {LastName}";


        /// <summary>Base constructor. Enforces Id and base invariants.</summary>
        protected Person(string id, string firstName, string lastName, DateTime birthDate)
        {
            Id = string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("Id is required") : id.Trim();
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }


        /// <summary>Virtual description – can be overridden by subclasses.</summary>
        public virtual string Describe() => $"Person: {FullName} (Id: {Id})";


        /// <summary>Override ToString for easy GUI display (FCL Object basics).</summary>
        public override string ToString() => Describe();
    }

    /// <summary>
    /// Student entity – derives from Person and implements IEnrollable +
    /// IComparable for sorting demonstrations. Includes auto-properties,
    /// constructor chaining, and overridden Describe for polymorphism.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public sealed class Student : Person, IEnrollable, IComparable<Student>
    {
        /// <summary>Faculty number, e.g., F113093.</summary>
        public string FacultyNumber { get; }


        /// <summary>Declared major (specialty).</summary>
        public string Major { get; set; }


        /// <summary>Enrollment flag (in-memory only; no DB required).</summary>
        public bool IsEnrolled { get; private set; }


        public Student(string id, string firstName, string lastName, DateTime birthDate, string facultyNumber, string major)
        : base(id, firstName, lastName, birthDate)
        {
            FacultyNumber = string.IsNullOrWhiteSpace(facultyNumber) ? throw new ArgumentException("FacultyNumber is required") : facultyNumber.Trim();
            Major = string.IsNullOrWhiteSpace(major) ? "Undeclared" : major.Trim();
        }


        /// <summary>Polymorphic description.</summary>
        public override string Describe() => $"Student: {FullName}, FN: {FacultyNumber}, Major: {Major}";


        /// <summary>Implements IEnrollable.Enroll.</summary>
        public void Enroll() => IsEnrolled = true;


        /// <summary>Implements IEnrollable.Withdraw.</summary>
        public void Withdraw() => IsEnrolled = false;


        /// <summary>IComparable implementation: primary by LastName, then FirstName.</summary>
        public int CompareTo(Student other)
        {
            if (other is null) return 1;
            int byLast = string.Compare(LastName, other.LastName, StringComparison.CurrentCultureIgnoreCase);
            if (byLast != 0) return byLast;
            int byFirst = string.Compare(FirstName, other.FirstName, StringComparison.CurrentCultureIgnoreCase);
            if (byFirst != 0) return byFirst;
            return string.Compare(FacultyNumber, other.FacultyNumber, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Override ToString for easy GUI display (FCL Object basics).</summary>
       public override string ToString()
        {
            return $"{FullName} (FN: {FacultyNumber}, Major: {Major}) | Born: {BirthDate:dd.MM.yyyy} | {(IsEnrolled ? "Enrolled" : "Withdrawn")}";
        }
    }
}
