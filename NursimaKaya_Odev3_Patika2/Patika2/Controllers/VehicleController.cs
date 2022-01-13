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
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<VehicleController> _logger;

        private readonly IMapper mapper;

        public VehicleController(ILogger<VehicleController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicleList = await unitOfWork.Vehicle.GetAll();
            var entityList = new List<VehicleEntity>();
            foreach(var vehicle in vehicleList)
            {
                var entity = mapper.Map<Vehicle, VehicleEntity>(vehicle);
                entityList.Add(entity);
            }
            return Ok(entityList);
        }


        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById([FromRoute] long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            var entity = mapper.Map<Vehicle, VehicleEntity>(vehicle);

            return Ok(entity);
        }

        [HttpGet("{id}/Containers")]
        public async Task<IActionResult> GetContainers(long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            var containerList = await unitOfWork.Vehicle.GetContainers(id);
            var entityList = new List<ContainerEntity>();
            foreach(var container in containerList)
            {
                var entity = mapper.Map<Container, ContainerEntity>(container);
                entityList.Add(entity);
            }
            return Ok(entityList);
        }


        [HttpGet("{id}/{n}")]
        public async Task<IActionResult> SplitContainers([FromRoute] long id, [FromRoute] int n)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            var containerList = await unitOfWork.Vehicle.GetContainers(id);

            // split into n batches
            var batches = new List<List<ContainerEntity>>();
            var temp = new List<ContainerEntity>();
            var count = 0;
            foreach(Container c in containerList)
            {
                var containerEntity = mapper.Map<Container, ContainerEntity>(c);
                temp.Add(containerEntity);
                count++;
                if(count == n || c == containerList.Last<Container>())
                {
                    batches.Add(temp);
                    temp = new List<ContainerEntity>();
                    count = 0;
                }
            }
            return Ok(batches);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleEntity entity)
        {
            var model = mapper.Map<VehicleEntity, Vehicle>(entity);
            var response = await unitOfWork.Vehicle.Add(model);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VehicleEntity entity, [FromQuery] long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var model = mapper.Map<VehicleEntity, Vehicle>(entity);
            model.Id = id;
            await unitOfWork.Vehicle.Update(model);
            unitOfWork.Complete();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }
            await unitOfWork.Vehicle.Delete(id);
            unitOfWork.Complete();

            return Ok();
        }
    }

}
