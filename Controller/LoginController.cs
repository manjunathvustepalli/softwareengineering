using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using MyBackendApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MyBackendApi.Controller
{
    [ApiController]
    [Route("login")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly Data.AppDbContext dbContext;

        public LoginController(Data.AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

[HttpPost]
[Route("login")]
public IActionResult Login([FromBody] LoginDto loginDto)
{
    // Check in Doctors table
    var doctor = dbContext.Doctors.FirstOrDefault(u => u.Username == loginDto.Username);
    if (doctor != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, doctor.PasswordHash))
    {
        // Generate JWT token for Doctor
        return GenerateJwtToken(doctor.Username, doctor.Role);
    }

    // Check in Patients table
    var patient = dbContext.Patients.FirstOrDefault(u => u.Username == loginDto.Username);
    if (patient != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, patient.PasswordHash))
    {
        // Generate JWT token for Patient
        return GenerateJwtToken(patient.Username, "Patient"); // Assuming "Patient" as the role
    }

    // If no match is found
    return Unauthorized(new { message = "Invalid username or password" });
}

// Helper method to generate JWT token
private IActionResult GenerateJwtToken(string username, string role)
{
    var tokenHandler = new JwtSecurityTokenHandler();
   var key = Encoding.UTF8.GetBytes("Zx9$NkjUj21!m%pQr#v$4MnbTwE7LgQd");// Replace with the same key as in Program.cs
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        }),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = "myapp",
        Audience = "localhost",
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return Ok(new { token = tokenHandler.WriteToken(token) });
}
private void SendEmail(string toEmail, string subject, string body)
{
    try
    {
        using (var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)) // Replace with your SMTP server and port
        {
            smtpClient.Credentials = new System.Net.NetworkCredential("your-email@gmail.com", "your-email-password"); // Replace with your email and password
            smtpClient.EnableSsl = true;

            var mailMessage = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress("your-email@gmail.com", "MyBackendApi"), // Replace with your email and display name
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending email: {ex.Message}");
    }
}
[HttpPost]
[Route("forgot-password")]
[AllowAnonymous] // Allow access without authentication
public IActionResult ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
{
    // Check if the email exists in the Doctors table
    var doctor = dbContext.Doctors.FirstOrDefault(d => d.Email == forgotPasswordDto.Email);
    if (doctor == null)
    {
        // Check if the email exists in the Patients table
        var patient = dbContext.Patients.FirstOrDefault(p => p.Email == forgotPasswordDto.Email);
        if (patient == null)
        {
            return NotFound(new { message = "Email not found" });
        }

        // Generate reset token for Patient
        var resetToken = GenerateResetToken(patient.Email, "Patient");
        // Simulate sending email (replace with actual email-sending logic)
        Console.WriteLine($"Reset token for Patient: {resetToken}");
        return Ok(new { message = "Password reset token sent to your email" });
    }

    // Generate reset token for Doctor
    var doctorResetToken = GenerateResetToken(doctor.Email, doctor.Role);
    // Simulate sending email (replace with actual email-sending logic)
    Console.WriteLine($"Reset token for Doctor: {doctorResetToken}");
    return Ok(new { message = "Password reset token sent to your email" });
}

// Helper method to generate reset token
private string GenerateResetToken(string email, string role)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes("Zx9$NkjUj21!m%pQr#v$4MnbTwE7LgQd"); // Replace with the same key as in Program.cs
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        }),
        Expires = DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
    }
}