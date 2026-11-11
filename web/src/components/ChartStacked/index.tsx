import { useTheme } from '@emotion/react';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';
import { useMemo } from 'react';
import {
	Bar,
	CartesianGrid,
	Cell,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis,
} from 'recharts';

import { BAR_ROUNDED, STROKE_DASH_LINE_SIZES } from '../Chart/constants';
import { useTooltipPosition } from '../ChartCustomizedTooltip/hooks/useTooltipPosition';
import { DashedMask } from '../DashedMask';
import { Loader } from '../Loader';

import {
	BAR_CHART_MARGINS,
	HEIGHT_OF_CHART,
	RESPONSIVE_CONTAINER_INLINE_STYLE,
	SCROLLBAR_PROPS,
	STACKED_BAR_STRAIGHT,
	TOTAL_BAR_NAME,
	X_AXIS_MONTHLY_STATS_HEIGHT,
	X_AXIS_NOT_MONTHLY_STATS_HEIGHT,
	X_AXIS_TICK_MARGIN,
	X_AXIS_WIDTH,
	Y_AXIS_DOMAIN_MAX_BASE_SIZE,
	YAXIS_WIDTH,
} from './constants';
import { CustomizedTooltip } from './CustomizedTooltip';
import { CustomizedXAxisTick } from './CustomizedXAxisTick';
import { CustomizedYAxisTick } from './CustomizedYAxisTick';
import { useChartSizes } from './hooks/useChartSizes';
import { useStackedTransformData } from './hooks/useStackedTransformData';
import { BarChartCustom, InnerBugFixBox, OuterBugFixBox } from './styles';
import type { ChartStackedProps } from './types';

export function ChartStacked({
	data,
	showLoader,
	xAxisLabel,
}: ChartStackedProps) {
	const theme = useTheme();

	const { keyWithNameAndColorMap, transformedData, calculatedYAxisTicks } =
		useStackedTransformData(data);

	const {
		activeBarRef,
		barCharWrapperRef,
		tooltipPosition,
		tooltipRef,
		isTooltipOnLeftSide,
		tooltipPayload,
		handleMouseMove,
		handleMouseEnter,
	} = useTooltipPosition(true);

	const isMonthlyStats = useMemo(() => {
		if (typeof data?.[0]?.name !== 'string') {
			return Boolean(data?.[0]?.name?.date && data?.[0]?.name?.weekDay);
		}

		return false;
	}, [data]);

	const { responsiveContainerMinWidth, barCategoryGapInPercents } =
		useChartSizes(data.length);

	return (
		<Loader isLoading={showLoader} size="small" height={HEIGHT_OF_CHART}>
			<OuterBugFixBox>
				<InnerBugFixBox>
					<Scrollbar {...SCROLLBAR_PROPS} ref={barCharWrapperRef}>
						<ResponsiveContainer
							style={RESPONSIVE_CONTAINER_INLINE_STYLE}
							minHeight={HEIGHT_OF_CHART}
							minWidth={responsiveContainerMinWidth}
						>
							<BarChartCustom
								data={transformedData}
								barCategoryGap={barCategoryGapInPercents}
								margin={BAR_CHART_MARGINS}
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
									type="category"
									values={
										isMonthlyStats ? (data as unknown as string) : undefined
									}
									tickMargin={X_AXIS_TICK_MARGIN}
									interval={0}
									tick={<CustomizedXAxisTick />}
									width={X_AXIS_WIDTH}
									color={theme.palette['text-gray']}
								/>
								<YAxis
									width={YAXIS_WIDTH}
									tick={<CustomizedYAxisTick />}
									allowDecimals={false}
									color={theme.palette['text-gray']}
									includeHidden
									domain={[
										0,
										calculatedYAxisTicks.at(-1) ?? Y_AXIS_DOMAIN_MAX_BASE_SIZE,
									]}
									ticks={calculatedYAxisTicks}
								/>
								<Tooltip
									content={
										<CustomizedTooltip
											ref={tooltipRef}
											{...{ tooltipPayload, isTooltipOnLeftSide }}
										/>
									}
									isAnimationActive={false}
									cursor={{ fill: 'transparent' }}
									position={tooltipPosition}
								/>
								{keyWithNameAndColorMap.map(
									({ projectId, projectName, projectColor }) => (
										<Bar
											ref={activeBarRef}
											key={projectId}
											dataKey={projectId}
											name={projectName}
											stackId="col"
											fill={
												projectId === TOTAL_BAR_NAME
													? 'transparent'
													: projectColor
											}
											radius={BAR_ROUNDED}
											shape={
												projectId === TOTAL_BAR_NAME && (
													<DashedMask
														radius={BAR_ROUNDED}
														color={theme.palette['graphics-input-stroke']}
													/>
												)
											}
											onMouseEnter={handleMouseEnter}
										>
											{transformedData.map((_, index) => (
												<Cell
													key={`${projectId}-${index}`}
													radius={
														transformedData[index]?.lastProjectIndex ===
														projectId
															? BAR_ROUNDED
															: STACKED_BAR_STRAIGHT
													}
												/>
											))}
										</Bar>
									)
								)}
							</BarChartCustom>
						</ResponsiveContainer>
					</Scrollbar>
				</InnerBugFixBox>
			</OuterBugFixBox>
		</Loader>
	);
}
