import { Dayjs } from 'dayjs';

import { FromToProps, GridCellDefaultProps } from '@/components/Grid';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';
import { Roles } from '@/constants/roles';
import { ByPageResponse } from '@/queries/types';
import { OdataParams } from '@/types/common';
import { Document } from '@/types/document';
import {
	FilterReducer,
	FilterReducerAction,
	FiltersHandlers,
} from '@/types/filter';

export type AbsenceDetailsFields =
	| 'name'
	| 'type'
	| 'trackingUnitType'
	| 'activityType'
	| 'comment'
	| 'isProhibitOnProbationPeriod'
	| 'isIncludeWeekends'
	| 'isIncludeHolidays'
	| 'isRequireAttachments'
	| 'accrualType'
	| 'transferType';

interface AcceptPolicesValues {
	id?: number;
	orderWeight: number;
	scope: string;
	userId: number | string | null;
	userFullName?: string | null;
	substituteScope: null | string;
	substituteUserId: number | string | null;
	substituteUserFullName?: string | null;
}
export type AgreementDetailsFields =
	| 'name'
	| 'daysPerContractYear'
	| 'isDefault';

export type AgreementItem = {
	id: number | string;
	name: string;
	daysPerContractYear: string | number | null;
	isDefault: boolean;
	assignedUsersCount?: number;
	hasAnyTransactions?: boolean;
};

export interface AbsencesFormValues {
	name: string;
	type: string;
	trackingUnitType: string;
	accrualType: string | null;
	transferType: string | null;
	activityType: string;
	comment: string | null;
	isProhibitOnProbationPeriod: boolean;
	isRequireAttachments: boolean;
	isIncludeHolidays: boolean;
	isIncludeWeekends: boolean;
	isSubstitutesToApproveAllowed: boolean;
	acceptPolicies: AcceptPolicesValues[];
	agreements: AgreementItem[];
}

export interface AcceptPolicesScope {
	name: string;
	scope: string;
}
export interface AcceptPolicesUser {
	name: string;
	id: number | string;
}

export interface AcceptPolicesItem {
	orderWeight: number;
	userId: string | null | number | undefined;
	substituteUserId: string | null | number | undefined;
	scope: AcceptPolicesScope | null;
	user: AcceptPolicesUser | null;
	substituteScope: AcceptPolicesScope | null;
	substituteUser: AcceptPolicesUser | null;
}

export interface Absence extends AbsencesFormValues {
	id: number | string;
	hasAnyTransactions?: boolean;
	assignedUsersCount?: number;
}

export type Agreement = Omit<AgreementItem, 'id'> & {
	inquiryTemplateId: number;
};

export interface MenageRequestProps {
	id: number;
	type: ManageRequestType;
}

export interface AbsenceRow extends GridCellDefaultProps {
	name: string;
	availableDays: number;
}

export enum AbsenceType {
	Vacation = 'Vacation',
	SickLeave = 'SickLeave',
	HomeOffice = 'HomeOffice',
	OwnExpense = 'OwnExpense',
	SickDay = 'SickDay',
	WeekendTransfer = 'WeekendTransfer',
	MilitaryEnlistmentOffice = 'MilitaryEnlistmentOffice',
	SocialVacation = 'SocialVacation',
	Documents = 'Documents',
	OtherType = 'OtherType',
}

export enum AccrualType {
	ProportianallyEachMonth = 'ProportianallyEachMonth',
	OnceInYear = 'OnceInYear',
}

export enum TransferType {
	Transferred = 'Transferred',
	Reset = 'Reset',
}

export interface AssignmentUser {
	id: number;
	fullName: string;
	fullNameRu: string | null;
}

export interface UsersAssignmentResponse {
	assignedUsers: AssignmentUser[];
	unassignedUsers: AssignmentUser[];
}

export enum TrackingUnitType {
	Days = 'Days',
	Hours = 'Hours',
}

export interface UserAbsenceItem {
	name: string;
	trackingUnitCount: number;
	type: AbsenceType;
	trackingUnitType: TrackingUnitType;
	isAgreement: boolean;
	agreementTemplateId?: number | null;
	inquiryTemplateId: number;
	dateFrom?: Dayjs;
}

export enum AbsenceRequestStatus {
	Open = 'Open',
	Rejected = 'Rejected',
	Approved = 'Approved',
	Canceled = 'Canceled',
}

export enum UserInquiryStatusByApprover {
	Pending = 'Pending',
	Rejected = 'Rejected',
	Accepted = 'Accepted',
	WaitingForApprove = 'WaitingForApprove',
	Cancelled = 'Cancelled',
}

