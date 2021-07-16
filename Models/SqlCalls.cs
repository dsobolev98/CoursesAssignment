using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Web;

namespace CoursesAssignment.Models
{
    public class SqlCalls
    {
        public string connectionString = "Data Source=DESKTOP-2S22EKT;Initial Catalog=LinearFT;Integrated Security=True";

        public int GetCount()
        {
            int count = new int();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT COUNT(*) as COUNT FROM course.slide";
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = int.Parse(reader["COUNT"].ToString()) + 1;  //we get a return
                    }
                    else
                    {
                        count = -1;                                         //we do not get a return
                    }
                }
                connection.Close();
            }
            return count;
        }

        public bool InsertSlide(Slide slide)
        {
            bool result = new bool();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "INSERT INTO course.slide(id, slideText) VALUES(@id, @slideText)";
                    command.Parameters.AddWithValue("@id", slide.Id);
                    command.Parameters.AddWithValue("@slideText", slide.Text);
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                    catch {
                        result = false;
                    }
                }
                connection.Close();
            }
            return result;
        }

        public List<Slide> GetSlides()
        {
            List<Slide> slides = new List<Slide>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT * FROM course.slide";
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        slides.Add(new Slide() { Id = (int)reader[0], Text = (string)reader[1] });
                    }
                }
                connection.Close();
            }
            return slides;
        }

        public Slide GetSlideById(int id)
        {
            Slide slide = new Slide();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT * FROM course.slide where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        slide.Id = (int)reader["id"];
                        slide.Text = reader["slideText"].ToString();
                    }
                }
                connection.Close();
            }

            return slide;
        }
        public int GetCourseCount()
        {
            int count = new int();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT COUNT(DISTINCT id) as COUNT FROM course.courses";
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        count = int.Parse(reader["COUNT"].ToString()) + 1;  //we get a return
                    }
                    else
                    {
                        count = -1;                                         //we do not get a return
                    }
                }
                connection.Close();
            }
            return count;
        }

        public bool InsertCourse(Course course)
        {
            bool result = new bool();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    try
                    {
                        connection.Open();
                        command.CommandText = "INSERT INTO course.courses(id, slideId) VALUES(@id, @slideId)";
                        command.Parameters.AddWithValue("@id", course.Id);

                        //insert initial slide (We must have at least one slide)
                        command.Parameters.AddWithValue("@slideId", course.Slides[0].Id);
                        command.ExecuteNonQuery();

                        //insert the other ones
                        for (int i = 1; i < course.Slides.Count(); i++)
                        {
                            command.Parameters["@slideId"].Value = course.Slides[i].Id;
                            command.ExecuteNonQuery();
                        }

                        //return true since we are finished inserting into the DB
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
                connection.Close();
            }
            return result;
        }
        
        public List<Course> GetCourses() 
        {
            List<Course> courses = new List<Course>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT * FROM course.courses";
                    connection.Open();

                    //track the course id
                    int prev = 0;
                    int index = -1;
                    Course course;

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int currentId = (int)reader["id"];

                        if(prev == currentId) 
                        {
                            courses[index].Slides.Add(GetSlideById((int)reader["slideId"]));
                        }
                        else
                        {
                            prev = currentId;
                            course = new Course() { Id = currentId, Slides = new List<Slide>() };
                            course.Slides.Add(GetSlideById((int)reader["slideId"]));
                            courses.Add(course);
                            index++;
                        }
                    }
                }
                connection.Close();
            }
            return courses;
        }

        public Course GetCourseById(int id)
        {
            Course course = new Course();
            course.Slides = new List<Slide>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "SELECT * FROM course.courses where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        course.Id = (int)reader["id"];
                        course.Slides.Add(GetSlideById((int)reader["slideId"]));
                    }
                }
                connection.Close();
            }
            return course;
        }
    }
}