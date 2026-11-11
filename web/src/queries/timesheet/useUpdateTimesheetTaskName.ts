import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosResponse } from 'axios';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { GET_TIMESHEET_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import dayjs from '@/lib/dayjs';

import { UpdateTaskNameVariables } from './types';

export const useUpdateTimesheetTaskName = (
	options?: UseMutationOptions<unknown, unknown, UpdateTaskNameVariables>
) =>
	useMutation<unknown, unknown, UpdateTaskNameVariables>({
		...options,
		mutationFn: ({
			userId,
			projectId,
			oldTaskName,
			newTaskName,
			startDate,
			endDate,
		}: UpdateTaskNameVariables) => {
			const formattedStartDate = dayjs(startDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			);
			const formattedEndDate = dayjs(endDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			);

			return axios
				.patch(`${GET_TIMESHEET_API_ENDPOINT}/taskName`, {
					userId,
					startDate: formattedStartDate,
					endDate: formattedEndDate,
					oldTaskName,
					taskName: newTaskName,
					projectId,
				})
				.then((res: AxiosResponse) => res.data)
				.catch(handleRequestError);
		},
	});
