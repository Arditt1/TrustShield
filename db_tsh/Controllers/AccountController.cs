using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using db_tsh.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using Npgsql;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Renci.SshNet;

namespace db_tsh.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<NpgsqlConnection> OpenDatabaseConnectionAsync()
        {
            var dbPort = 9999;         // Default PostgreSQL port
            var dbUser = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[2].Split('=')[1];
            var dbPassword = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[3].Split('=')[1];
            var dbName = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[4].Split('=')[1];

            var connectionString = $"Host=localhost;Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";

            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }

        // Other actions...
        [HttpGet]
        public ActionResult RegisterOrLogin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(Customer cust)
        {

            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {

                    // Check if email is already registered
                    string checkEmailQuery = "SELECT COUNT(*) FROM project.customer WHERE email = @Email";
                    using (NpgsqlCommand checkEmailCmd = new NpgsqlCommand(checkEmailQuery, con))
                    {
                        checkEmailCmd.Parameters.AddWithValue("@Email", cust.Email);
                        object existingEmailCount = checkEmailCmd.ExecuteScalar();

                        if (existingEmailCount != DBNull.Value && Convert.ToInt32(existingEmailCount) > 0)
                        {
                            ModelState.AddModelError(string.Empty, "Email is already registered.");
                            return View(cust);
                        }
                    }

                    // Insert new user into the database
                    string insertUserQuery = "INSERT INTO project.customer (name, email, password, type) VALUES (@Name, @Email, @Password, true)";
                    using (NpgsqlCommand insertUserCmd = new NpgsqlCommand(insertUserQuery, con))
                    {
                        insertUserCmd.Parameters.AddWithValue("@Name", cust.Name);
                        insertUserCmd.Parameters.AddWithValue("@Email", cust.Email);
                        insertUserCmd.Parameters.AddWithValue("@Password", cust.Password);
                        int rowsAffected = (int)insertUserCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ViewData["message"] = "User registered successfully.";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Failed to register user.");
                            return View(cust);
                        }
                    }
                }
            }
            // If model state is not valid, return the registration view with validation errors
            return View("RegisterOrLogin", cust);
        }


        [HttpPost]
        public async Task<ActionResult> RegisterOrLogin(Customer cust, string email, string password)
        {
            // Early exit if email or password is null or empty
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Display an error message if either the username or password is missing.
                ViewData["message"] = "Please enter both username and password.";
                cust.success = 0;
                return View();
            }

            try
            {
                // Use the helper method to get a connection
                using (NpgsqlConnection sqlcon = await OpenDatabaseConnectionAsync())
                {
                    string query = "SELECT email, password FROM project.customer WHERE email = @Email AND password = @Password";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, sqlcon))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (NpgsqlDataReader sdr = await cmd.ExecuteReaderAsync())
                        {
                            if (sdr.Read()) // If user found
                            {
                                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, email)
                            // You can add more claims as needed.
                        };

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                var authProperties = new AuthenticationProperties
                                {
                                    // You can customize authentication properties if needed.
                                };

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                                ViewData["message"] = "User logged in successfully";
                                cust.success = 1;

                                // No need to explicitly close the connection
                                return RedirectToAction("Index", "Home");
                            }
                            else // User not found
                            {
                                ViewBag.Errorpass = "Email and password are incorrect!";
                                cust.success = 0;
                                return View();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle error
                ViewData["message"] = "An error occurred while logging in: " + ex.Message;
                cust.success = 0;
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View("RegisterOrLogin");
        }
    }
}