export interface UserInquiryApprover {
	id: number;
	firstName: string;
	lastName: string;
	email: string;
	departmentId: string;
	countryId: null;
	avatarFieldId: null | string;
	position: null | string;
	color: string | null;
	registeredLocationId: number | null;
	canUserInfoBeRetrievedByCurrentUser: boolean;
}

export interface UserInquiryAccepts {
	id: number;
	status: UserInquiryStatusByApprover;
	acceptUser: UserInquiryApprover;
	rejectedByUser: UserInquiryApprover | null;
	acceptedByUser: UserInquiryApprover | null;
	acceptPolicyScope: AllRolesOptions;
	acceptUserPosition: string;
}

interface InquiryDates {
	id: number;
	date: string;
	requestedDays: number;
	dateType: string;
	isExcludedFromInquiry: boolean;
	excludedDayName: null | string;
}

export interface UserInquiryFiles {
	id: number;
	fileId: string;
	fileName: string;
	fileSize: number;
}

export interface AbsenceRequest {
	id: number;
	countryId: number;
	inquiryUserId: number;
	inquiryTemplateId: number;
	inquiryTemplateName: string;
	isAgreement: boolean;
	inquiryType: AbsenceType;
	status: AbsenceRequestStatus;
	statusOrderWeight: number;
	notes: string | null;
	dateFrom: string;
	dateTo: string;
	balanceDays: number | null;
	balanceHours: number | null;
	remainingDays: number | null;
	totalDays: number | null;
	totalHours: number | null;
	totalAccountantDays: number | null;
	compensationAmount: number | null;
	canCurrentUserApprove: boolean | null;
	canCurrentUserReject: boolean | null;
	createdById: number;
	createdAtUtc: string;
	updatedById: string;
	userInquiryAccepts: UserInquiryAccepts[];
	userInquiryDates: InquiryDates[];
	userInquiryFiles: UserInquiryFiles[];
	createdByUser: UserInquiryApprover;
	managerOfUser: UserInquiryApprover | null;
	createdForUser: UserInquiryApprover;
}

export type AbsenceRequestGridColumns =
	| 'dateFrom'
	| 'dateTo'
	| 'inquiryTemplateName'
	| 'status'
	| 'quantity';

export interface AbsenceRequestFilters {
	dateFrom: FromToProps<Date>;
	dateTo: FromToProps<Date>;
	inquiryTemplateName: string[];
	status: string[];
}

export enum AbsenceRequestsTableFiltersAction {
	DateFrom = 'dateFrom',
	DateTo = 'dateTo',
	InquiryTemplateName = 'inquiryTemplateName',
	Status = 'status',
	Reset = 'reset',
}

export type AbsenceRequestsFilterHandlers = FiltersHandlers<
	AbsenceRequestsTableFiltersAction,
	AbsenceRequestFilters
>;

export type AbsenceRequestsReducerAction = FilterReducerAction<
	AbsenceRequestsTableFiltersAction,
	AbsenceRequestFilters
>;

export type AbsenceRequestsFilterReducer = FilterReducer<
	AbsenceRequestFilters,
	AbsenceRequestsFilterHandlers
>;

export interface AbsenceStatusEntity {
	requestStatus: AbsenceRequestStatus;
	approvals: UserInquiryAccepts[];
	canCurrentUserApprove?: boolean | null;
	canCurrentUserReject?: boolean | null;
}

export interface AbsenceRequestGridRow extends GridCellDefaultProps {
	id: number;
	dateFrom: string;
	dateTo: string;
	inquiryTemplateName: string;
	inquiryType: string;
	status: AbsenceStatusEntity;
	quantity: string;
	createdForUser: UserInquiryApprover;
}

export type AbsenceTransactionHistoryGridColumns =
	| 'transactionDate'
	| 'inquiryTemplateName'
	| 'description'
	| 'used'
	| 'accrued'
	| 'remainder'
	| 'documents';

export enum AbsenceTransactionHistoryType {
	Accrual = 'Accrual',
	Using = 'Using',
	Disposal = 'Disposal',
}

export interface AbsenceTransactionHistory {
	id: number;
	amountOfDays: number;
	transactionDate: string;
	transactionType: AbsenceTransactionHistoryType;
	trackingUnitType: TrackingUnitType;
	inquiryTemplateName: string;
	inquiryTemplateId: number;
	used: number | null;
	accrued: number | null;
	remainder: number | null;
	inquiryType: AbsenceType;
	documents: Document[];
	inquiryId: number;
	description: string | null;
	userAgreementId: number | null;
}

