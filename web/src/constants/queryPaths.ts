import { PAGE_SIZE } from '@/pages/Payload/contants';

const API_BASE_PREFIX = '/api';
export const USER_SCORE_API_ENDPOINT = `${API_BASE_PREFIX}/user-score`;
export const USER_SCORE_LEADERBOARD_API_ENDPOINT = `${USER_SCORE_API_ENDPOINT}/leaderboard`;
export const GET_CLIENTS_API_ENDPOINT = `${API_BASE_PREFIX}/Clients`;
export const GET_PROJECTS_API_ENDPOINT = `${API_BASE_PREFIX}/Projects`;
export const GET_DEPARTMENTS_API_ENDPOINT = `${API_BASE_PREFIX}/Departments`;
export const GET_RESOURCE_GROUPS_API_ENDPOINT = `${API_BASE_PREFIX}/resource-groups/lookup`;
export const getDepartmentGroupManagersEndpoint = (departmentId: string) =>
	`${API_BASE_PREFIX}/departments/${departmentId}/resource-groups/managers`;
export const departmentGroupEndpoint = (id?: number) =>
	`${API_BASE_PREFIX}/resource-groups/${id}`;
export const createGroupEndpoint = (departmentId: string) =>
	`${API_BASE_PREFIX}/departments/${departmentId}/resource-groups`;

export const manageRequestEndpoint = (requestId: number, type: string) =>
	`${API_BASE_PREFIX}/inquiries/${requestId}/${type}`;

export const getNotifyRequestAcceptEndpoint = (
	acceptId: number,
	requestId: number
) =>
	`${API_BASE_PREFIX}/inquiries/${requestId}/accepts/${acceptId}/notifications`;

export const departmentGroupUsersEndpoint = (
	groupId: number,
	userId?: number
) => `${departmentGroupEndpoint(groupId)}/users/${userId ?? ''}`;
export const GET_USERS_API_ENDPOINT = `${API_BASE_PREFIX}/Users`;
export const GET_USERS_TAB_LOOKUP_API_ENDPOINT = `${GET_USERS_API_ENDPOINT}/tab/lookup`;
export const GET_USERS_TAB_SEARCH_API_ENDPOINT = `${GET_USERS_API_ENDPOINT}/tab/search`;
export const GET_COLOR_PICKER_API_ENDPOINT = `${API_BASE_PREFIX}/color-picker`;
export const PERFORMANCE_REVIEW_API_ENDPOINT = `${API_BASE_PREFIX}/performance-review`;
export const USER_PERFORMANCE_REVIEW_API_ENDPOINT = `${API_BASE_PREFIX}/performance-review/user`;
export const MICROSOFT_LOGIN_REDIRECT = `${API_BASE_PREFIX}/account/login`;
export const LOGOUT_URL = `${API_BASE_PREFIX}/account/logout`;
export const GET_TIMESHEET_API_ENDPOINT = `${API_BASE_PREFIX}/Timesheet`;
export const UPDATE_CURRENT_USER = `${API_BASE_PREFIX}/users/current`;
export const GET_CONFIGURATION = `${API_BASE_PREFIX}/Configuration`;
export const GET_REPORTS = `${API_BASE_PREFIX}/Reports`;
export const GET_PROJECTS_LOOKUP = `${GET_PROJECTS_API_ENDPOINT}/lookup`;
export const GET_PROJECT_MANAGERS_API_ENDPOINT = `${GET_PROJECTS_API_ENDPOINT}/managers`;
export const GET_REPORTS_EXCEL = `${GET_REPORTS}/excel`;
export const GET_REPORTS_EXCEL_SECOND_VERSION = `${GET_REPORTS}/excelAccount`;
export const GET_REPORTS_WORD = `${GET_REPORTS}/word`;
export const GET_REPORTS_PDF = `${GET_REPORTS}/pdf`;
export const GET_CERTIFICATES_API_ENDPOINT = `${API_BASE_PREFIX}/certificate`;
export const GET_NOTIFICATIONS = `${API_BASE_PREFIX}/Notifications`;
export const GET_RESERVATION_API_ENDPOINT = `${API_BASE_PREFIX}/Reservation`;
export const GET_BOOKING_API_ENDPOINT = `${API_BASE_PREFIX}/Booking`;
export const GET_MERCH_KITS_TRANSACTIONS_ENDPOINT = `${API_BASE_PREFIX}/ItemTransactions/kit`;

