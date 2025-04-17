using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MyBackendApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly Data.AppDbContext dbContext;

        public MedicalRecordController(Data.AppDbContext dbContext)
        {
            // Constructor logic here
            this.dbContext = dbContext;
        }
        [HttpGet]
        [Route("recent")]
        public IActionResult GetRecentMedicalRecords()
        {
    var records = dbContext.MedicalRecords
        .Include(record => record.Patient) // Include Patient details
        .OrderByDescending(record => record.DateCreated) // Sort by DateCreated in descending order
        .Select(record => new
        {
            record.RecordId,
            record.DateCreated,
            record.LastUpdated,
            record.Diagnosis,
            record.Treatment,
            record.Prescription,
            record.IsEditable,
            Patient = new
            {
                record.Patient.Id,
                record.Patient.Username,
                record.Patient.Email,
                record.Patient.Gender
            }
        })
           .Take(10) // Take only the last 10 records
        .ToList();

    return Ok(records);
             
  
        }

        [HttpGet]
public IActionResult GetMedicalRecords()
{
    var records = dbContext.MedicalRecords
        .Include(record => record.Patient) // Include Patient details
        .OrderByDescending(record => record.DateCreated) // Sort by DateCreated in descending order
        .Select(record => new
        {
            record.RecordId,
            record.DateCreated,
            record.LastUpdated,
            record.Diagnosis,
            record.Treatment,
            record.Prescription,
            record.IsEditable,
            Patient = new
            {
                record.Patient.Id,
                record.Patient.Username,
                record.Patient.Email,
                record.Patient.Gender
            }
        })
        .ToList();

    return Ok(records);
}
[HttpGet]
[Route("{id:int}")]
public IActionResult GetMedicalRecordById(int id)
{
    var medicalRecord = dbContext.MedicalRecords
        .Include(record => record.Patient) // Include Patient details
        .Where(record => record.RecordId == id) // Filter by RecordId
        .Select(record => new
        {
            record.RecordId,
            record.DateCreated,
            record.LastUpdated,
            record.Diagnosis,
            record.Treatment,
            record.Prescription,
            record.IsEditable,
            Patient = new
            {
                record.Patient.Id,
                record.Patient.Username,
                record.Patient.Email,
                record.Patient.Gender
            }
        })
        .FirstOrDefault();

    if (medicalRecord == null)
    {
        return NotFound(new { message = "Medical record not found" });
    }

    return Ok(medicalRecord);
}
[HttpGet]
[Route("by-patient/{patientId:int}")]
public IActionResult GetMedicalRecordsByPatientId(int patientId)
{
    var records = dbContext.MedicalRecords
        .Where(record => record.PatientId == patientId) // Filter by PatientId
        .Include(record => record.Patient) // Include Patient details
        .OrderByDescending(record => record.DateCreated) // Sort by DateCreated in descending order
        .Select(record => new
        {
            record.RecordId,
            record.DateCreated,
            record.LastUpdated,
            record.Diagnosis,
            record.Treatment,
            record.Prescription,
            record.IsEditable,
            Patient = new
            {
                record.Patient.Id,
                record.Patient.Username,
                record.Patient.Email,
                record.Patient.Gender
            }
        })
        .ToList();

    if (!records.Any())
    {
        return NotFound(new { message = "No medical records found for the given patient ID" });
    }

    return Ok(records);
}

        [HttpPost]
        public IActionResult AddMedicalRecord(AddMedicalRecordDto addMedicalRecordDto)
        {
            dbContext.MedicalRecords.Add(new MedicalRecord
            {
                DateCreated = addMedicalRecordDto.DateCreated,
                LastUpdated = addMedicalRecordDto.LastUpdated,
                // Symptoms = addMedicalRecordDto.Symptoms,
                // MedicalHistory = addMedicalRecordDto.MedicalHistory,
                // Allergies = addMedicalRecordDto.Allergies,
                Diagnosis = addMedicalRecordDto.Diagnosis,
                Treatment = addMedicalRecordDto.Treatment,
                Prescription = addMedicalRecordDto.Prescription,
                IsEditable = addMedicalRecordDto.IsEditable,
                PatientId = addMedicalRecordDto.PatientId,
                DoctorId = addMedicalRecordDto.DoctorId
            });
            dbContext.SaveChanges();
            return Ok(new { message = "Medical record added successfully" });
        }
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateMedicalRecord(int id, UpdateMedicalRecordDto updateMedicalRecordDto)
        {

            var patient = dbContext.Patients.Find(id);


            var medicalRecord = dbContext.MedicalRecords.Find(id);
            if (medicalRecord == null)
            {
                return NotFound(new { message = "Medical record not found" });
            }

            medicalRecord.DateCreated = updateMedicalRecordDto.DateCreated;
            medicalRecord.LastUpdated = updateMedicalRecordDto.LastUpdated;
            // medicalRecord.Symptoms = updateMedicalRecordDto.Symptoms;
            // medicalRecord.MedicalHistory = updateMedicalRecordDto.MedicalHistory;
            // medicalRecord.Allergies = updateMedicalRecordDto.Allergies;
            medicalRecord.Diagnosis = updateMedicalRecordDto.Diagnosis;
            medicalRecord.Treatment = updateMedicalRecordDto.Treatment;
            medicalRecord.Prescription = updateMedicalRecordDto.Prescription;
            medicalRecord.IsEditable = updateMedicalRecordDto.IsEditable;
            medicalRecord.PatientId = updateMedicalRecordDto.PatientId;
            medicalRecord.DoctorId = updateMedicalRecordDto.DoctorId;
            dbContext.SaveChanges();
            return Ok(new { message = "medicalrecord updated successfully", patient });
        }
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteMedicalRecord(int id)
        {
            var medicalRecord = dbContext.MedicalRecords.Find(id);
            if (medicalRecord == null)
            {
                return NotFound(new { message = "medicalrecord not found" });
            }
            dbContext.MedicalRecords.Remove(medicalRecord);
            dbContext.SaveChanges();
            return Ok(new { message = "medicalrecord deleted successfully" });
        }

    }
}