import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { TrackedTimeType } from '../types';

import { ChartStackedResponse } from '@/components/ChartStacked/types';
import { date } from '@/constants/date';
import { ReportsQueriesKeys } from '@/constants/queriesKeys';
import { GET_REPORTS } from '@/constants/queryPaths';
import { generateMonths } from '@/helpers/date';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

interface UseGetReportsProps {
	departmentId?: string | null;
	clientId?: number;
	projectId?: number;
	groupId?: string | number;
	year?: number;
	monthOfYear?: string[];
	userId?: number;
	trackedTimeType: TrackedTimeType;
}

export const useGetReports = ({
	departmentId = null,
	clientId,
	projectId,
	groupId,
	year = date.getUTCFullYear(),
	monthOfYear = [] as string[],
	userId,
	trackedTimeType,
}: UseGetReportsProps) =>
	useQueryWithCallback({
		queryKey: [
			ReportsQueriesKeys.reports,
			clientId,
			projectId,
			year,
			monthOfYear,
			userId,
			trackedTimeType,
			departmentId,
			groupId,
		],
		queryFn: () =>
			axios
				.get<ChartStackedResponse>(GET_REPORTS, {
					params: {
						departmentId,
						clientId,
						projectId,
						userId,
						year,
						monthOfYear: generateMonths(monthOfYear),
						trackedTimeType,
						resourceGroupId: groupId,
					},
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
	});
