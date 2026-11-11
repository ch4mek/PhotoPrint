using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Booking;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Booking.DataAccess.Npgsql.Seeds;

internal class OfficeSeed : ISeedEntitiesProvider<BookingDbContext>
{
    private readonly BookingDbContext _dbContext;

    public OfficeSeed(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        var offices = new[]
        {
            new
            {
                Name = "Minsk",
                Location = LocationType.Minsk,
                TimeZone = "Europe/Minsk",
                TimeZoneDisplayName = "Minsk",
                TotalWorkplacesWithoutSpace = 0,
                Workspaces = new []
                {
                    (Name: "Space 403", InternalCode: "MPN186-S403", TotalWorkplaces: 55, IsAvailableForBooking: true),
                    (Name: "Space 404", InternalCode: "MPN186-S404", TotalWorkplaces: 22, IsAvailableForBooking: true),
                    (Name: "Space 405", InternalCode: "MPN186-S405", TotalWorkplaces: 18, IsAvailableForBooking: true),
                    (Name: "Space 406", InternalCode: "MPN186-S406", TotalWorkplaces: 15, IsAvailableForBooking: true),
                    (Name: "Space 411", InternalCode: "MPN186-S411", TotalWorkplaces: 15, IsAvailableForBooking: true),
                    (Name: "Space 207", InternalCode: "MPN186-S207", TotalWorkplaces: 66, IsAvailableForBooking: true),
                },
                MeetingRooms = new []
                {
                    (
                        Name: "401",
                        CountOfSeats: 8,
                        IsActive: true,
                        Equipments: []
                    ),
                    (
                        Name: "403.1",
                        CountOfSeats: 2,
                        IsActive: true,
                        Equipments:
                        [
                            (Name: nameof(MeetingRoomItemType.Board), Type: MeetingRoomItemType.Board, Count: 1)
                        ]
                    ),
                    (
                        Name: "403.2",
                        CountOfSeats: 7,
                        IsActive: true,
                        Equipments:
                        [
                             (Name: nameof(MeetingRoomItemType.Board), Type: MeetingRoomItemType.Board, Count: 1),
                             (Name: nameof(MeetingRoomItemType.UsbHub), Type: MeetingRoomItemType.UsbHub, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Laptop), Type: MeetingRoomItemType.Laptop, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Screen), Type: MeetingRoomItemType.Screen, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Speakerphone), Type: MeetingRoomItemType.Speakerphone, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Camera), Type: MeetingRoomItemType.Camera, Count: 1)
                        ]
                    ),
                    (
                        Name: "407",
                        CountOfSeats: 5,
                        IsActive: true,
                        Equipments:
                        [
                            (Name: nameof(MeetingRoomItemType.Board), Type: MeetingRoomItemType.Board, Count: 2)
                        ]
                    ),
                    (
                        Name: "410",
                        CountOfSeats: 4,
                        IsActive: true,
                        Equipments: []
                    ),
                    (
                        Name: "Meeting room 5",
                        CountOfSeats: 5,
                        IsActive: false,
                        Equipments: []
                    ),
                    (
                        Name: "Meeting room 6",
                        CountOfSeats: 5,
                        IsActive: false,
                        Equipments: Array.Empty<(string Name, MeetingRoomItemType Type, int Count)>()
                    ),
                    (
                        Name: "203",
                        CountOfSeats: 0,
                        IsActive: true,
                        Equipments: []
                    ),
                }
            },
            new
            {
                Name = "Vitebsk",
                Location = LocationType.Vitebsk,
                TimeZone = "Europe/Minsk",
                TimeZoneDisplayName = "Minsk",
                TotalWorkplacesWithoutSpace = 0,
                Workspaces = new []
                {
                    (Name: "Space 29", InternalCode: "VBH30-S29", TotalWorkplaces: 21, IsAvailableForBooking: true),
                    (Name: "Space 30", InternalCode: "VBH30-S30", TotalWorkplaces: 18, IsAvailableForBooking: true),
                    (Name: "Space 48", InternalCode: "VBH30-S48", TotalWorkplaces: 21, IsAvailableForBooking: true),
                },
                MeetingRooms = new []
                {
                    (
                        Name: "25",
                        CountOfSeats: 4,
                        IsActive: true,
                        Equipments: []
                    ),
                    (
                        Name: "26",
                        CountOfSeats: 2,
                        IsActive: true,
                        Equipments: []
                    ),
                    (
                        Name: "44",
                        CountOfSeats: 6,
                        IsActive: true,
                        Equipments:
                        [
                            (Name: nameof(MeetingRoomItemType.Board), Type: MeetingRoomItemType.Board, Count: 1),
                             (Name: nameof(MeetingRoomItemType.UsbHub), Type: MeetingRoomItemType.UsbHub, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Speakerphone), Type: MeetingRoomItemType.Speakerphone, Count: 1)
                        ]
                    ),
                    (
                        Name: "45",
                        CountOfSeats: 6,
                        IsActive: true,
                        Equipments: new[]
                        {
                             (Name: nameof(MeetingRoomItemType.Board), Type: MeetingRoomItemType.Board, Count: 1),
                             (Name: nameof(MeetingRoomItemType.Speakers), Type: MeetingRoomItemType.Speakers, Count: 1),
                        }
                    ),
                }
            },
            new
            {
                Name = "Warsaw",
                Location = LocationType.Warsaw,
                TimeZone = "Europe/Warsaw",
                TimeZoneDisplayName = "Warsaw",
                TotalWorkplacesWithoutSpace = 9,
                Workspaces = Array.Empty<(string Name, string InternalCode, int TotalWorkplaces, bool IsAvailableForBooking)>(),
                MeetingRooms = Array.Empty<(string Name, int CountOfSeats, bool IsActive, (string Name, MeetingRoomItemType Type, int Count)[] Equipments)>()
            }
        };

        foreach (var officeDto in offices)
        {
            var office = UpsertOffice(officeDto.Name, officeDto.Location, officeDto.TimeZone, officeDto.TimeZoneDisplayName);
            for (var i = 1; i <= officeDto.TotalWorkplacesWithoutSpace; i++)
            {
                var workplaceNumber = i;
                var workplace = office.Workplaces
                    .Where(x => x.WorkspaceId == null)
                    .FirstOrDefault(x => x.WorkplaceNumber == workplaceNumber);
                if (workplace == null)
                {
                    office.Workplaces.Add(new Workplace
                    {
                        WorkplaceNumber = workplaceNumber,
                        WorkspaceId = null,
                        Office = office
                    });
                }
            }

            foreach (var workspaceDto in officeDto.Workspaces)
            {
                var workspace = office.Workspaces.FirstOrDefault(x => x.Name == workspaceDto.Name);
                if (workspace == null)
                {
                    workspace = new Workspace
                    {
                        Name = workspaceDto.Name,
                        Office = office,
                        InternalCode = workspaceDto.InternalCode,
                        IsAvailableForBooking = workspaceDto.IsAvailableForBooking,
                        Workplaces = new List<Workplace>()
                    };
                    office.Workspaces.Add(workspace);
                }

                workspace.InternalCode = workspaceDto.InternalCode;

                for (var i = 1; i <= workspaceDto.TotalWorkplaces; i++)
                {
                    var workplaceNumber = i;
                    var workplace = workspace.Workplaces.FirstOrDefault(x => x.WorkplaceNumber == workplaceNumber);
                    if (workplace == null)
                    {
                        workspace.Workplaces.Add(new Workplace
                        {
                            WorkplaceNumber = workplaceNumber,
                            WorkspaceId = null,
                            Office = office
                        });
                    }
                }
            }

            foreach (var meetingRoomDto in officeDto.MeetingRooms)
            {
                var meetingRoom = office.MeetingRooms.FirstOrDefault(x => x.Name == meetingRoomDto.Name);
                if (meetingRoom == null)
                {
                    meetingRoom = new MeetingRoomEntity
                    {
                        Office = office,
                        Name = meetingRoomDto.Name,
                        IsDeleted = false,

                    };
                    office.MeetingRooms.Add(meetingRoom);
                }

                foreach (var item in meetingRoomDto.Equipments)
                {
                    var existedItem = meetingRoom.Items.FirstOrDefault(i => i.Name == item.Name);
                    if (existedItem == null)
                    {
                        meetingRoom.Items.Add(new MeetingRoomItemEntity
                        {
                            Count = item.Count,
                            ItemType = item.Type,
                            Name = item.Name,
                        });
                    }
                }

                if (!meetingRoomDto.IsActive)
                {
                    var entriesToDelete = _dbContext.MeetingRoomEntries
                        .Include(mr => mr.MeetingRoomReservations.Where(r => !r.IsDeleted))
                        .Where(m => m.MeetingRoomId == meetingRoom.Id)
                        .Where(x => !x.IsDeleted)
                        .ToList();

                    entriesToDelete.ForEach(entry =>
                    {
                        entry.IsDeleted = true;
                        entry.UpdatedAtUtc = DateTime.UtcNow;
                        entry.UpdatedById = entry.CreatedById;

                        entry.MeetingRoomReservations.ToList().ForEach(reservation =>
                        {
                            reservation.IsDeleted = true;
                            reservation.UpdatedAtUtc = DateTime.UtcNow;
                            reservation.UpdatedById = reservation.CreatedById;
                        });

                    });
                }

                meetingRoom.IsActive = meetingRoomDto.IsActive;
                meetingRoom.CountOfSeats = meetingRoomDto.CountOfSeats;
            }
        }

        _dbContext.SaveChanges();
    }

    private OfficeEntity UpsertOffice(string name, LocationType type, string timeZone, string timeZoneName)
    {
        var office = _dbContext.Offices
            .AsSplitQuery()
            .Include(x => x.Workspaces)
            .Include(x => x.Workplaces)
            .Include(x => x.MeetingRooms)
            .ThenInclude(x => x.Items)
            .FirstOrDefault(x => x.Name == name);
        if (office == null)
        {
            office = new OfficeEntity
            {
                Name = name,
                MeetingRooms = new List<MeetingRoomEntity>(),
                Workplaces = new List<Workplace>(),
                Workspaces = new List<Workspace>()
            };

            _dbContext.Offices.Add(office);
        }

        office.TimeZone = timeZone;
        office.TimeZoneDisplayName = timeZoneName;
        office.LocationType = type;

        return office;
    }
}
