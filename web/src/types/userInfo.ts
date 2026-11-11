import { GridCellDefaultProps } from '@/components/Grid';
import { Roles } from '@/constants/roles';
import { ByPageResponse, Project } from '@/queries/types';

import { UserAccessClaims } from './claims';
import {
	LocationTypeId,
	PersistentConfigurationItem,
	ComplexConfigurableConfigurationItem,
	RegisteredLocationTypeId,
	CalendaryCountry,
} from './configuration';

export interface UserInfo {
	id: number;
	firstName: string;
	lastName: string;
	fullName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	fullNameRu: string | null;
	patronymicRu: string | null;
	color: string;
	avatarFileId: string;
	email: string;
	timeZone: string;
	position: Roles;
	rightPosition: string;
	departmentId: string;
	resourceGroupId: string | null;
	location: LocationTypeId;
	claims: UserAccessClaims;
	registeredLocation: RegisteredLocationTypeId;
	registeredLocationId: number | null;
	countryId?: string | number;
}

export interface ProjectManager {
	id: number;
	fullName: string;
}

export interface UserOnProject {
	userId: string;
	firstName: string;
	lastName: string;
	departmentId: string;
}

export interface LookupUserResponse {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	color: string;
	avatarFileId: string;
	positionDescription: string;
	isActive: boolean;
	departmentId: string | null;
	resourceGroupId: number | null;
	resourceGroupName?: string;
}

export type LookupFilterUserResponse = Omit<
	LookupUserResponse,
	'positionDescription'
>;

export interface UseUsersResponse {
	id: number;
	firstName: string;
	lastName: string;
	fullName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
	fullNameRu: string;
	color: string;
	avatarFileId: string;
	email: string;
	position: string;
	rightPosition: string;
	departmentId: string;
	rightDepartment: string;
	currentLocation: string | null;
	location: string;
	registeredLocation: string;
	registeredLocationId: number;
	isActive: boolean;
	resourceGroupName?: string;
	resourceGroupId?: number;
}
export interface UseUserResponse {
	id: number | string;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
	color: string;
	avatarFileId: string;
	email: string;
	position: Roles;
	rightPosition: string;
	departmentId: string;
	location: string;
	registeredLocation: string;
	registeredLocationId: number;
	currentLocation: string | null;
	projects: Array<{
		projectId: number;
		name: string;
		currentProjectId: number;
	}>;
	isActive: boolean;
	resourceGroupId?: number;
}

export interface UseUpdateUserParam {
	id: number;
	firstName: string | null;
	lastName: string | null;
	firstNameRu: string | null;
	lastNameRu: string | null;
	patronymicRu: string | null;
	email: string | null;
	color: string;
	position: PersistentConfigurationItem;
	currentLocation: string | null;
	location: PersistentConfigurationItem | null;
	registeredLocation: CalendaryCountry | null;
	department: ComplexConfigurableConfigurationItem | null;
	isActive: boolean;
	projects: Project[] | null;
	groupId?: number;
	countryId: number;
}
export interface UseUpdateUserResponse {
	id: number;
	firstName: string;
	lastName: string;
	fullName: string;
	color: string;
	avatarFileId: string;
	email: string;
	timeZone: string;
	position: string;
	rightPosition: string;
	departmentId: string;
	location: string;
	registeredLocation: string;
	isActive: boolean;
	groupId: number;
}

export interface UsersGridRow extends GridCellDefaultProps {
	fullName: string;
	id: number;
	firstName: string;
	lastName: string;
	email: string;
	position: string;
	positionKey: string;
	departmentfullname: string;
	group: string;
	location: string;
	color: string;
	isActive: boolean;
}

export interface UserSalary {
	id: number;
	userId: number;
	salaryFrom: string;
	salaryUsd: number;
	isCurrent: boolean;
}

export type UserSalaryResponse = ByPageResponse<UserSalary>;
