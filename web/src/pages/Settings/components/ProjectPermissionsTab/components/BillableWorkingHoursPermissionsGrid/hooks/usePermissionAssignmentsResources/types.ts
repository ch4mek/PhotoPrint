import { AxiosError } from 'axios';

import { ServerResponseError } from '@/queries/types';
import {
	PermissionTypeId,
	PermissionResourceTypeId,
} from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export interface UsePermissionAssignmentsResourcesProps {
	userId?: number;
	permissionType: PermissionTypeId;
	resourceType: PermissionResourceTypeId;
	queryOptions: PatchedQueryOptions<
		PermissionAssignmentsResourcesResponse,
		AxiosError<ServerResponseError>
	>;
}

interface PermissionAssignmentResource {
	resourceId: string;
	resourceName: string;
}

export interface PermissionAssignmentsResourcesResponse {
	userId: number;
	resources: PermissionAssignmentResource[];
}
