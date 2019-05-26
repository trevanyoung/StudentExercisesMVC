using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesMVC.Models;
using StudentExercisesMVC.Models.ViewModels;

namespace StudentExercisesMVC.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IConfiguration _config;

        public InstructorsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET: Instructors
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT i.Id,
                                i.FirstName,
                                i.LastName,
                                i.SlackHandle,
                                i.Speciality,
                                i.CohortId
                            FROM Instructor i
                        ";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Instructor> instructors = new List<Instructor>();
                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Speciality = reader.GetString(reader.GetOrdinal("Speciality")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return View(instructors);
                }
            }
        }

        // GET: Instructors/Details/5
        public ActionResult Details(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT i.Id,
                                i.FirstName,
                                i.LastName,
                                i.SlackHandle,
                                i.Speciality,
                                i.CohortId
                            FROM Instructor i
                            WHERE i.Id = @InstructorId
                        ";
                    cmd.Parameters.Add(new SqlParameter("@InstructorId", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    Instructor instructor = null;
                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            Speciality = reader.GetString(reader.GetOrdinal("Speciality")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };
                    }

                    reader.Close();

                    return View(instructor);
                }
            }
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            InstructorCreateViewModel model = new InstructorCreateViewModel(Connection);
            return View(model);
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([FromForm] InstructorCreateViewModel model)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, SlackHandle, Speciality, CohortId)         
                                         OUTPUT INSERTED.Id                                                       
                                         VALUES (@firstName, @lastName, @handle, @speciality, @cId)";
                    cmd.Parameters.Add(new SqlParameter("@firstName", model.Instructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", model.Instructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@handle", model.Instructor.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@speciality", model.Instructor.Speciality));
                    cmd.Parameters.Add(new SqlParameter("@cId", model.Instructor.CohortId));

                    int newId = (int)cmd.ExecuteScalar();
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Instructors/Edit/5
        public ActionResult Edit(int id)
        {
            InstructorEditViewModel model = new InstructorEditViewModel(Connection, id);
            return View(model);
        }

        // POST: Instructors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([FromForm] InstructorEditViewModel model, int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Instructor

                            SET FirstName = @firstName,
                                LastName = @lastName,
                                SlackHandle = @slackHandle,
                                Speciality = @speciality,
                                CohortId = @cId
           
                            WHERE Instructor.Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", model.Instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", model.Instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@slackHandle", model.Instructor.SlackHandle));
                        cmd.Parameters.Add(new SqlParameter("@speciality", model.Instructor.Speciality));
                        cmd.Parameters.Add(new SqlParameter("@cId", model.Instructor.CohortId));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        /*if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");*/
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}