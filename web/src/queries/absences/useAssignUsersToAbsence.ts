import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ASSIGN_USERS_TO_ABSENCE_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { AbsenceAssignmentValues } from '@/types/absences';

export const useAssignUsersToAbsence = (
	options?: UseMutationOptions<
		null,
		AxiosError<ServerResponseError>,
		AbsenceAssignmentValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(
					ASSIGN_USERS_TO_ABSENCE_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
