export const FileKeys = {
	userAvatar: 'user-avatar',
	blobFile: 'blob-file',
};

export const SettingsPortalSettingsQueriesKeys = {
	portalSettings: 'portalSettings',
};

export const SettingsDepartmentClaimsQueriesKeys = {
	departmentClaims: 'departmentClaims',
};

export const SettingsRoleClaimsQueriesKeys = {
	roleClaims: 'roleClaims',
};

export const SettingsUsersClaimsQueriesKeys = {
	usersClaims: 'usersClaims',
};

export const SettingsCalendarExclusionsQueriesKeys = {
	exclusions: 'exclusions',
	exclusionsByUserCountry: 'exclusions-by-user-country',
	exclusionsLookup: 'exclusions-lookup',
};

export const SettingsKPIColorsQueriesKeys = {
	kpiColors: 'kpiColors',
};

export const BookingQueriesKeys = {
	notifications: 'notifications',
	bookings: 'bookings',
	meetingRoomsBookings: 'meeting-rooms-bookings',
	reservations: 'reservations',
	meetingRoomReservations: 'meeting-room-reservations',
	collisions: 'collisions',
	meetingRoomCollisions: 'meeting-room-collisions',
	meetingRooms: 'meeting-rooms',
	spaceWorkplaces: 'space-workplaces',
};

export const TimesheetQueryKeys = {
	userTimesheet: 'user-timesheet',
	projectsWithDisabledDays: 'projects-with-disabled-days',
	timesheetStatus: 'timesheet-status',
	timesheetManager: 'timesheet-manager',
	timesheetCalender: 'timesheet-calendar',
};

export const jiraImportQueriesKeys = {
	projects: 'jira-import-projects',
};

export const ClientQuerieKeys = {
	clients: 'clients',
	clientsLookup: 'clients-lookup',
	clientsProjects: 'clients-projects',
	clientProjectsForForm: 'clients-projects-for-form',
};

export const ReportsQueriesKeys = {
	reports: 'reports',
	reportsDocument: 'reports-document',
};

export const ProjectsQueriesKeys = {
	projects: 'projects',
	projectsByClientLookup: 'projects-by-client-lookup',
};

export const UsersQueriesKeys = {
	users: 'users',
	usersLookup: 'users-lookup',
	usersTab: 'users-tab',
	usersTabLookup: 'users-tab-lookup',
	chiefs: 'chief-users',
	allUsers: 'users-all-lookup',
	email: 'user-email',
	usersByProject: 'users-by-project',
};

export const TeamboardQueriesKeys = {
	teamboard: 'teamboard',
	teamboardUsersLookup: 'teamboard-users-lookup',
	employmentHistory: 'employmentHistory',
	internship: 'internship',
	personalData: 'personalData',
	comment: 'comment',
	plan: 'plan',
	projects: 'users-projects',
	hardSkillsLookup: 'user-hard-skills-lookup',
	projectsLookup: 'users-projects-lookup',
	getProjectAssignmentById: 'user-project-assignment-by-id',
	userAssignmentsLookup: 'user-project-assignment-lookup',
	userSpecializationsLookup: 'user-hard-skills-lookup',
	userSpecializationsLookupForm: 'user-hard-skills-lookup-form',
	userSpecializationSnapshotByDate: 'user-specialization-snapshot-by-date',
	userHardSkills: 'user-hard-skills',
	getUserConversation: 'get-user-notifications',
	getUserDevelopmentPlans: 'get-user-development-plans',
	userSoftSkill: 'user-soft-skills',
	getUserFeedbackById: 'get-user-feedback-by-id',
	getUserFeedbacks: 'get-user-feedbacks',
	getUserFeedbacksClients: 'get-user-feedbacks-clients',
	getUserFeedbacksReviewers: 'get-user-feedbacks-reviewers',
	userContracts: 'user-contracts',
	userNotAssignedSpecializations: 'user-not-assigned-specializations',
	getUserSpecializationsSnapshots: 'get-user-specializations-snapshots',
	useGetUserSpecializationById: 'get-user-specialization-by-id',
	useGetAssignedUsersSoftSkills: 'get-assigned-users-soft-skills',
	getUserSalaries: 'get-user-salaries',
};

export const EquipmentQueriesKeys = {
	file: 'equipment-document',
	equipments: 'equipments',
	user: 'user-equipments',
	info: 'user-info',
	editing: 'equipment-editing-info',
	history: 'equipment-history',
	deleted: 'equipment-deleted-history',
	autoSelectFilters: 'equipment-autoselect-filters',
};

export const PayslipsQueriesKeys = {
	getUserPayslips: 'get-user-payslips',
	getPayslipFile: 'get-payslip-file',
};

export const ASSIGNMENTS_HISTORY_QUERY_KEY = 'assignments-history';

export const SignatureQueriesKeys = {
	signature: 'signature',
	file: 'file',
};

export const NotificationQueriesKeys = {
	notificationPreview: 'notifications-preview',
	userNotifications: 'user-notifications',
	reportFile: 'user-notifications',
};

export const SettingsSkillsQueriesKeys = {
	specializations: 'specializations',
	hardSkills: 'hard-skills',
	softSkills: 'soft-skills',
};

export const SettingsWorkingHoursQueriesKeys = {
	getWorkingHours: 'get-working-hours',
};

export const CONFIGURATION_QUERY_KEY = 'configuration';
export const CLAIMS_QUERY_KEY = 'claims';
export const GROUPS_CLAIMS_QUERY_KEY = 'groups-claims';

export const RedirectKeys = {
	getRedirectInfoById: 'get-redirect-info-by-id',
};

export const payloadQueriesKeys = {
	getPayloadData: 'get-payload-data',
	getPayloadDataByUser: 'get-payload-data-by-user',
	getTotalPayloadData: 'get-total-payload-data',
};

