namespace MyApp.Domain.ValueObjects;

/// <summary>
/// Address - Ett värdeobjekt (Value Object) enligt DDD-principer
///
/// Värdeobjekt:
/// - Är oförändrerliga (Immutable)
/// - Har ingen egen identitet (två Address-objekt med samma värden är likvärdiga)
/// - Kan delas mellan entiteter utan risk
///
/// Skillnad mot Entiteter:
/// - Entiteter: Order, OrderItem (har unik identitet via ID)
/// - Värdeobjekt: Address (identitetslös, bara värdena spelar roll)
/// </summary>
public class Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State cannot be empty", nameof(state));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty", nameof(country));

        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>
    /// Factory-metod för att skapa en Address
    /// </summary>
    public static Address Create(string street, string city, string state, string postalCode, string country)
    {
        return new Address(street, city, state, postalCode, country);
    }

    /// <summary>
    /// Två värdeobjekt är lika om alla deras värden är lika
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Address other)
            return false;

        return Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, State, PostalCode, Country);
    }

    public static bool operator ==(Address? left, Address? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Address? left, Address? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"{Street}, {PostalCode} {City}, {State}, {Country}";
    }
}
