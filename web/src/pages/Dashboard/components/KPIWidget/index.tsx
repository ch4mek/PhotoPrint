import { useTheme } from '@emotion/react';
import { useMemo, useCallback } from 'react';
import { useTranslation } from 'react-i18next';
import {
	Area,
	CartesianGrid,
	ComposedChart,
	Line,
	ResponsiveContainer,
	XAxis,
	YAxis,
} from 'recharts';

import { useDashboardFilters } from '../../context/DashboardFiltersContext';

import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { Departments } from '@/constants/departments';
import { isChiefRole } from '@/constants/roles';
import { Routes } from '@/constants/routes';
import { useAuth } from '@/context/AuthContext';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { getDateLocale } from '@/i18n/locale';
import dayjs from '@/lib/dayjs';
import { useGetPayloadTotal } from '@/queries/payload/useGetPayloadTotal';

import { MonthTick } from './components/MonthTick';
import {
	AREA_STROKE_WIDTH,
	CHART_DATA_KEYS,
	CHART_HEIGHT,
	CHART_MARGIN,
	GRID_STROKE_DASHARRAY,
	getLegendItems,
	MONTH_OF_YEAR_DOMAIN,
	X_AXIS_MONTH_PADDING_LEFT,
	Y_AXIS_LABEL_OFFSET_X,
	Y_AXIS_LABEL_OFFSET_Y,
	X_AXIS_HEIGHT,
	X_AXIS_LABEL,
	X_AXIS_LABEL_OFFSET_X,
	X_AXIS_LABEL_OFFSET_Y,
	X_AXIS_MONTH_PADDING_RIGHT,
	NO_DATA_Y_AXIS_TICKS,
	NO_DATA_Y_AXIS_DOMAIN,
	KPI_WIDGET_DEPARTMENTS,
} from './constants';
import {
	KPIWidgetWrapper,
	LegendItem,
	LegendItemCircle,
	LegendItemText,
	LegendWrapper,
} from './styles';
import { TickProps } from './types';
import { convertDataForChart } from './utils/convertDataForChart';
import { getEmptyChartData } from './utils/getEmptyChartData';
import { getYAxisTicks } from './utils/getYAxisTicks';