export const GET_MERCH_TRANSACTIONS_ENDPOINT = `${API_BASE_PREFIX}/ItemTransactions`;
export const getWorkplacesByOfficeIdAPIEndpoint = (officeId: number) =>
	`${API_BASE_PREFIX}/offices/${officeId}/workplaces`;
export const getClaimsByUserIdAPIEndpoint = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/claims`;
export const getMeetingRoomsBookingsAPIEndpoint = (officeId: number) =>
	`${API_BASE_PREFIX}/offices/${officeId}/meeting-room-entries`;
export const getMeetingRoomsReservationsAPIEndpoint = (
	officeId: number,
	date: string
) =>
	`${API_BASE_PREFIX}/offices/${officeId}/calendar/${date}/meeting-room-reservations`;
const BOOKING_MEETING_ROOMS_PREFIX = `${API_BASE_PREFIX}/meeting-rooms`;
export const createMeetingRoomDeleteBookingApiUrl = (roomBookingId: number) =>
	`${API_BASE_PREFIX}/meeting-rooms-entries/${roomBookingId}`;
export const createMeetingRoomBooking = (roomId: number) =>
	`${BOOKING_MEETING_ROOMS_PREFIX}/${roomId}/meeting-room-reservations`;
export const GET_COLLISIONS_API_ENDPOINT = `${API_BASE_PREFIX}/Collisions`;
export const getMeetingRoomsCollisionsApiUrl = (roomId: number) =>
	`${API_BASE_PREFIX}/meeting-rooms/${roomId}/collisions`;
export const GET_FILE = `${API_BASE_PREFIX}/ApplicationFile`;
export const GET_FILES = `${GET_FILE}/multiply`;
export const GET_EQUIPMENT_API_ENDPOINT = `${API_BASE_PREFIX}/Equipment`;
export const GET_EQUIPMENT_HISTORY_API_ENDPOINT = `${API_BASE_PREFIX}/EquipmentHistory`;
export const GET_ASSIGNMENTS_HISTORY_API_ENDPOINT = `${API_BASE_PREFIX}/EquipmentHistory/assignments`;
export const GET_SIGNATURE = `${GET_USERS_API_ENDPOINT}/current/signature`;
export const GET_EQUIPMENT_FILE = `${GET_EQUIPMENT_API_ENDPOINT}/file`;
export const GET_EQUIPMENT_USERS_LOOKUP = `${API_BASE_PREFIX}/equipment-users/lookup`;
const GET_PERMISSIONS_API_ENDPOINT = `${API_BASE_PREFIX}/permissions`;
export const GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT = `${GET_PERMISSIONS_API_ENDPOINT}/assignments`;
export const GET_PERMISSIONS_ASSIGNMENTS_RESOURCES_API_ENDPOINT = `${GET_PERMISSIONS_ASSIGNMENTS_API_ENDPOINT}/resources`;
export const GET_USER_CONTRACTS_API_ENDPOINT = `${API_BASE_PREFIX}/contracts`;
export const GET_USER_PROJECTS_ENDPOINT = `${API_BASE_PREFIX}/user-projects`;
export const GET_TIMESHEETS_CALENDAR_ENDPOINT = `${API_BASE_PREFIX}/users/timesheets/calendar`;
export const getUserPrivateProjectsEndpointUrl = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/projects`;
export const getClientProjectsEndpointUrl = (clientId: number | string) =>
	`${GET_CLIENTS_API_ENDPOINT}/${clientId}/projects`;
