using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        // private readonly IMediator _mediator;

        // private readonly DataContext _context;
        // public ActivitiesController(DataContext context)

        // bellow is replaced by "protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService(typeof(IMediator)) as IMediator;"
        // inside the BaseApiController.cs

        // public ActivitiesController(IMediator mediator)
        // {
        //     // _context = context;
        //     _mediator = mediator;
            
        // }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            // return await _context.Activities.ToListAsync();
            // return await _mediator.Send(new List.Query());
            return await Mediator.Send(new List.Query());
        }

        [HttpGet("{id}")] // /api/activities/id
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            // return await _context.Activities.FindAsync(id);
            return await Mediator.Send(new Details.Query{Id = id});
            // return Ok();
        }

        
    }
}