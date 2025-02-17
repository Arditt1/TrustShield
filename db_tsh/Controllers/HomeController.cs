using db_tsh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using OfficeOpenXml;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace db_tsh.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private async Task<NpgsqlConnection> OpenDatabaseConnectionAsync()
        {
            var dbPort = 9999;
            var dbUser = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[2].Split('=')[1];
            var dbPassword = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[3].Split('=')[1];
            var dbName = _configuration["ConnectionStrings:DefaultConnection"].Split(';')[4].Split('=')[1];

            var connectionString = $"Host=localhost;Port={dbPort};Username={dbUser};Password={dbPassword};Database={dbName}";

            var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();
            return conn;
        }


        public IActionResult GetLoggedInUserInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                // User is authenticated, you can retrieve information about the user
                string userName = User.Identity.Name;
                // You can also access other user properties like roles, claims, etc.

                // Example: Display the logged-in user's name
                ViewData["UserName"] = userName;

                return View();
            }
            else
            {
                // User is not authenticated, handle accordingly
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> IndexAsync(int? page)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            //List<Customer> cl = new List<Customer>();
            string userwhere = "";
            if (User.Identity.IsAuthenticated)
            { // Retrieve the user ID
                string userEmail = User.Identity.Name;
                userwhere = string.Format("where c.email='{0}'", userEmail);
                if (userEmail == "a@trustshield.com")
                    userwhere = "";
            }
            
            if (User.Identity.Name == "a@trustshield.com")
                ViewBag.isadmin = "Yes";

            using (var con = await OpenDatabaseConnectionAsync())
            {
                //con.Open();
                string query = string.Format(@"
SELECT p.p_id, 
       CASE 
           WHEN v.pol_id IS NOT NULL THEN 'Auto Policy'
           WHEN t.pol_id IS NOT NULL THEN 'Travel Health'
           ELSE 'Property Policy'
       END AS PolicyType,
       c.name AS CustomerName,
       p.sdate AS StartDate,
       p.edate AS EndDate,
       p.package AS PackageCode,
       pkg.title AS PackageTitle,
       pkg.total AS PackageTotal
FROM project.policy p
LEFT JOIN project.Auto_pol v ON p.p_id = v.pol_id
LEFT JOIN project.Travel_pol t ON p.p_id = t.pol_id
LEFT JOIN project.property_pol pp ON p.p_id = pp.pr_id 
LEFT JOIN project.pol_dog pd ON p.p_id = pd.policy
LEFT JOIN project.customer c ON pd.c_id = c.c_id --OR t.o_embg = c.c_id
LEFT JOIN project.package pkg ON p.package = pkg.code
{0} ORDER BY p.p_id DESC;
", userwhere);

                NpgsqlCommand com = new NpgsqlCommand(query, con);
                NpgsqlDataAdapter sqlda = new NpgsqlDataAdapter(com);
                DataSet ds = new DataSet();
                sqlda.Fill(ds);

                List<Policy> policies = new List<Policy>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        policies.Add(new Policy
                        {
                            P_id = Convert.ToInt32(dr["p_id"]),
                            PolicyType = Convert.ToString(dr["PolicyType"]),
                            CustomerName = Convert.ToString(dr["CustomerName"]),
                            Sdate = Convert.ToDateTime(dr["StartDate"]),
                            Edate = Convert.ToDateTime(dr["EndDate"]),
                            Package = Convert.ToInt32(dr["PackageCode"]),
                            PackageTitle = Convert.ToString(dr["PackageTitle"]),
                            PackageTotal = Convert.ToDecimal(dr["PackageTotal"])
                        });
                    }
                }
                else
                {
                    ViewBag.Error = "Nuk ka te dhena ne baze!";
                }

                int pageNumber = page ?? 1; // Default page number is 1
                int pageSize = 5; // Number of items to display per page

                // Apply paging
                List<Policy> pagedPolicies = policies.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.TotalPages = (int)Math.Ceiling(policies.Count / (double)pageSize);
                ViewBag.CurrentPage = pageNumber;

                ModelState.Clear();

                return View(pagedPolicies);
                //return View();
            }


        }

        public async Task<IActionResult> PrivacyAsync()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            NpgsqlConnection sqlcon = await OpenDatabaseConnectionAsync();
            //sqlcon.Open();
            string query = "";

            // Check if the user is "a@trustshield.com"
            if (User.Identity.Name == "a@trustshield.com")
            {
                // If the user is "a@trustshield.com", select all employees
                query = "SELECT email, name FROM project.Customer";
            }
            else
            {
                // If the user is not "a@trustshield.com", select only the current user
                query = string.Format("SELECT email, name FROM project.Customer WHERE email='{0}'", User.Identity.Name);
            }

            using (NpgsqlCommand command = new NpgsqlCommand(query, sqlcon))
            {
                // Execute the command and retrieve the data
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    // Create a list to store the data
                    List<Customer> items = new List<Customer>();

                    if (User.Identity.Name == "a@trustshield.com")
                    {
                        // If the user is "a@trustshield.com", add a row for "ALL"
                        string email = ""; // Define the customer ID for "ALL"
                        Customer allCustomer = new Customer
                        {
                            Email = email,
                            Name = "ALL"
                        };

                        items.Add(allCustomer);
                    }

                    // Read the data and add it to the list
                    while (reader.Read())
                    {
                        string email = (string)reader["email"];
                        string val = (string)reader["name"];
                        items.Add(new Customer { Email = email, Name = val });
                    }

                    // Pass the list to the view
                    ViewBag.Items = items;
                }
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> PrivacyAsync(string datef, string datem, string dropdown)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            NpgsqlConnection sqlcon = await OpenDatabaseConnectionAsync();
            //sqlcon.Open();
            string dropdown_params = string.Empty;
            if (dropdown != null)
                dropdown_params = string.Format(" and c.email = '{0}'", dropdown);

            string query = string.Format(@"SELECT p.p_id, 
       CASE 
           WHEN v.pol_id IS NOT NULL THEN 'Auto Policy'
           WHEN t.pol_id IS NOT NULL THEN 'Travel Health'
           ELSE 'Property Policy'
                END AS PolicyType,
                c.name AS CustomerName,
                p.sdate AS StartDate,
                p.edate AS EndDate,
                p.package AS PackageCode,
                pkg.title AS PackageTitle,
                pkg.total AS PackageTotal
            FROM project.policy p
            LEFT JOIN project.Auto_pol v ON p.p_id = v.pol_id
            LEFT JOIN project.Travel_pol t ON p.p_id = t.pol_id
            LEFT JOIN project.property_pol pp ON p.p_id = pp.pr_id 
            left join project.pol_dog pd on p.p_id =pd.policy
            LEFT JOIN project.customer c ON pd.c_id = c.c_id--OR t.o_embg = c.c_id
            LEFT JOIN project.package pkg ON p.package = pkg.code
            where p.sdate between  '{0}' and '{1}'  {2}", datef, datem, dropdown_params);

            DataTable dataTable = await GetDataFromSqlServerAsync(connectionString, query);

            if (dataTable.Rows.Count == 0)
            {
                TempData["Nodata"] = "Nuk ka te dhena per kete periudhe!!";
                return RedirectToAction("Privacy");
            }

            string fileName = "template.xlsx";
            return GenerateExcelFile(fileName, dataTable);
        }
        public async Task<DataTable> GetDataFromSqlServerAsync(string connectionString, string query)
        {
            DataTable dataTable = new DataTable();

            using (NpgsqlConnection connection = await OpenDatabaseConnectionAsync())
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command);

                adapter.Fill(dataTable);
                connection.Close();
            }

            return dataTable;
        }

        public FileResult GenerateExcelFile(string fileName, DataTable dataTable)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                // Add column headers
                int colIndex = 1;
                foreach (DataColumn column in dataTable.Columns)
                {
                    worksheet.Cells[1, colIndex].Value = column.ColumnName;
                    colIndex++;
                }

                // Add data rows
                int rowIndex = 2;
                foreach (DataRow row in dataTable.Rows)
                {
                    colIndex = 1;
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        object value = row[column];

                        // Format date values explicitly
                        if (column.DataType == typeof(DateTime))
                        {
                            DateTime dateValue = (DateTime)value;
                            worksheet.Cells[rowIndex, colIndex].Value = dateValue.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            worksheet.Cells[rowIndex, colIndex].Value = value;
                        }

                        colIndex++;
                    }
                    rowIndex++;
                }

                // Write the file to the response stream
                MemoryStream memoryStream = new MemoryStream();
                excelPackage.SaveAs(memoryStream);

                // Return the Excel file as a byte array
                byte[] fileBytes = memoryStream.ToArray();

                // Set the response headers for file download
                //string fileName = "YourFileName.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(fileBytes, contentType, fileName);
            }
        }

        public IActionResult Auto()//typepolicy = 3
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AutoAsync(Vehicle veh)
        {
            if (ModelState.IsValid)
            {
                NpgsqlTransaction transaction = null;
                try
                {
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                    {
                        transaction = (NpgsqlTransaction)await con.BeginTransactionAsync();
                        DateTime startDate = DateTime.Parse(Request.Form["startDate"]);
                        DateTime enddate = startDate.AddYears(1);

                        // Insert data into Policy table and get p_id (use RETURNING to get the inserted ID in PostgreSQL)
                        string insertPolicyQuery = "INSERT INTO project.Policy (sdate, edate, package) " +
                                                   "VALUES (@Sdate, @Edate, 4) " +
                                                   "RETURNING p_id";
                        using (NpgsqlCommand insertPolicyCmd = new NpgsqlCommand(insertPolicyQuery, con))
                        {
                            insertPolicyCmd.Parameters.AddWithValue("@Sdate", startDate);
                            insertPolicyCmd.Parameters.AddWithValue("@Edate", enddate);
                            int p_id = (int)insertPolicyCmd.ExecuteScalar();

                            // Insert data into Auto_pol table and get a_id (again using RETURNING)
                            string insertPolAutoQuery = "INSERT INTO project.Auto_pol (pol_id) " +
                                                        "VALUES (@Pol_Id) " +
                                                        "RETURNING a_id";
                            using (NpgsqlCommand insertPolAutoCmd = new NpgsqlCommand(insertPolAutoQuery, con))
                            {
                                insertPolAutoCmd.Parameters.AddWithValue("@Pol_Id", p_id);
                                int a_id = (int)insertPolAutoCmd.ExecuteScalar();

                                // Insert data into Vehicle table
                                string insertVehicleQuery = "INSERT INTO project.Vehicle (policy, type, marka, model, license_plate) " +
                                                            "VALUES (@Policy, @Type, @Marka, @Model, @LicensePlate)";
                                using (NpgsqlCommand insertVehicleCmd = new NpgsqlCommand(insertVehicleQuery, con))
                                {
                                    insertVehicleCmd.Parameters.AddWithValue("@Policy", a_id);
                                    insertVehicleCmd.Parameters.AddWithValue("@Type", veh.Type);
                                    insertVehicleCmd.Parameters.AddWithValue("@Marka", veh.Marka);
                                    insertVehicleCmd.Parameters.AddWithValue("@Model", veh.Model);
                                    insertVehicleCmd.Parameters.AddWithValue("@LicensePlate", veh.License_Plate);
                                    insertVehicleCmd.ExecuteNonQuery();
                                }

                                // Insert data into pol_dog table
                                string insertDogQuery = @"INSERT INTO project.pol_dog (d_embg, c_id, name, policy, birthdate)
                                                SELECT @a_id, c_id, name, @Policy, CURRENT_DATE 
                                                FROM project.Customer WHERE email = @Email";
                                using (NpgsqlCommand insertDogCmd = new NpgsqlCommand(insertDogQuery, con))
                                {
                                    insertDogCmd.Parameters.AddWithValue("@Policy", p_id);
                                    insertDogCmd.Parameters.AddWithValue("@Email", User.Identity.Name);
                                    insertDogCmd.Parameters.AddWithValue("@a_id", p_id); // a_id + 1 as per your logic
                                    insertDogCmd.ExecuteNonQuery();
                                }
                                await transaction.CommitAsync();
                                return RedirectToAction("Payment", new { policyId = p_id, package = 4 });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the auto policy.");
                    // Log the exception if needed
                }
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> TravelAsync()
        {
            List<Package> packages = new List<Package>();
            using (NpgsqlConnection con = await OpenDatabaseConnectionAsync()) // Replace NpgsqlConnection with your database connection type
            {
                string query = "SELECT code, title FROM project.Package WHERE type_pol = 1";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int code = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            packages.Add(new Package { Code = code, Title = title }); // Replace Package with your actual model class
                        }
                    }
                }
            }

            // Store packages data in ViewBag
            ViewBag.Packages = packages;

            // Return the view
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> TravelAsync(Osi polOsi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                    {
                        int packageId = int.Parse(Request.Form["package"]);

                        // Calculate end date based on the selected start date and number of days
                        DateTime startDate = DateTime.Parse(Request.Form["startDate"]);
                        int numberOfDays = int.Parse(Request.Form["numberOfDays"]);
                        DateTime endDate = startDate.AddDays(numberOfDays);

                        // Insert data into Policy table with automatic ID generation and returning the p_id
                        string insertPolicyQuery = "INSERT INTO project.Policy (sdate, edate, package) " +
                                                   "VALUES (@Sdate, @Edate, @Package) " +
                                                   "RETURNING p_id";
                        using (NpgsqlCommand insertPolicyCmd = new NpgsqlCommand(insertPolicyQuery, con))
                        {
                            insertPolicyCmd.Parameters.AddWithValue("@Sdate", startDate);
                            insertPolicyCmd.Parameters.AddWithValue("@Edate", endDate);
                            insertPolicyCmd.Parameters.AddWithValue("@Package", packageId);
                            int p_id = (int)insertPolicyCmd.ExecuteScalar();

                            // Insert data into PolTravel table and return tr_id
                            string insertPolTravelQuery = "INSERT INTO project.Travel_pol (pol_id) " +
                                                          "VALUES (@Pol_Id) " +
                                                          "RETURNING tr_id";
                            using (NpgsqlCommand insertPolTravelCmd = new NpgsqlCommand(insertPolTravelQuery, con))
                            {
                                insertPolTravelCmd.Parameters.AddWithValue("@Pol_Id", p_id);
                                int tr_id = (int)insertPolTravelCmd.ExecuteScalar();

                                // Insert data into PolOsi table
                                string insertPolOsiQuery = "INSERT INTO project.Pol_osi (o_embg, policy, name, surname, birthdate, kontakt) " +
                                                           "VALUES (@O_Embg, @Policy, @Name, @Surname, @Birthdate, @Kontakt)";
                                using (NpgsqlCommand insertPolOsiCmd = new NpgsqlCommand(insertPolOsiQuery, con))
                                {
                                    insertPolOsiCmd.Parameters.AddWithValue("@O_Embg", polOsi.OEmbg);
                                    insertPolOsiCmd.Parameters.AddWithValue("@Policy", tr_id);
                                    insertPolOsiCmd.Parameters.AddWithValue("@Name", polOsi.Name);
                                    insertPolOsiCmd.Parameters.AddWithValue("@Surname", polOsi.Surname);
                                    insertPolOsiCmd.Parameters.AddWithValue("@Birthdate", polOsi.Birthdate);
                                    insertPolOsiCmd.Parameters.AddWithValue("@Kontakt", polOsi.Kontakt);
                                    insertPolOsiCmd.ExecuteNonQuery();
                                }

                                // Insert data into pol_dog table
                                string insertDogQuery = @"INSERT INTO project.pol_dog (d_embg, c_id, name, policy, birthdate)
                                                SELECT @tr_id, c_id, name, @Policy, CURRENT_DATE 
                                                FROM project.Customer 
                                                WHERE email = @email";
                                using (NpgsqlCommand insertDogCmd = new NpgsqlCommand(insertDogQuery, con))
                                {
                                    insertDogCmd.Parameters.AddWithValue("@Policy", p_id);
                                    insertDogCmd.Parameters.AddWithValue("@email", User.Identity.Name);
                                    insertDogCmd.Parameters.AddWithValue("@tr_id", p_id); // tr_id + 3 as per your logic
                                    insertDogCmd.ExecuteNonQuery();
                                }

                                // Redirect to Payment action with policyId and packageId
                                return RedirectToAction("Payment", new { policyId = p_id, package = packageId });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the travel policy.");
                    // Log the exception if needed
                }
            }

            return View(polOsi);
        }

        [HttpGet]
        public async Task<IActionResult> Property()
        {
            List<Package> packages = new List<Package>();
            using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
            {
                string query = "SELECT code, title FROM project.Package WHERE type_pol = 2";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int code = reader.GetInt32(0);
                            string title = reader.GetString(1);
                            packages.Add(new Package { Code = code, Title = title });
                        }
                    }
                }
            }
            ViewBag.Packages = packages;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Property(Property property)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                int packageId = int.Parse(Request.Form["package"]);

                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {
                    DateTime startDate = DateTime.Parse(Request.Form["startDate"]);
                    int numberOfDays = int.Parse(Request.Form["numberOfDays"]);
                    DateTime endDate = startDate.AddDays(numberOfDays);

                    string insertPolicyQuery = "INSERT INTO project.Policy (sdate, edate, package) " +
                                               "VALUES (@Sdate, @Edate, @Package) " +
                                               "RETURNING p_id";
                    int p_id;
                    using (NpgsqlCommand insertPolicyCmd = new NpgsqlCommand(insertPolicyQuery, con))
                    {
                        insertPolicyCmd.Parameters.AddWithValue("@Sdate", startDate);
                        insertPolicyCmd.Parameters.AddWithValue("@Edate", endDate);
                        insertPolicyCmd.Parameters.AddWithValue("@Package", packageId);
                        p_id = (int)insertPolicyCmd.ExecuteScalar(); // Get the policy ID (p_id)
                    }

                    int pr_id = 0;
                    string policyQuery = "INSERT INTO project.Property_pol (pol_id) " +
                                         "VALUES (@pol_id) " +
                                         "RETURNING pr_id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(policyQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@pol_id", p_id); // Use p_id from Policy table
                        pr_id = (int)cmd.ExecuteScalar(); // Get the generated pr_id for Property_pol
                    }

                    string query = "INSERT INTO project.Property (policy, address, floor, year_build, security) " +
                                   "VALUES (@policy, @address, @floor, @year_build, @security)";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@policy", pr_id); // Use pr_id from Property_pol
                        cmd.Parameters.AddWithValue("@address", property.Address);
                        cmd.Parameters.AddWithValue("@floor", property.Floor);
                        cmd.Parameters.AddWithValue("@year_build", property.YearBuild);
                        cmd.Parameters.AddWithValue("@security", true);

                        cmd.ExecuteNonQuery(); // Insert into Property table
                    }

                    string insertdog = @"INSERT INTO project.pol_dog (d_embg, c_id, name, policy, birthdate)
                                 SELECT @a_id, c_id, name, @Policy, CURRENT_DATE
                                 FROM project.Customer WHERE email=@email";
                    using (NpgsqlCommand insertDogCmd = new NpgsqlCommand(insertdog, con))
                    {
                        insertDogCmd.Parameters.AddWithValue("@Policy", p_id);
                        insertDogCmd.Parameters.AddWithValue("@email", User.Identity.Name);
                        insertDogCmd.Parameters.AddWithValue("@a_id", p_id); // pr_id + 1 as per your logic
                        insertDogCmd.ExecuteNonQuery();
                    }

                    return RedirectToAction("Payment", new { policyId = p_id, package = packageId });
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(property);
            }
        }





        [HttpGet]
        public async Task<IActionResult> Package()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                List<Package> packages = new List<Package>();

                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {
                    string query = "SELECT * FROM project.package"; // Adjust query to fetch all packages
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Package package = new Package
                            {
                                Code = (int)reader["Code"],
                                Title = (string)reader["Title"],
                                Total = reader["Total"] as decimal?,
                                Valuet = (string)reader["Valuet"],
                                TypePol = (int)reader["type_pol"] // Read the policy type
                            };
                            packages.Add(package);
                        }
                    }
                }

                return View(packages);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }

        // POST: Insert or Update package
        [HttpPost]
        public async Task<IActionResult> Package(Package package)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                    {
                        if (package.Code == 0) // New package (insert)
                        {
                            string insertQuery = "INSERT INTO project.package (Title, Total, Valuet, Type_Pol) " +
                                                 "VALUES (@Title, @Total, @Valuet, @TypePol)";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@Title", package.Title);
                                cmd.Parameters.AddWithValue("@Total", package.Total);
                                cmd.Parameters.AddWithValue("@Valuet", package.Valuet);
                                cmd.Parameters.AddWithValue("@TypePol", package.TypePol);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            string updateQuery = "UPDATE project.package SET Title = @Title, Total = @Total, Valuet = @Valuet " +
                                                 "WHERE Code = @Code";

                            string typepolquery = string.Format("select type_pol from project.Package where code={0}", package.Code);
                            using (NpgsqlCommand cmd1 = new NpgsqlCommand(typepolquery, con))
                            {
                                int type = (int)cmd1.ExecuteScalar();
                                package.TypePol = type;
                            }

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@Title", package.Title);
                                cmd.Parameters.AddWithValue("@Total", package.Total);
                                cmd.Parameters.AddWithValue("@Valuet", package.Valuet);
                                cmd.Parameters.AddWithValue("@TypePol", package.TypePol);
                                cmd.Parameters.AddWithValue("@Code", package.Code);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    return RedirectToAction("Package"); // Redirect back to the package list after saving
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the package.");
                }
            }

            return View("Package", package); // Stay on the same view in case of errors
        }

        [HttpGet]
        public async Task<IActionResult> Covers()
        {
            try
            {
                // Get the connection string
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                List<SelectListItem> packages = new List<SelectListItem>();
                List<Covers> covers = new List<Covers>();

                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {
                    // Query to fetch packages
                    string query = "SELECT code, title FROM project.Package";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            packages.Add(new SelectListItem
                            {
                                Value = reader["code"].ToString(),
                                Text = reader["title"].ToString()
                            });
                        }
                    }
                }

                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {
                    // Query to fetch packages
                    string query = "SELECT (select title from project.Package where code=CAST(pc.package AS INTEGER)) as package_name,pc.* FROM project.covers pc";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Covers cover = new Covers
                            {
                                cov_id = (int)reader["cov_id"],
                                cov_amount = (string)reader["cov_amount"],
                                cov_type = (string)reader["cov_type"],
                                PackageName = (string)reader["package_name"]
                            };
                            covers.Add(cover);
                        }
                    }
                }

                // Pass the packages to the view for the dropdown
                ViewData["Packages"] = packages;

                return View(covers);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Covers(Covers cover)
        {
            try
            {
                // Get the connection string
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (NpgsqlConnection con = await OpenDatabaseConnectionAsync())
                {
                    
                    string package = null;
                    if (cover.cov_id > 0)
                    {
                        package = string.Format("select code from project.Package where title='{0}'", cover.package_code);
                        NpgsqlCommand cmd1 = new NpgsqlCommand(package, con);
                        object result = cmd1.ExecuteScalar();
                        cover.package_code = result.ToString();
                    }

                    // If the cover has a valid id, we're updating an existing cover, otherwise, it's a new cover (insert)
                    string query;

                    if (cover.cov_id > 0) // Update existing cover
                    {
                        query = "UPDATE project.Covers SET cov_amount = @cov_amount, package = @package, cov_type = @cov_type WHERE cov_id = @cov_id";
                    }
                    else // Insert new cover
                    {
                        query = "INSERT INTO project.Covers (cov_amount, package, cov_type) VALUES (@cov_amount, @package, @cov_type)";
                    }

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@cov_amount", cover.cov_amount);
                        cmd.Parameters.AddWithValue("@package", cover.package_code); // Use the selected package_code
                        cmd.Parameters.AddWithValue("@cov_type", cover.cov_type);

                        // If updating, include the ID in the parameters
                        if (cover.cov_id > 0)
                        {
                            cmd.Parameters.AddWithValue("@cov_id", cover.cov_id);
                        }

                        // Execute the query
                        int result = await cmd.ExecuteNonQueryAsync();
                        if (result > 0)
                        {
                            return RedirectToAction("Covers"); // Redirect after success
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "An error occurred while creating/updating the cover.";
                            return View(cover); // Return to the form with an error message
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(cover);
            }
        }


        [HttpGet]
        public async Task<IActionResult> PaymentAsync(int policyId, int package = 0)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            int total = 0;
            if (package != 0)
            {
                // SQL query to get total amount for the specified package
                string query = "SELECT total FROM project.Package WHERE code = @package";

                try
                {
                    using (NpgsqlConnection conn = await OpenDatabaseConnectionAsync())
                    {

                        // Create and configure the SQL command
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@package", package);

                            // Execute the query and retrieve the result as a single value (ExecuteScalar)
                            object result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                total = Convert.ToInt32(result);
                            }
                            else
                            {
                                // Handle the case when no result is found, if necessary
                                total = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    // You can return an error page or return a view with an error message
                    return View("Error", new { message = ex.Message });
                }
            }
            var model = new Payment
                {
                    PolicyId = policyId,
                    PAmount = total
                    // You can populate the model with any additional data you need for the payment form
                };

                return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PaymentAsync(Payment model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Connection string from appsettings.json
                    string connectionString = _configuration.GetConnectionString("DefaultConnection");

                    // SQL query to insert payment data
                    string query = "INSERT INTO project.Payment (policy, p_date, p_amount, visa_number) " +
                                   "VALUES (@PolicyId, @PaymentDate, @PaymentAmount, @VisaNumber);";

                    using (NpgsqlConnection conn = await OpenDatabaseConnectionAsync())
                    {
                        // Create and configure the SQL command
                        using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@PolicyId", model.PolicyId);
                            cmd.Parameters.AddWithValue("@PaymentDate", model.PDate);
                            cmd.Parameters.AddWithValue("@PaymentAmount", model.PAmount);
                            cmd.Parameters.AddWithValue("@VisaNumber", model.VisaNumber);

                            // Execute the query
                            int rowsAffected = (int)cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                // If the payment was inserted successfully, redirect or return a success message
                                return View("PaymentSuccess");
                            }
                            else
                            {
                                // Handle failure case
                                ModelState.AddModelError("", "Error occurred while processing the payment.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log exception or handle accordingly
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }

            // Return the view with error if model validation fails or something went wrong
            return View(model);
        }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
