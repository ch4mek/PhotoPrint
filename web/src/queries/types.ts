import { AxiosError } from 'axios';

import { TrackedTimeType } from '../types';

import { SortDirection } from '@/constants/sort';
import { Dayjs } from '@/lib/dayjs';
import { UserOption } from '@/pages/TimeSheet/components/UserDropdown/types';
import { OdataParams } from '@/types/common';
import { DepartmentType, JSStackType } from '@/types/configuration';
import { DevelopmentPlanFilters } from '@/types/developmentPlan';
import {
	AssignmentsHistoryItem,
	EquipmentItemResponse,
} from '@/types/equipment';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { MilitaryServiceStatus } from '@/types/personalData';
import { UserClaimTableFilters } from '@/types/settings';
import { SoftSkillTableFilters, UserSoftSkill } from '@/types/softSkills';
import { TeamboardTableMode, UserSpecializationInfo } from '@/types/teamboard';
import { ProjectManager, UserInfo } from '@/types/userInfo';

export interface UseUserInfoParams extends PatchedQueryOptions<UserInfo> {
	isChangedOnlyTimeZone: boolean;
}

interface ValidationFailureItem {
	ErrorMessage: string;
}

export interface ServerResponseError {
	Message: string;
	ValidationFailures?: Array<ValidationFailureItem>;
	message?: string;
}

export interface ByPageResponse<T> {
	page: number;
	pageSize: number;
	total: number;
	items: Array<T>;
}

interface ByPageRequest {
	page?: number;
	pageSize?: string | string[] | undefined;
}

export interface UpdateClientsParams {
	comments: string;
	name: string;
	legalEntityName?: string | null;
	registeredLocation: number;
	id: number;
	color: string;
	isDeleted?: boolean;
	salesManagerId: number | null;
}

export interface UpdateClientsResponse extends UpdateClientsParams {
	isProjectExist: boolean;
	trackedTime: number;
}

export interface UserReportsRequest extends Omit<ByPageRequest, 'pageSize'> {
	year?: number;
	monthOfYear?: string[];
	departmentId?: string;
	projectId?: number;
	clientId?: number;
	userId?: number;
	groupId?: number | string;
	trackedTimeType: TrackedTimeType;
	pageSize: string | number | string[];
}

export interface UserTasksRequest extends ByPageRequest {
	year: number | null;
	monthOfYear: string[];
	departmentId?: string;
	projectId: number;
	clientId?: number;
	userId: number;
	trackedTimeType: TrackedTimeType;
}

export interface UpdateProjectStatusParams {
	id: number;
	name: string;
	isActive: boolean;
}

export interface UpdateUserStatusParams {
	id: number;
	isActive: boolean;
	firstName: string;
	lastName: string;
}

export interface UsersLookupFiltersParams {
	keyWord: string;
	clientId: number | string | null;
	projectId: number | string | null;
	departmentId: string | null;
	period: Date[] | Dayjs[] | null;
	monthOfYear: string[];
	year: number;
	resourceGroupId?: number | string;
}

export interface CreateCertificateOptions {
	id?: string;
	firstName: string;
	lastName: string;
	date: Dayjs;
	department: DepartmentType;
	stack: JSStackType;
	initialFirstName: string;
	initialLastName: string;
}

export interface DeleteCertificateOptions {
	certificateId: number;
	userId: number;
}

export interface CreateClientOptions {
	name: string;
	legalEntityName?: string | null;
	comments?: string | null;
	salesManagerId: number | null;
	registeredLocation: number;
	color: string;
}

export interface CreateClientsResponse extends CreateClientOptions {
	id: number;
}

export interface CreateProjectOptions {
	client: { id: string | number };
	name: string;
	otherValues: unknown;
	isPublic: boolean;
	users: { id: string | number; firstName: string; lastName: string }[];
	manager: null | ProjectManager;
}

export interface Project {
	id: number;
	name: string;
	color: string;
	clientName: string;
}

export interface CreateUserOptions {
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
	dateOfBirth: string | null;
	militaryServiceStatus: MilitaryServiceStatus | null;

	personalPhone: string | null;
	telegram: string | null;
	personalEmail: string | null;
	personalEmailSecond: string | null;
	linkedIn: string | null;

	email: string | null;
	position: string;
	departmentId: string | null;
	resourceGroupId: number | null;

	hireDate: string | null;
	contractDateFrom: string | null;
	contractDateTo: string | null;
	isGraduateJobPlacement: boolean | null;

	currentLocation: string | null;
	location: string;
	registeredLocationId: number;
	countryId: number;
}

export interface ExportOptions {
	year: number;
	monthOfYear?: string[];
	departmentId?: string | null;
	groupId?: string | number | null;
	clientId?: number;
	projectId?: number;
	file?: string;
	userId?: number;
	trackedTimeType: TrackedTimeType;
	isPerUserExport: boolean;
}
export interface ExportConfig {
	url: {
		[key: string]: string;
	};
	headers: {
		[key: string]: {
			'Content-type': string;
			'Content-Disposition': string;
		};
	};
}

