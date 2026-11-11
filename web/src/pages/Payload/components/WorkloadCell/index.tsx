import { useTheme } from '@emotion/react';
import { FilledEditingButton } from '@mms/mms-ui-library/components/FilledEditingButton';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useRef, useState, MouseEvent } from 'react';
import type { KeyboardEvent as ReactKeyboardEvent } from 'react';

import { getCellBackgroundColor } from '../../utils/getCellBackgroundColor';

import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { useAuth } from '@/context/AuthContext';
import { usePayloadContext } from '@/context/PayloadContext';
import type { ActiveCellCoordinate } from '@/context/PayloadContext/types';
import { isCurrentYearMonth, isFutureMonth } from '@/helpers/date';
import { useKeyPress } from '@/hooks/useKeyPress';
import { useRowUpdater } from '@/pages/Payload/components/WorkloadCell/hooks/useRowUpdater';
import { TypeOfTimePart } from '@/pages/Payload/components/WorkloadCell/TimePart/types';
import {
	GRID_SUBROW_HEIGHT,
	HEIGHT_TIME_CELL,
	SUMMARY_ROW_HEIGHT,
	WORKLOAD_SUBROW_CLASS_NAME,
} from '@/pages/Payload/contants';
import { EditPayloadFteResponse } from '@/queries/types';
import { useUpdatePayloadFte } from '@/queries/useUpdatePayloadFte';
import { KeyboardKey } from '@/types/keyboard';

import {
	MASK_MAX,
	MASK_MIN,
	MASK_SCALE,
	REQUIRED_SPACE_FOR_BUTTONS,
} from './constants';
import {
	IconsContainer,
	LowerDateCell,
	PlannedTimeInput,
	PlannedTimeInputWrapper,
	UpperDateCell,
	WorkloadCellWrapper,
} from './styles';
import { TimePart } from './TimePart';
import { WorkloadCellProps } from './types';

const { Main, Secondary } = TypeOfTimePart;

