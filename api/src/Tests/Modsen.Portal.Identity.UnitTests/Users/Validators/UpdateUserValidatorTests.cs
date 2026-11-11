using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;
using Modsen.Portal.Identity.Requests.Users.Core.Validators;
using Modsen.Portal.Identity.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.Users.Validators
{
    public class UpdateUserValidatorTests
    {
        private readonly Mock<ICanRoleHaveDepartmentBusinessRule> _mockCanRoleHaveDepartmentBusinessRule;
        private readonly Mock<IAuthenticatedUserContext> _mockAuthenticatedUserContext;
        private readonly Mock<ICanUserWithResourceGroupBeCreatedBusinessRule> _mockCanUserWithResourceGroupBeCreatedBusinessRule;
        private readonly Mock<ICanRoleBeOccupiedBusinessRule> _mockCanRoleBeOccupiedBusinessRule;
        private readonly Mock<IIdentityDbContext> _mockIdentityDbContext;
        private readonly Mock<IIsManagerUserUpdateValidBusinessRule> _mockIsManagerUserUpdateValidBusinessRule;
        private readonly Mock<ITeamBoardContract> _mockTeamBoardContract;
        private readonly UpdateUserValidator _validator;

        public UpdateUserValidatorTests()
        {
            _mockCanRoleHaveDepartmentBusinessRule = new Mock<ICanRoleHaveDepartmentBusinessRule>();
            _mockAuthenticatedUserContext = new Mock<IAuthenticatedUserContext>();
            _mockCanUserWithResourceGroupBeCreatedBusinessRule = new Mock<ICanUserWithResourceGroupBeCreatedBusinessRule>();
            _mockCanRoleBeOccupiedBusinessRule = new Mock<ICanRoleBeOccupiedBusinessRule>();
            _mockIdentityDbContext = new Mock<IIdentityDbContext>();
            _mockIsManagerUserUpdateValidBusinessRule = new Mock<IIsManagerUserUpdateValidBusinessRule>();
            _mockTeamBoardContract = new Mock<ITeamBoardContract>();

            _validator = new UpdateUserValidator(
                _mockCanRoleHaveDepartmentBusinessRule.Object,
                _mockAuthenticatedUserContext.Object,
                _mockCanUserWithResourceGroupBeCreatedBusinessRule.Object,
                _mockCanRoleBeOccupiedBusinessRule.Object,
                _mockIdentityDbContext.Object,
                _mockIsManagerUserUpdateValidBusinessRule.Object,
                _mockTeamBoardContract.Object
            );
        }

        [Fact]
        public async Task ValidateShouldThrowInvalidDataAppExceptionWhenUserCanNotHaveDepartment()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: null,
                Position: PortalRoleNames.Employee);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Cto]
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Fail(""));
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Cto]);
            _mockIsManagerUserUpdateValidBusinessRule.Setup(x =>
                    x.IsCorrespondsToBusiness(
                        It.IsAny<IsManagerUserUpdateValidBusinessRule.Request>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<InvalidDataAppException>(result);
        }

        [Theory]
        [MemberData(nameof(InvalidPositionCases))]
        public async Task ValidateWhenIncorrectPositionsShouldThrowActionNotAllowedException(string authRole, string newRole)
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: null,
                Position: newRole);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var oldRole = newRole == PortalRoleNames.Employee ? PortalRoleNames.GroupManager : PortalRoleNames.Employee;

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [oldRole]
            };

            _mockAuthenticatedUserContext.Setup(x => x.GetRoles()).Returns([authRole]);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Fact]
        public async Task ValidateWhenTryToUpdatePositionForUserFromAnotherDepartmentThrowActionNotAllowedException()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: null,
                Position: PortalRoleNames.GroupManager);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Employee],
                DepartmentId = "Another department"
            };

            _mockAuthenticatedUserContext.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Fact]
        public async Task ValidateShouldThrowEntityConflictExceptionWhenEmailAlreadyExists()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: null,
                Position: PortalRoleNames.Employee);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                },
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com"
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Cto]);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<EntityConflictException>(result);
        }

        [Theory]
        [InlineData(PortalRoleNames.Ceo)]
        [InlineData(PortalRoleNames.Cto)]
        [InlineData(PortalRoleNames.HeadOfDepartment)]
        public async Task ValidateShouldThrowActionNotAllowedExceptionWhenChiefOrHeadWithResourceGroup(string role)
        {
            // Arrange
            var email = "existing@examples.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: role);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [role],
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Cto]);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Theory]
        [InlineData(PortalRoleNames.SystemAdministrator)]
        [InlineData(PortalRoleNames.GroupManager)]
        public async Task ValidateShouldThrowActionNotAllowedExceptionWhenSysAdminOrGmUpdates(string role)
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: null);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Employee],
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([role]);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Fact]
        public async Task ValidateShouldThrowActionNotAllowedExceptionWhenUserWithResourceGroupCantBeCreated()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: null);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Employee],
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Ceo]);
            _mockCanUserWithResourceGroupBeCreatedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Fail(""));

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Fact]
        public async Task ValidateShouldThrowActionNotAllowedExceptionWhenRoleCantBeOccupied()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: null);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.GroupManager],
                ResourceGroupId = 2,
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Ceo]);
            _mockCanUserWithResourceGroupBeCreatedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockCanRoleBeOccupiedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Fail(""));

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<ActionNotAllowedException>(result);
        }

        [Theory]
        [MemberData(nameof(ContractDatesCases))]
        public async Task ValidateUpdateInternPositionWithoutContractInformationShouldThrowActionNotAllowed(UserContractDatesOutput? contractDates)
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: PortalRoleNames.Employee);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                Id = request.Id,
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Intern],
                ResourceGroupId = 2,
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Ceo]);
            _mockCanUserWithResourceGroupBeCreatedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockCanRoleBeOccupiedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockTeamBoardContract
                .Setup(x => x.GetUserContractDates(request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(contractDates);

            // Act
            Task result() => _validator.Validate(userDto, request, CancellationToken.None);

            // Assert
            var exception = await Assert.ThrowsAsync<ActionNotAllowedException>(result);
            exception.Message.Should().Be("Before updating the intern's position, the contract information must be completed");
        }

        [Fact]
        public async Task ValidateUpdateInternPositionWithContractInformationShouldNotThrowException()
        {
            // Arrange
            var email = "existing@example.com";

            var request = new UpdateUserGeneralInfoRequest.InternalCommand(
                Id: 1,
                FirstName: "First",
                LastName: "Last",
                FirstNameRu: null,
                LastNameRu: null,
                PatronymicRu: null,
                Email: email,
                DepartmentId: null,
                ResourceGroupId: 1,
                Position: PortalRoleNames.Employee);

            var existingUsers = new List<User>
            {
                new()
                {
                    FirstName = "test",
                    LastName = "test",
                    Email = "old@example.com",
                }
            };

            var userDto = new UserDto
            {
                Id = request.Id,
                FirstName = "test",
                Email = "old@example.com",
                RoleNames = [PortalRoleNames.Intern],
                ResourceGroupId = 2,
            };

            _mockIdentityDbContext
                .Setup(x => x.Users)
                .Returns(existingUsers.BuildMockDbSet().Object);
            _mockCanRoleHaveDepartmentBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleHaveDepartmentBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockAuthenticatedUserContext
                .Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Ceo]);
            _mockCanUserWithResourceGroupBeCreatedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanUserWithResourceGroupBeCreatedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockCanRoleBeOccupiedBusinessRule
                .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<CanRoleBeOccupiedBusinessRule.Request>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());
            _mockTeamBoardContract
                .Setup(x => x.GetUserContractDates(request.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserContractDatesOutput
                {
                    UserId = request.Id,
                    ContractStartDate = DateOnly.MaxValue,
                    ContractEndDate = DateOnly.MaxValue,
                    HireDate = DateOnly.MaxValue,
                    FireDate = DateOnly.MaxValue
                });
            _mockIsManagerUserUpdateValidBusinessRule.Setup(x =>
                    x.IsCorrespondsToBusiness(
                        It.IsAny<IsManagerUserUpdateValidBusinessRule.Request>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(BusinessRuleResult.Success());

            // Act
            await _validator.Validate(userDto, request, CancellationToken.None);
        }

        public static IEnumerable<object[]> InvalidPositionCases =
            [
                [
                    PortalRoleNames.Ceo,
                    PortalRoleNames.Ceo
                ],
                [
                    PortalRoleNames.Ceo,
                    PortalRoleNames.Cto
                ],
                [
                    PortalRoleNames.Cto,
                    PortalRoleNames.Ceo
                ],
                [
                    PortalRoleNames.Cto,
                    PortalRoleNames.Cto
                ],
                [
                    PortalRoleNames.HeadOfDepartment,
                    PortalRoleNames.Ceo
                ],
                [
                    PortalRoleNames.HeadOfDepartment,
                    PortalRoleNames.Cto
                ],
                [
                    PortalRoleNames.HeadOfDepartment,
                    PortalRoleNames.HeadOfDepartment
                ],
                [
                    PortalRoleNames.HeadOfDepartment,
                    PortalRoleNames.SystemAdministrator
                ],
                [
                    PortalRoleNames.GroupManager,
                    PortalRoleNames.Ceo
                ],
                [
                    PortalRoleNames.GroupManager,
                    PortalRoleNames.Cto
                ],
                [
                    PortalRoleNames.GroupManager,
                    PortalRoleNames.HeadOfDepartment
                ],
                [
                    PortalRoleNames.GroupManager,
                    PortalRoleNames.SystemAdministrator
                ],
                [
                    PortalRoleNames.GroupManager,
                    PortalRoleNames.GroupManager
                ],
            ];

        public static IEnumerable<object?[]> ContractDatesCases =>
        [
            [null],
            [new UserContractDatesOutput
            {
                UserId = 1,
                ContractStartDate = DateOnly.MaxValue,
                ContractEndDate = null,
                HireDate = DateOnly.MaxValue,
                FireDate = DateOnly.MaxValue
            }],
            [new UserContractDatesOutput
            {
                UserId = 1,
                ContractStartDate = null,
                ContractEndDate = DateOnly.MaxValue,
                HireDate = DateOnly.MaxValue,
                FireDate = DateOnly.MaxValue
            }]
        ];
    }
}