export interface ResponseUser {
	id: number;
	firstName: string;
	lastName: string;
}

interface FilterValues {
	hasAvailable: boolean;
	equipmentTypeIds: number[];
	equipmentLocations: number[];
	approvers: ResponseUser[];
	users: ResponseUser[];
}

export interface GetEquipmentsResponse
	extends ByPageResponse<EquipmentItemResponse> {
	filterValues: FilterValues;
}
export interface GetAssignmentsHistoryResponse
	extends ByPageResponse<AssignmentsHistoryItem> {}

export interface SignatureResponse {
	fullName: string;
	position: string | null;
	phone: string | null;
	email: string;
}
export interface PageableListOfItems<T> {
	blockedDays: string[];
	pageableListOfItems: ByPageResponse<T>;
}

export interface ProjectsLookupProps {
	isHeadOfDepartment?: boolean;
	roleFilter?: UserOption;
	search?: string;
	isPublic?: boolean;
	isActive?: boolean;
	enabled?: boolean;
	onlyManagedProjects?: boolean;
}

export interface UpdateClientStatusParams {
	id: number;
	newStatus: boolean;
}

export interface EditUserProjectAssignParams {
	id: number;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
	isTimeOnProjectBillable: boolean;
	fte: number;
	userOnProjectSeniorityLevelId: string;
	projectId: number;
	userId: number;
}

export interface UserProjectAssignmentsByIdResponse {
	id: number;
	projectId: number;
	userId: number;
	projectName: string;
	userFullName: string;
	isTimeOnProjectBillable: boolean;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
	fte: number;
	seniorityLevel: string;
	isAssignEditable: boolean;
}

export interface RequestUserSoftSkill {
	seniorityLevelId: string;
	softSkillId: number;
}

export interface CreateUserSoftSkill extends UpdateUserSoftSkill {
	snapshotDate: string;
}

export interface UpdateUserSoftSkill {
	userSoftSkills: RequestUserSoftSkill[];
}

export interface AddUserProjectAssignParams {
	userId: number;
	projectId: number;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
	isTimeOnProjectBillable: boolean;
	userOnProjectSeniorityLevelId: string;
	fte: number;
}

export interface AddUserProjectAssignResponse {
	userId: number;
	projectId: number;
}
export interface EditUserProjectAssignResponse {
	id: number;
	userId: number;
	projectId: number;
}

export interface EditPayloadFteResponse {
	userId: number;
	year: number;
	month: number;
	fte: number;
	plannedWorkloadInHours: number;
}

export interface UpdateUserFte {
	fte: number;
}

export interface UseUpdatePayloadFteProps {
	year: number;
	month: number;
	userId: number;
}

export type UserHardSkillsResponse = ByPageResponse<UserSpecializationInfo>;

export interface GetAdminPortalSettingsResponse {
	name: string;
	value: string;
}

interface Claims {
	name: string;
	value: string | null;
}

interface GroupClaim {
	name: string;
	status: string;
}

export interface UserClaimsSubmitParams {
	userId: number;
	permissions: string[];
}
export interface GetAdminDepartmentClaimsResponse {
	departmentId: string;
	claims: Claims[];
}
export interface GetAdminRoleClaimsResponse {
	roleId: number;
	roleName: string;
	claims: Claims[];
}

export interface GetAdminUsersClaimsItemsResponse {
	userId: number;
	color: string;
	userFullName: string;
	departmentId: string;
	groups: GroupClaim[];
}

export interface GetAdminUsersClaimsResponse {
	page: number;
	pageSize: number;
	total: number;
	items: GetAdminUsersClaimsItemsResponse[];
}

export interface GetAdminUsersClaimsRequestParams {
	page: number;
	rowsPerPage: number;
	order?: SortDirection;
	orderByCondition?: string;
	filters?: Partial<UserClaimTableFilters>;
	queryOptions?: PatchedQueryOptions<
		GetAdminUsersClaimsResponse,
		AxiosError<ServerResponseError>
	>;
	allDepartmentsLength: number;
	claimName: string | null;
}

export interface EquipmentAutoSelectFiltersResponse {
	approvers: ResponseUser[];
	users: ResponseUser[];
}

export interface EmployeePayloadResponse {
	userId: number;
	comment: string | null;
	plan: string | null;
	needProject: boolean;
}

export interface UserDetailInfo {
	id: number;
	year: number;
}

export type UserSoftSkillsResponse = ByPageResponse<UserSoftSkill>;

export interface SoftSkillsParams {
	userId: number | undefined;
	top: number;
	skip: number;
	order: SortDirection;
	orderBy: string;
	filters?: Partial<SoftSkillTableFilters>;
}

export interface GetDevelopmentPlanQueryParams
	extends OdataParams<DevelopmentPlanFilters> {
	userId: number | undefined;
	mode?: TeamboardTableMode;
	conversationId?: number | undefined;
}

export interface NeedProjectSubmitParams {
	userId: number;
	needProject: boolean;
}

export interface MerchElementsLookupProps {
	enabled?: boolean;
}
