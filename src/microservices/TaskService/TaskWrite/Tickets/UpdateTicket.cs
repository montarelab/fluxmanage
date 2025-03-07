using Common.Domain.Aggregates;
using Common.DTO;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Tickets;

public static class UpdateTicket
{
    public record UpdateTicketRequest : TicketUpdateData;
    public record UpdateTicketResponse(Guid Id);
    public class Endpoint : Endpoint<UpdateTicketRequest, UpdateTicketResponse>
    {
        public IEventSourcingHandler<TicketAggregate> EventSourcingHandler { get; set; } = null!;
        public override void Configure()
        {
            Put("/tickets");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(UpdateTicketRequest req, CancellationToken ct)
        {
            var task = (await EventSourcingHandler.GetAggregateByIdAsync(req.Id))!;
            task.Update(req);
            await EventSourcingHandler.SaveAggregateAsync(task);
            await SendOkAsync(new UpdateTicketResponse(task.Entity.Id), ct);
        }
    }
    
     public class Validator : Validator<UpdateTicketRequest>
        {
            public Validator(IEventSourcingHandler<TicketAggregate> eventSourcingHandler)
            {
                RuleFor(x => x.Id)
                    .MustAsync(async (id, _) => await eventSourcingHandler.GetAggregateByIdAsync(id) != null)
                    .WithMessage((_, id) => $"Task by id {id} not found");

                RuleFor(x => x.Title)
                    .NotEmpty()
                    .WithMessage("Title is required")
                    .MaximumLength(20)
                    .WithMessage("Title must be less than 20 characters")
                    .When(x => x.Title != null);
                
                RuleFor(x => x.Description)
                    .MaximumLength(500)
                    .WithMessage("Description must be less than 500 characters")
                    .When(x => x.Description != null);
                
                RuleFor(x => x.StartDate)
                    .Must((req, startDate) => startDate <= req.DueDate)
                    .WithMessage("The start date must be before the due date")
                    .When(x => x.StartDate != null);
                
                // todo validate if Assignee id is valid and a user exists with that id
                // todo validate if epic exists
                RuleFor(x => x.ParentTicketId)
                    .MustAsync(async (ParentTicketId, _) => 
                        await eventSourcingHandler.GetAggregateByIdAsync(ParentTicketId!.Value) != null)
                    .WithMessage((_, id) => $"Parent task by id {id} does not exist.")
                    .When(x => x.ParentTicketId != null);
                
                RuleFor(x => x.CustomFields)
                    .Must((_, customFields) => customFields!.Count <= 10)
                    .WithMessage("There cannot be more than 10 custom fields!")
                    .Must((_, customFields) => 
                        customFields!.All(x => x.Key.Length <= 20 && x.Value.Length <= 200))
                    .WithMessage("Custom field key must be less than 20 characters and value must be less than 200 characters!")
                    .When(x => x.CustomFields != null);
            }
        }
        
}