using Common.Auth;
using Common.Domain.Aggregates;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Ticket;

public static class CreateTicket
{
    public record CreateTicketRequest(Guid ProjectId, string Title);
    public record CreateTicketResponse(Guid Id);
    
    public class Validator : Validator<CreateTicketRequest>
    {
        public Validator(IEventSourcingHandler<ProjectAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
            
            RuleFor(x => x.ProjectId)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetAggregateByIdAsync(id) != null)
                .WithMessage((_, id) => $"Project by id {id} not found");
        }
    }
    
    public class Endpoint : Endpoint<CreateTicketRequest, CreateTicketResponse>
    {
        public IEventSourcingHandler<TicketAggregate> EventSourcingHandler { get; set; } = null!;
        public ICurrentUserService CurrentUserService { get; set; } = null!;
        
        public override void Configure()
        {
            Post("/tickets");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(CreateTicketRequest req, CancellationToken ct)
        {
            var task = new TicketAggregate(
                id: Guid.NewGuid(),
                projectId: req.ProjectId,
                title: req.Title,
                createdBy: CurrentUserService.GetUserId());
            
            await EventSourcingHandler.SaveAggregateAsync(task);
            await SendOkAsync(new CreateTicketResponse(task.Entity.Id), ct);
        }
    }
}