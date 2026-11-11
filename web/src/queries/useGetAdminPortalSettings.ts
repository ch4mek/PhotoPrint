import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SettingsPortalSettingsQueriesKeys } from '@/constants/queriesKeys';
import { GET_SETTINGS_PORTAL_SETTINGS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { GetAdminPortalSettingsResponse, ServerResponseError } from './types';

export const useGetPortalSettings = (
	options?: PatchedQueryOptions<
		GetAdminPortalSettingsResponse[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		GetAdminPortalSettingsResponse[],
		AxiosError<ServerResponseError>
	>({
		...options,
		queryKey: [SettingsPortalSettingsQueriesKeys.portalSettings],
		queryFn: () =>
			axios
				.get(GET_SETTINGS_PORTAL_SETTINGS_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
