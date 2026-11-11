import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getAbsenceByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { Absence } from '@/types/absences';

export const useUpdateAbsence = (
	options?: UseMutationOptions<
		Absence,
		AxiosError<ServerResponseError>,
		Absence
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.put(
					getAbsenceByIdApiEndpoint(data.id),
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
