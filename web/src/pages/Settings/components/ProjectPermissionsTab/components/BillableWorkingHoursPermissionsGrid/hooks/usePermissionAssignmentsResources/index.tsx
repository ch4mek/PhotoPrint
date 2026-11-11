import axios, { AxiosError } from 'axios';

import { GET_PERMISSIONS_ASSIGNMENTS_RESOURCES_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';

import {
	PermissionAssignmentsResourcesResponse,
	UsePermissionAssignmentsResourcesProps,
} from './types';

export const usePermissionAssignmentsResources = ({
	userId,
	permissionType,
	resourceType,
	queryOptions,
}: UsePermissionAssignmentsResourcesProps) =>
	useQueryWithCallback<
		PermissionAssignmentsResourcesResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [userId, permissionType, resourceType],
		queryFn: async () => {
			const { data } = await axios.get<PermissionAssignmentsResourcesResponse>(
				GET_PERMISSIONS_ASSIGNMENTS_RESOURCES_API_ENDPOINT,
				{
					params: {
						userId,
						permissionType,
						resourceType,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,
		...queryOptions,
	});
