import { TrackedTimeType } from '.';

import { GridCellDefaultProps } from '@/components/Grid';
import { GridCellWithTrackedTimeProps } from '@/components/Grid/renderers/types';
import { ProjectManager, UserOnProject } from '@/types/userInfo';

export interface ProjectRequest {
	userId: number;
	year: number;
	monthOfYear: string[];
	departmentId?: string;
	clientId?: number;
	projectId?: number;
	trackedTimeType: TrackedTimeType;
}

export interface ProjectResponse {
	projectId: number;
	projectName: string;
	clientName: string;
	totalTimeInMinutes: number;
	tasksCount: number;
	color: string;
	userIsActive: boolean;
}

export interface ProjectEntity {
	id: number;
	name: string;
	color: string;
	clientId?: number;
	clientName: string;
	factTrackedTime: number | null;
	billableTrackedTime: number | null;
	managerId?: number;
	managerFullName?: string;
	isPublic: boolean;
	isBillable: boolean;
	isActive: boolean;
	projectUsers: Array<UserOnProject>;
	isSelfCreatedProject: boolean;
}

export interface ProjectLookupResponse {
	id: number;
	name: string;
	clientName: string;
	color: string;
	isBillable: boolean;
	isCurrentUserAbleToSeeBillableTrackedTime: boolean;
}

export enum ProjectType {
	Public = 'Public',
	Private = 'Private',
}

export interface UseUpdateProjectParams {
	name: string;
	color: string;
	clientId: number;
	isPublic: boolean;
	isBillable: boolean;
	client: {
		id: number;
		name: string;
	};
	manager: null | ProjectManager;
}

export interface PrivateProjectsLookupResponse {
	id: number;
	name: string;
	clientName: string;
	color: string;
	isBillable: boolean;
	isCurrentUserAbleToSeeBillableTrackedTime: boolean;
}

export interface ProjectsGridRow
	extends GridCellDefaultProps,
		GridCellWithTrackedTimeProps {
	id: number;
	name: string;
	color: string;
	clientName: string;
	factTrackedTime: number | null;
	billableTrackedTime: number | null;
	isBillable: boolean;
	isPublic: boolean;
	isPrivate: boolean;
	isActive: boolean;
	projectUsers: Array<string>;
}
