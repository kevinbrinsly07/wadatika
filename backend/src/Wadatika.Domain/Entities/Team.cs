using System.Data.Common;
using Microsoft.VisualBasic;

namespace Wadatika.Domain.Entities;

public sealed class Team
{
     private Team()
     {

     }

     public Team(Guid organizationId, string name)
     {
          if (organizationId == Guid.Empty)
          {
               throw new ArgumentException(
                    "Organization ID is required. ",
                    nameof(organizationId));
          }

          Id = Guid.NewGuid();
          OrganizationId = organizationId;
          Name = ValidateName(name);
          CreatedAt = DateTimeOffset.UtcNow;
     }

     public Guid Id { get; private set; }
     public Guid OrganizationId { get; private set; }
     public string Name { get; private set; } = string.Empty;
     public DateTimeOffset CreatedAt { get; private set; }
     public DateTimeOffset? UpdateAt { get; private set; }
     public void Rename(string newName)
     {
          Name = ValidateName(newName);

          UpdateAt = DateTimeOffset.UtcNow;
     }

     private static string ValidateName(string name)
     {
          if (string.IsNullOrWhiteSpace(name))
          {
               throw new ArgumentException(
                    "Team name is required. ",
                    nameof(name));
          }

          var normalizedName = name.Trim();

          if (normalizedName.Length > 100)
          {
               throw new ArgumentException(
                    "team name cannot exceed 100 characters.",
                    nameof(name));
          }

          return normalizedName;
     }



}