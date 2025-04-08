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
        public IActionResult Addpatient(AddPatientDto addpatientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(addpatientDto.Password);
            dbContext.Patients.Add(new Patient
            {


                Username = addpatientDto.Username,
                PasswordHash = hashedPassword,
                Role = addpatientDto.Role,
                DateOfBirth = addpatientDto.DateOfBirth,

                // Age = addpatientDto.Age,
                Gender = addpatientDto.Gender,
                Email = addpatientDto.Email,
                PhoneNumber = addpatientDto.PhoneNumber,
                Address = addpatientDto.Address,



            });
            dbContext.SaveChanges();
            return Ok(new { message = "patient added successfully" });
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