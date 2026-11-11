import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError, AxiosResponse } from 'axios';

import { GET_FILE } from '@/constants/queryPaths';
import { AppFile, FilePurpose } from '@/types/file';

import { ServerResponseError } from './types';

export const useUploadFile = (
	options?: UseMutationOptions<
		AxiosResponse<AppFile>,
		AxiosError<ServerResponseError>,
		{ file: File; purpose?: FilePurpose }
	>
) =>
	useMutation({
		...options,
		mutationFn: ({ file, purpose = FilePurpose.General }) =>
			axios.post(
				GET_FILE,
				{
					file,
				},
				{
					headers: {
						'Content-Type': 'multipart/form-data',
					},
					params: {
						purpose,
					},
				}
			),
	});
