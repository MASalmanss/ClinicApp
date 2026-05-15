using ClinicApp.Domain.Exceptions;
using ClinicApp.Domain.ValueObjects;
using FluentAssertions;

namespace ClinicApp.UnitTests.Domain;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("mehmet@test.org")]
    [InlineData("a@b.co")]
    public void Create_ValidEmail_ReturnsEmail(string value)
    {
        var email = Email.Create(value);

        email.Value.Should().Be(value.ToLowerInvariant());
    }

    [Fact]
    public void Create_UppercaseEmail_NormalizesToLowercase()
    {
        var email = Email.Create("MEHMET@TEST.COM");

        email.Value.Should().Be("mehmet@test.com");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("notanemail")]
    [InlineData("@nodomain.com")]
    [InlineData("noatsign.com")]
    public void Create_InvalidEmail_ThrowsDomainException(string value)
    {
        Action act = () => Email.Create(value);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_TooLongEmail_ThrowsDomainException()
    {
        var longEmail = new string('a', 295) + "@b.com"; // 301 karakter

        Action act = () => Email.Create(longEmail);

        act.Should().Throw<DomainException>()
           .WithMessage("*300*");
    }

    [Fact]
    public void FromDatabase_InvalidEmail_ThrowsDataCorruptionException()
    {
        Action act = () => Email.FromDatabase("bozukdeger");

        act.Should().Throw<DataCorruptionException>();
    }

    [Fact]
    public void TwoEmailsWithSameValue_AreEqual()
    {
        var email1 = Email.Create("test@test.com");
        var email2 = Email.Create("test@test.com");

        email1.Should().Be(email2);
    }
}
