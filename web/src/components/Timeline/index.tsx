import { ChangeEvent, useCallback, useMemo } from 'react';

import { isDateTodayWithTimezone } from '@/helpers/bookingTime';

import {
	ALL_HOURS_START,
	BUSSINESS_HOURS_START,
	DEFAULT_TIMEZONE,
	TimelinePeriod,
} from './constants';
import { TimelineTable, TimelineTableWrapper, TimelineWrapper } from './styles';
import { TimelineProvider } from './TimelineContext';
import { TimelineControls } from './TimelineControls';
import { TimelineGrid } from './TimelineGrid';
import { TimelineSectionsColumn } from './TimelineSectionsColumn';
import { Section, TimelineProps } from './types';
import { convertTimeToCellIndex } from './utils/convertTimeToCellIndex';
import { createTimelineSkeleton } from './utils/createTimelineSkeleton';
import { parseCellIndexPeriod } from './utils/parseCellIndexPeriod';

export function Timeline({
	data = [],
	selectedTimePeriod,
	onTimePeriodSelection,
	selectedSectionId,
	selectedSectionName,
	onSectionSelection,
	selectedDate,
	onGoBackwardDate,
	onGoForwardDate,
	onGoTodayDate,
	isWorkingHoursOnly = true,
	onIsWorkingHoursOnlyChange,
	selectionPeriod = TimelinePeriod.HOUR_PERIOD,
	selectFromPeriodStart = false,
	isPeriodSelectionDisabled = false,
	hasCurrentTimeIndicator = true,
	showLoader = false,
	scrollToCurrentTime,
	scrollToSelectedPeriod,
	timezone = DEFAULT_TIMEZONE,
	meetingRoomsConfig,
	onSetItemToDelete,
	handleCancelClick,
}: TimelineProps) {
	const baseHour = isWorkingHoursOnly ? BUSSINESS_HOURS_START : ALL_HOURS_START;

	const parsedSelectedPeriod = useMemo(() => {
		if (!selectedTimePeriod) {
			return null;
		}

		const { from, to, isOverlapped, isLoading } = selectedTimePeriod;

		const fromCellIndex = convertTimeToCellIndex(from, baseHour);
		const toCellIndex = convertTimeToCellIndex(to, baseHour);

		return {
			from: fromCellIndex,
			to: toCellIndex,
			isOverlapped,
			isLoading,
		};
	}, [selectedTimePeriod, baseHour]);

	const dataToShow = useMemo(() => {
		if (showLoader) {
			return createTimelineSkeleton(meetingRoomsConfig);
		}

		return data;
	}, [showLoader, data, meetingRoomsConfig]);

	const sections: Array<Section> = useMemo(
		() =>
			dataToShow.map((item) => ({
				meetingRoomId: item.meetingRoomId,
				meetingRoomName: item.meetingRoomName,
				countOfSeats: item.countOfSeats,
				equipments: item.equipments,
			})),
		[dataToShow]
	);

	const handleTimePeriodSelection = useCallback(
		(section: Section, from: number, to: number) => {
			if (typeof onTimePeriodSelection === 'function') {
				const { from: parsedFrom, to: parsedTo } = parseCellIndexPeriod(
					{
						from,
						to,
						isOverlapped: false,
						isLoading: false,
					},
					baseHour
				);

				onTimePeriodSelection(section, parsedFrom, parsedTo);
			}
		},
		[baseHour, onTimePeriodSelection]
	);

	const handleWorkingHoursChange = useCallback(
		(_: ChangeEvent<HTMLInputElement>, { checked }: { checked: boolean }) => {
			if (typeof onIsWorkingHoursOnlyChange === 'function') {
				onIsWorkingHoursOnlyChange(checked);
			}
		},
		[onIsWorkingHoursOnlyChange]
	);

	const timezoneName = timezone.timezone;

	const isSelectedDayToday = useMemo(
		() => isDateTodayWithTimezone(selectedDate, timezoneName),
		[timezoneName, selectedDate]
	);

	return (
		<TimelineWrapper>
			<TimelineControls
				selectedDate={selectedDate}
				isWorkingHoursOnly={isWorkingHoursOnly}
				onGoBackwardDate={onGoBackwardDate}
				onGoForwardDate={onGoForwardDate}
				timezone={timezone}
				onGoTodayDate={onGoTodayDate}
				onWorkingHoursChange={handleWorkingHoursChange}
				isSelectedDayToday={isSelectedDayToday}
			/>
			<TimelineTableWrapper scroll="both" maxHeight="100%" maxWidth="100%">
				<TimelineTable isPeriodSelectionDisabled={isPeriodSelectionDisabled}>
					<TimelineProvider
						isWorkingHoursOnly={isWorkingHoursOnly}
						selectFromPeriodStart={selectFromPeriodStart}
						selectionPeriod={selectionPeriod}
						isPeriodSelectionDisabled={isPeriodSelectionDisabled}
						hasCurrentTimeIndicator={hasCurrentTimeIndicator}
						showLoader={showLoader}
						scrollToCurrentTime={scrollToCurrentTime}
						scrollToSelectedPeriod={scrollToSelectedPeriod}
						timezone={timezone}
						isSelectedDayToday={isSelectedDayToday}
					>
						<TimelineSectionsColumn
							selectedSectionId={selectedSectionId}
							onSectionSelection={onSectionSelection}
							sections={sections}
						/>
						<TimelineGrid
							data={dataToShow}
							selectedDate={selectedDate}
							selectedSectionId={selectedSectionId}
							selectedSectionName={selectedSectionName}
							selectedPeriod={parsedSelectedPeriod}
							onPeriodSelect={handleTimePeriodSelection}
							onSetItemToDelete={onSetItemToDelete}
							handleCancelClick={handleCancelClick}
						/>
					</TimelineProvider>
				</TimelineTable>
			</TimelineTableWrapper>
		</TimelineWrapper>
	);
}
