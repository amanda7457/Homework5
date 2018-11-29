using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Barron_Amanda_HW5.Models;
using Barron_Amanda_HW5.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Barron_Amanda_HW5.Controllers
{

    public enum StarDetailedSearch {GreaterThan, LessThan }
    public enum SortOrder { Ascending, Decending}

    public class HomeController : Controller
    {

        // GET: Home
        public ActionResult Index(String SearchString)
        {

            ViewBag.TotalRepositories = _db.Repositories.Count();
            List<Repository> SelectedRepositories = new List<Repository>();
            var query = from r in _db.Repositories
                        select r;
            if (SearchString != null && SearchString != "")
            {
                query = query.Where(r => r.RepositoryName.Contains(SearchString) || r.UserName.Contains(SearchString));
            }

            SelectedRepositories = query.Include(r => r.Language).ToList();       
            ViewBag.SelectedRepositories = SelectedRepositories.Count();
            return View(SelectedRepositories.OrderByDescending(r => r.StarCount));

        }

        private AppDbContext _db;

        public HomeController(AppDbContext context)
        {
            _db = context; 
        }

        public IActionResult Details(int? id)
        {
            if (id == null) //Repo id not specified
            {
                return View("Error", new String[] { "Repository ID not specified - which repo do you want to view?" });
            }

            Repository repo = _db.Repositories.Include(r => r.Language).FirstOrDefault(r => r.RepositoryID == id);

            if (repo == null) //Repo does not exist in database
            {
                return View("Error", new String[] { "Repository not found in database" });
            }

            //if code gets this far, all is well
            return View(repo);

        }


        public ActionResult DetailedSearch()
        {
            //do I need a ViewBag.SelectedRepositories ?
            ViewBag.AllLanguages = GetAllLanguages();
            return View();
        }



        public ActionResult DisplaySearchResults(String SearchString, String Description, int SelectedLanguage, String NumberofStars, StarDetailedSearch SelectedStars, DateTime? datSelectedDate)
        {
            List <Repository> RepositoriesToDisplay = new List<Repository>();
            // Where do we use LINQ--------------------------
            var query = from c in _db.Repositories
                        select c;

            if (SearchString != null && SearchString != "") //they input something
            {
                query = query.Where(c => c.UserName.Contains(SearchString) || c.RepositoryName.Contains(SearchString));
                //do we code a view for every single one???? which view ?????
            }


            if (Description != null && Description != "") //they input something
            {
                query = query.Where(c => c.Description.Contains(Description));
            }


            if (SelectedLanguage == 0) // they chose "all months from the drop-down
            {
                query = query.OrderByDescending(c => c.StarCount);

            }
            else //language was chosen
            {
                //query = query.Where(c => c.Language == SelectedLanguage);
                //int max = _db.Languages.Max(l => l.LanguageID);
                //Language lang = _db.Languages.Last();
                //int lastlangid = lang.LanguageID;
                Language LanguageToDisplay = _db.Languages.Find(SelectedLanguage);
                query = query.Where(c => c.Language == LanguageToDisplay);
            }


            if (NumberofStars != null && NumberofStars != "")
            //make sure string is a valid number
            {
                Decimal decNumberofStars;
                try
                {
                    decNumberofStars = Convert.ToDecimal(NumberofStars);
                }
                catch  //this code will display when something is wrong
                {
                    //Re-populate dropdown
                    ViewBag.AllLanguages = GetAllLanguages();
                   
                    //Send user back to home page
                    return View("DetailedSearch");
                }

                //Add variable to store this decimal? JK we have that variable


                //ViewBag.UpdatedNumberofStars = "The desired number of stars is " + decNumberofStars.ToString("n2");
            }
            //else  they didn't specify GPA
            //{
                //ViewBag.UpdatedNumberofStars = "No number of stars was specified";
            //}

            switch (SelectedStars)
            {

                case StarDetailedSearch.GreaterThan:
                    Decimal decNumberofStars;
                    decNumberofStars = Convert.ToDecimal(NumberofStars);
                    query = query.Where(c => c.StarCount >= decNumberofStars);
                    break;
                case StarDetailedSearch.LessThan:
                    Decimal decNumberofStarsLess;
                    decNumberofStarsLess = Convert.ToDecimal(NumberofStars);
                    query = query.Where(c => c.StarCount <= decNumberofStarsLess);
                    break; 
               //do i need a default?
                default:
                    query = query.OrderByDescending(c => c.StarCount);
                    break;
            }


            if (datSelectedDate != null)
            {
                //convert date to non-nullable type.  ?? means if the datSelectedDate is null, set it equal to Jan 1, 1900
                DateTime datSelected = datSelectedDate ?? new DateTime(1900, 1, 1);
                query = query.Where(c => c.LastUpdate >= datSelectedDate);
            }
            else //They didn't pick a date
            {
                query = query.OrderByDescending(c => c.StarCount);
            }


            List<Repository> SelectedRepositories = query.ToList();
            SelectedRepositories = (System.Collections.Generic.List<Barron_Amanda_HW5.Models.Repository>)query.Include(c => c.Language).ToList();
            ViewBag.SelectedRepositories = SelectedRepositories.Count();
            ViewBag.TotalRepositories = _db.Repositories.Count();
            return View("Index", SelectedRepositories);

        }


        public SelectList GetAllLanguages()
        {
            List<Language> Languages = _db.Languages.ToList();

            //add a record for all months (Languages)
            Language SelectNone = new Language() { LanguageID = 0, Name = "All Languages" };
            Languages.Add(SelectNone);

            //convert list to select list
            SelectList AllLanguages = new SelectList(Languages.OrderBy(m => m.LanguageID), "LanguageID", "Name");

            //return the select list
            return AllLanguages;
        }
    }
}

 