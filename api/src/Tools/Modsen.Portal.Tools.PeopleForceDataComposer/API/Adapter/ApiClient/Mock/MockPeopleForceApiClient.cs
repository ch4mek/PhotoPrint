using System.Text.Json;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.Contract;
using Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.ApiClient.Mock;

internal sealed class MockPeopleForceApiClient : IPeopleForceApiClient
{
    public async Task<ListOfEmployeesApiModel> GetEmployeesAsync(int page, string status, CancellationToken cancellationToken)
    {
        return new ListOfEmployeesApiModel
        {
            Metadata = new MetadataApiModel { Pagination = new PaginationApiModel { Pages = 2 } },
            Employees = [
                new EmployeeApiModel { Email = "TestEmployee1@mail.com", Id = 1},
                new EmployeeApiModel { Email = "TestEmployee2@mail.com", Id = 2}]
        };

    }

    public async Task<byte[]> GetFileByUriAsync(Uri uri, CancellationToken cancellationToken)
    {
        return "This is a test file content."u8.ToArray();
    }

    public async Task<JsonDocument> GetByUriAsync(string uri, int? page, CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken);

        var _mockData = "{}";

        if (uri.Contains("employees/birthdays"))
        {
            if (page == 1)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 1, ""name"": ""John Smith"", ""active"": true },
                        { ""id"": 2, ""name"": ""Jane Doe"", ""active"": false }
                    ],
                    ""metadata"": { ""page"": 1, ""pages"": 2 }
                }";
            }
            else if (page == 2)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 3, ""name"": ""Alice Johnson"", ""active"": true }
                    ],
                    ""metadata"": { ""page"": 2, ""pages"": 2 }
                }";
            }
        }
        if (uri.Contains("/recruitment/vacancies"))
        {
            if (page == 1)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 1, ""name"": ""John Smith"", ""active"": true },
                        { ""id"": 2, ""name"": ""Jane Doe"", ""active"": false }
                    ],
                    ""metadata"": { ""page"": 1, ""pages"": 2 }
                }";
            }
            else if (page == 2)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 3, ""name"": ""Alice Johnson"", ""active"": true }
                    ],
                    ""metadata"": { ""page"": 2, ""pages"": 2 }
                }";
            }
        }
        else if (uri.Contains("recruitment/vacancy_fields")) // no pagination
        {
            _mockData = @"
            {
                ""data"": [
                    { ""id"": 1, ""type"": ""Vacation"", ""days"": 5 },
                    { ""id"": 2, ""type"": ""Sick Leave"", ""days"": 3 }
                ],
            }";
        }
        else if (uri.Contains("tasks"))
        {
            if (page == 1)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 1, ""title"": ""Task A"", ""completed"": false },
                        { ""id"": 2, ""title"": ""Task B"", ""completed"": true }
                    ],
                    ""metadata"": { ""page"": 1, ""pages"": 2 }
                }";
            }
            else if (page == 2)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 3, ""title"": ""Task C"", ""completed"": false }
                    ],
                    ""metadata"": { ""page"": 2, ""pages"": 2 }
                }";
            }
        }
        else if (uri == "/api/public/v2/leave_requests")
        {
            if (page == 1)
            {
                _mockData = @"
        {
            ""data"": [
                {
                    ""id"": 1, 
                    ""title"": ""Task A"", 
                    ""completed"": false,
                    ""attachments"": [
                        {
                            ""filename"": ""fileA.txt"",
                            ""url"": ""http://example.com/fileA"",
                            ""expires_at"": ""2026-02-16T09:31:44.100Z""
                        }
                    ]
                },
                {
                    ""id"": 2, 
                    ""title"": ""Task B"", 
                    ""completed"": true,
                    ""attachments"": [
                        {
                            ""filename"": ""fileB.txt"",
                            ""url"": ""http://example.com/fileB"",
                            ""expires_at"": ""2026-02-16T09:31:44.100Z""
                        }
                    ]
                }
            ],
            ""metadata"": { ""page"": 1, ""pages"": 2 }
        }";
            }
            else if (page == 2)
            {
                _mockData = @"
        {
            ""data"": [
                {
                    ""id"": 3, 
                    ""title"": ""Task C"", 
                    ""completed"": false,
                    ""attachments"": [
                        {
                            ""filename"": ""fileC.txt"",
                            ""url"": ""http://example.com/fileC"",
                            ""expires_at"": ""2026-02-16T09:31:44.100Z""
                        }
                    ]
                }
            ],
            ""metadata"": { ""page"": 2, ""pages"": 2 }
        }";
            }
        }
        else if (uri == "/api/public/v2/recruitment/candidates")
        {
            if (page == 1)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 1, ""name"": ""John Smith"", ""active"": true },
                        { ""id"": 2, ""name"": ""Jane Doe"", ""active"": false }
                    ],
                    ""metadata"": { ""page"": 1, ""pages"": 2 }
                }";
            }
            else if (page == 2)
            {
                _mockData = @"
                {
                    ""data"": [
                        { ""id"": 3, ""name"": ""Alice Johnson"", ""active"": true }
                    ],
                    ""metadata"": { ""page"": 2, ""pages"": 2 }
                }";
            }
        }
        else if (uri.Contains("/api/public/v3/recruitment/candidates/"))
        {
            _mockData = @"
        {
          ""data"": {
            ""id"": ""string"",
            ""full_name"": ""string"",
            ""email"": ""string"",
            ""phone_numbers"": [
              ""string""
            ],
            ""date_of_birth"": ""string"",
            ""position"": ""string"",
            ""urls"": [
              ""string""
            ],
            ""skills"": [
              ""string""
            ],
            ""location"": ""string"",
            ""created_at"": ""2026-02-18T07:32:07.701Z"",
            ""updated_at"": ""2026-02-18T07:32:07.701Z"",
            ""gender"": ""string"",
            ""source"": ""string"",
            ""level"": ""string"",
            ""desired_salary"": 0,
            ""currency_code"": ""string"",
            ""last_contacted_at"": ""string"",
            ""cover_letter"": ""string"",
            ""resume"": true,
            ""resume_url"": ""https://fakesite.com"",
            ""tags"": [
              ""string""
            ],
            ""languages"": [
              {
                ""id"": 0,
                ""code"": ""string"",
                ""level"": ""string""
              }
            ],
            ""created_by"": {
              ""id"": 0,
              ""first_name"": ""string"",
              ""last_name"": ""string"",
              ""email"": ""user@example.com""
            },
            ""applications"": [
              {
                ""id"": 0,
                ""disqualified_at"": ""2026-02-18T07:32:07.701Z"",
                ""vacancy"": {
                  ""id"": 0,
                  ""title"": ""string""
                },
                ""pipeline_state"": {
                  ""id"": 0,
                  ""name"": ""string""
                },
                ""disqualify_reason"": {
                  ""id"": 0,
                  ""name"": ""string""
                }
              }
            ],
            ""custom_fields"": [
              {
                ""candidate_field_id"": 0,
                ""name"": ""string"",
                ""internal_name"": ""string"",
                ""value"": [
                  null
                ]
              },
              {
                ""candidate_field_id"": 0,
                ""name"": ""string"",
                ""internal_name"": ""string"",
                ""value"": [
                  null
                ]
              }
            ]
          }
        }";
    }

        return JsonDocument.Parse(_mockData);
    }
}
