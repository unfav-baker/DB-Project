using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class ReportItemData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _reportId;
        private string _reportType = string.Empty;
        private DateTime _dateGenerated;
        private string _status = string.Empty;
        private int? _employeeId;
        private string _employeeName = string.Empty;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public int ReportId
        {
            get => _reportId;
            set { if (_reportId != value) { _reportId = value; OnPropertyChanged(); } }
        }

        public string ReportType
        {
            get => _reportType;
            set { if (_reportType != value) { _reportType = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public DateTime DateGenerated
        {
            get => _dateGenerated;
            set { if (_dateGenerated != value) { _dateGenerated = value; OnPropertyChanged(); } }
        }

        public string Status // Maps to 'Statuss' from DB if that's still the column name
        {
            get => _status;
            set { if (_status != value) { _status = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public int? EmployeeId
        {
            get => _employeeId;
            set { if (_employeeId != value) { _employeeId = value; OnPropertyChanged(); } }
        }

        public string EmployeeName
        {
            get => _employeeName;
            set { if (_employeeName != value) { _employeeName = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { if (_updatedAt != value) { _updatedAt = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
