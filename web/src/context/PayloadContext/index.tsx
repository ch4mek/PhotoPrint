import {
	PropsWithChildren,
	createContext,
	useCallback,
	useContext,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';

import { useAuth } from '../AuthContext';

import { currentDate, PERIOD } from '@/constants/date';
import { formatToFixedPrecision } from '@/helpers/formatToFixedPrecision';
import { useTableState } from '@/pages/hooks/useTableState';
import {
	INITIAL_SPECIALIZATION_TITLE,
	START_MONTH_COLUMN_INDEX,
} from '@/pages/Payload/contants';
import { usePayloadFilters } from '@/pages/Payload/hooks/usePayloadFilters';
import { HoursDetail, PayloadGridRow } from '@/pages/Payload/types';

import { ActiveCellCoordinate, PayloadContextProps } from './types';

const PayloadContext = createContext({} as PayloadContextProps);

export function PayloadProvider({ children }: PropsWithChildren) {
	const [gridRows, setGridRows] = useState<PayloadGridRow[]>([]);
	const [year, setYear] = useState(currentDate.year());
	const [specializationTitle, setSpecializationTitle] = useState(
		INITIAL_SPECIALIZATION_TITLE
	);
	const [showTime, setShowTime] = useState(false);
	const gridWrapperBottomRef = useRef(0);
	const [gridTotalRow, setGridTotalRow] = useState<PayloadGridRow | null>(null);
	const { role, departmentId } = useAuth();

	const { order, orderBy, sortColumns, handleSortColumnChange } =
		useTableState();

	const {
		displayDepartmentColumn,
		displayGroupColumn,
		responseBodyFilters,
		gridFilters,
		setters,
	} = usePayloadFilters({
		departmentId,
		order,
		orderBy,
		role,
		specializationTitle,
	});

	const payloadGridRef = useRef<HTMLElement | null>(null);
	const scrollPositionRef = useRef(0);
	const activeCellRef = useRef<ActiveCellCoordinate | null>(null);

	const findRowIndexByUserId = useCallback(
		(userId: number) => gridRows.findIndex((row) => row.id === userId),
		[gridRows]
	);

	const findCellElement = useCallback(
		(cell: ActiveCellCoordinate): HTMLElement | null => {
			if (!payloadGridRef.current) {
				return null;
			}

			const monthIndex = cell.month - 1;
			const hoverColIndex = START_MONTH_COLUMN_INDEX + monthIndex;

			if (cell.projectId !== undefined) {
				const subrowIndex = `${cell.userId}-${cell.projectId}`;
				const selector = `[data-hovercolindex="${hoverColIndex}"] [data-subrowhoverindex="${subrowIndex}"]`;

				return payloadGridRef.current.querySelector(
					selector
				) as HTMLElement | null;
			}

			const selector = `[data-userid="${cell.userId}"][data-hovercolindex="${hoverColIndex}"]`;

			const result = payloadGridRef.current.querySelector(
				selector
			) as HTMLElement | null;

			return result;
		},
		[]
	);

	const activateCellInDOM = useCallback(
		(cell: ActiveCellCoordinate) => {
			const cellElement = findCellElement(cell);

			if (!cellElement) {
				return;
			}

			const clickableElement = cellElement.querySelector(
				'[data-clickable="true"]'
			);

			if (clickableElement instanceof HTMLElement) {
				clickableElement.click();
				activeCellRef.current = cell;
			}
		},
		[findCellElement]
	);

	const navigateToCell = useCallback(
		(
			direction: 'next' | 'previous',
			currentCell: ActiveCellCoordinate
		): void => {
			const actualRowIndex = findRowIndexByUserId(currentCell.userId);

			if (actualRowIndex === -1) {
				return;
			}

			const delta = direction === 'next' ? 1 : -1;
			const targetIndex = actualRowIndex + delta;

			if (targetIndex < 0 || targetIndex >= gridRows.length) {
				return;
			}

			const targetRow = gridRows[targetIndex];
			const monthKey = PERIOD[
				currentCell.month - 1
			].toLowerCase() as keyof PayloadGridRow;
			const monthData = targetRow[monthKey] as HoursDetail;

			const nextCell: ActiveCellCoordinate = {
				userId: targetRow.id,
				month: currentCell.month,
				year: targetRow.year,
				projectId: currentCell.projectId,
				rowIndex: targetIndex,
			};

			if (nextCell.projectId && !targetRow.expanded) {
				nextCell.projectId = undefined;
			}

			if (targetRow.disabled || !monthData?.hasActivity) {
				return navigateToCell(direction, {
					...nextCell,
					rowIndex: targetIndex,
				});
			}

			activateCellInDOM(nextCell);
		},
		[gridRows, findRowIndexByUserId, activateCellInDOM]
	);

	const updateWorkloadValueInRow = (
		month: number,
		updatedWorkloadValue: number,
		userId?: number
	) => {
		const updatedMonth = PERIOD[
			month - 1
		].toLowerCase() as keyof PayloadGridRow;

		setGridRows((prevState) => {
			if (!prevState) {
				return [];
			}

			return prevState.map((row) => {
				const payloadMonthData = row[updatedMonth] as HoursDetail;

				if (row.id !== userId || !payloadMonthData) {
					return row;
				}

				return {
					...row,
					[updatedMonth]: {
						...payloadMonthData,
						plannedWorkloadInHours:
							formatToFixedPrecision(updatedWorkloadValue),
					},
				};
			});
		});
	};

	const toggleShowTime = () => {
		setShowTime((prevShowTime) => !prevShowTime);
	};

	useEffect(() => {
		if (!showTime) {
			setGridRows((prevState) =>
				prevState.map((prevItem) => ({ ...prevItem, expanded: false }))
			);
		}
	}, [showTime]);

	const payloadContextValue: PayloadContextProps = useMemo(
		() => ({
			gridRows,
			setGridRows,
			year,
			setYear,
			specializationTitle,
			setSpecializationTitle,
			gridTotalRow,
			setGridTotalRow,
			updateWorkloadValueInRow,
			order,
			orderBy,
			sortColumns,
			handleSortColumnChange,
			displayDepartmentColumn,
			displayGroupColumn,
			responseBodyFilters,
			gridFilters,
			setters,
			showTime,
			toggleShowTime,
			payloadGridRef,
			scrollPositionRef,
			gridWrapperBottomRef,
			activeCellRef,
			navigateToCell,
			findRowIndexByUserId,
		}),
		[
			gridRows,
			year,
			specializationTitle,
			gridTotalRow,
			showTime,
			order,
			orderBy,
			sortColumns,
			handleSortColumnChange,
			displayDepartmentColumn,
			displayGroupColumn,
			responseBodyFilters,
			gridFilters,
			setters,
			navigateToCell,
			findRowIndexByUserId,
		]
	);

	return (
		<PayloadContext.Provider value={payloadContextValue}>
			{children}
		</PayloadContext.Provider>
	);
}

export const usePayloadContext = () => useContext(PayloadContext);
