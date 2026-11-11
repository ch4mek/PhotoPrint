using System.Net.Http.Json;
using Modsen.Portal.Booking.Controllers.Contracts.Input;
using Modsen.Portal.Booking.Controllers.Contracts.Output;
using Modsen.Portal.Booking.Requests.Dto;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.Booking.Contracts;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;

internal static class HttpClientBookingExtensions
{
    private static readonly CancellationToken _cancellationToken = CancellationToken.None;

    public static Task<List<GetReservationForDate>?> GetReservation(
        this HttpClient client,
        DateTime? fromDate,
        DateTime? toDate,
        int? officeId)
    {
        return client.GetFromJsonAsync<List<GetReservationForDate>>(
                $"api/Reservation?FromDate={fromDate:O}&ToDate={toDate:O}&officeId={officeId}",
                _cancellationToken);
    }

    public static Task<List<GetReservationForDate>?> GetReservation(
        this HttpClient client,
        string? fromDate,
        string? toDate,
        int? officeId)
    {
        return client.GetFromJsonAsync<List<GetReservationForDate>>(
                $"api/Reservation?FromDate={fromDate}&ToDate={toDate}&officeId={officeId}",
                _cancellationToken);
    }

    public static Task<List<DateTime>?> GetCollisions(
        this HttpClient client,
        int? userId = null,
        int? workplaceId = null,
        int? officeId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var userIdSection = userId is null ? "" : $"userId={userId.Value}&";
        return client.GetFromJsonAsync<List<DateTime>>(
                $"api/Collisions?{userIdSection}WorkplaceId={workplaceId}&OfficeId={officeId}&FromDate={dateFrom:yyyy-MM-ddZ}&ToDate={dateTo:yyyy-MM-ddZ}",
                _cancellationToken);
    }

    public static async Task<BookingAndMeetingRoomsDto?> GetBookings(this HttpClient client,
        int? userId = null,
        int? officeId = null,
        int? workspaceId = null,
        bool includeEmployee = false)
    {
        var bookings = await client.GetFromJsonAsync<BookingAndMeetingRoomsDto>(
            $"api/Booking?OfficeId={officeId}&WorkspaceId={workspaceId}&UserId={userId}&IncludeEmployee={includeEmployee}",
            _cancellationToken);

        return bookings;
    }

    public static async Task<int> CreateSelfBooking(this HttpClient client,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int weeksInInterval = 0)
    {
        var createSelfBookingInput = new CreateSelfBookingTestInput
        {
            RequestType = CreateBookingBaseInput.CreateSelf,
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId
        };

        var selfBooking = await client.PostAsJsonAsync<CreateSelfBookingTestInput, BookingOutput>(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        if (selfBooking is null)
        {
            throw new ApplicationException("Booking is not created");
        }

        return selfBooking.BookingId;
    }

    public static async Task<BookingOutput> CreateSelfBooking(this HttpClient client, CreateSelfBookingTestInput createSelfBookingInput)
    {
        var selfBooking = await client.PostAsJsonAsync<CreateSelfBookingTestInput, BookingOutput>(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        return selfBooking;
    }

    public static async Task<HttpResponseMessage> CreateSelfBookingAsHttpMessage(this HttpClient client,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int weeksInInterval = 0)
    {
        var createSelfBookingInput = new CreateSelfBookingTestInput
        {
            RequestType = CreateBookingBaseInput.CreateSelf,
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId
        };

        var httpResponse = await client.PostAsJsonAsync(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        return httpResponse;
    }

    public static async Task<HttpResponseMessage> CreateSelfBookingAsHttpMessage(this HttpClient client,
        CreateSelfBookingTestInput createSelfBookingInput)
    {
        var httpResponse = await client.PostAsJsonAsync(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        return httpResponse;
    }

    public static async Task<int> CreateBlockBooking(this HttpClient client,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int? userId = null,
        int weeksInInterval = 0)
    {
        var createSelfBookingInput = new CreateBlockTestInput
        {
            RequestType = CreateBookingBaseInput.CreateBlock,
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId,
            UserId = userId
        };

        var blockBooking = await client.PostAsJsonAsync<CreateBlockTestInput, BookingOutput>(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        if (blockBooking is null)
        {
            throw new ApplicationException("Booking is not created");
        }

        return blockBooking.BookingId;
    }

    public static async Task<HttpResponseMessage> CreateBlockBookingAsHttpMessage(this HttpClient client,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int? userId = null,
        int weeksInInterval = 0)
    {
        var createSelfBookingInput = new CreateBlockTestInput
        {
            RequestType = CreateBookingBaseInput.CreateBlock,
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId,
            UserId = userId
        };

        var httpResponse = await client.PostAsJsonAsync(
            "api/Booking",
            createSelfBookingInput,
            _cancellationToken);

        return httpResponse;
    }

    public static async Task<int> CreateUserBooking(this HttpClient client,
        int userId,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int weeksInInterval = 0)
    {
        var createUserBookingInput = new CreateBookingBaseInput
        {
            UserId = userId,
            RequestType = "CreateForUser",
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId
        };

        var userBooking = await client.PostAsJsonAsync<CreateBookingBaseInput, BookingOutput>("api/Booking",
            createUserBookingInput,
            _cancellationToken);

        if (userBooking is null)
        {
            throw new ApplicationException("Booking is not created");
        }

        return userBooking.BookingId;
    }

    public static async Task<HttpResponseMessage> CreateUserBookingAsHttpMessage(this HttpClient client,
        int userId,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        int weeksInInterval = 0)
    {
        var createUserBookingInput = new CreateBookingBaseInput
        {
            UserId = userId,
            RequestType = "CreateForUser",
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId
        };

        var httpResponse = await client.PostAsJsonAsync("api/Booking", createUserBookingInput,
            _cancellationToken);

        return httpResponse;
    }

    public static async Task<HttpResponseMessage> CreateGuestBookingAsHttpMessage(this HttpClient client,
        int workplaceId,
        DateTime fromDate,
        DateTime toDate,
        string? guestName = null,
        int weeksInInterval = 0)
    {
        var createUserBookingInput = new CreateBookingBaseInput
        {
            GuestName = guestName ?? Guid.CreateVersion7().ToAlphabetic(),
            RequestType = "CreateForGuest",
            FromDate = fromDate,
            ToDate = toDate,
            WeeksInInterval = weeksInInterval,
            WorkplaceId = workplaceId
        };

        var httpResponse = await client.PostAsJsonAsync("api/Booking",
            createUserBookingInput,
            _cancellationToken);

        return httpResponse;
    }

    public static async Task DeleteBookings(this HttpClient client, int[] ids)
    {
        foreach (var id in ids)
        {
            await client.DeleteAsync(
                $"api/booking/{id}",
                _cancellationToken);
        }
    }

    public static async Task<HttpResponseMessage> DeleteBookingAsHttpMessage(this HttpClient client, int id)
    {
        var response = await client.DeleteAsync(
            $"api/booking/{id}",
            _cancellationToken);

        return response;
    }
}
