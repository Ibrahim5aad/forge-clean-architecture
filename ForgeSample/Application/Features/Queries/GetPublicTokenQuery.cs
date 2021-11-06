using ForgeSample.Application.DTOs;
using ForgeSample.Application.Wrappers;
using MediatR;

namespace ForgeSample.Application.Features.Queries
{
    public class GetPublicTokenQuery : IRequest<Response<Token>>
    {

    }
}
