import { useMemo } from 'react';

import { BookedLabel } from '../BookedLabel';
import { usePeriodSelection } from '../hooks/usePeriodSelection';
import { HoursCell } from '../HourCell';
import { useTimelineContext } from '../TimelineContext';
import { BookedData } from '../types';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';

import { isAlmostEqual, normalizeCellIndex } from '@/helpers/bookingTime';

import { TimelineRowSkeleton, TimelineRowWrapper } from './styles';
import { TimelineRowProps } from './types';

export function TimelineRow({
	section,
	bookedData,
	selectedSectionName,
	selectedSectionId,
	selectedPeriod,
	onPeriodSelect,
	handleCancelClick,
	onSetItemToDelete,
}: TimelineRowProps) {
	const {
		showLoader,
		selectionPeriod,
		isPeriodSelectionDisabled,
		cellsAmount,
		timelineSectionsHeight,
		baseHour,
	} = useTimelineContext();

	const hourCellsArray = useMemo(
		() => Array(cellsAmount).fill(''),
		[cellsAmount]
	);

	const IS_SELECTION_DISABLED = isPeriodSelectionDisabled || showLoader;

	const { handleMouseDown } = usePeriodSelection(
		section,
		selectionPeriod,
		onPeriodSelect,
		IS_SELECTION_DISABLED,
		bookedData,
		baseHour
	);

	const isBookingSelected = (booking: BookedData) => {
		if (!selectedPeriod) {
			return false;
		}

		const rawBookingFrom = convertTimeToCellIndex(
			booking.fromDateTime,
			baseHour
		);
		const rawBookingTo = convertTimeToCellIndex(booking.toDateTime, baseHour);

		const rawSelectedFrom = selectedPeriod.from;
		const rawSelectedTo = selectedPeriod.to;

		const bookingFrom = normalizeCellIndex(rawBookingFrom);
		const bookingTo = normalizeCellIndex(rawBookingTo);
		const selFrom = normalizeCellIndex(rawSelectedFrom);
		const selTo = normalizeCellIndex(rawSelectedTo);

		return (
			isAlmostEqual(bookingFrom, selFrom) &&
			isAlmostEqual(bookingTo, selTo) &&
			booking.meetingRoomId === selectedSectionId
		);
	};

	return (
		<TimelineRowWrapper
			data-testid={`meeting-room-${section.meetingRoomId}`}
			timelineSectionsHeight={timelineSectionsHeight[section.meetingRoomId]}
			onMouseDown={handleMouseDown}
			cellsAmount={cellsAmount}
		>
			{hourCellsArray.map((_, index) => (
				<HoursCell key={index} isLast={index === hourCellsArray.length - 1} />
			))}
			{showLoader ? (
				<TimelineRowSkeleton animation="wave" cellsAmount={cellsAmount} />
			) : (
				bookedData.map((data) => (
					<BookedLabel
						{...data}
						key={data.id}
						selectedSectionName={selectedSectionName}
						isSelected={isBookingSelected(data)}
						onSetItemToDelete={onSetItemToDelete}
						handleCancelClick={handleCancelClick}
					/>
				))
			)}
		</TimelineRowWrapper>
	);
}
