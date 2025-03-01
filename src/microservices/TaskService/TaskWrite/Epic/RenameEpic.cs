using Common.Domain.Models;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Epic;

public static class RenameEpic
{
    public record RenameEpicRequest(Guid Id, string NewTitle);
    public record RenameEpicResponse(Guid Id);
    
    public class Validator : Validator<RenameEpicRequest>
    {
        public Validator(IEventSourcingHandler<EpicAggregate> eventSourcingHandler)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
                .WithMessage((_, id) => $"Epic by id {id} not found");
            
            RuleFor(x => x.NewTitle)
                .NotEmpty()
                .WithMessage("Title is required")
                .MaximumLength(20)
                .WithMessage("Title must be less than 20 characters");
        }
    }
    
    public class Endpoint : Endpoint<RenameEpicRequest, RenameEpicResponse>
    {
        public IEventSourcingHandler<EpicAggregate> EventSourcingHandler { get; set; } = null!;
        
        public override void Configure()
        {
            Put("/epics");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(RenameEpicRequest req, CancellationToken ct)
        {
            var epic = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            epic.EditName(req.NewTitle);
            await EventSourcingHandler.SaveAsync(epic);
            await SendOkAsync(new RenameEpicResponse(epic.Id), ct);
        }
    }
}