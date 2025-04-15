using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBackendApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace MyBackendApi.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly Data.AppDbContext dbContext;

        public PatientController(Data.AppDbContext dbContext)
        {
            // Constructor logic here
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetPatient()
        {
            var records = dbContext.Patients.ToList();
            return Ok(records);
        }
        [HttpGet]
        [Route("search")]
        public IActionResult GetPatientByName([FromQuery] string name)
        {
            var patients = dbContext.Patients
                .Where(p => p.Username.Contains(name)) // Search for patients whose names contain the input
                .ToList();

            if (patients == null || !patients.Any())
            {
                return NotFound(new { message = "No patients found with the given name" });
            }

            return Ok(patients);
        }
        [HttpGet]
        [Route("{id:int}")]

        public IActionResult GetPatientById(int id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found" });
            }
            return Ok(patient);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddPatient(AddPatientDto addPatientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the username already exists
            var existingPatient = dbContext.Patients.FirstOrDefault(p => p.Username == addPatientDto.Username);
            if (existingPatient != null)
            {
                return BadRequest(new { message = "Username is already taken. Please choose a different username." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(addPatientDto.Password);
            dbContext.Patients.Add(new Patient
            {
                Username = addPatientDto.Username,
                PasswordHash = hashedPassword,
                Role = addPatientDto.Role,
                DateOfBirth = addPatientDto.DateOfBirth,
                Gender = addPatientDto.Gender,
                Email = addPatientDto.Email,
                PhoneNumber = addPatientDto.PhoneNumber,
                Address = addPatientDto.Address
            });
            dbContext.SaveChanges();
            return Ok(new { message = "Patient added successfully" });
        }
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult Updatepatient(int id, UpdatePatientDto updatepatientDto)
        {

            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound(new { message = "patient not found" });
            }
            patient.Username = updatepatientDto.Username;
            // Hash the password if it is being updated
            if (!string.IsNullOrEmpty(updatepatientDto.Password))
            {
                patient.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatepatientDto.Password);
            }
            patient.Role = updatepatientDto.Role;
            patient.DateOfBirth = updatepatientDto.DateOfBirth;

            // patient.Age = updatepatientDto.Age;
            patient.Gender = updatepatientDto.Gender;
            patient.Email = updatepatientDto.Email;
            patient.PhoneNumber = updatepatientDto.PhoneNumber;
            patient.Address = updatepatientDto.Address;
            dbContext.SaveChanges();
            return Ok(new { message = "patient updated successfully", patient });
        }
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeletePatient(int id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound(new { message = "patient not found" });
            }
            dbContext.Patients.Remove(patient);
            dbContext.SaveChanges();
            return Ok(new { message = "patient deleted successfully" });
        }

    }
}