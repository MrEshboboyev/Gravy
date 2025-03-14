using System;
using Gravy.Domain.Primitives;
using Gravy.Domain.Enums.Orders;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Orders;

/// <summary>
/// Represents delivery instructions for an order.
/// </summary>
public sealed class DeliveryInstructions : ValueObject
{
    public string Instructions { get; }
    public ContactPreference ContactPreference { get; }

    private DeliveryInstructions(
        string instructions,
        ContactPreference contactPreference)
    {
        Instructions = instructions;
        ContactPreference = contactPreference;
    }

    public static Result<DeliveryInstructions> Create(
        string instructions,
        ContactPreference contactPreference)
    {
        if (string.IsNullOrWhiteSpace(instructions))
        {
            return Result.Failure<DeliveryInstructions>("Instructions cannot be empty.");
        }

        return Result.Success(new DeliveryInstructions(
            instructions,
            contactPreference));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Instructions;
        yield return ContactPreference;
    }
}