export function WorkloadCell({
	projectId,
	disabled,
	workloads: {
		actualFte,
		plannedFte,
		requiredHours,
		trackedBillableInHours,
		plannedWorkloadInHours,
	},
	options: { isSubRows, needBackground, newHeight, isExpanded } = {},
	handleRowChange,
	month,
	userId,
	year,
	isLastRow = false,
	hasActivity = false,
}: WorkloadCellProps) {
	const { id: currentUserId } = useAuth();
	const { mode } = useTheme();
	const queryClient = useQueryClient();
	const {
		updateWorkloadValueInRow,
		showTime,
		gridWrapperBottomRef,
		navigateToCell,
		findRowIndexByUserId,
	} = usePayloadContext();

	const {
		inputState,
		inputRef,
		setPlannedHours,
		setIsInputActive,
		handlePlannedHoursAccept,
		acceptChanges,
		handleUpdateError,
	} = useRowUpdater(plannedFte, handleRowChange);

	const [shouldPlaceButtonsAbove, setShouldPlaceButtonsAbove] = useState(false);
	const cellRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		if (!cellRef.current || !hasActivity || !isLastRow) {
			return;
		}

		const cellRect = cellRef.current.getBoundingClientRect();

		if (
			gridWrapperBottomRef.current - cellRect.bottom <
			SUMMARY_ROW_HEIGHT + REQUIRED_SPACE_FOR_BUTTONS
		) {
			setShouldPlaceButtonsAbove(true);
		}
	}, []);

	const isFutureDate = Boolean(month && year && isFutureMonth(month, year));
	const isCurrentMonth = Boolean(
		month && year && isCurrentYearMonth(month, year)
	);

	const totalRequiredHours =
		isFutureDate && !isSubRows && plannedWorkloadInHours != null
			? plannedWorkloadInHours
			: requiredHours;

	const dependenceForBackgroundValue = isFutureDate
		? Number(inputState.plannedHours)
		: actualFte;

	const backgroundColor = getCellBackgroundColor(
		dependenceForBackgroundValue * 100,
		mode
	);

	const isCurrentUserCell = currentUserId === userId;

	const handleUpdateSuccess = (updatedFteData: EditPayloadFteResponse) => {
		if (isFutureDate) {
			updateWorkloadValueInRow(
				month!,
				updatedFteData.plannedWorkloadInHours,
				userId
			);
		}

		queryClient.invalidateQueries({
			queryKey: [payloadQueriesKeys.getTotalPayloadData],
		});
	};

	const { mutateAsync, isPending } = useUpdatePayloadFte(
		{ userId: userId!, month: month!, year: year! },
		{
			onError: handleUpdateError(plannedFte),
			onSuccess: handleUpdateSuccess,
		}
	);

	const handleCellClick = useCallback(() => {
		if (!isPending && !isCurrentUserCell) {
			setIsInputActive(true);
		}
	}, [isPending, isCurrentUserCell]);

	const handleSave = useCallback(
		async (event?: MouseEvent) => {
			event?.stopPropagation();

			if (+inputState.plannedHours === plannedFte) {
				return;
			}

			const newValue = +inputState.plannedHours;
			acceptChanges(newValue);
			await mutateAsync({ fte: newValue });
		},
		[inputState.plannedHours, plannedFte, mutateAsync, acceptChanges]
	);

	const handleTabNavigation = useCallback(
		async (event: ReactKeyboardEvent<HTMLInputElement>) => {
			if (!inputState.isActive) {
				return;
			}

			if (event.key !== KeyboardKey.Tab) {
				return;
			}

			event.preventDefault();

			try {
				await handleSave();
			} catch (error) {
				return;
			}

			setIsInputActive(false);

			const currentCell: ActiveCellCoordinate = {
				userId: userId!,
				month: month!,
				year: year!,
				projectId,
				rowIndex: findRowIndexByUserId(userId!),
			};

			const direction = event.shiftKey ? 'previous' : 'next';
			navigateToCell(direction, currentCell);
		},
		[
			inputState.isActive,
			userId,
			month,
			year,
			projectId,
			handleSave,
			findRowIndexByUserId,
			navigateToCell,
			setIsInputActive,
		]
	);

	useKeyPress(KeyboardKey.Enter, () => {
		if (inputState.isActive) {
			handleSave();
		}
	});

	const handleBlur = useCallback(() => {
		setPlannedHours(plannedFte.toString());
		setIsInputActive(false);
	}, [plannedFte]);

	const additionalHeight = showTime ? HEIGHT_TIME_CELL : 0;
	const workloadCellMaxHeight = isSubRows
		? GRID_SUBROW_HEIGHT + additionalHeight
		: newHeight! + additionalHeight;

	const upperCellContent =
		isSubRows || !inputState.isActive ? (
			<TimePart
				disabled={disabled}
				type={Main}
				firstTime={actualFte}
				secondTime={+inputState.plannedHours}
				isCurrentDate={isCurrentMonth}
				isFutureDate={isFutureDate}
				isSubRow={isSubRows}
			/>
		) : (
			<PlannedTimeInputWrapper>
				<PlannedTimeInput
					mask={Number}
					disabled={disabled}
					scale={MASK_SCALE}
					radix="."
					signed={false}
					value={inputState.plannedHours}
					onAccept={handlePlannedHoursAccept}
					onBlur={handleBlur}
					onKeyDownCapture={handleTabNavigation}
					onKeyDown={handleTabNavigation}
					inputRef={inputRef}
					min={MASK_MIN}
					max={MASK_MAX}
					unmask
				/>
				<IconsContainer
					isLastRow={isLastRow}
					minHeight={isSubRows || !newHeight ? GRID_SUBROW_HEIGHT : newHeight}
					shouldPlaceButtonsAbove={shouldPlaceButtonsAbove}
				>
					<FilledEditingButton variant="cancel" onMouseDown={handleBlur} />
					<FilledEditingButton variant="apply" onMouseDown={handleSave} />
				</IconsContainer>
			</PlannedTimeInputWrapper>
		);

	return (
		<WorkloadCellWrapper
			maxHeight={isExpanded ? workloadCellMaxHeight : undefined}
			className={isSubRows ? WORKLOAD_SUBROW_CLASS_NAME : ''}
			data-subrowhoverindex={isSubRows ? `${userId}-${projectId}` : undefined}
			ref={cellRef}
		>
			{hasActivity ? (
				<>
					<UpperDateCell
						data-clickable="true"
						backgroundColor={needBackground ? backgroundColor : 'inherit'}
						isSubRows={isSubRows}
						onClick={handleCellClick}
						isDisabled={isPending || disabled}
						minHeight={isSubRows ? GRID_SUBROW_HEIGHT : newHeight}
						isClickable={!isCurrentUserCell}
					>
						{upperCellContent}
					</UpperDateCell>
					{showTime && (
						<LowerDateCell {...{ disabled }} height={HEIGHT_TIME_CELL}>
							<TimePart
								disabled={disabled}
								type={Secondary}
								firstTime={trackedBillableInHours}
								secondTime={totalRequiredHours}
								isCurrentDate={isCurrentMonth}
								isFutureDate={isFutureDate}
								isSubRow={isSubRows}
							/>
						</LowerDateCell>
					)}
				</>
			) : (
				<UpperDateCell
					backgroundColor="inherit"
					isSubRows={isSubRows}
					isDisabled
				/>
			)}
		</WorkloadCellWrapper>
	);
}
