import { useQueryClient } from '@tanstack/react-query';
import { useContext, useEffect, useRef, FocusEvent, Dispatch } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { TimeSheetContext } from '../../../../..';
import {
	BillableTimesheet,
	FactTimesheet,
	TimeSheetRow,
} from '../../../../types';
import { getYYYYMMDDFormattedDateFromString } from '../../../../utils';
import { CellInput } from '../../../CellInput';

import { hoursToMinutes, minutesToHours } from '@/helpers/minutesToHours';
import { checkIsFocusOnCurrentDoubleCell } from '@/pages/TimeSheet/components/TimesheetEngineer/components/DoubleTimeInputCell/utils/checkIsFocusOnCurrentDoubleCell';
import { getTimeFromTrackItem } from '@/pages/TimeSheet/components/TimesheetEngineer/components/DoubleTimeInputCell/utils/getTimeFromTrackItem';
import { TIMESHEET_CACHE_KEY } from '@/pages/TimeSheet/components/TimesheetEngineer/constants';
import { useBillableTimesheetMutations } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useBillableTimesheetMutations';
import { useFactTimesheetMutations } from '@/pages/TimeSheet/components/TimesheetEngineer/hooks/useFactTimesheetMutations';
import {
	MutateBillableUpsertTimesheet,
	TimesheetDeleteResponse,
	TimesheetUpsertResponse,
} from '@/queries/timesheet/types';
import { TrackedTimeType } from '@/types/index';

const { Fact, Billable } = TrackedTimeType;

