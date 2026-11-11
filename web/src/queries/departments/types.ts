import { DepartmentGroupManager } from '@/types/department';

export type GetDepartmentGroupManagersResponse = DepartmentGroupManager[];

export interface GetDepartmentGroupManagersEndpointParams {
	hasActiveGroup?: boolean;
}

export interface CreateGroupResponse {
	id: number;
}
