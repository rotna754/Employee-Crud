using jQueryAjaxCrud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQueryAjaxCrud.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewAll()
        {
            return View(GetAllEmployee());
        }

        IEnumerable<Employee> GetAllEmployee()
        {
            using (jQueryAjaxCrudEntities db = new jQueryAjaxCrudEntities())
            {
                return db.Employees.ToList<Employee>();
            }
        }

        public ActionResult AddOrEdit(int id = 0)
        {
            Employee employee = new Employee();
            if(id != 0)
            {
                using (jQueryAjaxCrudEntities db = new jQueryAjaxCrudEntities())
                {
                    employee = db.Employees.Where(x => x.EmployeeId == id).FirstOrDefault<Employee>();
                }
            }
            return View(employee);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Employee employee)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if (employee.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(employee.ImageUpload.FileName);
                        string extension = Path.GetExtension(employee.ImageUpload.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        employee.ImagePath = "~/AppFiles/Images/" + fileName;
                        employee.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/AppFiles/Images/"), fileName));
                    }
                    using (jQueryAjaxCrudEntities db = new jQueryAjaxCrudEntities())
                    {
                        if (employee.EmployeeId == 0)
                        {
                            db.Employees.Add(employee);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Submitted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, html = GlobalClass.RenderRazorViewToString(this, "AddOrEdit", employee), message = "Validation Not Successfully completed", }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                using (jQueryAjaxCrudEntities db = new jQueryAjaxCrudEntities())
                {
                    Employee employee = db.Employees.Where(x => x.EmployeeId == id).FirstOrDefault<Employee>();
                    db.Employees.Remove(employee);
                    db.SaveChanges();
                }
                return Json(new { success = true, html = GlobalClass.RenderRazorViewToString(this, "ViewAll", GetAllEmployee()), message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}