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

namespace db_tsh.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public IActionResult Register(Customer cust)
        {

            if (ModelState.IsValid)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Check if email is already registered
                    string checkEmailQuery = "SELECT COUNT(*) FROM project.customer WHERE email = @Email";
                    using (SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, con))
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
                    using (SqlCommand insertUserCmd = new SqlCommand(insertUserQuery, con))
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
            using (SqlConnection sqlcon = new SqlConnection(connectionString))
            {
                sqlcon.Open();
                if (email == null || password == null)
                {
                    // Display an error message if either the username or password is missing.
                    ViewData["message"] = "Please enter both username and password.";
                    cust.success = 0;
                    sqlcon.Close();
                    return View();
                }

                string query = "SELECT email, password FROM project.customer WHERE email = @Email AND password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, sqlcon))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader sdr = cmd.ExecuteReader())
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
