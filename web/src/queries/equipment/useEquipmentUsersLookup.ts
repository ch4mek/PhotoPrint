import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_EQUIPMENT_USERS_LOOKUP } from '@/constants/queryPaths';
import { getKeywordQuery } from '@/helpers/queries';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { EquipmentUsersLookupResponse } from './types';

export const useEquipmentUsersLookup = (
	roles: Array<string> = [],
	queryOptions?: PatchedQueryOptions<
		Array<EquipmentUsersLookupResponse>,
		AxiosError
	>
) => {
	const query = getKeywordQuery({ roles });

	return useQueryWithCallback<Array<EquipmentUsersLookupResponse>, AxiosError>({
		...queryOptions,
		queryKey: ['equipment-users-lookup', query],
		queryFn: () =>
			axios
				.get<Array<EquipmentUsersLookupResponse>>(
					`${GET_EQUIPMENT_USERS_LOOKUP}${query}`
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};
