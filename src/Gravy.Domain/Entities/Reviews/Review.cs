using Gravy.Domain.Enums.Reviews;
using Gravy.Domain.Primitives;
using Gravy.Domain.ValueObjects.Reviews;

namespace Gravy.Domain.Entities.Reviews;

public sealed class Review : AggregateRoot, IAuditableEntity
{
    public Guid ReviewId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? RestaurantId { get; private set; }
    public Guid? DeliveryPersonId { get; private set; }
    public Guid OrderId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public ReviewStatus Status { get; private set; }
    public string Response { get; private set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ModifiedOnUtc { get; set; }

    private Review(
        Guid reviewId,
        Guid customerId,
        Guid? restaurantId,
        Guid? deliveryPersonId,
        Guid orderId,
        Rating rating,
        string comment,
        DateTime reviewDate,
        ReviewStatus status,
        string response)
    {
        ReviewId = reviewId;
        CustomerId = customerId;
        RestaurantId = restaurantId;
        DeliveryPersonId = deliveryPersonId;
        OrderId = orderId;
        Rating = rating;
        Comment = comment;
        ReviewDate = reviewDate;
        Status = status;
        Response = response;
    }

    public static Review Create(
        Guid reviewId,
        Guid customerId,
        Guid? restaurantId,
        Guid? deliveryPersonId,
        Guid orderId,
        Rating rating,
        string comment,
        ReviewStatus status,
        string response)
    {
        return new Review(
            reviewId,
            customerId,
            restaurantId,
            deliveryPersonId,
            orderId,
            rating,
            comment,
            DateTime.UtcNow,
            status,
            response);
    }

    public void UpdateStatus(ReviewStatus status)
    {
        Status = status;
        ModifiedOnUtc = DateTime.UtcNow;
    }

    public void AddResponse(string response)
    {
        Response = response;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
