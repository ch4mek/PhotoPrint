import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import {
	LabboardQueriesKeys,
	TeamboardQueriesKeys,
} from '@/constants/queriesKeys';
import { getInternshipEndpoint } from '@/constants/queryPaths';
import { UpdateInternship } from '@/forms/TeamboardForm/Internship/types';
import { getInternshipRequestFromFormValues } from '@/forms/TeamboardForm/Internship/utils';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { InternshipResponse } from '@/types/internship';

export const useUpdateInternship = (
	options?: UseMutationOptions<
		InternshipResponse,
		AxiosError<ServerResponseError>,
		UpdateInternship
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		InternshipResponse,
		AxiosError<ServerResponseError>,
		UpdateInternship
	>({
		...options,
		onSuccess: (...args) => {
			const [, { userId }] = args;

			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.internship, userId],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.employmentHistory, userId],
			});
			queryClient.invalidateQueries({
				queryKey: [LabboardQueriesKeys.labboardInterns],
			});

			return options?.onSuccess?.(...args);
		},
		mutationFn: async (values) => {
			const payload = getInternshipRequestFromFormValues(values);

			try {
				const { data } = await axios.put<InternshipResponse>(
					getInternshipEndpoint(values.userId),
					payload
				);

				return data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
	});
};
