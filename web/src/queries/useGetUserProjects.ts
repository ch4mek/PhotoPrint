import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_REPORTS } from '@/constants/queryPaths';
import { generateMonths } from '@/helpers/date';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import type { ProjectRequest, ProjectResponse } from '@/types/project';

export const useGetUserProjects = (
	{
		userId,
		projectId,
		clientId,
		year,
		monthOfYear,
		departmentId,
		trackedTimeType,
	}: ProjectRequest,
	options: PatchedQueryOptions<Array<ProjectResponse>, AxiosError>
) =>
	useQueryWithCallback<Array<ProjectResponse>, AxiosError>({
		...options,
		queryKey: [
			`${GET_REPORTS}/${userId}`,
			userId,
			year,
			monthOfYear,
			projectId,
			trackedTimeType,
			clientId,
		],
		queryFn: () =>
			axios
				.get<Array<ProjectResponse>>(`${GET_REPORTS}/${userId}`, {
					params: {
						monthOfYear: monthOfYear?.length
							? generateMonths(monthOfYear as unknown as string[])
							: null,
						year,
						projectId,
						clientId,
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
