using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaClinic.WebAPI.Models
{
    // ===== CLIENT =====
    public class Client : INotifyPropertyChanged
    {
        private int _id;
        private string? _firstName;
        private string? _lastName;
        private string? _email;
        private string? _phone;
        private string? _address;
        private ClientStatus _status;
        private DateTime _joinDate;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string? FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        public string? LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string? Phone
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        public string? Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        private string? _city;
        private string? _state;
        private string? _zipCode;

        public string? City
        {
            get => _city;
            set
            {
                if (_city != value)
                {
                    _city = value;
                    OnPropertyChanged(nameof(City));
                }
            }
        }

        public string? State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public string? ZipCode
        {
            get => _zipCode;
            set
            {
                if (_zipCode != value)
                {
                    _zipCode = value;
                    OnPropertyChanged(nameof(ZipCode));
                }
            }
        }

        public ClientStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public DateTime JoinDate
        {
            get => _joinDate;
            set
            {
                if (_joinDate != value)
                {
                    _joinDate = value;
                    OnPropertyChanged(nameof(JoinDate));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (_updatedAt != value)
                {
                    _updatedAt = value;
                    OnPropertyChanged(nameof(UpdatedAt));
                }
            }
        }

        // Navigation property
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ===== ANIMAL =====
    public class Animal : INotifyPropertyChanged
    {
        private int _id;
        private int _clientId;
        private string? _name;
        private string? _species;
        private string? _breed;
        private DateTime _dateOfBirth;
        private string? _gender;
        private string? _color;
        private decimal _weight;
        private string? _chipId;
        private string? _vaccinationRecords;
        private string? _allergies;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private Client? _client;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int ClientId
        {
            get => _clientId;
            set
            {
                if (_clientId != value)
                {
                    _clientId = value;
                    OnPropertyChanged(nameof(ClientId));
                }
            }
        }

        public string? Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string? Species
        {
            get => _species;
            set
            {
                if (_species != value)
                {
                    _species = value;
                    OnPropertyChanged(nameof(Species));
                }
            }
        }

        public string? Breed
        {
            get => _breed;
            set
            {
                if (_breed != value)
                {
                    _breed = value;
                    OnPropertyChanged(nameof(Breed));
                }
            }
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (_dateOfBirth != value)
                {
                    _dateOfBirth = value;
                    OnPropertyChanged(nameof(DateOfBirth));
                }
            }
        }

        public string? Gender
        {
            get => _gender;
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        public string? Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

        public decimal Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        public string? ChipId
        {
            get => _chipId;
            set
            {
                if (_chipId != value)
                {
                    _chipId = value;
                    OnPropertyChanged(nameof(ChipId));
                }
            }
        }

        public string? VaccinationRecords
        {
            get => _vaccinationRecords;
            set
            {
                if (_vaccinationRecords != value)
                {
                    _vaccinationRecords = value;
                    OnPropertyChanged(nameof(VaccinationRecords));
                }
            }
        }

        public string? Allergies
        {
            get => _allergies;
            set
            {
                if (_allergies != value)
                {
                    _allergies = value;
                    OnPropertyChanged(nameof(Allergies));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (_updatedAt != value)
                {
                    _updatedAt = value;
                    OnPropertyChanged(nameof(UpdatedAt));
                }
            }
        }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client
        {
            get => _client;
            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged(nameof(Client));
                }
            }
        }

        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ===== APPOINTMENT =====
    public class Appointment : INotifyPropertyChanged
    {
        private int _id;
        private int _animalId;
        private int _veterinarianId;
        private string? _petName;
        private string? _ownerName;
        private string? _appointmentType;
        private DateTime _appointmentDateTime;
        private string? _veterinarianName;
        private AppointmentStatus _status;
        private string? _description;
        private string? _notes;
        private DateTime _updatedAt;
        private DateTime _createdAt;
        private Animal? _animal;

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int AnimalId
        {
            get => _animalId;
            set
            {
                if (_animalId != value)
                {
                    _animalId = value;
                    OnPropertyChanged(nameof(AnimalId));
                }
            }
        }

        public int VeterinarianId
        {
            get => _veterinarianId;
            set
            {
                if (_veterinarianId != value)
                {
                    _veterinarianId = value;
                    OnPropertyChanged(nameof(VeterinarianId));
                }
            }
        }

        public string? PetName
        {
            get => _petName;
            set
            {
                if (_petName != value)
                {
                    _petName = value;
                    OnPropertyChanged(nameof(PetName));
                }
            }
        }

        public string? OwnerName
        {
            get => _ownerName;
            set
            {
                if (_ownerName != value)
                {
                    _ownerName = value;
                    OnPropertyChanged(nameof(OwnerName));
                }
            }
        }

        public string? AppointmentType
        {
            get => _appointmentType;
            set
            {
                if (_appointmentType != value)
                {
                    _appointmentType = value;
                    OnPropertyChanged(nameof(AppointmentType));
                }
            }
        }

        public DateTime AppointmentDateTime
        {
            get => _appointmentDateTime;
            set
            {
                if (_appointmentDateTime != value)
                {
                    _appointmentDateTime = value;
                    OnPropertyChanged(nameof(AppointmentDateTime));
                }
            }
        }

        public string? VeterinarianName
        {
            get => _veterinarianName;
            set
            {
                if (_veterinarianName != value)
                {
                    _veterinarianName = value;
                    OnPropertyChanged(nameof(VeterinarianName));
                }
            }
        }

        public AppointmentStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string? Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (_updatedAt != value)
                {
                    _updatedAt = value;
                    OnPropertyChanged(nameof(UpdatedAt));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        // Navigation property
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal
        {
            get => _animal;
            set
            {
                if (_animal != value)
                {
                    _animal = value;
                    OnPropertyChanged(nameof(Animal));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ===== MEDICAL RECORD =====
    public class MedicalRecord : INotifyPropertyChanged
    {
        private int _id;
        private int _animalId;
        private int? _appointmentId;
        private int? _veterinarianId;
        private string? _diagnosis;
        private string? _treatment;
        private string? _medication;
        private string? _notes;
        private DateTime? _nextCheckupDate;
        private DateTime _recordDate;
        private DateTime _createdAt;
        private DateTime _updatedAt;
        private Animal? _animal;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int AnimalId
        {
            get => _animalId;
            set
            {
                if (_animalId != value)
                {
                    _animalId = value;
                    OnPropertyChanged(nameof(AnimalId));
                }
            }
        }

        public int? AppointmentId
        {
            get => _appointmentId;
            set
            {
                if (_appointmentId != value)
                {
                    _appointmentId = value;
                    OnPropertyChanged(nameof(AppointmentId));
                }
            }
        }

        public int? VeterinarianId
        {
            get => _veterinarianId;
            set
            {
                if (_veterinarianId != value)
                {
                    _veterinarianId = value;
                    OnPropertyChanged(nameof(VeterinarianId));
                }
            }
        }

        public string? Diagnosis
        {
            get => _diagnosis;
            set
            {
                if (_diagnosis != value)
                {
                    _diagnosis = value;
                    OnPropertyChanged(nameof(Diagnosis));
                }
            }
        }

        public string? Treatment
        {
            get => _treatment;
            set
            {
                if (_treatment != value)
                {
                    _treatment = value;
                    OnPropertyChanged(nameof(Treatment));
                }
            }
        }

        public string? Medication
        {
            get => _medication;
            set
            {
                if (_medication != value)
                {
                    _medication = value;
                    OnPropertyChanged(nameof(Medication));
                }
            }
        }

        public string? Notes
        {
            get => _notes;
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    OnPropertyChanged(nameof(Notes));
                }
            }
        }

        public DateTime? NextCheckupDate
        {
            get => _nextCheckupDate;
            set
            {
                if (_nextCheckupDate != value)
                {
                    _nextCheckupDate = value;
                    OnPropertyChanged(nameof(NextCheckupDate));
                }
            }
        }

        public DateTime RecordDate
        {
            get => _recordDate;
            set
            {
                if (_recordDate != value)
                {
                    _recordDate = value;
                    OnPropertyChanged(nameof(RecordDate));
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (_updatedAt != value)
                {
                    _updatedAt = value;
                    OnPropertyChanged(nameof(UpdatedAt));
                }
            }
        }

        // Navigation property
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal
        {
            get => _animal;
            set
            {
                if (_animal != value)
                {
                    _animal = value;
                    OnPropertyChanged(nameof(Animal));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // ===== INVOICE =====
    public class Invoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int AnimalId { get; set; }
        public string? InvoiceNumber { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal { get; set; }
    }

    // ===== INVOICE ITEM =====
    public class InvoiceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        // Navigation property
        [ForeignKey("InvoiceId")]
        public virtual Invoice? Invoice { get; set; }
    }

    // ===== VETERINARIAN =====
    public class Veterinarian
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LicenseNumber { get; set; }
        public string? Specialization { get; set; }
        public bool IsActive { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ===== CLINIC SETTINGS =====
    public class ClinicSettings
    {
        [Key]
        public int Id { get; set; }
        public string? ClinicName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Logo { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool SmsNotificationsEnabled { get; set; }
        public int AppointmentReminderHours { get; set; } = 24;
        public string? BusinessHours { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    // ===== USER =====
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // ===== ENUMS =====
    public enum UserRole
    {
        Admin,
        Veterinarian,
        Receptionist
    }

    public enum ClientStatus
    {
        Active,
        Inactive,
        Pending
    }

    public enum AppointmentStatus
    {
        Confirmed,
        Waiting,
        Completed,
        Cancelled,
        NoShow
    }

    public enum InvoiceStatus
    {
        Pending,
        Paid,
        Overdue,
        Cancelled
    }
}