export const GET_HARD_SKILLS_API_ENDPOINT = `${API_BASE_PREFIX}/hard-skills`;
export const getUserNotificationPrefixEndpointUrl = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/notifications`;
export const getUserNotificationsEndpointUrl = (userId: number) =>
	`${getUserNotificationPrefixEndpointUrl(userId)}/search`;
export const getUserNotificationsPreviewEndpointUrl = (userId: number) =>
	`${getUserNotificationPrefixEndpointUrl(userId)}/unread/preview`;
export const getReportFileEndpointUrl = (fileId: string) =>
	`${API_BASE_PREFIX}/applicationfile/${fileId}`;
export const getUserTimesheetStatusByMonthEndpointUrl = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/timesheets/status`;
export const getEquipmentByUserId = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/equipment`;

export const GET_DISCOUNT_CATEGORIES_API_ENDPOINT = `${API_BASE_PREFIX}/discount-categories`;
export const GET_DISCOUNT_CATEGORIES_ORDER_API_ENDPOINT = `${GET_DISCOUNT_CATEGORIES_API_ENDPOINT}/order`;
export const GET_DISCOUNTS_API_ENDPOINT = `${API_BASE_PREFIX}/discounts`;
export const GET_DISCOUNTS_ORDER_API_ENDPOINT = `${GET_DISCOUNTS_API_ENDPOINT}/order`;
export const GET_SEARCHED_DISCOUNTS_API_ENDPOINT = `${GET_DISCOUNTS_API_ENDPOINT}/search`;
export const GET_DISCOUNTS_TAGS_API_ENDPOINT = `${API_BASE_PREFIX}/tags`;
export const GET_DISCOUNTS_CITIES_API_ENDPOINT = `${GET_DISCOUNTS_API_ENDPOINT}/cities`;

export const GET_INQUIRIES_ENDPOINT = `${API_BASE_PREFIX}/inquiries`;
export const GET_SELECTED_DAYS_INQUIRIES_ENDPOINT = `${GET_INQUIRIES_ENDPOINT}/selected-days`;
export const GET_INQUIRIES_CALENDAR_ENDPOINT = `${GET_INQUIRIES_ENDPOINT}/calendar`;
export const GET_APPROVED_INQUIRIES_ENDPOINT = `${GET_INQUIRIES_ENDPOINT}/approved-absences`;

export const GET_CALENDAR_EXCLUSIONS_API_ENDPOINT = `${API_BASE_PREFIX}/calendar/exclusions`;

export const GET_SOFT_SKILLS_API_ENDPOINT = `${API_BASE_PREFIX}/soft-skills`;
export const GET_SPECIALIZATION_API_ENDPOINT = `${API_BASE_PREFIX}/specialization-templates`;
export const GET_TEAMBOARD_API_ENDPOINT = `${API_BASE_PREFIX}/teamboard-users`;

export const GET_PERSONAL_DATA_API_ENDPOINT = `${API_BASE_PREFIX}/personal-info`;
export const GET_USER_COMMENT_API_ENDPOINT = `${GET_PERSONAL_DATA_API_ENDPOINT}/comment`;
export const GET_USER_PLAN_API_ENDPOINT = `${GET_PERSONAL_DATA_API_ENDPOINT}/plan`;
export const GET_USER_NEED_PROJECT_API_ENDPOINT = `${GET_PERSONAL_DATA_API_ENDPOINT}/need-project`;
export const UPDATE_USER_LOCATION_ENDPOINT = `${GET_USERS_API_ENDPOINT}/location`;
export const GET_EMPLOYMENT_HISTORY_API_ENDPOINT = `${API_BASE_PREFIX}/employment-history`;
export const GET_USER_SPECIALIZATIONS_ENDPOINT = `${API_BASE_PREFIX}/user-specialization`;
export const GET_USER_CONVERSATIONS_ENDPOINT = `${API_BASE_PREFIX}/conversations`;
export const GET_USER_DEVELOPMENT_PLANS_ENDPOINT = `${API_BASE_PREFIX}/development-plans`;
export const UPDATE_USER_DEVELOPMENT_PLAN_STATUS_ENDPOINT = `${API_BASE_PREFIX}/development-plans/development-plans/status`;
export const GET_USER_FEEDBACKS_ENDPOINT = `${API_BASE_PREFIX}/external-feedbacks`;
export const GET_USER_FEEDBACKS_CLIENTS_ENDPOINT = `${GET_USER_FEEDBACKS_ENDPOINT}/clients`;
export const GET_USER_FEEDBACKS_REVIEWERS_ENDPOINT = `${GET_USER_FEEDBACKS_ENDPOINT}/reviewers`;
export const GET_USER_SUPERVISORS_ENDPOINT = `${GET_USER_CONVERSATIONS_ENDPOINT}/supervisors`;

export const GET_MERCH_ELEMENTS_ENDPOINT = `${API_BASE_PREFIX}/items`;
export const GET_MERCH_KITS_ENDPOINT = `${API_BASE_PREFIX}/kits`;
export const GET_POST_MERCH_KITS_ENDPOINT = `${GET_MERCH_KITS_ENDPOINT}/search`;
export const GET_MERCH_ELEMENTS_LOOKUP_ENDPOINT = `${GET_MERCH_ELEMENTS_ENDPOINT}/lookup`;
export const GET_POST_MERCH_ELEMENTS_ENDPOINT = `${GET_MERCH_ELEMENTS_ENDPOINT}/search`;
export const GET_POST_MERCH_STORAGE_ENDPOINT = `${API_BASE_PREFIX}/storage`;
export const GET_POST_MERCH_TRANSACTIONS_ENDPOINT = `${GET_MERCH_TRANSACTIONS_ENDPOINT}/search`;
export const GET_MERCH_RECIPIENTS_ENDPOINT = `${API_BASE_PREFIX}/merch-users/lookup`;

export const getMerchKitSizesEndpoint = (
	merchId: string | number,
	warehouseId: string | number
) => `${GET_MERCH_KITS_ENDPOINT}/${merchId}/warehouse/${warehouseId}/counts`;

export const updateMerchElementStatusEndpoint = (merchId: string | number) =>
	`${GET_MERCH_ELEMENTS_ENDPOINT}/${merchId}/status`;

export const getMerchElementByIdEndpoint = (merchId: string | number) =>
	`${GET_MERCH_ELEMENTS_ENDPOINT}/${merchId}`;

export const getMerchKitByIdEndpoint = (kitId: string | number) =>
	`${GET_MERCH_KITS_ENDPOINT}/${kitId}`;

export const getMerchKitStatusByIdEndpoint = (kitId: string | number) =>
	`${GET_MERCH_KITS_ENDPOINT}/${kitId}/status`;

export const getRegisteredLocationEndPointUrl = (userId: string) =>
	`${API_BASE_PREFIX}/Users/${userId}/registered-location`;

export const getDiscountByIdEndPoint = (discountId: string | number) =>
	`${GET_DISCOUNTS_API_ENDPOINT}/${discountId}`;

export const getDiscountCategoryByIdEndpoint = (categoryId: string | number) =>
	`${GET_DISCOUNT_CATEGORIES_API_ENDPOINT}/${categoryId}`;

export const getDiscountTagsByCategoryIdEndpoint = (
	categoryId: string | number
) => `${GET_DISCOUNT_CATEGORIES_API_ENDPOINT}/${categoryId}/tags`;

export const getDevelopmentPlanEndpointUrl = (developmentPlanId: number) =>
	`${GET_USER_DEVELOPMENT_PLANS_ENDPOINT}/${developmentPlanId}`;
export const getFeedbackEndpointUrl = (feedbackId: number) =>
	`${GET_USER_FEEDBACKS_ENDPOINT}/${feedbackId}`;
export const GET_USER_SALARY_ENDPOINT = `${API_BASE_PREFIX}/user-salary`;
export const getUserSalaryEndpoint = (userId: number) =>
	`${GET_USERS_API_ENDPOINT}/${userId}/salaries`;

export const getUserPayslipsEndpoint = (userId: number) =>
	`${GET_USERS_API_ENDPOINT}/${userId}/payslips`;

export const getUserSoftSkillsEndpoint = (userId?: number | string) =>
	`${API_BASE_PREFIX}/users/${userId}/soft-skills`;

export const getAssignedUserSoftSkillsEndpoint = (
	userId: number | string | undefined
) => `${getUserSoftSkillsEndpoint(userId)}/assigned`;

export const getUpdateUserSoftSkillEndpoint = (snapshotId?: number | string) =>
	`${API_BASE_PREFIX}/users/soft-skills/${snapshotId}`;

export const getUserInquiries = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/inquiries`;

