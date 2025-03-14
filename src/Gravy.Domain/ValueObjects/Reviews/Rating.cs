using Gravy.Domain.Primitives;
using Gravy.Domain.Shared;

namespace Gravy.Domain.ValueObjects.Reviews;

public sealed class Rating : ValueObject
{
    public int FoodQuality { get; }
    public int DeliverySpeed { get; }
    public int ValueForMoney { get; }
    public int Accuracy { get; }
    public int OverallExperience { get; }

    private Rating(int foodQuality, int deliverySpeed, int valueForMoney, int accuracy, int overallExperience)
    {
        FoodQuality = foodQuality;
        DeliverySpeed = deliverySpeed;
        ValueForMoney = valueForMoney;
        Accuracy = accuracy;
        OverallExperience = overallExperience;
    }

    public static Result<Rating> Create(int foodQuality, int deliverySpeed, int valueForMoney, int accuracy, int overallExperience)
    {
        if (foodQuality < 1 || foodQuality > 5)
            return Result.Failure<Rating>("Food quality rating must be between 1 and 5.");
        if (deliverySpeed < 1 || deliverySpeed > 5)
            return Result.Failure<Rating>("Delivery speed rating must be between 1 and 5.");
        if (valueForMoney < 1 || valueForMoney > 5)
            return Result.Failure<Rating>("Value for money rating must be between 1 and 5.");
        if (accuracy < 1 || accuracy > 5)
            return Result.Failure<Rating>("Accuracy rating must be between 1 and 5.");
        if (overallExperience < 1 || overallExperience > 5)
            return Result.Failure<Rating>("Overall experience rating must be between 1 and 5.");

        return Result.Success(new Rating(foodQuality, deliverySpeed, valueForMoney, accuracy, overallExperience));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return FoodQuality;
        yield return DeliverySpeed;
        yield return ValueForMoney;
        yield return Accuracy;
        yield return OverallExperience;
    }
}
