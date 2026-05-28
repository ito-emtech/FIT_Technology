using DynamicDll.Db;
using FIT_Technology.Models.Daos;
using FIT_Technology.Models.Entities;
using FIT_Technology.Models.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FIT_Technology.Controllers
{
    public class EmployeeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(EmployeeController.List));
        }

        [HttpGet]
        public IActionResult List()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll();
                mng.Commit();
            }
            ViewBag.Info = info;
            return View(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
        }

        [HttpPost]
        public IActionResult List(List<string> empcd, string btn_action)
        {
            if(btn_action == "update")
            {
                return RedirectToAction(nameof(EmployeeController.Update), Ctrl.Get<EmployeeController>());
            }
            else if(btn_action == "alert")
            {
                return RedirectToAction(nameof(EmployeeController.Alert), Ctrl.Get<EmployeeController>());
            }
            else
            {
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
        }

        [HttpGet]
        public IActionResult Insert()
        {
            return View(nameof(EmployeeController.Insert));
        }

        [HttpPost]
        public IActionResult Insert(string btn_action)
        {
            if(btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            else
            {
                return RedirectToAction(nameof(AccountController.Menu), Ctrl.Get<AccountController>());
            }
        }

        [HttpGet]
        public IActionResult Alert()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll();
                mng.Commit();
            }
            ViewBag.Info = info;
            return View(nameof(EmployeeController.Alert));
        }

        [HttpPost]
        public IActionResult Alert(string btn_action)
        {
            if( btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            else
            {
                return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
            }
        }

        [HttpGet]
        public IActionResult Update()
        {
            List<EmployeeEntity> info = new List<EmployeeEntity>();
            using (TranMng mng = TranMng.BeginTransaction("empdb"))
            {
                EmployeeDao dao = new EmployeeDao();
                info = dao.FindAll();
                mng.Commit();
            }
            ViewBag.Info = info;
            return View(nameof(EmployeeController.Update));
        }

        [HttpPost]
        public IActionResult Update(string btn_action)
        {
            if (btn_action == "result")
            {
                return RedirectToAction(nameof(ResultController.Index), Ctrl.Get<ResultController>());
            }
            else
            {
                return RedirectToAction(nameof(EmployeeController.List), Ctrl.Get<EmployeeController>());
            }
        }
    }
}
