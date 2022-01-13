using Data;
using Data.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patika2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<ContainerController> _logger;

        public ContainerController(ILogger<ContainerController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var containerList = await unitOfWork.Container.GetAll();
            return Ok(containerList);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var container = await unitOfWork.Container.GetById(id);

            if (container is null)
            {
                return NotFound();
            }

            return Ok(container);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Container entity)
        {
            var response = await unitOfWork.Container.Add(entity);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Container entity)
        {
            var response = await unitOfWork.Container.Update(entity);
            unitOfWork.Complete();
            if(response == false)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            var container = await unitOfWork.Container.GetById(id);

            if (container is null)
            {
                return NotFound();
            }
            await unitOfWork.Container.Delete(id);
            unitOfWork.Complete();

            return Ok();
        }
    }

}
