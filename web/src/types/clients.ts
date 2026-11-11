export interface ClientResponse {
	id: number;
	name: string;
	legalEntityName?: string | null;
	color: string;
	comments: string;
	TrackedTimeInMinutes: number;
	isActive: boolean;
	isProjectExist: boolean;
	registeredLocation: string;
	salesManagerId: number | null;
	salesManagerFullName: string | null;
}

export interface ClientProjectsResponse {
	id: number;
	name: string;
	color: string;
	isActive: boolean;
	factTrackedTime: number | null;
	billableTrackedTime: number | null;
}

interface Client {
	id: number;
	name: string;
	color: string;
	comments: string;
	registeredLocation: string;
	factTrackedTimeInMinutes: number | null;
	billableTrackedTimeInMinutes: number | null;
	isActive: boolean;
	isProjectExist: boolean;
	hasActiveProject: boolean;
	hasActiveAssignements: boolean | null;
}

export interface ClientsResponse {
	page: number;
	pageSize: number;
	total: number;
	items: Client[];
}

export interface ClientsLookupResponse {
	id: number;
	name: string;
	color?: string;
	comments?: string;
	trackedTime?: number;
	isDeleted?: boolean;
	isProjectExist?: boolean;
}

export interface ClientUpdateStatusResponse {
	isSuccessful: boolean;
	activeProjectsNames: string[];
	clientName: string;
}
