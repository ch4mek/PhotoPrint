export enum GroupMutationType {
	Add,
	Edit,
	Delete,
	None,
}

export interface CreateGroupRequestBody {
	groupName: string;
	groupManagerId?: number;
	departmentId: string;
}

export interface UpdateGroupRequestBody {
	id: number;
	name: string;
	departmentId: string;
	groupManagerId?: number;
}

export interface DepartmentGroup {
	id: number;
	name: string;
	groupManagerEmail?: string;
}

export interface DepartmentResponse {
	departmentName: string;
	headOfDepartmentId?: string;
	groups?: DepartmentGroup[];
}

export interface DepartmentGroupManager {
	id: number;
	firstName: string;
	lastName: string;
}

export interface Group {
	id: number;
	groupManagerId: number;
	departmentId: string;
	managerFirstName: string | null;
	managerLastName: string | null;
	name: string;
}

export interface GroupMutationOptions {
	isMutationLoading: boolean;
	handleMutation(
		values: CreateGroupRequestBody | UpdateGroupRequestBody | number
	): Promise<void>;
}
