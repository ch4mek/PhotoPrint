import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getUserTimesheetStatusByMonthEndpointUrl } from '@/constants/queryPaths';
import { TimesheetStatusType } from '@/pages/TimeSheet/types';

export interface UpdateTimesheetStatusProps {
	userId: number;
	status: TimesheetStatusType;
}

export const useUpdateTimesheetStatus = (
	userId: number,
	month: number,
	year: number,
	options?: UseMutationOptions<
		UpdateTimesheetStatusProps,
		AxiosError,
		UpdateTimesheetStatusProps,
		unknown
	>
) =>
	useMutation<
		UpdateTimesheetStatusProps,
		AxiosError,
		UpdateTimesheetStatusProps
	>({
		...options,
		mutationFn: async ({ status }) => {
			try {
				const { data } = await axios.put(
					`${getUserTimesheetStatusByMonthEndpointUrl(userId)}`,
					{ status },
					{
						params: {
							month,
							year,
						},
					}
				);

				return data;
			} catch (error: unknown | AxiosError) {
				return Promise.reject();
			}
		},
	});