export function KPIWidget() {
	const theme = useTheme();
	const navigate = useCustomNavigate();
	const { role, departmentId } = useAuth();
	const {
		t,
		i18n: { language },
	} = useTranslation('dashboard', { keyPrefix: 'kpi' });
	const { selectedDepartmentId } = useDashboardFilters();
	const dateLocale = getDateLocale(language);

	const departmentIds = useMemo(() => {
		if (!role) {
			return [];
		}

		if (!isChiefRole(role)) {
			return [departmentId as Departments];
		}

		return selectedDepartmentId
			? [selectedDepartmentId]
			: KPI_WIDGET_DEPARTMENTS;
	}, [selectedDepartmentId, role, departmentId]);

	const { data, isFetching: isLoading } = useGetPayloadTotal(dayjs().year(), {
		needProject: null,
		comment: null,
		plan: null,
		specializationName: null,
		projectName: null,
		userId: null,
		userFullName: null,
		departmentIds,
		resourceGroupName: null,
		orderingField: null,
		isDescendingOrdering: false,
	});

	const handleRedirectToTimesheet = () => {
		navigate({ to: `${Routes.payload}` });
	};

	const KPIChartData = useMemo(
		() => convertDataForChart(data ?? [], dateLocale),
		[data, dateLocale]
	);

	const emptyChartData = useMemo(
		() => getEmptyChartData(dateLocale),
		[dateLocale]
	);

	const yAxisTicks = useMemo(() => getYAxisTicks(KPIChartData), [KPIChartData]);

	const hasData = useMemo(
		() =>
			KPIChartData.some(
				(item) =>
					item[CHART_DATA_KEYS.users] || item[CHART_DATA_KEYS.totalActualFte]
			),
		[KPIChartData]
	);

	const chartData = hasData ? KPIChartData : emptyChartData;

	const renderMonthTick = useCallback(
		(props: TickProps) => (
			<MonthTick {...{ theme }} {...props} data={chartData} />
		),
		[chartData, theme]
	);

	return (
		<Widget
			title={t('title')}
			handleClick={handleRedirectToTimesheet}
			height={DASHBOARD_WIDGET_HEIGHT}
		>
			<Loader {...{ isLoading }} size="small">
				<KPIWidgetWrapper>
					<LegendWrapper>
						{getLegendItems(theme).map(({ id, content, color }) => (
							<LegendItem key={id}>
								<LegendItemCircle {...{ color }} />
								<LegendItemText variant="s-400">{t(content)}</LegendItemText>
							</LegendItem>
						))}
					</LegendWrapper>

					<ResponsiveContainer width="100%" height={CHART_HEIGHT}>
						<ComposedChart data={chartData} margin={CHART_MARGIN}>
							<CartesianGrid
								stroke={theme.palette['graphics-divider-2']}
								strokeDasharray={GRID_STROKE_DASHARRAY}
								vertical={false}
							/>

							<XAxis
								dataKey={CHART_DATA_KEYS.monthName}
								scale="band"
								stroke={theme.palette['graphics-divider-2']}
								tickLine={false}
								padding={{
									left: X_AXIS_MONTH_PADDING_LEFT,
									right: X_AXIS_MONTH_PADDING_RIGHT,
								}}
								tick={renderMonthTick}
								height={X_AXIS_HEIGHT}
								tickMargin={0}
								interval={0}
								label={{
									value: X_AXIS_LABEL,
									position: 'left',
									dx: X_AXIS_LABEL_OFFSET_X,
									dy: X_AXIS_LABEL_OFFSET_Y,
									fill: theme.palette['text-gray'],
									...theme.typography.body['xs-400'],
								}}
							/>
							<XAxis
								xAxisId="values"
								dataKey={CHART_DATA_KEYS.monthOfYear}
								type="number"
								domain={[...MONTH_OF_YEAR_DOMAIN]}
								hide
							/>

							<YAxis
								dataKey={CHART_DATA_KEYS.users}
								ticks={hasData ? yAxisTicks : NO_DATA_Y_AXIS_TICKS}
								domain={
									hasData
										? [0, yAxisTicks[yAxisTicks.length - 1]]
										: NO_DATA_Y_AXIS_DOMAIN
								}
								tick={{
									fill: theme.palette['text-gray'],
									...theme.typography.body['s-400'],
								}}
								strokeWidth={0}
								label={{
									value: t('users'),
									dx: Y_AXIS_LABEL_OFFSET_X,
									dy: Y_AXIS_LABEL_OFFSET_Y,
									position: 'top',
									fill: theme.palette['text-gray'],
									...theme.typography.body['xs-400'],
								}}
							/>

							<Line
								xAxisId="values"
								type="monotone"
								dataKey={hasData ? CHART_DATA_KEYS.users : ''}
								stroke={theme.palette['text-violet']}
								dot={false}
							/>
							<Area
								xAxisId="values"
								type="monotone"
								dataKey={hasData ? CHART_DATA_KEYS.totalActualFte : ''}
								stroke="url(#strokeGradient)"
								fill="url(#fillGradient)"
								strokeWidth={AREA_STROKE_WIDTH}
							/>

							<defs>
								<linearGradient id="strokeGradient" x1="0" y1="0" x2="1" y2="0">
									<stop
										offset="0%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0.2}
									/>
									<stop
										offset="25%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0.5}
									/>
									<stop
										offset="50%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={1}
									/>
									<stop
										offset="75%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0.5}
									/>
									<stop
										offset="100%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0.2}
									/>
								</linearGradient>
								<linearGradient id="fillGradient" x1="0" y1="0" x2="0" y2="1">
									<stop
										offset="0%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0.7}
									/>
									<stop
										offset="100%"
										stopColor={theme.palette['graphics-accent']}
										stopOpacity={0}
									/>
								</linearGradient>
							</defs>
						</ComposedChart>
					</ResponsiveContainer>
				</KPIWidgetWrapper>
			</Loader>
		</Widget>
	);
}
