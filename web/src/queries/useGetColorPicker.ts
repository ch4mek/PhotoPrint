import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_COLOR_PICKER_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ServerResponseError } from './types';

export const GET_USER_COLORS_QUERY_KEY = 'user-colors';

export const useGetColorPicker = (
	options?: PatchedQueryOptions<string[], AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback<string[], AxiosError<ServerResponseError>>({
		queryKey: [GET_USER_COLORS_QUERY_KEY],
		queryFn: () =>
			axios.get(GET_COLOR_PICKER_API_ENDPOINT).then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
