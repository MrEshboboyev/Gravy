using Gravy.Domain.Primitives;

namespace Gravy.Domain.ValueObjects.Restaurants;

/// <summary>
/// Represents nutritional information for a menu item.
/// </summary>
public sealed class NutritionalInfo : ValueObject
{
    public int Calories { get; }
    public double Protein { get; }
    public double Carbohydrates { get; }
    public double Fat { get; }

    private NutritionalInfo(
        int calories,
        double protein,
        double carbohydrates,
        double fat)
    {
        Calories = calories;
        Protein = protein;
        Carbohydrates = carbohydrates;
        Fat = fat;
    }

    public static NutritionalInfo Create(
        int calories,
        double protein,
        double carbohydrates,
        double fat)
    {
        return new NutritionalInfo(
            calories,
            protein,
            carbohydrates,
            fat);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Calories;
        yield return Protein;
        yield return Carbohydrates;
        yield return Fat;
    }
}