export interface AbsenceTransactionHistoryResponse
	extends ByPageResponse<AbsenceTransactionHistory> {
	totalUsed: number | null;
}

export interface AbsenceTransactionHistoryGridRow
	extends DynamicHeightCellDefaultProps {
	id: number;
	inquiryId: number;
	transactionDate: string;
	transactionType: AbsenceTransactionHistoryType;
	inquiryTemplateName: string;
	description: string;
	correctionDescription: string | null;
	used: string;
	accrued: string;
	remainder: string;
	files: Document[];
}

export enum AbsenceTrackingUnitType {
	Hour = 'hour',
	Day = 'day',
}

export interface AbsenceAssignmentValues {
	assigneUserIds: number[];
	unassigneUserIds: number[];
	inquiryTemplateId: number | null;
	agreementId: number | null;
	effectiveFrom: string | null;
}
export interface InquiryData {
	id: number;
	userId: number;
	userFirstName: string;
	userLastName: string;
	userFirstNameRu: string | null;
	userLastNameRu: string | null;
	userFullName: string;
	userPosition: Roles;
	userResourceGroupId: number | null;
	isUserActive: boolean;
	agreementTemplateId: number | null;
	userDepartmentId: string | null;
	inquiryType: AbsenceType;
	inquiryTemplateId: number;
	inquiryTemplateName: string;
	isAgreement: boolean;
	managedByRoleRequest: boolean;
	dateFrom: string;
	dateTo: string;
	totalDays: number | null;
	totalHours: number | null;
	totalAccountantDays: number | null;
	status: AbsenceRequestStatus;
	notes: string | null;
	createdById: number | null;
	createdAtUtc: string;
	userInquiryAccepts: UserInquiryAccepts[];
}

export type UseGetInquiryResponse = ByPageResponse<InquiryData>;

type AbsenceRequestQueryFilter = Partial<
	AbsenceRequestFilters & { id: number }
>;

export interface GetAbsenceRequestQueryParams
	extends OdataParams<AbsenceRequestQueryFilter> {
	userId: number | undefined;
	numberOfAbsences: number;
}

interface TransactionHistoryFilters {
	inquiryTemplateName: string | null;
	year: string | null;
}

type TransactionHistoryQueryFilter = Partial<TransactionHistoryFilters>;

export interface GetTransactionHistoryQueryParams
	extends OdataParams<TransactionHistoryQueryFilter> {
	userId: number | undefined;
}

export enum ManageRequestType {
	Accept = 'accept',
	Reject = 'reject',
}

export type AgreementAssignsItem = {
	userId: number;
	userFullName: string;
	department: string | null;
	position: Roles;
	avatarFileId: string | null;
	userFirstName: string;
	userLastName: string;
	dateFrom: string;
	dateTo: string | null;
};

export interface AgreementCorrectionResponse {
	id: number;
}

export interface AgreementCorrectionValues {
	userId: number | string;
	agreementId: number | string;
	daysCount: string;
	description: string;
}

export enum AllRolesOptions {
	CEO_CTO = `${Roles.CEO}, ${Roles.CTO}`,
	HOD = Roles.HD,
	GM = Roles.GM,
	PM = 'ProjectManager',
	AnyUser = 'User',
}

export interface InquiryLookup {
	id: number;
	name: string;
	inquiryType: AbsenceType;
}

export interface InquiryCalendarItem {
	id: number;
	inquiryType: AbsenceType;
	inquiryName: string;
	status: AbsenceRequestStatus;
	dateFrom: string;
	dateTo: string;
	totalAcceptUsersCount: number;
	totalAcceptedCount: number;
	totalHours?: number;
}

export type InquiryCalendarItemWithLevel = InquiryCalendarItem & {
	level: number;
	isStartPart?: boolean;
	isEndPart?: boolean;
};

export interface InquiryCalendarInfo<InquiryItemType> {
	userId: number;
	userFullName: string;
	departmentId: string;
	inquiries: InquiryItemType[];
	userPosition: Roles;
	isManagedByRoleRequest: boolean;
}

export type InquiryCalendarGroupRow = GridCellDefaultProps &
	InquiryCalendarInfo<InquiryCalendarItemWithLevel> & {
		maxLevel: number;
		department: string;
	};

export type UseGetInquiriesCalendarResponse = ByPageResponse<
	InquiryCalendarInfo<InquiryCalendarItem>
>;
