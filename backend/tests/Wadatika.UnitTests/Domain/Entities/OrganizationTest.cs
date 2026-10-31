using Wadatika.Domain.Entities;

namespace Wadatika.UnitTests.Domain.Entities;

public class OrganizationTests
{
     [Fact]
     public void Constructor_ShouldCreateOrganization_WhenDataIsValid()
     {

          //Arrage
          var name = "ABC Technologies ";
          var slug = "ABC-Technologies ";

          //Act
          var organization = new Organization(name, slug);

          //Assert
          Assert.NotEqual(Guid.Empty, organization.Id);
          Assert.Equal("ABC Technologies", organization.Name);
          Assert.Equal("abc-technologies", organization.Slug);
          Assert.NotEqual(default, organization.CreatedAt);
          Assert.Null(organization.UpdatedAt);
     }

     [Fact]
     public void Constructor_ShouldThrowException_WhenNameIsEmpty()
     {
          //Arrange
          var slug = "abc-technologies";

          // Act
          var exception = Assert.Throws<ArgumentException>(
              () => new Organization("", slug));

          // Assert
          Assert.Contains("name is required", exception.Message);
     }

     [Fact]
     public void Constructor_ShouldThrowException_WhenSlugIsInvalid()
     {
          // Arrange
          var name = "ABC Technologies";
          var slug = "ABC Technologies!";

          // Act & Assert
          Assert.Throws<ArgumentException>(
              () => new Organization(name, slug));
     }

     [Fact]
     public void Rename_ShouldUpdateOrganizationName()
     {
          // Arrange
          var organization = new Organization(
              "ABC Technologies",
              "abc-technologies");

          // Act
          organization.Rename("XYZ Solutions");

          // Assert
          Assert.Equal("XYZ Solutions", organization.Name);
          Assert.NotNull(organization.UpdatedAt);
     }

     [Fact]
     public void Rename_ShouldThrowException_WhenNameIsEmpty()
     {
          // Arrange
          var organization = new Organization(
              "ABC Technologies",
              "abc-technologies");

          // Act & Assert
          Assert.Throws<ArgumentException>(
              () => organization.Rename(" "));
     }
}