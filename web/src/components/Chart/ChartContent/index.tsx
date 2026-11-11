import { useTheme } from '@emotion/react';
import { useMemo } from 'react';
import {
	Bar,
	CartesianGrid,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis,
} from 'recharts';

import { useTooltipPosition } from '../../ChartCustomizedTooltip/hooks/useTooltipPosition';
import {
	HEIGHT_OF_CHART,
	BAR_ROUNDED,
	STROKE_DASH_LINE_SIZES,
} from '../constants';
import { CustomizedTooltip } from '../CustomizedTooltip';
import { CustomizedXAxisTick } from '../CustomizedXAxisTick';
import { CustomizedYAxisTick } from '../CustomizedYAxisTick';
import { ChartProps } from '../types';

import { DashedMask } from '@/components/DashedMask';

import {
	RESPONSIVE_CONTAINER_INLINE_STYLE,
	BAR_CHART_MARGINS,
	X_AXIS_MONTHLY_STATS_HEIGHT,
	X_AXIS_NOT_MONTHLY_STATS_HEIGHT,
	X_AXIS_TICK_MARGIN,
	X_AXIS_WIDTH,
	YAXIS_WIDTH,
	BAR_CHART_THROTTLE_DELAY,
} from './constants';
import { useChartSizes } from './hooks/useChartSizes';
import { BarChartCustom, ChartWrapper } from './styles';
import { extendChartData } from './utils';

export function ChartContent({ data, xAxisLabel }: ChartProps) {
	const theme = useTheme();
	const extendedData = useMemo(() => extendChartData(data), [data]);

	const isMonthlyStats = useMemo(() => {
		if (typeof data?.[0]?.name !== 'string') {
			return Boolean(data?.[0]?.name?.date && data?.[0]?.name?.weekDay);
		}

		return false;
	}, [data]);

	const { responsiveContainerMinWidth, barCategoryGapInPercents } =
		useChartSizes(extendedData.length);

	const {
		activeBarRef,
		isTooltipOnLeftSide,
		tooltipRef,
		barCharWrapperRef,
		tooltipPosition,
		handleMouseMove,
	} = useTooltipPosition();

	return (
		<ChartWrapper ref={barCharWrapperRef}>
			<ResponsiveContainer
				style={RESPONSIVE_CONTAINER_INLINE_STYLE}
				minHeight={HEIGHT_OF_CHART}
				minWidth={responsiveContainerMinWidth}
			>
				<BarChartCustom
					data={extendedData}
					barCategoryGap={barCategoryGapInPercents}
					margin={BAR_CHART_MARGINS}
					throttleDelay={BAR_CHART_THROTTLE_DELAY}
					onMouseMove={handleMouseMove}
				>
					<CartesianGrid
						stroke={theme.palette['graphics-divider-2']}
						vertical={false}
						strokeDasharray={STROKE_DASH_LINE_SIZES}
					/>
					<XAxis
						height={
							isMonthlyStats
								? X_AXIS_MONTHLY_STATS_HEIGHT
								: X_AXIS_NOT_MONTHLY_STATS_HEIGHT
						}
						dataKey={xAxisLabel}
						values={isMonthlyStats ? (data as unknown as string) : undefined}
						tickMargin={X_AXIS_TICK_MARGIN}
						interval={0}
						tick={<CustomizedXAxisTick />}
						width={X_AXIS_WIDTH}
						color={theme.palette['text-gray']}
					/>
					<YAxis
						tick={<CustomizedYAxisTick />}
						allowDecimals={false}
						color={theme.palette['text-gray']}
						width={YAXIS_WIDTH}
					/>
					<Tooltip
						content={
							<CustomizedTooltip
								ref={tooltipRef}
								isTooltipOnLeftSide={isTooltipOnLeftSide}
							/>
						}
						cursor={{ fill: 'transparent' }}
						position={tooltipPosition}
					/>
					<Bar
						ref={activeBarRef}
						dataKey="hoursForBar"
						fill={theme.palette['graphics-primary-2']}
						radius={BAR_ROUNDED}
						background={
							<DashedMask
								radius={BAR_ROUNDED}
								color={theme.palette['graphics-input-stroke']}
							/>
						}
					/>
				</BarChartCustom>
			</ResponsiveContainer>
		</ChartWrapper>
	);
}