export function EditableInputCell(
	props: RenderCellProps<TimeSheetRow, unknown> & {
		setFactTimeInput: Dispatch<React.SetStateAction<string>>;
		setBillableTimeInput: Dispatch<React.SetStateAction<string>>;
		isFactDisabled: boolean;
		isBillableDisabled: boolean;
		isCellDisabled: boolean;
		billableTimeInput: string;
		factTimeInput: string;
		billableInputRef: any;
		factInputRef: any;
	}
) {
	const {
		column,
		rowIdx,
		row,
		billableTimeInput,
		factTimeInput,
		setFactTimeInput,
		setBillableTimeInput,
		isFactDisabled,
		isBillableDisabled,
		isCellDisabled,
		factInputRef,
		billableInputRef,
	} = props;
	const {
		onTaskTimeChange,
		onUpdateTrackItemAfterSuccessRequest,
		setIsCellLoading,
		userId,
	} = useContext(TimeSheetContext);

	const { key } = column;
	const { timeMap, rowId, isProjectPrivate, billabilityDays } = row;

	const { isBillable } = billabilityDays.find(
		({ date }) => date === getYYYYMMDDFormattedDateFromString(key)
	) ?? { isBillable: false };

	const trackItem = timeMap.get(key);
	const uniqueCellKey = rowIdx.toString() + column.key;
	const queryId = trackItem?.trackId || uniqueCellKey;
	const cacheKey = [TIMESHEET_CACHE_KEY, queryId.toString()];

	const queryClient = useQueryClient();

	const shouldDuplicateInputRef = useRef(
		isBillable &&
			isProjectPrivate &&
			getTimeFromTrackItem(trackItem, Fact) === 0 &&
			getTimeFromTrackItem(trackItem, Billable) === 0
	);
	const isCurrentFocusOnNextInput = useRef(false);

	useEffect(() => {
		if (
			isBillable &&
			isProjectPrivate &&
			getTimeFromTrackItem(trackItem, Fact) === 0 &&
			getTimeFromTrackItem(trackItem, Billable) === 0
		) {
			shouldDuplicateInputRef.current = true;
		}
	}, [billabilityDays, isProjectPrivate]);

	const handleUpdateAllLoaders = (isVisibleLoader: boolean) => {
		setIsCellLoading(rowIdx, key, {
			fact: isVisibleLoader,
			billable: isVisibleLoader,
		});
	};

	const handleFactInputChange = (value: string) => {
		if (isCellDisabled || value === factTimeInput) {
			return;
		}

		const newFactTime = hoursToMinutes(value);
		const newBillableTime = hoursToMinutes(billableTimeInput);
		const cachedFactValue = (
			queryClient.getQueryData([...cacheKey, Fact]) as FactTimesheet | undefined
		)?.factTrackedInMinutes;

		if (
			isBillable &&
			isProjectPrivate &&
			newFactTime === 0 &&
			newBillableTime === 0 &&
			getTimeFromTrackItem(trackItem, Fact) === 0 &&
			getTimeFromTrackItem(trackItem, Billable) === 0 &&
			cachedFactValue === 0
		) {
			shouldDuplicateInputRef.current = true;
		}

		setFactTimeInput(value);

		if (shouldDuplicateInputRef.current) {
			setBillableTimeInput(value);
			onTaskTimeChange(rowIdx, rowId, key, newFactTime, newFactTime);

			return;
		}

		onTaskTimeChange(rowIdx, rowId, key, newFactTime, null);
	};

	const handleBillableInputChange = (value: string) => {
		if (isCellDisabled || value === billableTimeInput) {
			return;
		}

		const billableTimeInMinutes = hoursToMinutes(value);

		setBillableTimeInput(value);

		onTaskTimeChange(rowIdx, rowId, key, null, billableTimeInMinutes);
	};

	const returnPreviousTime = (
		inputType: TrackedTimeType,
		previousTime: number
	) => {
		if (inputType === Fact) {
			setFactTimeInput(minutesToHours(previousTime));
			onTaskTimeChange(rowIdx, rowId, key, previousTime, null);

			return;
		}

		setBillableTimeInput(minutesToHours(previousTime));
		onTaskTimeChange(rowIdx, rowId, key, null, previousTime);
	};

	const handleFactError = (previousTrackItem: FactTimesheet) => {
		returnPreviousTime(Fact, previousTrackItem?.factTrackedInMinutes || 0);

		if (shouldDuplicateInputRef.current && !isCurrentFocusOnNextInput.current) {
			returnPreviousTime(
				Billable,
				previousTrackItem?.factTrackedInMinutes || 0
			);

			handleUpdateAllLoaders(false);
		}
	};

	const handleBillableError = (previousTrackItem: BillableTimesheet) => {
		returnPreviousTime(
			Billable,
			previousTrackItem?.billableTrackedInMinutes || 0
		);
	};

	const handleSettled = (
		inputType: TrackedTimeType,
		isVisibleLoader: boolean
	) => {
		setIsCellLoading(rowIdx, key, {
			[inputType]: isVisibleLoader,
		});
	};

	const handleFactSettled = (error?: unknown) => {
		const isSendBillableRequest =
			shouldDuplicateInputRef.current && !isCurrentFocusOnNextInput.current;

		handleSettled(Fact, isSendBillableRequest && !error);
	};

	const handleBillableSettled = () => {
		if (shouldDuplicateInputRef.current && !isCurrentFocusOnNextInput.current) {
			handleUpdateAllLoaders(false);
		} else {
			handleSettled(Billable, false);
		}
	};

	const handleSuccess =
		(
			inputType: TrackedTimeType,
			mutateBillableUpsertTimesheet?: MutateBillableUpsertTimesheet
		) =>
		(newTrackItem: TimesheetDeleteResponse | TimesheetUpsertResponse) => {
			const billableTrackedInMinutes =
				inputType === Billable
					? newTrackItem.btm
					: hoursToMinutes(billableTimeInput);
			const factTrackedInMinutes =
				inputType === Fact ? newTrackItem.ftm : hoursToMinutes(factTimeInput);

			const isSendBillableRequest =
				inputType === Fact &&
				shouldDuplicateInputRef.current &&
				!isCurrentFocusOnNextInput.current;

			const cacheField =
				inputType === Fact
					? 'factTrackedInMinutes'
					: 'billableTrackedInMinutes';

			queryClient.setQueryData(
				[TIMESHEET_CACHE_KEY, newTrackItem.id.toString(), inputType],
				{
					id: newTrackItem.id,
					[cacheField]:
						inputType === Billable ? newTrackItem.btm : newTrackItem.ftm,
				}
			);

			onUpdateTrackItemAfterSuccessRequest(rowIdx, rowId, key, {
				...trackItem,
				trackId: newTrackItem.id,
				billableTrackedInMinutes,
				factTrackedInMinutes,
				reportForDate: getYYYYMMDDFormattedDateFromString(key),
				isLoading: {
					[inputType]: isSendBillableRequest,
				},
			});

			if (isSendBillableRequest) {
				handleUpdateAllLoaders(true);
				const {
					id,
					userId: newTrackItemUserId,
					projectId,
					reportForDate,
					ftm,
					taskName,
				} = newTrackItem;

				const billableTimeTrackItem = {
					id,
					taskName,
					reportForDate,
					projectId,
					userId: newTrackItemUserId,
					billableTrackedInMinutes: ftm,
				};

				mutateBillableUpsertTimesheet?.(billableTimeTrackItem);
			}
		};

	const {
		mutateBillableUpsertTimesheet,
		mutateBillableDeleteTimesheet,
		setDefaultBillableCacheData,
		getBillableCacheData,
	} = useBillableTimesheetMutations(
		cacheKey,
		handleSuccess(Billable),
		handleBillableError,
		handleBillableSettled
	);

	const {
		mutateFactDeleteTimesheet,
		mutateFactUpsertTimesheet,
		setDefaultFactCacheData,
		getFactCacheData,
	} = useFactTimesheetMutations(
		cacheKey,
		handleSuccess(Fact, mutateBillableUpsertTimesheet),
		handleFactError,
		handleFactSettled
	);

	const handleFactCellFocus = () => {
		const previousData = getFactCacheData();
		const newFactTime = hoursToMinutes(factTimeInput);
		const newBillableTime = hoursToMinutes(billableTimeInput);

		setDefaultFactCacheData({
			id: previousData?.id || queryId,
			factTrackedInMinutes: newFactTime,
		});

		shouldDuplicateInputRef.current =
			isBillable &&
			isProjectPrivate &&
			getTimeFromTrackItem(trackItem, Fact) === 0 &&
			getTimeFromTrackItem(trackItem, Billable) === 0 &&
			newFactTime === 0 &&
			newBillableTime === 0;
	};

	const handleBillableCellFocus = () => {
		const previousData = getBillableCacheData();

		const previousBillableValue = shouldDuplicateInputRef.current
			? previousData?.billableTrackedInMinutes
			: hoursToMinutes(billableTimeInput);

		setDefaultBillableCacheData({
			id: previousData?.id || queryId,
			billableTrackedInMinutes: previousBillableValue,
		});
	};

	const handleFactBlur = (e: FocusEvent<HTMLInputElement>) => {
		if (isFactDisabled || !trackItem) {
			return;
		}

		isCurrentFocusOnNextInput.current = checkIsFocusOnCurrentDoubleCell(e);

		e.stopPropagation();
		e.preventDefault();

		const factMinutes = hoursToMinutes(e.target.value);

		const previousData = getFactCacheData();
		const previousFactTime = previousData?.factTrackedInMinutes || 0;
		const previousCellId = previousData?.id || 0;

		if (previousCellId === queryId && previousFactTime === factMinutes) {
			return;
		}

		setIsCellLoading(rowIdx, key, {
			fact: true,
			billable:
				shouldDuplicateInputRef.current && !isCurrentFocusOnNextInput.current,
		});

		if (factMinutes !== 0) {
			const { taskName, projectId } = row;

			const timeTrackItem = {
				id: previousCellId,
				taskName,
				projectId,
				reportForDate: getYYYYMMDDFormattedDateFromString(key),
				factTrackedInMinutes: factMinutes,
				userId,
			};

			mutateFactUpsertTimesheet(timeTrackItem);

			return;
		}

		if (factMinutes === 0 && trackItem?.trackId != null) {
			mutateFactDeleteTimesheet({ id: trackItem.trackId });
		}
	};

	const handleBillableBlur = (e: FocusEvent<HTMLInputElement>) => {
		if (isBillableDisabled || !trackItem) {
			return;
		}

		e.stopPropagation();
		e.preventDefault();

		const billableMinutes = hoursToMinutes(e.target.value);

		const previousData = getBillableCacheData();
		const previousBillableTime = previousData?.billableTrackedInMinutes || 0;
		const previousCellId = previousData?.id || 0;

		if (
			previousCellId === trackItem.trackId &&
			previousBillableTime === billableMinutes
		) {
			return;
		}

		if (billableMinutes === 0 && !trackItem.trackId) {
			return;
		}

		setIsCellLoading(rowIdx, key, {
			billable: true,
		});

		if (billableMinutes !== 0) {
			const { taskName, projectId } = row;

			const timeTrackItem = {
				id: previousCellId,
				taskName,
				projectId,
				reportForDate: getYYYYMMDDFormattedDateFromString(key),
				billableTrackedInMinutes: billableMinutes,
				userId,
			};

			mutateBillableUpsertTimesheet(timeTrackItem);

			return;
		}

		if (billableMinutes === 0 && trackItem.trackId !== 0) {
			mutateBillableDeleteTimesheet({ id: trackItem.trackId });
		}
	};

	return (
		<>
			<CellInput
				value={factTimeInput}
				onChange={handleFactInputChange}
				onBlur={handleFactBlur}
				disabled={isFactDisabled}
				onFocus={handleFactCellFocus}
				cellPosition="top"
				ref={factInputRef}
			/>
			<CellInput
				value={billableTimeInput}
				disabled={isBillableDisabled}
				onBlur={handleBillableBlur}
				onChange={handleBillableInputChange}
				onFocus={handleBillableCellFocus}
				cellPosition="bottom"
				ref={billableInputRef}
			/>
		</>
	);
}
