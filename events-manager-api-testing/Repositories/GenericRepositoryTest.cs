using events_manager_api.Domain.Entities;
using events_manager_api.Domain.Repositories;
using events_manager_api.Domain.Repositories.Impl;
using events_manager_api_testing.Util;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace events_manager_api_testing.Repositories;

[TestClass]
public class GenericRepositoryTest
{
    private IRepository<DeveloperEntity> _repository = default!;
    private ApplicationDbContext _dbContext = default!;
    private readonly IReadOnlyList<DeveloperEntity> _fakeDeveloper = FakeDataHelper.GenerateFakeDeveloperEntity().Generate(10);

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "EventsSystem")
            .Options;

        _dbContext = new ApplicationDbContext(options);

        _dbContext.Developers.AddRange(_fakeDeveloper);
        _dbContext.SaveChanges();

        _repository = new Repository<DeveloperEntity>(_dbContext);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task GetAllAsyncShouldReturnAllDevelopers()
    {
        // Arrange
        var expectedDevelopers = _fakeDeveloper;

        // Act
        var actualDevelopers = await _repository.GetAllAsync();

        // Assert
        actualDevelopers.Should().BeEquivalentTo(expectedDevelopers);
    }

    [TestMethod]
    public void GetShouldReturnDeveloper()
    {
        // Arrange
        var expectedDeveloper = _fakeDeveloper.First();

        // Act
        var actualDeveloper = _repository.Get(expectedDeveloper.Email);

        // Assert
        actualDeveloper.Should().BeEquivalentTo(expectedDeveloper);
    }

    [TestMethod]
    public void GetShouldReturnNullWhenTheEmailIsNotInSaved()
    {
        // Act
        var actualDeveloper = _repository.Get("NOT_A_REAL_EMAIL");

        // Assert
        actualDeveloper.Should().BeNull();
    }

    [TestMethod]
    public void FindWhereShouldReturnDeveloper()
    {
        // Arrange
        var expectedDeveloper = _fakeDeveloper.First();

        // Act
        var actualDeveloper = _repository.FindWhere(x => x.Email == expectedDeveloper.Email).First();

        // Assert
        actualDeveloper.Should().BeEquivalentTo(expectedDeveloper);
    }

    [TestMethod]
    public void FindWhereShouldReturnEmptyListWhenADeveloperWithAnAttributelIsNotInSaved()
    {
        // Act
        var actualDeveloper = _repository.FindWhere(x => x.Name == "NOT_A_REAL_NAME");

        // Assert
        actualDeveloper.Should().BeEmpty();
    }

    [TestMethod]
    public void AddShouldAddDeveloper()
    {
        // Arrange
        var developer = FakeDataHelper.GenerateFakeDeveloperEntity().Generate();
        developer.Email = "adssadasddnow" + developer.Email; // Salt to avoid duplicate key

        // Act
        _repository.Add(developer);
        _dbContext.SaveChanges();

        // Assert
        _repository.Get(developer.Email).Should().BeEquivalentTo(developer);
    }

    [TestMethod]
    public void AddShouldNotAddDeveloperWhenTheEmailIsAlreadyInSaved()
    {
        // Arrange
        var developer = _fakeDeveloper.First();

        // Act
        _repository.Add(developer);

        // Assert
        _dbContext.Invoking(x => x.SaveChanges()).Should().Throw<ArgumentException>()
        .WithMessage("An item with the same key has already been added. Key: " + developer.Email);
    }

    [TestMethod]
    public void UpdateShouldUpdateDeveloper()
    {
        // Arrange
        var developer = _fakeDeveloper.First();
        developer.Name = "New Name";

        // Act
        _repository.Update(developer);
        _dbContext.SaveChanges();

        // Assert
        _repository.Get(developer.Email).Should().BeEquivalentTo(developer);
    }
}