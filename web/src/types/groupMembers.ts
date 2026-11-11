export type GroupMembersTableColumns =
	| 'userId'
	| 'userName'
	| 'email'
	| 'location';

export type GroupMembersTableFilters = {
	userId: string;
	userName: string;
	email: string;
	location: string[];
};

export type GroupMembersTableFilterSetters = {
	userId: (userId: string) => void;
	userName: (userName: string) => void;
	email: (email: string) => void;
	location: (locations: string[]) => void;
	set: (filters: GroupMembersTableFilters) => void;
	clear: () => void;
};

export enum GroupMembersActionKeys {
	userId = 'userId',
	userName = 'userName',
	email = 'email',
	location = 'location',
	set = 'set',
	clear = 'clear',
}

export type GroupMembersAction = {
	type: GroupMembersActionKeys;
	payload?: string | string[] | GroupMembersTableFilters;
};

export interface DepartmentGroup {
	id: number | string;
	name: string;
	departmentId: number | string;
}
