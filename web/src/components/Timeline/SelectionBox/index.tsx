import { useEffect, useMemo, useRef } from 'react';

import { useTimelineContext } from '../TimelineContext';

import { SelectionBoxWrapper } from './styles';
import { SelectionBoxProps } from './types';
import { calculateSelectedRangeStyle } from './utils';

export function SelectionBox({
	selectedPeriod,
	selectedSectionPositionIndex,
	selectedSectionId,
}: SelectionBoxProps) {
	const { scrollToSelectedPeriod, cellsAmount, timelineSectionsHeight } =
		useTimelineContext();

	const selectionRef = useRef<HTMLDivElement>(null);

	const { to, from, ...restProps } = selectedPeriod;

	const IS_OUT_OF_LIMIT = to > cellsAmount;

	useEffect(() => {
		if (scrollToSelectedPeriod) {
			selectionRef.current?.scrollIntoView({
				behavior: scrollToSelectedPeriod?.behavior,
				block: 'nearest',
			});
		}
	}, [selectedPeriod]);

	const selectedRangeStyle = calculateSelectedRangeStyle(
		from,
		Math.min(to, cellsAmount)
	);

	const top = useMemo(() => {
		const sections = Object.values(timelineSectionsHeight);

		return sections
			.slice(0, selectedSectionPositionIndex)
			.reduce((acc, height) => acc + height, 0);
	}, [timelineSectionsHeight, selectedSectionPositionIndex]);

	return (
		<SelectionBoxWrapper
			isOutOfLimit={IS_OUT_OF_LIMIT}
			top={top}
			timelineSectionsHeight={timelineSectionsHeight[selectedSectionId]}
			ref={selectionRef}
			{...restProps}
			{...selectedRangeStyle}
		/>
	);
}
