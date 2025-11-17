// ─────────────────────────────────────────────────────────────────────────────
// CSCB579: Programming Applications with Microsoft Visual C# .NET
// Windows Forms OOP Project – "Student Enrollment" (no database)
// Author: Kalin Miroslavov Toshev, Faculty No: F113093
// Notes: This solution demonstrates OOP pillars (encapsulation, inheritance,
// polymorphism), interfaces, collections, delegates/events, and Windows Forms
// controls & events. All classes and key methods are documented inline.
// ─────────────────────────────────────────────────────────────────────────────


// =============================== Program.cs ===============================
using System;
using System.Windows.Forms;

namespace StudentEnrollmentApp
{
    /// <summary>
    /// Entry point of the Windows Forms application.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Main method: configures WinForms styles and starts MainForm.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
        }
    }
}
