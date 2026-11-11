import debounce from 'lodash/debounce';
import { useCallback, useMemo, useState } from 'react';

import { NO_TRACKED_HOURS_MESSAGE } from '../constants';

import { Chart } from '@/components/Chart';
import { ChartStacked } from '@/components/ChartStacked';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { NotificationType } from '@/constants/notifications';
import { PageTitles } from '@/constants/pageTitles';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { getDataByPeriod, getStackedDataByPeriod } from '@/helpers/reportStats';
import { minsToHrsAndMinsObject } from '@/helpers/translateTimeUtils';
import { ReportsStatistic } from '@/pages/Reports/components/ReportsStatistic';
import {
	DEBOUNCE_COUNT,
	FILTERS_MESSAGE,
	REPORTS_LOADER_TEST_ID,
} from '@/pages/Reports/constants';
import { useGetReports } from '@/queries/useGetReports';

import {
	ReportControls,
	ReportsCollapsibleTable,
	ReportsList,
	getInitialReportsSearchParams,
} from './components';
import {
	PageWrapper,
	HeaderWrapper,
	StatisticsBlockWrapper,
	Warning,
	ReportsListContainer,
	ReportsListWrapper,
	ActivityTableWrapper,
} from './styles';
import { ReportsQueryParams } from './types';
import { getInitialDate, getInitialTimePeriodFilter } from './utils';

export function ReportsPage() {
	const { role, id } = useAuth();
	const [date, setDate] = useState(() => getInitialDate(id));
	const [queryParams, setQueryParams] = useState<ReportsQueryParams>(() =>
		getInitialReportsSearchParams(getInitialTimePeriodFilter(id), role, id)
	);
	const { open } = useToast();

	const debouncedShowToast = useCallback(
		debounce(open, DEBOUNCE_COUNT, { leading: true, trailing: false }),
		[open]
	);

	const showToast = useCallback(
		() =>
			debouncedShowToast(createToast(NotificationType.INFO, FILTERS_MESSAGE)),
		[]
	);

	const {
		data: reportsData,
		isFetching,
		isLoading,
		refetch,
	} = useGetReports(queryParams);

	const isReportsLoading = isLoading || isFetching;

	const totalTime = useMemo(() => {
		const { minutes, hours } = minsToHrsAndMinsObject(
			reportsData?.totalTimeInMinutes ?? 0
		);

		return `${hours}:${minutes}`;
	}, [reportsData?.totalTimeInMinutes]);

	const stackedChartData = useMemo(
		() => (reportsData ? getStackedDataByPeriod(date, reportsData) : []),
		[reportsData, date]
	);

	const chartData = useMemo(
		() => (reportsData ? getDataByPeriod(date, reportsData) : []),
		[reportsData, date]
	);

	const noHoursTracked = useMemo(
		() => !isFetching && !isLoading && !reportsData?.totalTimeInMinutes,
		[reportsData, isFetching, isLoading]
	);

	return (
		<PageWrapper>
			<HeaderWrapper>
				<PageHeader title={PageTitles.Reports} />
				<ReportControls
					date={date}
					queryParams={queryParams}
					setQueryParams={setQueryParams}
					showToast={showToast}
					setDate={setDate}
					role={role}
					refetch={refetch}
				/>
			</HeaderWrapper>

			<Loader isLoading={isReportsLoading} dataTestId={REPORTS_LOADER_TEST_ID}>
				{!noHoursTracked ? (
					<>
						<ReportsStatistic totalTime={totalTime} />
						<StatisticsBlockWrapper data-testid="barchart">
							{queryParams?.clientId || queryParams.userId ? (
								<ChartStacked
									data={stackedChartData}
									showLoader={isReportsLoading}
									xAxisLabel="name"
								/>
							) : (
								<Chart
									data={chartData}
									showLoader={isReportsLoading}
									xAxisLabel="name"
								/>
							)}
						</StatisticsBlockWrapper>

						{!isReportsLoading && (
							<ReportsListWrapper>
								<ReportsListContainer>
									<ReportsCollapsibleTable
										queryParams={queryParams}
										reports={reportsData}
									/>
									<ActivityTableWrapper>
										<ReportsList
											queryParams={queryParams}
											reports={reportsData}
										/>
									</ActivityTableWrapper>
								</ReportsListContainer>
							</ReportsListWrapper>
						)}
					</>
				) : (
					<Warning>{NO_TRACKED_HOURS_MESSAGE}</Warning>
				)}
			</Loader>
		</PageWrapper>
	);
}
