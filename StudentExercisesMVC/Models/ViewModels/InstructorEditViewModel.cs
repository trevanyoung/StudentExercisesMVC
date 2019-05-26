using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class InstructorEditViewModel
    {
        // A single student
        public Instructor Instructor { get; set; } = new Instructor();

        // All cohorts
        public List<SelectListItem> Cohorts;

        public SqlConnection Connection;


        public InstructorEditViewModel() { }

        public InstructorEditViewModel(SqlConnection connection, int id)
        {
            Connection = connection;
            GetAllStuff(id);
        }

        public void GetAllStuff(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Designation from Cohort c";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Designation = reader.GetString(reader.GetOrdinal("Designation"))
                        };

                        cohorts.Add(cohort);
                    }

                    Cohorts = cohorts.Select(li => new SelectListItem
                    {
                        Text = li.Designation,
                        Value = li.Id.ToString()
                    }).ToList();

                    reader.Close();
                }
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
                            WHERE i.Id = @id
                        ";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Instructor.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        Instructor.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        Instructor.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        Instructor.SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle"));
                        Instructor.Speciality = reader.GetString(reader.GetOrdinal("Speciality"));
                        Instructor.CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"));
                    };
                    reader.Close();
                }
            }
        }
    }
}
