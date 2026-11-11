import axios, { AxiosError } from 'axios';

import { RedirectKeys } from '@/constants/queriesKeys';
import { getRedirectInfoBuIdEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { RedirectInfo } from '@/types/redirect';

export const useGetRedirectInfoById = (id: string | undefined) =>
	useQueryWithCallback<RedirectInfo | undefined, AxiosError>({
		queryKey: [RedirectKeys.getRedirectInfoById, id],
		queryFn: async () => {
			if (id) {
				const res = await axios.get<RedirectInfo>(
					getRedirectInfoBuIdEndpointUrl(id)
				);

				return res.data;
			}

			return undefined;
		},
		refetchOnWindowFocus: false,
		enabled: !!id,
		retry: 0,
	});
