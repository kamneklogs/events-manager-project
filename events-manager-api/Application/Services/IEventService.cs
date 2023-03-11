using events_manager_api.Domain.Entities;
using events_manager_api.Application.Dtos;

namespace events_manager_api.Application.Services;
public interface IEventService
{

    /// <summary>
    ///  Creates a new event using the provided <see cref="EventCreationDto" /> object.
    /// </summary>
    /// <param name="eventEntity">The <see cref="EventCreationDto" /> object to create.</param>
    /// <returns> The created <see cref="EventRetrievalDto" /> object.</returns>
    public Task<EventRetrievalDto> CreateEventAsync(EventCreationDto eventEntity);

    /// <summary>
    /// Returns an event by id using the provided <see cref="EventRetrievalDto" /> object.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A <see cref="EventRetrievalDto" /> object.</returns>
    /// <exception cref="WebApiException">Thrown when the event is not found with HTTP status code 404.</exception>
    public EventRetrievalDto GetEventById(int id);

    /// <summary>
    /// Returns all created events
    /// </summary>
    /// <returns>A collection of <see cref="EventRetrievalDto" /> objects.</returns>
    public Task<ICollection<EventRetrievalDto>> GetEventsAsync();
}