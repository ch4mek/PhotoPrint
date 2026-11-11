import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_REPORTS } from '@/constants/queryPaths';
import { generateMonths } from '@/helpers/date';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type { UserTasksRequest } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import type { TasksResponse } from '@/types/tasks';

export const useGetUserTasks = (
	{
		userId,
		projectId,
		year,
		monthOfYear,
		departmentId,
		trackedTimeType,
	}: UserTasksRequest,
	options: PatchedQueryOptions<Array<TasksResponse>, AxiosError>
) =>
	useQueryWithCallback<Array<TasksResponse>, AxiosError>({
		...options,
		queryKey: [`user-tasks/${userId}/${projectId}`],
		queryFn: () =>
			axios
				.get<Array<TasksResponse>>(`${GET_REPORTS}/${userId}/${projectId}`, {
					params: {
						year,
						monthOfYear: monthOfYear ? generateMonths(monthOfYear) : null,
						departmentId,
						trackedTimeType,
					},
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
