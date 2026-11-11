import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_REPORTS } from '@/constants/queryPaths';
import { generateMonths } from '@/helpers/date';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type { UserReportsRequest, ByPageResponse } from '@/queries/types';
import type { ReportResponseItem } from '@/types/report';

export const useGetUserReports = ({
	year,
	monthOfYear,
	departmentId,
	projectId,
	page,
	pageSize,
	clientId,
	userId,
	trackedTimeType,
	groupId,
}: UserReportsRequest) =>
	useQueryWithCallback<ByPageResponse<ReportResponseItem>, AxiosError>({
		queryKey: [
			'user-reports',
			year,
			monthOfYear,
			departmentId,
			projectId,
			page,
			pageSize,
			clientId,
			userId,
			trackedTimeType,
			groupId,
		],
		queryFn: () =>
			axios
				.get<ByPageResponse<ReportResponseItem>>(`${GET_REPORTS}/users`, {
					params: {
						year,
						monthOfYear: monthOfYear ? generateMonths(monthOfYear) : null,
						departmentId,
						projectId,
						page,
						pageSize,
						clientId,
						userId,
						trackedTimeType,
						resourceGroupId: groupId,
					},
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
