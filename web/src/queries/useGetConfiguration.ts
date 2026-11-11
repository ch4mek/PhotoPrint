import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { CONFIGURATION_QUERY_KEY } from '@/constants/queriesKeys';
import { GET_CONFIGURATION } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { Configuration } from '@/types/configuration';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetConfiguration = (
	options: PatchedQueryOptions<Configuration>
) =>
	useQueryWithCallback<Configuration>({
		...options,
		queryKey: [CONFIGURATION_QUERY_KEY],
		queryFn: () => axios.get(GET_CONFIGURATION).then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
