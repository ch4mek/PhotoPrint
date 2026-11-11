import { useMemo } from 'react';

import { employeeChartTypes, employerChartTypes } from '@/constants/charts';
import { date, months } from '@/constants/date';
import { isRegularEmployee } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { sumDuration } from '@/pages/Reports/components';
import { useGetPieChart } from '@/queries/useQueryPieChart';

import { EmployeeList } from './EmployeeList';
import { EmployerList } from './EmployerList';
import type { ReportListProps } from './types';

export function ReportsList(props: ReportListProps) {
	const { queryParams, reports } = props;
	const { role } = useAuth();
	const isEmployeeRole = isRegularEmployee(role);

	const {
		year,
		monthOfYear,
		departmentId,
		clientId,
		projectId,
		userId,
		trackedTimeType,
		groupId,
	} = queryParams;

	const queryPayload = useMemo(
		() => ({
			year: !year ? date.getUTCFullYear() : year,
			monthOfYear: monthOfYear
				? monthOfYear.map((month) => months.findIndex((m) => m === month) + 1)
				: undefined,
			departmentId,
			clientId,
			projectId,
			userId,
			trackedTimeType,
			groupId,
		}),
		[queryParams]
	);

	const {
		data: chartData,
		isFetching: isPieChartFetching,
		dataUpdatedAt: PieChartDataUpdatedAt,
	} = useGetPieChart(
		queryPayload.year,
		queryPayload.trackedTimeType,
		isEmployeeRole,
		queryPayload?.monthOfYear,
		queryPayload?.departmentId,
		queryPayload?.clientId,
		queryPayload?.projectId,
		queryPayload?.userId,
		queryPayload?.groupId
	);

	const allChartTypes = useMemo(() => {
		if (isEmployeeRole) {
			return employeeChartTypes;
		}

		return employerChartTypes;
	}, [isEmployeeRole]);

	const generalDuration = useMemo(
		() =>
			allChartTypes?.reduce(
				(prev, acc) => ({
					...prev,
					[acc]: sumDuration(chartData && chartData[acc]),
				}),
				{} as { [key: string]: string }
			),
		[PieChartDataUpdatedAt]
	);

	if (!reports) {
		return null;
	}

	if (!isEmployeeRole) {
		return (
			<EmployerList
				chartData={chartData || {}}
				generalDuration={generalDuration}
				isFetching={isPieChartFetching}
			/>
		);
	}

	return (
		<EmployeeList chartData={chartData} generalDuration={generalDuration} />
	);
}
