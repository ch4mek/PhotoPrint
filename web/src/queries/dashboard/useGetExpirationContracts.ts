import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { dashboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_ACTIVE_EXPIRING_CONTRACTS } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ContractExpirationItem } from '@/types/dashboard';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetExpirationContracts = (
	departmentId: string | null,
	options?: Partial<
		PatchedQueryOptions<
			ContractExpirationItem[],
			AxiosError<ServerResponseError>
		>
	>
) =>
	useQueryWithCallback<
		ContractExpirationItem[],
		AxiosError<ServerResponseError>
	>({
		queryKey: [dashboardQueriesKeys.getContractExpirationLookup, departmentId],
		queryFn: () =>
			axios
				.get(GET_ACTIVE_EXPIRING_CONTRACTS, {
					params: { departmentId },
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		...options,
	});
