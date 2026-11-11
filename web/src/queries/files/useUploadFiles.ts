import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_FILES } from '@/constants/queryPaths';
import { FileWithName } from '@/forms/Absence/RequestAbsenceForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { DocumentResponse } from '@/types/document';

export const useUploadFiles = (
	options?: UseMutationOptions<
		DocumentResponse[],
		AxiosError<ServerResponseError>,
		FileWithName[]
	>
) =>
	useMutation<
		DocumentResponse[],
		AxiosError<ServerResponseError>,
		FileWithName[]
	>({
		...options,
		mutationFn: (files: FileWithName[]) => {
			const formData = new FormData();

			files.forEach((file) => {
				formData.append('Files', file);
			});

			return axios
				.post(GET_FILES, formData, {
					headers: {
						'Content-Type': 'multipart/form-data',
					},
				})
				.then((res) => res.data)
				.catch(handleRequestError);
		},
	});