export const SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT = `${API_BASE_PREFIX}/working-hours-per-months`;
export const SETTINGS_CALENDAR_COUNTRIES_API_ENDPOINT = `${SETTINGS_CALENDAR_WORKING_HOURS_API_ENDPOINT}/countries`;

export const getRedirectInfoBuIdEndpointUrl = (id: string) =>
	`${API_BASE_PREFIX}/redirect-rules/${id}`;

export const createUserProjectAssignmentEndpointUrl = (
	userId: number,
	projectId: number
) => `${API_BASE_PREFIX}/users/${userId}/projects/${projectId}`;

export const createUserProjectWithDisabledDaysEndpointUrl = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/projects/with-disabled-days`;

export const getUserSpecializationsEndpointUrl = (
	userId: number | undefined,
	areAssigned: boolean
) =>
	`${API_BASE_PREFIX}/users/${userId}/specializations?areAssigned=${areAssigned}`;

export const GET_USER_SPECIALIZATIONS_SNAPSHOTS = `${API_BASE_PREFIX}/user-skills-snapshot`;
export const GET_USER_PRIMARY_SPECIALIZATIONS_ENDPOINT = `${GET_USER_SPECIALIZATIONS_ENDPOINT}/primary`;
export const getUserSpecializationById = (
	specializationId: number | undefined
) => `${GET_USER_SPECIALIZATIONS_ENDPOINT}/${specializationId}`;

export const GET_SETTINGS_PORTAL_SETTINGS_ENDPOINT = `${API_BASE_PREFIX}/portal-settings`;
export const GET_SETTINGS_DEPARTMENT_CLAIMS_ENDPOINT = `${API_BASE_PREFIX}/departments/claims`;
export const GET_SETTINGS_ROLE_CLAIMS_ENDPOINT = `${API_BASE_PREFIX}/roles/claims`;
export const GET_SETTINGS_USERS_CLAIMS_ENDPOINT = `${API_BASE_PREFIX}/users/claims`;
export const GET_CLAIM_GROUPS_ENDPOINT = `${API_BASE_PREFIX}/claim-groups/claims`;
export const ARTICLES_API_ENDPOINT = `${API_BASE_PREFIX}/articles`;
export const KNOWLEDGE_GROUPS_API_ENDPOINT = `${API_BASE_PREFIX}/knowledge-groups`;
export const UPDATE_KNOWLEDGE_GROUPS_ORDER_API_ENDPOINT = `${KNOWLEDGE_GROUPS_API_ENDPOINT}/order`;
export const GET_EQUIPMENT_AUTOSELECT_FILTERS_ENDPOINT = `${GET_EQUIPMENT_API_ENDPOINT}/availableFilters`;
const GET_DIGITAL_HR_API_ENDPOINT = `${API_BASE_PREFIX}/DigitalHr`;
export const GET_ANALYZE_QUESTION_API_ENDPOINT = `${GET_DIGITAL_HR_API_ENDPOINT}/analyze-question`;
const GET_CHAT_HISTORY_API_ENDPOINT = `${GET_DIGITAL_HR_API_ENDPOINT}/chat-history`;

export const getPayloadApiEndpoint = (
	year: number,
	pageSize: number = PAGE_SIZE,
	pageNumber: number = 0
) =>
	`${API_BASE_PREFIX}/calendar/${year}/employees-payload?pageSize=${pageSize}&pageNumber=${pageNumber}`;

export const GET_PROJECT_RATES_API_ENDPOINT = `${API_BASE_PREFIX}/project-rates`;

export const getProjectRateByIdEndpoint = (id: number) =>
	`${GET_PROJECT_RATES_API_ENDPOINT}/${id}`;

export const getProjectRatesApiEndpoint = (
	year: number,
	month: number,
	pageSize: number,
	pageNumber: number
) =>
	`${GET_PROJECT_RATES_API_ENDPOINT}/${year}/${month}?pageSize=${pageSize}&pageNumber=${pageNumber}`;

export const getManagementsApiEndpoint = (
	year: number,
	month: number,
	pageSize: number,
	pageNumber: number
) =>
	`${API_BASE_PREFIX}/managements/${year}/${month}?pageSize=${pageSize}&pageNumber=${pageNumber}`;

export const getFteChangeEndpoint = (
	year: number,
	month: number,
	userId: number
) => `${API_BASE_PREFIX}/calendar/${year}/month/${month}/users/${userId}`;

export const getUserByIdEndPoint = (id: number) =>
	`${GET_USERS_API_ENDPOINT}/${id}`;

export const getPayloadTotalEndpoint = (year: number) =>
	`${API_BASE_PREFIX}/calendar/${year}/totals-employees-payload`;

export const getEmployeePayloadChangeEndpoint = (userId: number) =>
	`${API_BASE_PREFIX}/users/${userId}/payload-details`;

export const getArticleByIdEndpoint = (id: number | string) =>
	`${ARTICLES_API_ENDPOINT}/${id}`;

export const getKnowledgeGroupByIdEndpoint = (id: number | string) =>
	`${KNOWLEDGE_GROUPS_API_ENDPOINT}/${id}`;

export const getKnowledgeGroupNameUpdateEndpoint = (
	id: number | string,
	name: string
) => `${getKnowledgeGroupByIdEndpoint(id)}/name/${name}`;

export const getInquiryByIdEndpoint = (id: number | string) =>
	`${GET_INQUIRIES_ENDPOINT}/${id}`;

export const getInquryCommentsEndpoint = (inquryId: number | string) =>
	`${getInquiryByIdEndpoint(inquryId)}/comments`;

export const SEARCH_ARTICLES_API_ENDPOINT = `${ARTICLES_API_ENDPOINT}/search`;

export const KPI_COLORS_ENDPOINT = `${API_BASE_PREFIX}/kpi-colors`;

export const EDUCATIONS_ENDPOINT = `${GET_TEAMBOARD_API_ENDPOINT}/educations`;
export const getUserEducationsEndpoint = (userId: number | string) =>
	`${GET_TEAMBOARD_API_ENDPOINT}/${userId}/educations`;

export const getChatHistoryApiEndpoint = (
	orderBy: string,
	top: number,
	skip: number
) =>
	`${GET_CHAT_HISTORY_API_ENDPOINT}/?orderby=${orderBy}&top=${top}&skip=${skip}`;

export const GET_ABSENCES_API_ENDPOINT = `${API_BASE_PREFIX}/inquiry-templates`;
export const GET_INQUIRIES_LOOKUP_API_ENDPOINT = `${GET_ABSENCES_API_ENDPOINT}/lookup`;
export const GET_REQUEST_USERS_LOOKUP_API_ENDPOINT = `${API_BASE_PREFIX}/inquiry-users/lookup`;
export const GET_APPROVERS_USERS_LOOKUP_API_ENDPOINT = `${API_BASE_PREFIX}/inquiry-users/approvers/lookup`;
export const AGREEMENT_API_ENDPOINT = `${API_BASE_PREFIX}/agreement-templates`;
export const GET_USERS_FOR_ABSENCES_API_ENDPOINT = `${API_BASE_PREFIX}/users/templates/lookup`;
export const ASSIGN_USERS_TO_ABSENCE_API_ENDPOINT = `${GET_INQUIRIES_ENDPOINT}/assigns`;

export const geAbsencesByUserIdEndpoint = (userId: number | string) =>
	`${API_BASE_PREFIX}/users/${userId}/inquiries/assigns`;

export const getAbsenceByIdApiEndpoint = (id: number | string) =>
	`${GET_ABSENCES_API_ENDPOINT}/${id}`;

export const getAgreementByIdApiEndpoint = (id: number | string) =>
	`${AGREEMENT_API_ENDPOINT}/${id}`;

export const getAbsencesRequestsByUserIdEndpoint = (id: number | string) =>
	`${API_BASE_PREFIX}/users/${id}/inquiries/search`;

export const getAgreementCorrectionByIdEndpoint = (
	userId: number | string,
	agreementTemplateId: number | string
) =>
	`${API_BASE_PREFIX}/users/${userId}/agreements/${agreementTemplateId}/correction`;

export const getTransactionsHistoryByUserIdEndpoint = (id: number | string) =>
	`${API_BASE_PREFIX}/users/${id}/history`;

const GET_EMPLOYEE_HOME = `${API_BASE_PREFIX}/home`;

export const getUserUpcomingEvents = (userId: string | number) =>
	`${GET_EMPLOYEE_HOME}/${userId}/events`;

export const API_URL = import.meta.env.VITE_API_URL;

export const getFileUri = (fileId: string) => `${GET_FILE}/${fileId}`;
export const getPayslipFileUri = (fileId: string) =>
	`${getFileUri(fileId)}/payslip`;
export const getUsersByBirthdayDate = (date: string) =>
	`${GET_EMPLOYEE_HOME}/birthdays/${date}`;

export const GET_ALL_BIRTHDAYS_API_ENDPOINT = `${GET_EMPLOYEE_HOME}/birthdays`;

export const GET_EMPLOYEE_EVENTS_API_ENDPOINT = `${API_BASE_PREFIX}/home/employees-events`;

export const getHireDateEndpoint = (userId: number | string) =>
	`${GET_USERS_API_ENDPOINT}/${userId}/hireDate`;

export const getAssignsEndpoint = (entityName: string, id: number) =>
	`${API_BASE_PREFIX}/${entityName}/${id}/assigns/search`;

export const GET_MICROFRONTENDS_CONFIG = `${API_BASE_PREFIX}/microfrontends/config`;

const GET_ATTENDANCES_ENDPOINT = `${API_BASE_PREFIX}/attendances`;

export const GET_ATTENDANCES_TOTAL_ENDPOINT = `${GET_ATTENDANCES_ENDPOINT}/total`;
export const GET_ATTENDANCES_DETAILED_ENDPOINT = `${GET_ATTENDANCES_ENDPOINT}/detailed`;

export const GET_SALE_MANAGERS_ENDPOINT = `${API_BASE_PREFIX}/sale-managers/lookup`;

export const GET_INTERVIEWS_ENDPOINT = `${API_BASE_PREFIX}/interviews`;

export const GET_INTERVIEWS_SEARCH_ENDPOINT = `${GET_INTERVIEWS_ENDPOINT}/search`;

export const getInterviewByIdApiEndpoint = (id: number) =>
	`${GET_INTERVIEWS_ENDPOINT}/${id}`;

export const getInterviewUsersLookupEndpoint = (onlyActive?: boolean) =>
	`${API_BASE_PREFIX}/interview-users/lookup${
		onlyActive ? '?onlyActive=true' : ''
	}`;

export const TODAYS_INTERVIEWS_API_ENDPOINT = `${GET_INTERVIEWS_ENDPOINT}/employees-interviews`;

export const PROBATION_API_ENDPOINT = `${API_BASE_PREFIX}/users/probation-periods-ends`;

export const GET_JIRA_SETTINGS_API_ENDPOINT = `${API_BASE_PREFIX}/jira-settings`;
export const GET_JIRA_SETTINGS_SEARCH_ENDPOINT = `${GET_JIRA_SETTINGS_API_ENDPOINT}/search`;
export const getJiraSettingsByIdEndpoint = (id: number) =>
	`${GET_JIRA_SETTINGS_API_ENDPOINT}/${id}`;

export const GET_ACTIVE_EXPIRING_CONTRACTS = `${GET_USER_CONTRACTS_API_ENDPOINT}/active-expiring`;

export const GET_PENDING_INQUIRIES = `${API_BASE_PREFIX}/inquiries/accepts/pending`;

export const GET_EXITS_API_ENDPOINT = `${API_BASE_PREFIX}/project-exits`;
export const GET_PROJECT_EXIT_REASONS_API_ENDPOINT = `${GET_EXITS_API_ENDPOINT}/closing-reasons`;
export const GET_PROJECT_EXIT_USERS_API_ENDPOINT = `${API_BASE_PREFIX}/project-exit-users/lookup`;
export const getProjectExitByIdApiEndpoint = (id: number | string) =>
	`${GET_EXITS_API_ENDPOINT}/${id}`;
export const getRestoreProjectExitApiEndpoint = (id: number | string) =>
	`${getProjectExitByIdApiEndpoint(id)}/restore`;

const JIRA_IMPORT_API_PREFIX = `${API_BASE_PREFIX}/jira-import`;

export const JIRA_IMPORT_TIMESHEETS_API_ENDPOINT = `${JIRA_IMPORT_API_PREFIX}/timesheets`;
export const JIRA_IMPORT_PROJECTS_API_ENDPOINT = `${JIRA_IMPORT_API_PREFIX}/projects`;

export const getFinanceUsersLookupApiEndpoint = (year: number, month: number) =>
	`${API_BASE_PREFIX}/finance-users/lookup/${year}/${month}`;

export const getRateHistoryApiEndpoint = (
	projectId: string | number,
	userId: string | number
) => `${API_BASE_PREFIX}/projects/${projectId}/users/${userId}/rates`;

export const GET_CURRENCIES_LOOKUP_API_ENDPOINT = `${API_BASE_PREFIX}/currencies/lookup`;

export const getExchangeRatesApiEndpoint = (date: string) =>
	`${API_BASE_PREFIX}/exchange-rates/${date}`;

export const EXPENSES_API_ENDPOINT = `${API_BASE_PREFIX}/expenses`;
export const EXPENSE_CATEGORIES_API_ENDPOINT = `${API_BASE_PREFIX}/expense-categories`;
export const GET_EXPENSE_CATEGORIES_LOOKUP_ENDPOINT = `${EXPENSE_CATEGORIES_API_ENDPOINT}/lookup`;

export const getExpenseCategoryByIdEndpoint = (expenseCategoryId: number) =>
	`${EXPENSE_CATEGORIES_API_ENDPOINT}/${expenseCategoryId}`;

export const getExpenseByIdEndpoint = (expenseId: number) =>
	`${EXPENSES_API_ENDPOINT}/${expenseId}`;

export const getExpensesApiEndpoint = (
	year: string,
	pageSize: number = PAGE_SIZE,
	pageNumber: number = 0
) =>
	`${EXPENSES_API_ENDPOINT}/${year}?pageSize=${pageSize}&pageNumber=${pageNumber}`;

export const GET_LABBOARD_INTERNS_API_ENDPOINT = `${API_BASE_PREFIX}/labboard`;
export const GET_LABBOARD_USERS_LOOKUP_API_ENDPOINT = `${API_BASE_PREFIX}/labboard/lookup`;
export const getInternshipEndpoint = (userId: number) =>
	`${GET_LABBOARD_INTERNS_API_ENDPOINT}/${userId}/internship`;
export const GET_INTERNSHIP_SPECIALIZATIONS_API_ENDPOINT = `${GET_LABBOARD_INTERNS_API_ENDPOINT}/internship/specializations`;
