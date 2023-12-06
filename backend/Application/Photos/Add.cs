using Application.Core;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Result<Photo>>
        {
            public IFormFile File { get; set; }
        }
    }
}