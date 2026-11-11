import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	EquipmentAutoSelectFiltersResponse,
	ServerResponseError,
} from '../types';

import { EquipmentQueriesKeys } from '@/constants/queriesKeys';
import { GET_EQUIPMENT_AUTOSELECT_FILTERS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetEquipmentAutoSelectFilters = (
	options?: PatchedQueryOptions<
		EquipmentAutoSelectFiltersResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		EquipmentAutoSelectFiltersResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [EquipmentQueriesKeys.autoSelectFilters],
		queryFn: () =>
			axios
				.get(GET_EQUIPMENT_AUTOSELECT_FILTERS_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
