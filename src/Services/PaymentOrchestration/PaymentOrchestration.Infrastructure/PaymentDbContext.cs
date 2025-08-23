using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentOrchestration.Domain.Aggregates;
using PaymentOrchestration.Domain.Common.Abstract;

namespace PaymentOrchestration.Infrastructure;

public class PaymentDbContext : DbContext
{
    private readonly IPublisher _publisher;

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }
    
    public DbSet<PaymentRequest> PaymentRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder.Entity<PaymentRequest>().OwnsOne(pr => pr.Amount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount");
            money.Property(m => m.Currency).HasColumnName("Currency");
        });
        
        
        modelBuilder.Entity<PaymentRequest>().ToTable("payment_requests");
        
        base.OnModelCreating(modelBuilder);
        
       
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = GetDomainEvents();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEvents(domainEvents);
        return result;
    }
    
    private List<IDomainEvent> GetDomainEvents()
    {
        var entitiesWithEvents = ChangeTracker.Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();
        
        var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();
        
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());
        
        return domainEvents;
    }

    private async Task DispatchDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}