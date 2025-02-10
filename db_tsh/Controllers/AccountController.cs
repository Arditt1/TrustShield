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
            // Get SSH and DB connection info from appsettings.json
            var sshHost = _configuration["SSH:Host"];
            var sshUser = _configuration["SSH:Username"];
            var sshPassword = _configuration["SSH:Password"];
            var sshPort = int.Parse(_configuration["SSH:Port"]);

            var dbHost = "localhost";  // As SSH tunnel will forward to localhost
            var dbPort = 5432;         // Default PostgreSQL port
            var dbUser = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[2].Split('=')[1];
            var dbPassword = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[3].Split('=')[1];
            var dbName = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[4].Split('=')[1];

            // Set up the SSH tunnel using SSH.NET
            using (var sshClient = new SshClient(sshHost, sshPort, sshUser, sshPassword))
            {
                sshClient.Connect();
                var portForwarded = new ForwardedPortLocal("localhost", (uint)dbPort, dbHost, (uint)dbPort);
                sshClient.AddForwardedPort(portForwarded);
                portForwarded.Start();

                Console.WriteLine("SSH Tunnel established.");

                // Create the PostgreSQL connection string
                var connectionString = $"Host=localhost;Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";

                // Open and return the PostgreSQL connection
                var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                return conn;
            }
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
                        int existingEmailCount = (int)checkEmailCmd.ExecuteScalar();

                        if (existingEmailCount > 0)
                        {
                            ModelState.AddModelError(string.Empty, "Email is already registered.");
                            return View(cust);
                        }
                    }

                    // Insert new user into the database
                    string insertUserQuery = "INSERT INTO project.customer (name, email, password, type) VALUES (@Name, @Email, @Password, 1)";
                    using (NpgsqlCommand insertUserCmd = new NpgsqlCommand(insertUserQuery, con))
                    {
                        insertUserCmd.Parameters.AddWithValue("@Name", cust.Name);
                        insertUserCmd.Parameters.AddWithValue("@Email", cust.Email);
                        insertUserCmd.Parameters.AddWithValue("@Password", cust.Password);
                        int rowsAffected = insertUserCmd.ExecuteNonQuery();

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
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (NpgsqlConnection sqlcon = await OpenDatabaseConnectionAsync())
            {
                if (email == null || password == null)
                {
                    // Display an error message if either the username or password is missing.
                    ViewData["message"] = "Please enter both username and password.";
                    cust.success = 0;
                    sqlcon.Close();
                    return View();
                }

                string query = "SELECT email, password FROM project.customer WHERE email = @Email AND password = @Password";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (NpgsqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.Read())
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
                            sqlcon.Close();
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ViewBag.Errorpass = "Email and password are wrong!";
                            cust.success = 0;
                            sqlcon.Close();
                            return View();
                        }
                    }
                }
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
