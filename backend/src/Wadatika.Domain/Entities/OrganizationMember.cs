using System.Data.Common;
using Wadatika.Domain.Enums;

namespace Wadatika.Domain.Entities;

public sealed class OrganizationMember
{
     private OrganizationMember()
     {

     }

     public OrganizationMember(
          Guid organizationId,
          Guid userId,
          OrganizationRole role)
     {
          if (organizationId == Guid.Empty)
          {
               throw new ArgumentException(
                    "Organization ID is required. ",
                    nameof(organizationId));
          }

          if (userId == Guid.Empty)
          {
               throw new ArgumentException(
                    "Organization ID is required. ",
                    nameof(organizationId));
          }

          if (userId == Guid.Empty)
          {
               throw new ArgumentException(
                    "User ID is required. ",
                    nameof(organizationId));
          }

          if (!Enum.IsDefined(role))
          {
               throw new ArgumentOutOfRangeException(nameof(role));

          }

          Id = Guid.NewGuid();
          OrganizationId = organizationId;
          UserId = userId;
          Role = role;
          JoinedAt = DateTimeOffset.UtcNow;
     }

     public Guid Id { get; private set; }
     public Guid OrganizationId { get; private set; }
     public Guid UserId { get; private set; }
     public OrganizationRole Role { get; private set; }
     public DateTimeOffset JoinedAt { get; private set; }

     public void ChangeRole(OrganizationRole newRole)
     {
          if (!Enum.IsDefined(newRole))
          {
               throw new ArgumentOutOfRangeException(nameof(newRole));
          }

          Role = newRole;
     }

}