import { useQueryClient } from '@tanstack/react-query';
import { ChangeEvent, useContext, useEffect, useMemo, useState } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { TimeSheetContext } from '../../..';
import { FactTimesheet, TimeSheetRow, TimeTrackItem } from '../../types';
import { getYYYYMMDDFormattedDateFromString } from '../../utils';
import { CellInput } from '../CellInput';

import { hoursToMinutes, minutesToHours } from '@/helpers/minutesToHours';
import { TIMESHEET_CACHE_KEY } from '@/pages/TimeSheet/components/TimesheetEngineer/constants';
import { useFactTimesheetMutations } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useFactTimesheetMutations';
import { useTooltipHandler } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useTooltipHandler';
import {
	TimesheetDeleteResponse,
	TimesheetUpsertResponse,
} from '@/queries/timesheet/types';
import { TrackedTimeType } from '@/types/index';

import { TimeInputCellWrapper } from './styles';

const getInitialInputValue = (trackItem: TimeTrackItem | undefined) =>
	minutesToHours(trackItem?.factTrackedInMinutes ?? 0);

export function TimeInputCell(props: RenderCellProps<TimeSheetRow, unknown>) {
	const { column, rowIdx, row } = props;

	const {
		isDisabledTimesheet,
		displayTooltips,
		onTaskTimeChange,
		onUpdateTrackItemAfterSuccessRequest,
		setTooltipContent,
		setIsCellLoading,
		isEngineer,
		userId,
	} = useContext(TimeSheetContext);

	const { key } = column;
	const { timeMap, areTimeCellsEditable, errorMessage, rowId } = row;

	const trackItem = timeMap.get(key);
	const uniqueCellKey = rowIdx.toString() + key;
	const queryId = trackItem?.trackId || uniqueCellKey;
	const cacheKey = [TIMESHEET_CACHE_KEY, queryId.toString()];

	const queryClient = useQueryClient();

	const isLoading = trackItem?.isLoading?.fact;

	const { handleMouseOut, handleMouseOver, tooltipMessage } = useTooltipHandler(
		{
			row,
			column,
			isEngineer,
			displayTooltips,
			setTooltipContent,
		}
	);

	const isCellDisabled = useMemo(
		() =>
			Boolean(isLoading) ||
			isDisabledTimesheet ||
			Boolean(tooltipMessage) ||
			Boolean(errorMessage) ||
			!areTimeCellsEditable,
		[
			isDisabledTimesheet,
			tooltipMessage,
			errorMessage,
			areTimeCellsEditable,
			isLoading,
		]
	);

	const [timeInput, setTimeInput] = useState<string>(
		getInitialInputValue(trackItem)
	);

	const handleInputChange = (value: string) => {
		if (isCellDisabled) {
			return;
		}

		if (value === timeInput) {
			return;
		}

		const factTimeMinutes = hoursToMinutes(value);

		setTimeInput(value);

		onTaskTimeChange(rowIdx, rowId, key, factTimeMinutes, null);
	};

	useEffect(() => {
		const newMinutes = trackItem?.factTrackedInMinutes ?? 0;

		if (newMinutes !== hoursToMinutes(timeInput)) {
			setTimeInput(getInitialInputValue(trackItem));
		}
	}, [trackItem]);

	const returnPreviousTime = (previousTime: number) => {
		setTimeInput(minutesToHours(previousTime));
		onTaskTimeChange(rowIdx, rowId, key, previousTime, null);
	};

	const handleError = (previousTrackItem: FactTimesheet) => {
		returnPreviousTime(previousTrackItem?.factTrackedInMinutes || 0);
	};

	const handleSettled = () => {
		setIsCellLoading(rowIdx, key, {
			fact: false,
		});
	};

	const handleSuccess = ({
		ftm,
		id,
	}: TimesheetDeleteResponse | TimesheetUpsertResponse) => {
		queryClient.setQueryData(
			[TIMESHEET_CACHE_KEY, id.toString(), TrackedTimeType.Fact],
			{
				id,
				factTrackedInMinutes: ftm,
			}
		);

		onUpdateTrackItemAfterSuccessRequest(rowIdx, rowId, key, {
			trackId: id,
			factTrackedInMinutes: ftm,
			reportForDate: getYYYYMMDDFormattedDateFromString(key),
			isLoading: {
				fact: false,
			},
		});
	};

	const {
		mutateFactDeleteTimesheet,
		mutateFactUpsertTimesheet,
		setDefaultFactCacheData,
		getFactCacheData,
	} = useFactTimesheetMutations(
		cacheKey,
		handleSuccess,
		handleError,
		handleSettled
	);

	const handleFactCellFocus = () => {
		setDefaultFactCacheData({
			id: queryId,
			factTrackedInMinutes: hoursToMinutes(timeInput),
		});
	};

	const handleInputBlur = (e: ChangeEvent<HTMLInputElement>) => {
		if (isCellDisabled || !trackItem) {
			return;
		}

		e.stopPropagation();
		e.preventDefault();

		const minutes = hoursToMinutes(e.target.value);

		const previousData = getFactCacheData();
		const previousFactTime = previousData?.factTrackedInMinutes || 0;
		const previousCellId = previousData?.id || 0;

		if (previousCellId === queryId && previousFactTime === minutes) {
			return;
		}

		setIsCellLoading(rowIdx, key, {
			fact: true,
		});

		if (minutes !== 0) {
			const { taskName, projectId } = row;

			const timeTrackItem = {
				id: previousCellId,
				taskName,
				projectId,
				reportForDate: getYYYYMMDDFormattedDateFromString(key),
				factTrackedInMinutes: minutes,
				userId,
			};

			mutateFactUpsertTimesheet(timeTrackItem);

			return;
		}

		if (minutes === 0 && trackItem?.trackId != null) {
			mutateFactDeleteTimesheet({ id: trackItem.trackId });
		}
	};

	return (
		<TimeInputCellWrapper
			onMouseEnter={handleMouseOver}
			onMouseOut={handleMouseOut}
		>
			<CellInput
				value={timeInput}
				onChange={handleInputChange}
				onBlur={handleInputBlur}
				onFocus={handleFactCellFocus}
				disabled={isCellDisabled}
			/>
		</TimeInputCellWrapper>
	);
}
