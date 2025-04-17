using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyBackendApi.Models
{
    public class MedicalRecord
    {
        [Key]  // Correct attribute for defining the primary key
        public int RecordId { get; set; }
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime? LastUpdated { get; set; }
        [Required]
        public string Diagnosis { get; set; }
        [Required]
        public string Treatment { get; set; }
        [Required]
        public string Prescription { get; set; }
        [Required]
        public bool IsEditable { get; set; } = true;

        // Foreign key for Patient
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        public Patient Patient { get; set; } // Navigation property for Patient

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        public Doctor Doctor { get; set; } // Navigation property for Doctor

    }
}
