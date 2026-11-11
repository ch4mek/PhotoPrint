import { UseMutationOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { ByPageResponse, ServerResponseError } from '../types';

import { SortDirection } from '@/constants/sort';
import { LocationType } from '@/types/configuration';
import { GroupMembersTableFilters } from '@/types/groupMembers';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UseUserResponse, UseUsersResponse } from '@/types/userInfo';

export type UseGetGroupUsersProps = {
	groupId: number;
	options?: PatchedQueryOptions<
		ByPageResponse<GroupUsersContract>,
		AxiosError<ServerResponseError>
	>;
	rowsPerPage: number;
	page: number;
	filters: GroupMembersTableFilters;
	allLocations: LocationType[];
	order: SortDirection;
	orderBy: string;
};

export type UseGetDepartmentUsersWithoutGroupProps = {
	departmentId: string;
	options?: PatchedQueryOptions<
		Array<UseUsersResponse>,
		AxiosError<ServerResponseError>
	>;
};

export type UseAddUserToGroupProps = {
	groupId: number;
	options?: UseMutationOptions<
		UseUserResponse,
		AxiosError<ServerResponseError>,
		number
	>;
};

export type UseRevokeGroupMemberProps = {
	groupId: number;
	options?: UseMutationOptions<
		UseUserResponse,
		AxiosError<ServerResponseError>,
		number
	>;
};

export type GroupUsersContract = Pick<
	UseUsersResponse,
	'id' | 'fullName' | 'email' | 'isActive' | 'location'
>;
