import { MutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_COLOR_PICKER_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';

import { ServerResponseError } from './types';

export const useDeleteColorPicker = (
	options: MutationOptions<
		string[],
		AxiosError<ServerResponseError>,
		{ color: string }
	>
) =>
	useMutation({
		...options,
		mutationFn: ({ color }: { color: string }) => {
			const modifiedColor = replaceSpecialCharacters(color);

			return axios
				.delete(`${GET_COLOR_PICKER_API_ENDPOINT}/${modifiedColor}`)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				});
		},
	});
