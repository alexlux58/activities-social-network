using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<Activity>>> { }

        public class Handler : IRequestHandler<Query, Result<List<Activity>>>
        {
            // private readonly ILogger<List> _logger;
            private readonly DataContext _context;
            public Handler(DataContext context, ILogger<List> logger)
            {
                // _logger = logger;
                _context = context;
            }
        
            public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // try{
                //     for (var i = 0; i < 10; i++)
                //     {
                //         cancellationToken.ThrowIfCancellationRequested();
                //         await Task.Delay(1000, cancellationToken);
                //         _logger.LogInformation($"Task {i} has completed");
                //     }
                // } catch(System.Exception ex) when (ex is TaskCanceledException) {
                //     _logger.LogInformation("Task was cancelled");
                // }

                var activities = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(u => u.AppUser)
                    .ToListAsync(cancellationToken);

                return Result<List<Activity>>.Success(activities);
            }


        }

        
    }
}