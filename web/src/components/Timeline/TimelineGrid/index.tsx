import { useRef } from 'react';

import { CurrentTimeIndicator } from '../CurrentTimeIndicator';
import { useScrollTimelineToCurrentTime } from '../hooks/useScrollTimelineToCurrentTime';
import { SelectionBox } from '../SelectionBox';
import { useTimelineContext } from '../TimelineContext';
import { TimelineRow } from '../TimelineRow';

import { CurrentDate } from './CurrentDate';
import { TimelineGridCellsWrapper, TimelineGridWrapper } from './styles';
import { TimeLabels } from './TimeLabels';
import { TimelineGridProps } from './types';

export function TimelineGrid({
	data,
	selectedDate,
	selectedPeriod,
	onPeriodSelect,
	selectedSectionId,
	selectedSectionName,
	onSetItemToDelete,
	handleCancelClick,
}: TimelineGridProps) {
	const {
		isPeriodSelectionDisabled,
		hasCurrentTimeIndicator,
		isSelectedDayToday,
		showLoader,
		cellsAmount,
		scrollToCurrentTime,
	} = useTimelineContext();

	const timelineGridRef = useRef<HTMLDivElement>(null);

	const IS_SELECTION_IN_RANGE =
		selectedPeriod && selectedPeriod.from < cellsAmount;

	const IS_SELECTION_SHOWN =
		!showLoader &&
		!isPeriodSelectionDisabled &&
		IS_SELECTION_IN_RANGE &&
		selectedSectionId &&
		selectedSectionName;

	useScrollTimelineToCurrentTime(
		timelineGridRef,
		selectedPeriod,
		scrollToCurrentTime
	);

	const displayIndicator = hasCurrentTimeIndicator && isSelectedDayToday;

	const positionIndex = data.findIndex(
		({ meetingRoomId }) => meetingRoomId === selectedSectionId
	);

	return (
		<TimelineGridWrapper ref={timelineGridRef} cellsAmount={cellsAmount}>
			{displayIndicator && <CurrentTimeIndicator />}
			<CurrentDate selectedDate={selectedDate} />
			<TimeLabels />
			<TimelineGridCellsWrapper>
				{IS_SELECTION_SHOWN && (
					<SelectionBox
						selectedSectionPositionIndex={positionIndex}
						selectedSectionId={selectedSectionId}
						selectedPeriod={selectedPeriod}
					/>
				)}
				{data.map(({ reservations, ...restProps }) => (
					<TimelineRow
						section={restProps}
						key={restProps.meetingRoomId}
						bookedData={reservations}
						onPeriodSelect={onPeriodSelect}
						selectedSectionName={selectedSectionName}
						selectedSectionId={selectedSectionId}
						selectedPeriod={selectedPeriod}
						onSetItemToDelete={onSetItemToDelete}
						handleCancelClick={handleCancelClick}
					/>
				))}
			</TimelineGridCellsWrapper>
		</TimelineGridWrapper>
	);
}
