import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_ABSENCES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { Absence, AbsencesFormValues } from '@/types/absences';

export const useCreateAbsence = (
	options?: UseMutationOptions<
		Absence,
		AxiosError<ServerResponseError>,
		AbsencesFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(GET_ABSENCES_API_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
