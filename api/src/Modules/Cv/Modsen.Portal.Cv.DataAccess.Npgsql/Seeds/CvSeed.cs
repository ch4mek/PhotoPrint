using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.Seeds;

internal class CvSeed : ISeedEntitiesProvider<CvDbContext>
{
    private readonly CvDbContext _cvDbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public CvSeed(
        CvDbContext cvDbContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _cvDbContext = cvDbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        var createdAt = DateTime.UtcNow;

        foreach (var cvToCreate in GetCvs(createdAt))
        {
            if (!_cvDbContext.Cvs.Any(x => x.CvName == cvToCreate.CvName))
            {
                _cvDbContext.Cvs.Add(cvToCreate);
            }
        }

        _cvDbContext.SaveChanges();
    }

    private IEnumerable<CvEntity> GetCvs(DateTime createdAt)
    {
        var employees = _cvDbContext.CvUsers
            .Where(u =>
                u.Position == PortalRoleNames.Employee ||
                u.Position == PortalRoleNames.SystemAdministrator ||
                u.Position == PortalRoleNames.GroupManager)
            .Select(u => new { u.UserId, u.FirstName, u.LastName })
            .ToList();

        if (employees.Count == 0)
        {
            yield break;
        }

        var cvsCount = _cvDbContext.Cvs.Count();

        if (cvsCount >= 10)
        {
            yield break;
        }

        var clientIds = _cvDbContext.CvClients
            .Select(c => c.ClientId)
            .ToList();

        var projectIds = _cvDbContext.CvProjects
            .Select(c => c.ProjectId)
            .ToList();

        var random = Random.Shared;
        var cvData = new[]
        {
            new
            {
                Status = CvStatus.Draft,
                UserProjectPosition = "Software Developer",
                UserLocation = "Minsk",
                UserEducation = "BS in Computer Science, Belarusian State University, 2018-2022",
                UserSkills = "C#, .NET\n SQL\n JavaScript",
                UserAbout = "Experienced developer with a focus on backend systems.",
                Website = "https://www.modsen-software.com",
                IsCorporateDesign = true,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Backend Developer",
                        ExperienceInYears = 3,
                        CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.B2, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "Developer",
                        TeamSize = "5",
                        StartDate = new DateOnly(2023, 1, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Developed backend services for e-commerce platform.",
                        Responsibilities = "API development, database design",
                        Environment = "C#, .NET Core, PostgreSQL",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Technical Skills",
                        NameRu = "Технические навыки",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "C#",
                                    Level = CvSkillLevel.Professional,
                                    ExperienceInYears = 3,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Sent,
                UserProjectPosition = "Frontend Developer",
                UserLocation = "Vitebsk",
                UserEducation = "BS in Information Technology, Vitebsk State University, 2019-2023",
                UserSkills = "JavaScript\n React\n HTML, CSS",
                UserAbout = "Passionate about creating user-friendly interfaces.",
                Website = "https://www.modsen-software.com",
                IsCorporateDesign = true,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Frontend Developer",
                        ExperienceInYears = 2,
                        CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.C1, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "Frontend Developer",
                        TeamSize = "8",
                        StartDate = new DateOnly(2022, 6, 1),
                        EndDate = new DateOnly(2024, 12, 31),
                        IsCurrent = false,
                        Description = "Built responsive UI for a web application.",
                        Responsibilities = "UI development, component design",
                        Environment = "React, TypeScript, CSS",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Frontend Skills",
                        NameRu = "Навыки фронтенд-разработки",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "React",
                                    Level = CvSkillLevel.Medium,
                                    ExperienceInYears = 2,
                                    LastUsedYear = 2024,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Draft,
                UserProjectPosition = "Full Stack Developer",
                UserLocation = "Minsk",
                UserEducation = "MS in Software Engineering, Grodno State University, 2017-2022",
                UserSkills = "Python\n Django\n JavaScript\n PostgreSQL",
                UserAbout = "Versatile developer with experience in full stack development.",
                Website = (string)null!,
                IsCorporateDesign = false,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Full Stack Developer",
                        ExperienceInYears = 4,
                        CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "German", Level = CvLanguageLevel.A2, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "Lead Developer",
                        TeamSize = "10",
                        StartDate = new DateOnly(2021, 3, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Led development of a CRM system.",
                        Responsibilities = "System architecture, team coordination",
                        Environment = "Python, Django, React",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Programming Skills",
                        NameRu = "Навыки программирования",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Python",
                                    Level = CvSkillLevel.Professional,
                                    ExperienceInYears = 4,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Sent,
                UserProjectPosition = "DevOps Engineer",
                UserLocation = "Warsaw",
                UserEducation = "BS in Computer Engineering, Brest State Technical University, 2018-2022",
                UserSkills = "Docker, Kubernetes\n AWS\n Jenkins",
                UserAbout = "Skilled in CI/CD pipelines and cloud infrastructure.",
                Website = "https://www.modsen-software.com",
                IsCorporateDesign = true,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "DevOps Engineer", ExperienceInYears = 3, CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.B1, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "DevOps Engineer",
                        TeamSize = "6",
                        StartDate = new DateOnly(2022, 9, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Set up CI/CD pipelines for microservices.",
                        Responsibilities = "Infrastructure automation, deployment",
                        Environment = "Docker, Kubernetes, AWS",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "DevOps Skills",
                        NameRu = "Навыки DevOps",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Docker",
                                    Level = CvSkillLevel.Medium,
                                    ExperienceInYears = 3,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Draft,
                UserProjectPosition = "QA Engineer",
                UserLocation = "Warsaw",
                UserEducation = "BS in Software Testing, Mogilev State University, 2019-2023",
                UserSkills = "Selenium\n JUnit\n TestRail",
                UserAbout = "Detail-oriented QA with experience in automated testing.",
                Website = (string)null!,
                IsCorporateDesign = false,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "QA Engineer", ExperienceInYears = 2, CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity
                    {
                        Name = "Russian", Level = CvLanguageLevel.Native, CreatedAtUtc = createdAt
                    },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "QA Engineer",
                        TeamSize = "7",
                        StartDate = new DateOnly(2023, 2, 1),
                        EndDate = new DateOnly(2024, 10, 31),
                        IsCurrent = false,
                        Description = "Performed automated testing for web applications.",
                        Responsibilities = "Test case creation, automation scripts",
                        Environment = "Selenium, Java, TestRail",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Testing Skills",
                        NameRu = "Навыки тестирования",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Selenium",
                                    Level = CvSkillLevel.Medium,
                                    ExperienceInYears = 2,
                                    LastUsedYear = 2024,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Sent,
                UserProjectPosition = "Data Analyst",
                UserLocation = "Minsk",
                UserEducation = "BS in Data Science, Belarusian State University, 2017-2021",
                UserSkills = "Python\n SQL\n Tableau",
                UserAbout = "Expert in data analysis and visualization.",
                Website = "https://www.modsen-software.com",
                IsCorporateDesign = true,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Data Analyst", ExperienceInYears = 4, CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.C2, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "Data Analyst",
                        TeamSize = "4",
                        StartDate = new DateOnly(2022, 1, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Analyzed customer data for business insights.",
                        Responsibilities = "Data cleaning, visualization",
                        Environment = "Python, SQL, Tableau",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Data Skills",
                        NameRu = "Навыки анализа данных",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Python",
                                    Level = CvSkillLevel.Professional,
                                    ExperienceInYears = 4,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Draft,
                UserProjectPosition = "Mobile Developer",
                UserLocation = "Minsk",
                UserEducation = "BS in Computer Science, Gomel State University, 2018-2022",
                UserSkills = "Swift\n Xcode\n iOS",
                UserAbout = "Specialized in iOS app development.",
                Website = (string)null!,
                IsCorporateDesign = false,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Mobile Developer", ExperienceInYears = 3, CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.B1, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "iOS Developer",
                        TeamSize = "3",
                        StartDate = new DateOnly(2022, 5, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Developed iOS application for retail.",
                        Responsibilities = "App development, UI design",
                        Environment = "Swift, Xcode",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Mobile Skills",
                        NameRu = "Навыки мобильной разработки",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Swift",
                                    Level = CvSkillLevel.Medium,
                                    ExperienceInYears = 3,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Sent,
                UserProjectPosition = "Backend Developer",
                UserLocation = "Minsk",
                UserEducation = "BS in Software Engineering, BSUIR, 2016-2020",
                UserSkills = "Java\n Spring\n MySQL",
                UserAbout = "Experienced in building scalable backend systems.",
                Website = (string)null!,
                IsCorporateDesign = false,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Backend Developer",
                        ExperienceInYears = 5,
                        CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.B2, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "Backend Developer",
                        TeamSize = "6",
                        StartDate = new DateOnly(2021, 7, 1),
                        EndDate = new DateOnly(2023, 12, 31),
                        IsCurrent = false,
                        Description = "Developed REST APIs for a financial platform.",
                        Responsibilities = "API development, database optimization",
                        Environment = "Java, Spring, MySQL",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Backend Skills",
                        NameRu = "Навыки бэкенд-разработки",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Java",
                                    Level = CvSkillLevel.Professional,
                                    ExperienceInYears = 5,
                                    LastUsedYear = 2023,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Draft,
                UserProjectPosition = "UI/UX Designer",
                UserLocation = "Vitebsk",
                UserEducation = "BA in Graphic Design, Vitebsk Art Academy, 2019-2023",
                UserSkills = "Figma\n Adobe XD\n Sketch",
                UserAbout = "Creative designer with a focus on user experience.",
                Website = "https://www.modsen-software.com",
                IsCorporateDesign = true,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "UI/UX Designer", ExperienceInYears = 2, CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.A2, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "UI Designer",
                        TeamSize = "4",
                        StartDate = new DateOnly(2023, 3, 1),
                        EndDate = null,
                        IsCurrent = true,
                        Description = "Designed user interfaces for a mobile app.",
                        Responsibilities = "Wireframing, prototyping",
                        Environment = "Figma, Adobe XD",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup =
                    new SkillGroupEntity
                    {
                        Name = "Design Skills",
                        NameRu = "Навыки дизайна",
                        IsCustom = false,
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt,
                        Skills =
                            new List<SkillEntity>
                            {
                                new()
                                {
                                    Name = "Figma",
                                    Level = CvSkillLevel.Medium,
                                    ExperienceInYears = 2,
                                    LastUsedYear = 2025,
                                    OrderWeight = 1,
                                    CreatedAtUtc = createdAt
                                }
                            }
                    }
            },
            new
            {
                Status = CvStatus.Sent,
                UserProjectPosition = "Database Administrator",
                UserLocation = "Minsk",
                UserEducation = "BS in Database Systems, BSUIR, 2015-2019",
                UserSkills = "SQL\n Oracle\n MongoDB",
                UserAbout = "Expert in database management and optimization.",
                Website = (string)null!,
                IsCorporateDesign = false,
                WorkExperience =
                    new WorkExperienceEntity
                    {
                        Specialization = "Database Administrator",
                        ExperienceInYears = 6,
                        CreatedAtUtc = createdAt
                    },
                Language =
                    new LanguageEntity { Name = "English", Level = CvLanguageLevel.B1, CreatedAtUtc = createdAt },
                ProjectExperience =
                    new ProjectExperienceEntity
                    {
                        Position = "DBA",
                        TeamSize = "3",
                        StartDate = new DateOnly(2020, 1, 1),
                        EndDate = new DateOnly(2023, 6, 30),
                        IsCurrent = false,
                        Description = "Managed and optimized enterprise databases.",
                        Responsibilities = "Database tuning, backup management",
                        Environment = "Oracle, SQL Server",
                        OrderWeight = 1,
                        CreatedAtUtc = createdAt
                    },
                SkillGroup = new SkillGroupEntity
                {
                    Name = "Database Skills",
                    NameRu = "Навыки работы с базами данных",
                    IsCustom = false,
                    OrderWeight = 1,
                    CreatedAtUtc = createdAt,
                    Skills = new List<SkillEntity>
                    {
                        new()
                        {
                            Name = "SQL",
                            Level = CvSkillLevel.Professional,
                            ExperienceInYears = 6,
                            LastUsedYear = 2023,
                            OrderWeight = 1,
                            CreatedAtUtc = createdAt
                        }
                    }
                }
            }
        };

        for (var i = 0; i < 10; i++)
        {
            var employee = employees[random.Next(employees.Count)];
            var cvName = $"{employee.LastName + " " + employee.FirstName}_CV_{i + 1}";
            var data = cvData[i];

            var clientId = clientIds.Count != 0 && random.NextDouble() < 0.7
                ? clientIds[random.Next(clientIds.Count)]
                : null;

            var projectId = (clientId != null && projectIds.Count != 0 && random.NextDouble() < 0.7)
                ? projectIds[random.Next(projectIds.Count)]
                : null;

            yield return new CvEntity
            {
                CvName = cvName,
                UserId = employee.UserId,
                ProjectId = projectId,
                ClientId = clientId,
                Status = data.Status,
                StatusOrderWeight = (int)data.Status,
                IsVisibleToUser = true,
                UserFirstName = employee.FirstName,
                UserLastName = employee.LastName,
                UserProjectPosition = data.UserProjectPosition,
                UserLocation = data.UserLocation,
                UserEducation = data.UserEducation,
                UserSkills = data.UserSkills,
                UserAbout = data.UserAbout,
                Website = data.Website,
                IsCorporateDesign = data.IsCorporateDesign,
                CreatedById = null,
                CreatedAtUtc = createdAt,
                WorkExperiences = new List<WorkExperienceEntity> { data.WorkExperience },
                Languages = new List<LanguageEntity> { data.Language },
                ProjectExperiences = new List<ProjectExperienceEntity> { data.ProjectExperience },
                SkillGroups = new List<SkillGroupEntity> { data.SkillGroup }
            };
        }
    }
}
