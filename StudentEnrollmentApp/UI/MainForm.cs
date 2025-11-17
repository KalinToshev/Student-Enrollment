using StudentEnrollmentApp.Domain;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StudentEnrollmentApp.UI
{
    /// <summary>
    /// MainForm – demonstrates WinForms controls, properties, and events.
    /// It uses EnrollmentService to manage students and reacts to its events.
    /// Author: Kalin Miroslavov Toshev, F113093
    /// </summary>
    public sealed partial class MainForm : Form
    {
        // UI controls (kept minimal and programmatically created)
        private readonly TextBox txtFirstName = new TextBox { Text = "First name" };
        private readonly TextBox txtLastName = new TextBox { Text = "Last name" };
        private readonly DateTimePicker dpBirth = new DateTimePicker { MaxDate = DateTime.Today, Value = new DateTime(2000, 1, 1) };
        private readonly TextBox txtFacultyNo = new TextBox { Text = "Faculty No (e.g. F113093)" };
        private readonly ComboBox cmbMajor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly Button btnAdd = new Button { Text = "Add" };
        private readonly Button btnRemove = new Button { Text = "Remove Selected" };
        private readonly Button btnUndo = new Button { Text = "Undo" };
        private readonly Button btnSortName = new Button { Text = "Sort: Name" };
        private readonly Button btnSortFN = new Button { Text = "Sort: Faculty No" };
        private readonly TextBox txtSearch = new TextBox { Text = "Search by name/FN" };
        private readonly ListBox lstStudents = new ListBox();
        private readonly StatusStrip status = new StatusStrip();
        private readonly ToolStripStatusLabel lblStatus = new ToolStripStatusLabel();

        // Domain service
        private readonly EnrollmentService _service = new EnrollmentService();

        public MainForm()
        {
            // Form properties
            Text = "Student Enrollment – CSCB579 (Kalin Toshev F113093)";
            Width = 900;
            Height = 600;
            StartPosition = FormStartPosition.CenterScreen;

            // Prepare major choices
            cmbMajor.Items.AddRange(new object[]
            {
                "Computer Science",
                "Software Engineering",
                "Information Systems",
                "Business Informatics",
                "Cybersecurity",
                "Undeclared"
            });
            cmbMajor.SelectedIndex = 0;

            // Layout using a TableLayoutPanel for clarity
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 8,
                Padding = new Padding(12),
                AutoSize = true
            };
            for (int i = 0; i < 4; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            for (int r = 0; r < 8; r++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Row 0–2: input fields
            grid.Controls.Add(new Label { Text = "First Name:" }, 0, 0);
            grid.Controls.Add(txtFirstName, 1, 0);
            grid.Controls.Add(new Label { Text = "Last Name:" }, 2, 0);
            grid.Controls.Add(txtLastName, 3, 0);

            grid.Controls.Add(new Label { Text = "Birth Date:" }, 0, 1);
            grid.Controls.Add(dpBirth, 1, 1);
            grid.Controls.Add(new Label { Text = "Faculty No:" }, 2, 1);
            grid.Controls.Add(txtFacultyNo, 3, 1);

            grid.Controls.Add(new Label { Text = "Major:" }, 0, 2);
            grid.Controls.Add(cmbMajor, 1, 2);
            grid.Controls.Add(new Label { Text = "Search:" }, 2, 2);
            grid.Controls.Add(txtSearch, 3, 2);

            // Row 3: action buttons
            grid.Controls.Add(btnAdd, 0, 3);
            grid.Controls.Add(btnRemove, 1, 3);
            grid.Controls.Add(btnUndo, 2, 3);
            grid.Controls.Add(btnSortName, 3, 3);
            grid.Controls.Add(btnSortFN, 3, 4);

            // Row 5–7: list box spans 4 columns
            grid.Controls.Add(new Label { Text = "Students:" }, 0, 5);
            grid.SetColumnSpan(lstStudents, 4);
            grid.Controls.Add(lstStudents, 0, 6);
            lstStudents.Height = 300;
            lstStudents.Dock = DockStyle.Fill;
            lstStudents.HorizontalScrollbar = true;
            lstStudents.Font = new Font(FontFamily.GenericMonospace, 10f);

            // Status bar
            status.Items.Add(lblStatus);
            Controls.Add(grid);
            Controls.Add(status);

            // Event subscriptions (WinForms events + domain events via delegate)
            Load += OnLoad;
            btnAdd.Click += OnAddClicked;
            btnRemove.Click += OnRemoveClicked;
            btnUndo.Click += (s, e) => { if (_service.Undo()) RefreshList(); };
            btnSortName.Click += (s, e) => Bind(_service.SortedByName());
            btnSortFN.Click += (s, e) => Bind(_service.SortedByFaculty());
            txtSearch.TextChanged += (s, e) => Bind(_service.Search(txtSearch.Text));
            _service.EnrollmentChanged += OnEnrollmentChanged; // custom delegate event
        }

        /// <summary>Initial data seeding for easier testing.</summary>
        private void OnLoad(object sender, EventArgs e)
        {
            // Seed a few demo students
            _service.Add(new Student(Guid.NewGuid().ToString(), "Ivan", "Petrov", new DateTime(2002, 5, 12), "F100001", "Computer Science"));
            _service.Add(new Student(Guid.NewGuid().ToString(), "Maria", "Georgieva", new DateTime(2001, 11, 3), "F100002", "Software Engineering"));
            _service.Add(new Student(Guid.NewGuid().ToString(), "Georgi", "Ivanov", new DateTime(2003, 2, 28), "F100003", "Cybersecurity"));
            RefreshList();
        }

        /// <summary>Handles click on Add: validates, constructs Student, calls service.</summary>
        private void OnAddClicked(object sender, EventArgs e)
        {
            try
            {
                var s = new Student(
                id: Guid.NewGuid().ToString(),
                firstName: txtFirstName.Text,
                lastName: txtLastName.Text,
                birthDate: dpBirth.Value.Date,
                facultyNumber: txtFacultyNo.Text,
                major: cmbMajor.SelectedItem?.ToString() ?? "Undeclared"
                );
                _service.Add(s);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Validation error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // <summary>Handles Remove Selected using selected item in the list.</summary>
        private void OnRemoveClicked(object sender, EventArgs e)
        {
            if (lstStudents.SelectedItem is Student s)
            {
                _service.RemoveByFacultyNumber(s.FacultyNumber);
            }
        }

        /// <summary>Responds to domain events using the custom delegate.</summary>
        private void OnEnrollmentChanged(object sender, EnrollmentChangedEventArgs e)
        {
            lblStatus.Text = $"{e.OccurredAt:HH:mm:ss} – {e.Message}";
            RefreshList();
        }

        /// <summary>Rebinds the ListBox to the current full list.</summary>
        private void RefreshList() => Bind(_service.All());

        /// <summary>Binds a given list to the ListBox and ensures nice display.</summary>
        private void Bind(System.Collections.Generic.IReadOnlyList<Student> items)
        {
            lstStudents.BeginUpdate();
            try
            {
                lstStudents.DataSource = null;
                lstStudents.DataSource = items.ToList();
            }
            finally { lstStudents.EndUpdate(); }
        }

        /// <summary>Utility: clears input fields after successful add.</summary>
        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtFacultyNo.Clear();
            txtFirstName.Focus();
        }
    }
}
