using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VitaClinic.WebAPI.Models
{
    // ===== CLIENT =====
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public ClientStatus Status { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }

    // ===== ANIMAL =====
    public class Animal
    {
        [Key]
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string? Name { get; set; }
        public string? Species { get; set; }
        public string? Breed { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Color { get; set; }
        public decimal Weight { get; set; }
        public string? ChipId { get; set; }
        public string? VaccinationRecords { get; set; }
        public string? Allergies { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

    // ===== APPOINTMENT =====
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int VeterinarianId { get; set; }
        public string? PetName { get; set; }
        public string? OwnerName { get; set; }
        public string? AppointmentType { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string? VeterinarianName { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal { get; set; }
    }

    // ===== MEDICAL RECORD =====
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public int? AppointmentId { get; set; }
        public int? VeterinarianId { get; set; }
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? Medication { get; set; }
        public string? Notes { get; set; }
        public DateTime? NextCheckupDate { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("AnimalId")]
        public virtual Animal? Animal { get; set; }
    }

    // ===== INVOICE =====
    public class Invoice
    {
        [Key]
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

    // ===== ENUMS =====
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