import { ByPageResponse } from '../types';

export interface UsersByProjectId {
	id: number;
	projectId: number;
	projectName: string;
	userId: number;
	userFullName: string;
	departmentId: string;
	isTimeOnProjectBillable: boolean;
	userOnProjectFrom: string;
	userOnProjectTo: string;
	isActive: boolean;
}

export type UseGetUsersByProjectIdResponse = ByPageResponse<UsersByProjectId>;
