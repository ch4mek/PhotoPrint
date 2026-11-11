import { MutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_COLOR_PICKER_API_ENDPOINT } from '@/constants/queryPaths';

import { ServerResponseError } from './types';

export const useCreateColorPicker = (
	options: MutationOptions<
		string[],
		AxiosError<ServerResponseError>,
		{ color: string }
	>
) =>
	useMutation<string[], AxiosError<ServerResponseError>, { color: string }>({
		...options,
		mutationFn: (data) =>
			axios
				.post(GET_COLOR_PICKER_API_ENDPOINT, data)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
