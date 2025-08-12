using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class UserRegisteredEvent : IDomainEvent
    {
        public User User { get; }
        public DateTime OccurredOn { get; }

        public UserRegisteredEvent(User user)
        {
            User = user;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
