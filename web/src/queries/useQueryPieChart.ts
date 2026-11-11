import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { TrackedTimeType } from '../types';

import { GET_REPORTS } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PieChartResponseFull } from '@/types/pieChart';

export const useGetPieChart = (
	year: number,
	trackedTimeType: TrackedTimeType,
	isEmployee: boolean,
	monthOfYear?: Array<number>,
	departmentId?: string,
	clientId?: number,
	projectId?: number,
	userId?: number,
	resourceGroupId?: string | number
) =>
	useQueryWithCallback<{ [key: string]: PieChartResponseFull[] }, AxiosError>({
		queryKey: [
			'reports-pie-chart',
			year,
			monthOfYear,
			departmentId,
			clientId,
			projectId,
			userId,
			trackedTimeType,
			resourceGroupId,
		],
		queryFn: () =>
			axios
				.get(`${GET_REPORTS}/${isEmployee ? 'pieChart' : 'pieCharts'}`, {
					params: {
						year,
						monthOfYear,
						departmentId,
						clientId,
						projectId,
						userId,
						trackedTimeType,
						resourceGroupId,
					},
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((res) => (isEmployee ? { perEmployee: res.data } : res.data)),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
