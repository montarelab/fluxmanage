using Common.Domain.Models;
using Common.DTO;
using Common.EventSourcing;
using FastEndpoints;
using FluentValidation;

namespace TaskWrite.Tasks;

public static class UpdateTask
{
    public record UpdateTaskRequest : TaskUpdateData;
    public record UpdateTaskResponse(Guid Id);
    public class Endpoint : Endpoint<UpdateTaskRequest, UpdateTaskResponse>
    {
        public IEventSourcingHandler<TaskAggregate> EventSourcingHandler { get; set; } = null!;
        public override void Configure()
        {
            Put("/tasks");
            AllowAnonymous();

            // todo introduce permissions
        }

        public override async Task HandleAsync(UpdateTaskRequest req, CancellationToken ct)
        {
            var task = (await EventSourcingHandler.GetByIdAsync(req.Id))!;
            task.Update(req);
            await EventSourcingHandler.SaveAsync(task);
            await SendOkAsync(new UpdateTaskResponse(task.Id), ct);
        }
    }
    
     public class Validator : Validator<UpdateTaskRequest>
        {
            public Validator(IEventSourcingHandler<TaskAggregate> eventSourcingHandler)
            {
                RuleFor(x => x.Id)
                    .MustAsync(async (id, _) => await eventSourcingHandler.GetByIdAsync(id) != null)
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
                RuleFor(x => x.ParentTaskId)
                    .MustAsync(async (parentTaskId, _) => 
                        await eventSourcingHandler.GetByIdAsync(parentTaskId!.Value) != null)
                    .WithMessage((_, id) => $"Parent task by id {id} does not exist.")
                    .When(x => x.ParentTaskId != null);
                
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