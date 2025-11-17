using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentEnrollmentApp.Domain
{
    /// <summary>
    /// In-memory domain service that manages Student entities using generic
    /// collections (List, SortedList) and exposes an event based on a custom
    /// delegate. Also demonstrates a simple Undo stack.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public sealed class EnrollmentService
    {
        // Primary storage – dynamic sized generic list
        private readonly List<Student> _students = new List<Student>();

        // Index by faculty number for quick lookup (SortedList ensures sorted keys)
        private readonly SortedList<string, Student> _byFaculty = new SortedList<string, Student>(StringComparer.OrdinalIgnoreCase);

        // Undo stack (LIFO) – demonstrates Stack<T>
        private readonly Stack<Action> _undo = new Stack<Action>();

        /// <summary>
        /// Event that fires when enrollment list changes (add/remove/undo).
        /// </summary>
        public event EnrollmentChangedHandler EnrollmentChanged;

        /// <summary>Snapshot of all students (copy) for binding.</summary>
        public IReadOnlyList<Student> All() => _students.ToList();

        /// <summary>
        /// Adds a student; raises event; pushes an undo action on the stack.
        /// </summary>
        public void Add(Student s)
        {
            if (s is null) throw new ArgumentNullException(nameof(s));
            if (_byFaculty.ContainsKey(s.FacultyNumber))
                throw new InvalidOperationException($"Student with FN={s.FacultyNumber} already exists.");

            _students.Add(s);
            _byFaculty.Add(s.FacultyNumber, s);
            s.Enroll();

            // Register undo: remove what we just added
            _undo.Push(() =>
            {
                _students.Remove(s);
                _byFaculty.Remove(s.FacultyNumber);
                s.Withdraw();
                EnrollmentChanged?.Invoke(this, new EnrollmentChangedEventArgs($"Undo: removed {s.FullName} (FN {s.FacultyNumber}).", s));
            });

            EnrollmentChanged?.Invoke(this, new EnrollmentChangedEventArgs($"Added {s.FullName} (FN {s.FacultyNumber}).", s));
        }

        /// <summary>
        /// Removes a student by faculty number if present; registers undo.
        /// </summary>
        public bool RemoveByFacultyNumber(string facultyNumber)
        {
            if (string.IsNullOrWhiteSpace(facultyNumber)) return false;
            if (!_byFaculty.TryGetValue(facultyNumber.Trim(), out var s)) return false;


            _students.Remove(s);
            _byFaculty.Remove(facultyNumber.Trim());
            s.Withdraw();


            // Undo: re-add the same student
            _undo.Push(() =>
            {
                _students.Add(s);
                _byFaculty.Add(s.FacultyNumber, s);
                s.Enroll();
                EnrollmentChanged?.Invoke(this, new EnrollmentChangedEventArgs($"Undo: re-added {s.FullName} (FN {s.FacultyNumber}).", s));
            });


            EnrollmentChanged?.Invoke(this, new EnrollmentChangedEventArgs($"Removed {s.FullName} (FN {s.FacultyNumber}).", s));
            return true;
        }

        /// <summary>Executes the last undo action if available.</summary>
        public bool Undo()
        {
            if (_undo.Count == 0) return false;
            var action = _undo.Pop();
            action();
            return true;
        }

        /// <summary>Returns students sorted by IComparable (LastName, FirstName, FN).</summary>
        public IReadOnlyList<Student> SortedByName() => _students.OrderBy(s => s).ToList();


        /// <summary>Returns students sorted by faculty number.</summary>
        public IReadOnlyList<Student> SortedByFaculty() => _students.OrderBy(s => s.FacultyNumber, StringComparer.OrdinalIgnoreCase).ToList();


        /// <summary>Simple search by substring in name or FN.</summary>
        public IReadOnlyList<Student> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return All();
            term = term.Trim();
            return _students.Where(s => s.FullName.IndexOf(term, StringComparison.CurrentCultureIgnoreCase) >= 0
            || s.FacultyNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();
        }
    }
}
