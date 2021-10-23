using ETS.DataAccess.Repository.IRepository;
using ETS.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETS.Controllers
{
    public class DutyController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public DutyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public class Person
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string user_id { get; set; }
        }

        [HttpGet]
        [Route("~/Prop/User")]
        public IActionResult PropList()
        {
            List<Person> people = new List<Person>();

            people.Add(new Person { first_name = "Julekha", last_name = "Begum", user_id = "010002" });
            people.Add(new Person { first_name = "Sabuj",   last_name = "Molla", user_id = "010003" });
            people.Add(new Person { first_name = "Ikbal",   last_name = "Nayem", user_id = "010004" });
            people.Add(new Person { first_name = "Habib", last_name =   "Imrul",   user_id = "010005" });

            return Json(new { success = true, message = people });
        }




        [HttpGet]
        [Route("~/Duty/paging")]
        public async Task<IActionResult> Paging(int page_no, int page_size)
        {

            IEnumerable<Duty> list = await _unitOfWork.Duty.GetAllAsync();
            var Pagedlist = list.Skip((page_no - 1) * page_size).Take(page_size).ToList();
            return Json(new { success = true, message = list });
        }

        [HttpGet]
        [Route("~/Duty/GetList")]
        public async Task<IActionResult> List()
        {
            string user = GetUserId();
            IEnumerable<Duty> list = await _unitOfWork.Duty.GetAllAsync(u=>u.user_id == user);
            return Json(new { success = true, message = list });
        }




        [HttpPost]
        [Route("~/Duty/Add")]
        public async Task<IActionResult> AddUpdate([FromBody] Duty duty)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    if (duty.assign_to == null) throw new Exception("Assign to cannot be empty");
                    if (duty.task_name == null) throw new Exception("task name cannot be empty");

                    if (duty.id == 0)
                    {

                        string user = GetUserId();
                        duty.user_id = user;
                        await _unitOfWork.Duty.AddAsync(duty);

                    }
                    else
                    {
                        _unitOfWork.Duty.Update(duty);

                    }
                    await _unitOfWork.SaveAsync();

                    return Json(new { success = true, message = duty });

                }
                else
                {
                    throw new InvalidOperationException("The Data sent to API is invalid!");
                    //return Json(new { success = false, message = "The Data sent to API is invalid!" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }


        }



        [HttpDelete]
        [Route("~/Duty/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            Duty duty = await _unitOfWork.Duty.GetFirstOrDefaultAsync(u => u.id == id);
            if (duty == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Duty.Remove(duty);
            await _unitOfWork.SaveAsync();
            return Json(new { success = true, message = "Delete Successful" });

        }




    }
}
