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
    public class DoctorController : ControllerBase
    {
        private readonly Data.AppDbContext dbContext;

        public DoctorController(Data.AppDbContext dbContext)
        {
            // Constructor logic here
            this.dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult GetDoctor()
        {
            var records = dbContext.Doctors.ToList();
            return Ok(records);
        }
        [HttpGet]
        [Route("search")]
        public IActionResult SearchDoctorByName([FromQuery] string name)
        {
            var doctors = dbContext.Doctors
                .Where(d => d.Username.Contains(name)) // Search for doctors whose names contain the input
                .ToList();

            if (doctors == null || !doctors.Any())
            {
                return NotFound(new { message = "No doctors found with the given name" });
            }

            return Ok(doctors);
        }
        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetDoctorById(int id)
        {
            var doctor = dbContext.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }
            return Ok(doctor);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddDoctor(AddDoctorDto addDoctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the username already exists
            var existingDoctor = dbContext.Doctors.FirstOrDefault(d => d.Username == addDoctorDto.Username);
            if (existingDoctor != null)
            {
                return BadRequest(new { message = "Username is already taken. Please choose a different username." });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(addDoctorDto.Password);
            dbContext.Doctors.Add(new Doctor
            {
                Username = addDoctorDto.Username,
                PasswordHash = hashedPassword,
                Role = addDoctorDto.Role,
                DateCreated = addDoctorDto.DateCreated,
                LastUpdated = addDoctorDto.LastUpdated,
                HospitalName = addDoctorDto.HospitalName,
                Specailty = addDoctorDto.Specailty,
                Age = addDoctorDto.Age,
                Gender = addDoctorDto.Gender,
                Email = addDoctorDto.Email,
                PhoneNumber = addDoctorDto.PhoneNumber,
                Address = addDoctorDto.Address
            });
            dbContext.SaveChanges();
            return Ok(new { message = "Doctor added successfully" });
        }
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateDoctor(int id, UpdateDoctorDto updateDoctorDto)
        {

            var doctor = dbContext.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }
            doctor.Username = updateDoctorDto.Username;
            doctor.Password = updateDoctorDto.Password;
            if (!string.IsNullOrEmpty(updateDoctorDto.Password))
            {
                doctor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDoctorDto.Password);
            }
            doctor.Role = updateDoctorDto.Role;
            doctor.DateCreated = updateDoctorDto.DateCreated;
            doctor.LastUpdated = updateDoctorDto.LastUpdated;
            doctor.HospitalName = updateDoctorDto.HospitalName;
            doctor.Specailty = updateDoctorDto.Specailty;
            doctor.Age = updateDoctorDto.Age;
            doctor.Gender = updateDoctorDto.Gender;
            doctor.Email = updateDoctorDto.Email;
            doctor.PhoneNumber = updateDoctorDto.PhoneNumber;
            doctor.Address = updateDoctorDto.Address;
            dbContext.SaveChanges();
            return Ok(new { message = "Doctor updated successfully", doctor });
        }
        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult DeleteDoctor(int id)
        {
            var doctor = dbContext.Doctors.Find(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }
            dbContext.Doctors.Remove(doctor);
            dbContext.SaveChanges();
            return Ok(new { message = "Doctor deleted successfully" });
        }



    }
}