import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getFteChangeEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';

import {
	UpdateUserFte,
	EditPayloadFteResponse,
	UseUpdatePayloadFteProps,
	ServerResponseError,
} from './types';

export const useUpdatePayloadFte = (
	{ month, userId, year }: UseUpdatePayloadFteProps,
	options: UseMutationOptions<
		EditPayloadFteResponse,
		AxiosError<ServerResponseError>,
		UpdateUserFte
	>
) =>
	useMutation<
		EditPayloadFteResponse,
		AxiosError<ServerResponseError>,
		UpdateUserFte
	>({
		...options,
		mutationFn: async (params) =>
			axios
				.put<EditPayloadFteResponse>(
					getFteChangeEndpoint(year, month, userId),
					params
				)
				.then((res) => res.data)
				.catch(handleRequestError),
	});
