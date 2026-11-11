import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import {
	GET_EMPLOYMENT_HISTORY_API_ENDPOINT,
	getHireDateEndpoint,
} from '@/constants/queryPaths';
import { UpdateEmploymentHistory } from '@/forms/TeamboardForm/EmploymentHistory/types';
import { getDateFormat } from '@/helpers/getDateFormat';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { EmploymentHistoryResponse } from '@/types/employmentHistory';

export const useUpdateEmploymentHistory = (
	options:
		| UseMutationOptions<
				EmploymentHistoryResponse,
				AxiosError<ServerResponseError>,
				UpdateEmploymentHistory
		  >
		| undefined
) => {
	const queryClient = useQueryClient();

	return useMutation<
		EmploymentHistoryResponse,
		AxiosError<ServerResponseError>,
		UpdateEmploymentHistory
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.teamboardUsersLookup],
			});

			return options?.onSuccess?.(...args);
		},
		mutationFn: ({
			professionalActivityStartDate,
			hireDate,
			fireDate,
			ndaStartDate,
			ndaEndDate,
			userId,
		}) =>
			Promise.all([
				axios.patch(GET_EMPLOYMENT_HISTORY_API_ENDPOINT, {
					professionalActivityStartDate: getDateFormat(
						professionalActivityStartDate,
						YYYY_MM_DD_DAYJS_DATE_FORMAT
					),
					fireDate: getDateFormat(fireDate, YYYY_MM_DD_DAYJS_DATE_FORMAT),
					ndaStartDate: getDateFormat(
						ndaStartDate ?? null,
						YYYY_MM_DD_DAYJS_DATE_FORMAT
					),
					ndaEndDate: getDateFormat(
						ndaEndDate ?? null,
						YYYY_MM_DD_DAYJS_DATE_FORMAT
					),
					userId,
				}),
				axios.patch(getHireDateEndpoint(userId), {
					hireDate: getDateFormat(hireDate, YYYY_MM_DD_DAYJS_DATE_FORMAT),
				}),
			])
				.then(([employmentHistoryResponse, hireDateResponse]) => ({
					...employmentHistoryResponse.data,
					hireDate: hireDateResponse.data,
				}))
				.catch(handleRequestError),
	});
};
