using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoursesAssignment.Models;

namespace CoursesAssignment.Controllers
{
    public class HomeController : Controller
    {
        //GET: Home/Index
        public ActionResult Index()
        {
            var model = new List<Course>();
            SqlCalls availableCourses = new SqlCalls();
            model = availableCourses.GetCourses();

            return View(model);
        }

        public ActionResult Slide() 
        {
            ViewBag.MessageSuccess = "hidden";
            ViewBag.MessageFail = "hidden";

            var model = new List<Slide>();
            SqlCalls availableSlides = new SqlCalls();
            model = availableSlides.GetSlides();

            return View(model);
        }

        public ActionResult Course()
        {
            ViewBag.MessageSuccess = "hidden";
            ViewBag.MessageFail = "hidden";

            var model = new List<Slide>();
            SqlCalls availableSlides = new SqlCalls();
            model = availableSlides.GetSlides();

            ViewBag.choosenSlides = null;

            return View(model);
        }

        [HttpGet]
        public ActionResult DisplayCourse(string courseid, string index)
        {
            SqlCalls getInfo = new SqlCalls();
            var model = new Course();
            ViewBag.warningMessage = "hidden";

            //this will set the courseid from the get method, if there is no courseid, then 1 will be the default
            if (courseid == null)
            {
                model = getInfo.GetCourseById(1);
            }
            else 
            {
                model = getInfo.GetCourseById(int.Parse(courseid));
            }

            //this will set the slide index from the get method, if there is no slide id, then the first slide will be default
            if(index == null)
            {
                if (0 >= model.Slides.Count() - 1)
                {
                    ViewBag.warningMessage = null;
                }
                ViewBag.index = 0;
            }
            else
            {
                if(int.Parse(index) > model.Slides.Count() - 1)
                {
                    ViewBag.warningMessage = null;
                    ViewBag.index = model.Slides.Count() - 1;
                }
                else
                {
                    ViewBag.index = int.Parse(index);
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult slideForm(string txtSlide)
        {
            SqlCalls slideSQL = new SqlCalls();
            int id = slideSQL.GetCount();           //this will get the number of slides in the DB for slide ID

            //create the slide object
            Slide slide = new Slide() { Id = id, Text = txtSlide };
            ViewBag.Id = id;

            //insert into the DB
            if (slideSQL.InsertSlide(slide))
            {
                ViewBag.MessageSuccess = null;
                ViewBag.MessageFail = "hidden";
            }
            else
            {
                ViewBag.MessageFail = null;
                ViewBag.MessageSuccess = "hidden";
            }

            var model = new List<Slide>();
            SqlCalls availableSlides = new SqlCalls();
            model = availableSlides.GetSlides();

            return View("Slide", model);
        }
    
        [HttpPost]
        public ActionResult courseForm(string txtSlides)
        {
            SqlCalls slideSQL = new SqlCalls();
            int id = slideSQL.GetCourseCount();           //this will get the number of slides in the DB for slide ID

            //filter the list of slides in course
            List<Slide> slides = new List<Slide>();
            string[] split = txtSlides.Split(',');

            //add the request slide numbers to a list
            foreach(var item in split)
            {
                slides.Add(new Slide() { Id = int.Parse(item)});
            }

            //set the course id and the slide deck of the course
            Course course = new Course() { Id = id, Slides = slides };
            ViewBag.Id = id;

            //insert into the DB
            if (slideSQL.InsertCourse(course))
            {
                ViewBag.MessageSuccess = null;
                ViewBag.MessageFail = "hidden";
            }
            else
            {
                ViewBag.MessageFail = null;
                ViewBag.MessageSuccess = "hidden";
            }

            //update the page with a refreshed model
            var model = new List<Slide>();
            SqlCalls availableSlides = new SqlCalls();
            model = availableSlides.GetSlides();

            return View("Course", model);
        }
    }
}