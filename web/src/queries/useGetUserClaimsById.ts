import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { CLAIMS_QUERY_KEY } from '@/constants/queriesKeys';
import { getClaimsByUserIdAPIEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { AvailableUserClaimResponse } from '@/types/settings';

export const useGetUserClaimsById = (
	id: number,
	options?: PatchedQueryOptions<AvailableUserClaimResponse[]>
) =>
	useQueryWithCallback<AvailableUserClaimResponse[]>({
		...options,
		queryKey: [CLAIMS_QUERY_KEY],
		queryFn: () =>
			axios
				.get<AvailableUserClaimResponse[]>(getClaimsByUserIdAPIEndpoint(id))
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
