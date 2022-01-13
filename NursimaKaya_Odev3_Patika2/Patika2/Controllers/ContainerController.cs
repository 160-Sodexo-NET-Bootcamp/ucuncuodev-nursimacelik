using AutoMapper;
using Data;
using Data.Uow;
using Entity;
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

        private readonly IMapper mapper;

        public ContainerController(ILogger<ContainerController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var containerList = await unitOfWork.Container.GetAll();

            var entityList = new List<ContainerEntity>();

            foreach (var container in containerList)
            {
                var entity = mapper.Map<Container, ContainerEntity>(container);
                entityList.Add(entity);
            }
            return Ok(entityList);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var container = await unitOfWork.Container.GetById(id);

            if (container is null)
            {
                return NotFound();
            }

            var entity = mapper.Map<Container, ContainerEntity>(container);

            return Ok(entity);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContainerEntity entity)
        {
            var model = mapper.Map<ContainerEntity, Container>(entity);
            var response = await unitOfWork.Container.Add(model);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ContainerEntity entity, [FromQuery] long id)
        {
            var container = await unitOfWork.Container.GetById(id);
            if(container == null)
            {
                return NotFound();
            }

            var model = mapper.Map<ContainerEntity, Container>(entity);
            model.Id = id;
            var response = await unitOfWork.Container.Update(model);
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
