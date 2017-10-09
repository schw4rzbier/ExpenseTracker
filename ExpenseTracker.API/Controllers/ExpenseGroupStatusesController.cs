using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;

namespace ExpenseTracker.API.Controllers
{
    public class ExpenseGroupStatusesController : ApiController
    {
        IExpenseTrackerRepository _repository;
        ExpenseMasterDataFactory _expenseMasterDataFactory = new ExpenseMasterDataFactory();

        public ExpenseGroupStatusesController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ExpenseGroupStatusesController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }


        public IHttpActionResult Get()
        {

            try
            {
                // get expensegroupstatusses & map to DTO's
                var expenseGroupStatusses = _repository.GetExpenseGroupStatusses().ToList()
                    .Select(egs => _expenseMasterDataFactory.CreateExpenseGroupStatus(egs));

                return Ok(expenseGroupStatusses);

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
