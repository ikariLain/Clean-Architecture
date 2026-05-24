namespace MyApp.Domain.Tests;

using Xunit;
using MyApp.Domain.ValueObjects;

/// <summary>
/// Unit Tests för Address Value Object
///
/// Visar att även värdeobjekt kan testas enkelt och snabbt
/// </summary>
public class AddressTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange & Act
        var address = Address.Create(
            "Main Street 123",
            "Stockholm",
            "Stockholm",
            "10100",
            "Sweden");

        // Assert
        Assert.NotNull(address);
        Assert.Equal("Main Street 123", address.Street);
        Assert.Equal("Stockholm", address.City);
    }

    [Fact]
    public void Create_WithEmptyStreet_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create("", "Stockholm", "Stockholm", "10100", "Sweden"));
    }

    [Fact]
    public void Create_WithEmptyPostalCode_ShouldThrow()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() =>
            Address.Create("Main Street 123", "Stockholm", "Stockholm", "", "Sweden"));
    }

    [Fact]
    public void TwoAddressesWithSameValues_ShouldBeEqual()
    {
        // Arrange
        var address1 = Address.Create("Main St", "Stockholm", "Stockholm", "10100", "Sweden");
        var address2 = Address.Create("Main St", "Stockholm", "Stockholm", "10100", "Sweden");

        // Act & Assert
        Assert.Equal(address1, address2);
        Assert.True(address1 == address2);
    }

    [Fact]
    public void TwoAddressesWithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var address1 = Address.Create("Main St", "Stockholm", "Stockholm", "10100", "Sweden");
        var address2 = Address.Create("Side St", "Stockholm", "Stockholm", "10100", "Sweden");

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }
}
