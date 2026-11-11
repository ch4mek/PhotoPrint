import { useCallback, useEffect, useRef, useState } from 'react';
import { Bar } from 'recharts';
import { BarRectangleItem } from 'recharts/types/cartesian/Bar';
import { CategoricalChartState } from 'recharts/types/chart/types';
import { Coordinate } from 'recharts/types/util/types';

import { CHART_TOOLTIP_ARROW_SIZE, CHART_TOOLTIP_GAP } from '../constants';

import { TooltipPayload } from '@/components/ChartStacked/types';

export function useTooltipPosition(isYAxisPositionFixed?: boolean) {
	const [barChartPayload, setBarChartPayload] =
		useState<CategoricalChartState | null>(null);
	const [tooltipPayload, setTooltipPayload] = useState<TooltipPayload | null>(
		null
	);
	const [isTooltipOnLeftSide, setIsTooltipOnLeftSide] = useState(false);
	const [tooltipPosition, setTooltipPosition] = useState<
		Partial<Coordinate> | undefined
	>();

	const barCharWrapperRef = useRef<HTMLDivElement>(null);
	const tooltipRef = useRef<HTMLDivElement>(null);
	const activeBarRef = useRef<Bar & SVGPathElement>(null);

	const calculateTooltipXAxisPosition = (
		activeBarData: BarRectangleItem | undefined,
		barChartWrapperWidth: number,
		tooltipWidth: number
	) => {
		let tooltipX;

		if (activeBarData && activeBarData.x && activeBarData.width) {
			const { x: activeBarPositionX, width: activeBarWidth } = activeBarData;

			tooltipX =
				activeBarPositionX +
				activeBarWidth +
				CHART_TOOLTIP_ARROW_SIZE +
				CHART_TOOLTIP_GAP;

			if (tooltipX + tooltipWidth >= barChartWrapperWidth) {
				tooltipX =
					tooltipX -
					activeBarWidth -
					tooltipWidth -
					CHART_TOOLTIP_ARROW_SIZE * 2 -
					CHART_TOOLTIP_GAP * 2;

				setIsTooltipOnLeftSide(true);
			} else {
				setIsTooltipOnLeftSide(false);
			}
		}

		return tooltipX;
	};

	const calculateTooltipYAxisPosition = (tooltipHeight: number) => {
		let tooltipY;

		if (tooltipPayload && tooltipPayload.y && tooltipPayload.height) {
			const { y, height } = tooltipPayload;

			tooltipY = y - tooltipHeight / 2 + height / 2;
		}

		return tooltipY;
	};

	const calculateTooltipPosition = (
		prevTooltipPosition: Partial<Coordinate> | undefined
	) => {
		if (barChartPayload === null) {
			return prevTooltipPosition;
		}

		const { activeTooltipIndex } = barChartPayload;
		const barChartWrapperRefCurrent = barCharWrapperRef.current;
		const tooltipRefCurrent = tooltipRef.current;
		const activeBarRefCurrent = activeBarRef.current;

		if (
			barChartWrapperRefCurrent &&
			tooltipRefCurrent &&
			activeBarRefCurrent &&
			activeTooltipIndex !== undefined
		) {
			const activeBarData =
				activeBarRefCurrent.props.data?.[activeTooltipIndex];
			const barChartWrapperWidth =
				barChartWrapperRefCurrent.getBoundingClientRect().width +
				barChartWrapperRefCurrent.scrollLeft;
			const tooltipBounds = tooltipRefCurrent.getBoundingClientRect();

			const tooltipX = calculateTooltipXAxisPosition(
				activeBarData,
				barChartWrapperWidth,
				tooltipBounds.width
			);

			const tooltipY = isYAxisPositionFixed
				? calculateTooltipYAxisPosition(tooltipBounds.height)
				: undefined;

			return { x: tooltipX, y: tooltipY };
		}
	};

	useEffect(() => {
		setTooltipPosition(calculateTooltipPosition);
	}, [barChartPayload, tooltipPayload, tooltipRef.current]);

	const handleMouseMove = useCallback((data: CategoricalChartState) => {
		setBarChartPayload((prevTooltipPayload) => {
			if (prevTooltipPayload === null) {
				return data;
			}

			if (!data.isTooltipActive) {
				return prevTooltipPayload;
			}

			return prevTooltipPayload.activeTooltipIndex === data.activeTooltipIndex
				? prevTooltipPayload
				: data;
		});
	}, []);

	const handleMouseEnter = useCallback((payload: TooltipPayload) => {
		setTooltipPayload(payload);
	}, []);

	return {
		activeBarRef,
		isTooltipOnLeftSide,
		tooltipRef,
		barCharWrapperRef,
		tooltipPosition,
		handleMouseMove,
		handleMouseEnter,
		tooltipPayload,
	};
}
