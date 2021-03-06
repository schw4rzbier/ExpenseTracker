﻿using ExpenseTracker.DTO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.WebClient.Helpers;
using ExpenseTracker.WebClient.Models;
using Marvin.JsonPatch;
using PagedList;

namespace ExpenseTracker.WebClient.Controllers
{
    public class ExpenseGroupsController : Controller
    {

        public async Task<ActionResult> Index(int? page = 1)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            var model = new ExpenseGroupsViewModel(); 

            var egsResponse = await client.GetAsync("api/expensegroupstatuses");

            if (egsResponse.IsSuccessStatusCode)
            {
                string egsContent = await egsResponse.Content.ReadAsStringAsync();

                model.ExpenseGroupStatuses =
                    JsonConvert.DeserializeObject<IEnumerable<ExpenseGroupStatus>>(egsContent);
            }
            else
            {
                return Content("An error occurred.");
            }

            HttpResponseMessage response = await client.GetAsync("api/expensegroups?sort=expensegroupstatusid,title&page=" + page + "&pagesize=5");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                var pagingInfo = HeaderParser.FindAndParsePagingInfo(response.Headers);

                var lstExpenseGroups = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);

                var pagedExpenseGroupsList = new StaticPagedList<ExpenseGroup>(lstExpenseGroups, pagingInfo.CurrentPage,
                    pagingInfo.PageSize, pagingInfo.TotalCount);

                model.ExpenseGroups = pagedExpenseGroupsList;
                model.PagingInfo = pagingInfo;
            }
            else
            {
                return Content("An error occurred.");
            }

            return View(model);

        }

 
        // GET: ExpenseGroups/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response =
                await client.GetAsync("api/expensegroups/" + id + "?fields=id,description,title,expenses");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }
            return Content("An error occurred.");
        }

        // GET: ExpenseGroups/Create
 
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExpenseGroups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseGroup expenseGroup)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();

                // an expensegroup is created with status "Open", for the current user
                expenseGroup.ExpenseGroupStatusId = 1;
                expenseGroup.UserId = @"https://expensetrackeridsrv3/embedded_1";

                var serializedItemToCreate = JsonConvert.SerializeObject(expenseGroup);

                var response = await client.PostAsync("api/expensegroups",
                    new StringContent(serializedItemToCreate, 
                        System.Text.Encoding.Unicode, 
                        "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred.");
                }
            }
            catch
            {
                return Content("An error occurred.");
            }
        }

        // GET: ExpenseGroups/Edit/5
 
        public async Task<ActionResult> Edit(int id)
        {
            var client = ExpenseTrackerHttpClient.GetClient();

            HttpResponseMessage response = await client.GetAsync("api/expensegroups/" + id + "?fields=id,title,description");
            string content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var model = JsonConvert.DeserializeObject<ExpenseGroup>(content);
                return View(model);
            }

            return Content("An error occurred.");  
        }

        // POST: ExpenseGroups/Edit/5   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ExpenseGroup expenseGroup)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();

                var patchDoc = new JsonPatchDocument<ExpenseGroup>();
                patchDoc.Replace(eg => eg.Title, expenseGroup.Title);
                patchDoc.Replace(eg => eg.Description, expenseGroup.Description);

                var serializedItemToUpdate = JsonConvert.SerializeObject(patchDoc);

                var response = await client.PatchAsync("api/expensegroups/" + id,
                    new StringContent(serializedItemToUpdate, System.Text.Encoding.Unicode, "application/json"));
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred.");
                }
            }
            catch
            {
                return Content("An error occurred.");
            }
        }
         

        // POST: ExpenseGroups/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();
                var response = await client.DeleteAsync("api/expensegroups/" + id);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred.");
                }
            }
            catch
            {
                return Content("An error occurred.");
            }
        }
    }
}
