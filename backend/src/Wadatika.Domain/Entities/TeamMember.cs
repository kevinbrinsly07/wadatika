using Wadatika.Domain.Enums;

namespace Wadatika.Domain.Entities;

public sealed class TeamMember
{
     private TeamMember()
     {

     }

     public TeamMember(

          Guid teamId,
          Guid userId,
          TeamRole role
     )
     {
          if (teamId == Guid.Empty)
          {
               throw new ArgumentException(
                    "Team Id is rquired",
                    nameof(teamId));
          }

          if (userId == Guid.Empty)
          {
               throw new ArgumentException(
                    "User Id is rquired",
                    nameof(userId));
          }

          if (!Enum.IsDefined(role))
          {
               throw new ArgumentOutOfRangeException(nameof(role));
          }

          Id = Guid.NewGuid();
          TeamId = teamId;
          UserId = userId;
          Role = role;
          JoinedAt = DateTimeOffset.UtcNow;

     }

     public Guid Id { get; private set; }
     public Guid TeamId { get; private set; }
     public Guid UserId { get; private set; }
     public TeamRole Role { get; private set; }
     public DateTimeOffset JoinedAt { get; private set; }




}