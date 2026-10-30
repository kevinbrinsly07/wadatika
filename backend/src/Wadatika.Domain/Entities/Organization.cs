using System.Data.Common;

namespace Wadatika.Domain.Entities;

public sealed class Organization
{
     private Organization()
     {

     }

     public Organization(string name, string slug)
     {
          Id = Guid.NewGuid();

          Name = ValidateName(name);

          Slug = ValidateSlug(slug);

          CreatedAt = DateTimeOffset.UtcNow;
     }

     public Guid Id { get; private set; }

     public string Name { get; private set; } = string.Empty;

     public string Slug { get; private set; } = string.Empty;

     public DateTimeOffset CreatedAt { get; private set; }

     public DateTimeOffset? UpdatedAt { get; private set; }

     public void Rename(string newName)
     {
          Name = ValidateName(newName);

          UpdatedAt = DateTimeOffset.UtcNow;
     }

     private static string ValidateName(String name)
     {
          if (string.IsNullOrWhiteSpace(name))
          {
               throw new ArgumentException(
                    "Organize name is required",
                    nameof(name));
          }

          var normalizedName = name.Trim();

          if (normalizedName.Length > 100)
          {
               throw new ArgumentException(
                    "Organization name cannot exceed 100 characters.",
                    nameof(name));
          }

          return normalizedName;

     }

     private static string ValidateSlug(string slug)
     {
          if (string.IsNullOrWhiteSpace(slug))
          {
               throw new ArgumentException(
                    "Organization slug is required.",
                    nameof(slug));
          }

          var normalizedSlug = slug.Trim().ToLowerInvariant();

          if (normalizedSlug.Length > 100 ||
          !System.Text.RegularExpressions.Regex.IsMatch(
               normalizedSlug,
               @"^[a-z0-9]+(?:-[a-z0-9]+)*$"))

          {

               throw new ArgumentException(
                    "Organization slug must contain lowercase letters, numbers and singel hyphens.",
                    nameof(slug));

          }

          return normalizedSlug;
     }
}