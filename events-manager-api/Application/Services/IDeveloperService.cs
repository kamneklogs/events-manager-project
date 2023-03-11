using events_manager_api.Application.Dtos;

namespace events_manager_api.Application.Services;

public interface IDeveloperService
{
    /// <summary>
    /// Creates a new developer using the provided <see cref="DeveloperDto"/> object.
    /// </summary>
    /// <param name="developer">The <see cref="DeveloperDto"/> object to create.</param>
    /// <returns> The created <see cref="DeveloperDto"/> object.</returns>
    public Task<DeveloperDto> CreateDeveloperAsync(DeveloperDto developer);

    /// <summary>
    /// Returns a developer by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns>A <see cref="DeveloperDto"/> object.</returns>
    public DeveloperDto GetDeveloperByEmail(string email);

    /// <summary>
    /// Returns all developers using the provided <see cref="DeveloperDto"/> object.
    /// </summary>
    /// <returns> A collection of <see cref="DeveloperDto"/> objects.</returns>
    public Task<ICollection<DeveloperDto>> GetDevelopersAsync();
}