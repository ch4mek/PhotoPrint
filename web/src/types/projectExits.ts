import { GridCellDefaultProps } from '@/components/Grid';
import { Roles } from '@/constants/roles';

export interface ProjectExitEntity {
	id: number;
	userId: number;
	fullName: string;
	departmentId: string;
	departmentName: string;
	userResourceGroupName: string;
	clientId: number;
	clientName: string;
	projectId: number;
	projectName: string;
	closingDate: string;
	closingReasonName: string;
	closingReasonId: number;
	isActive: boolean;
	isUserActive: boolean;
	userPosition: Roles;
	isManagedByRole: boolean;
}

export interface ProjectExitsGridRow extends GridCellDefaultProps {
	id: number;
	userId: number;
	fullName: string;
	department: string;
	group: string;
	clientName: string;
	projectName: string;
	closingDate: string | null;
	closingReason: string;
	isActive: boolean;
	canViewPersonalInfo: boolean;
}

export interface AddProjectExitPayload {
	userId: number;
	clientId: number;
	projectId: number;
	closingDate: string;
	closingReasonName: string;
}

export interface EditProjectExitPayload extends AddProjectExitPayload {
	projectExitId: number;
}

export interface ProjectExitUserResponse {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string | null;
	lastNameRu: string | null;
	color: string;
	avatarFileId: string;
	isActive: boolean;
	departmentId: string | null;
	resourceGroupId: number | null;
	resourceGroupName?: string;
}
