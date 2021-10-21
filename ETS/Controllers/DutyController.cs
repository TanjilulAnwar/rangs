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
            IEnumerable<Duty> list = await _unitOfWork.Duty.GetAllAsync();
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
                    if (duty.id == 0)
                    {

                        await _unitOfWork.Duty.AddAsync(duty);

                    }
                    else
                    {
                        _unitOfWork.Duty.Update(duty);

                    }
                    await _unitOfWork.SaveAsync();
                    return Json(new { success = true, message = "Data has been inserted Successfully!" });

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
        [Route("~/Duty/delete/{id}")]
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
