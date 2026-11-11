namespace Modsen.Portal.Inquiries.Requests.Dto;

internal record InquiriesUserDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public int? RegisteredLocationId { get; set; }
    public string? AvatarFileId { get; set; }
    public string? Color { get; set; }
    public string? Position { get; set; }

    /// <summary>
    /// Property is used at the UI to define could the requesting (current) user see details of the specified user.
    /// This is used because sometimes manager from another department could still be required to approve the inquiry,
    /// but won't be able to view the actual user info
    /// </summary>
    public bool? CanUserInfoBeRetrievedByCurrentUser { get; set; }
}