export const GroupQueriesKeys = {
	getGroups: 'get-groups',
	groupMembers: 'group-members',
	groupMembersLookup: 'group-members-lookup',
	departmentsWithGroups: 'departments-with-groups',
	departmentUsersWithoutGroup: 'department-users-without-group',
};

export const CERTIFICATES_QUERY_KEY = 'user-certificates';

export const EducationsQueriesKeys = {
	userEducations: 'user-educations',
};

export const departmentQueryKeys = {
	getDepartments: 'get-departments-data',
	getDepartmentGroupManager: 'get-department-group-managers-data',
	getGroupById: 'get-group-by-id',
};

export const wikiQueriesKeys = {
	getKnowledgeGroupsData: 'get-knowledge-groups-data',
	getKnowledgeGroupById: 'get-knowledge-group-by-id-data',
	getArticleById: 'get-article-by-id-data',
	searchArticleData: 'search-article-data',
};

export const discountQueriesKeys = {
	getDiscountCategoriesData: 'get-discount-categories-data',
	getDiscountCategoryByIdData: 'get-discount-category-by-id-data',
	getDiscountsData: 'get-discounts-data',
	getDiscountsTagsData: 'get-discounts-tags-data',
	getDiscountsTagsByCategoryIdData: 'get-discounts-tags-by-category-id-data',
	getDiscountsCitiesData: 'get-discounts-cities-data',
	getDiscountByIdData: 'get-discount-by-id-data',
};

export const merchQueriesKeys = {
	getMerchElementsData: 'get-merch-elements-data',
	getMerchStorageData: 'get-merch-storage-data',
	merchElementsLookup: 'merch-elements-lookup',
	getMerchTransactions: 'get-merch-transactions-data',
	getMerchElementByIdData: 'get-merch-element-by-id-data',
	getMerchKitsData: 'get-merch-kits-data',
	getMerchKitByIdData: 'get-merch-kit-by-id-data',
	getMerchKitSizes: 'get-merch-kit-sizes',
	getMerchRecipients: 'get-merch-recipients',
};

export const digitalHrQueriesKeys = {
	getChatHistory: 'get-chat-history',
};

export const absencesQueriesKeys = {
	getAbsenceByIdData: 'get-absence-by-id-data',
	getAbsencesData: 'get-absences-data',
	getUserAbsences: 'get-user-absences',
	getUserAbsencesPopup: 'get-user-absences-popup',
	getUsersForAssignmentsData: 'get-users-for-assignments-data',
	getRequestsAbsences: 'get-requests-absences',
	getUserAbsencesRequest: 'get-user-absences-requests',
	getUserTransactionsHistory: 'get-user-transactions-history',
	getRequestsUsersLookup: 'get-request-user-lookup',
	getAbsencesRequestById: 'get-absences-request-by-id',
	getAssigns: 'get-assigns',
	getInquiriesLookup: 'get-inquiries-lookup',
	getInquiriesCalendar: 'get-inquiries-calendar',
};

export const requestQueriesKeys = {
	getRequestSelectedDays: 'get-request-selected-days',
};

export const microfrontendsQueriesKeys = {
	microfrontends: 'microfrontends',
};

export const performanceReviewQueriesKeys = {
	getUserPerformanceReview: 'get-user-performance-review',
	getAllPerformanceReviews: 'get-all-performance-reviews',
};

export const homeQueriesKeys = {
	getUserUpcomingEvents: 'get-user-upcoming-events',
	getBirthdayEmployeesByDate: 'get-birthday-employees-by-date',
	getAllBirthdays: 'get-all-birthdays',
	getProgrammerDayResults: 'get-programmer-day-results',
};

export const dashboardQueriesKeys = {
	getEmployeeEvents: 'get-employee-events',
	getRequestsWidget: 'get-requests-widget',
	getContractExpirationLookup: 'get-contract-expiration-lookup',
	getPerformanceReviewLookup: 'get-dashboard-performance-review-lookup',
	getTimesheetUsersLookup: 'get-dashboard-timesheet-users-lookup',
	getProbationLookup: 'get-dashboard-probation-lookup',
	getTodaysInterviewsLookup: 'get-todays-interviews-lookup',
	getApprovedAbsences: 'get-dashboard-approved-absences',
};

export const attendancesQueriesKeys = {
	getAttendancesDetailedData: 'get-attendances-detailed-data',
	getAttendancesTotalData: 'get-attendances-total-data',
};

export const interviewQueriesKeys = {
	getInterviewsData: 'get-interviews-data',
	getInterviewUsersLookup: 'get-interviews-user-lookup',
};

export const projectExitQueriesKeys = {
	getProjectExits: 'get-project-exits',
	getProjectExitReasons: 'get-project-exit-reasons',
	getProjectExitUsers: 'get-project-exit-users',
};

export const jiraQueriesKeys = {
	getJiraSettings: 'get-jira-settings',
};

export const ratesQueriesKeys = {
	getRates: 'get-rates',
	getRatesUsers: 'get-rates-users-lookup',
	getRateHistory: 'get-rate-history',
	createProjectRate: 'create-project-rate',
	getManagement: 'get-management',
	getExchangeRates: 'get-exchange-rates',
};

export const currenciesQueriesKeys = {
	getCurrenciesLookup: 'get-currencies-lookup',
};

export const expensesQueriesKeys = {
	getExpenses: 'get-expenses',
	getExpenseCategoriesLookup: 'get-expense-categories-lookup',
};

export const LabboardQueriesKeys = {
	labboardInterns: 'labboard-interns',
	labboardUsersLookup: 'labboard-users-lookup',
	internshipSpecializations: 'internship-specializations',
};
