import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';

import { TOTAL_ROW_INDEX, EMPTY_ROW_ID } from '../constants';
import { TimeSheetRow, TimeTrackItem } from '../types';
import {
	getRowsWithUpdatedRowTime,
	getTotalTimeByDay,
	getDataGridRowsWithUpdatedAddRow,
	getRowsWithUpdatedTrackItem,
	createInitialNewTaskRow,
	createInitialAddTaskRow,
} from '../utils';

const NEW_TASK_ROW_OFFSET = 2;

export const useTimesheetRowsOperations = (
	setRows: Dispatch<SetStateAction<TimeSheetRow[]>>,
	isEngineer: boolean
) => {
	const handleTaskTimeChange = useCallback(
		(
			rowIdx: number,
			rowId: string,
			dayKey: string,
			newFactTime: number | null,
			newBillableTime: number | null,
			isLoading?: {
				fact?: boolean;
				billable?: boolean;
			}
		) => {
			setRows((prevDataGridRows) => {
				if (!prevDataGridRows.length) {
					return prevDataGridRows;
				}

				const rowsWithUpdatedTimeMap = getRowsWithUpdatedRowTime(
					prevDataGridRows,
					rowIdx,
					dayKey,
					newFactTime,
					newBillableTime,
					isLoading
				);

				const [totalFactTime, totalBillableTime] = getTotalTimeByDay(
					rowsWithUpdatedTimeMap,
					dayKey
				);

				const rowsWithUpdatedTimeMapAndTotal = getRowsWithUpdatedRowTime(
					rowsWithUpdatedTimeMap,
					TOTAL_ROW_INDEX,
					dayKey,
					totalFactTime,
					totalBillableTime,
					isLoading
				);

				if (rowId === EMPTY_ROW_ID) {
					return getDataGridRowsWithUpdatedAddRow(
						rowsWithUpdatedTimeMapAndTotal[rowIdx],
						rowsWithUpdatedTimeMapAndTotal
					);
				}

				return rowsWithUpdatedTimeMapAndTotal;
			});
		},
		[]
	);

	const handleUpdateTrackItemAfterSuccessRequest = useCallback(
		(
			rowIdx: number,
			rowId: string,
			dayKey: string,
			timeTrackItem: TimeTrackItem | undefined
		) => {
			setRows((prevDataGridRows) => {
				const rowsLength = prevDataGridRows.length;

				if (!rowsLength || rowIdx > rowsLength - 1) {
					return prevDataGridRows;
				}

				const rowsWithUpdatedTimeMap = getRowsWithUpdatedTrackItem(
					prevDataGridRows,
					rowIdx,
					dayKey,
					timeTrackItem
				);

				return rowsWithUpdatedTimeMap;
			});
		},
		[]
	);

	const handleRowChange = useCallback(
		(rowIdx: number, rowId: string, newProjectInfo: Partial<TimeSheetRow>) => {
			setRows((prevDataGridRows) => {
				if (!prevDataGridRows.length) {
					return prevDataGridRows;
				}

				const prevDataGridCopy = [...prevDataGridRows];

				prevDataGridCopy[rowIdx] = {
					...prevDataGridCopy[rowIdx],
					...newProjectInfo,
				};

				if (rowId === EMPTY_ROW_ID) {
					return getDataGridRowsWithUpdatedAddRow(
						prevDataGridCopy[rowIdx],
						prevDataGridCopy
					);
				}

				return prevDataGridCopy;
			});
		},
		[]
	);

	const handleTaskInfoChange = useCallback(
		(
			rowIdx: number,
			rowId: string,
			prevTaskName: string,
			prevProjectId: number,
			newTaskName: string,
			newProjectId: number,
			onRowHasPrevInfo: (row: TimeSheetRow) => TimeSheetRow
		) => {
			setRows((prevDataGridRows) => {
				if (!prevDataGridRows.length) {
					return prevDataGridRows;
				}

				if (prevTaskName === newTaskName && prevProjectId === newProjectId) {
					return prevDataGridRows;
				}
				const prevDataGridCopy = prevDataGridRows.map((row) =>
					row.taskName === prevTaskName && row.projectId === prevProjectId
						? onRowHasPrevInfo(row)
						: row
				);

				prevDataGridCopy[rowIdx] = {
					...prevDataGridCopy[rowIdx],
					taskName: newTaskName,
					projectId: newProjectId,
				};

				if (rowId === EMPTY_ROW_ID) {
					return getDataGridRowsWithUpdatedAddRow(
						prevDataGridCopy[rowIdx],
						prevDataGridCopy
					);
				}

				return prevDataGridCopy;
			});
		},
		[]
	);

	const addNewRow = useCallback(() => {
		setRows((prevDataGridRows) => {
			if (!prevDataGridRows.length) {
				return prevDataGridRows;
			}

			const prevDataGridRowsCopy = [...prevDataGridRows];
			const row =
				prevDataGridRowsCopy[prevDataGridRowsCopy.length - NEW_TASK_ROW_OFFSET];

			prevDataGridRowsCopy.splice(
				prevDataGridRows.length - NEW_TASK_ROW_OFFSET,
				2,
				{
					...row,
					rowId: String(new Date().getTime()),
					previousTaskName: row.taskName,
				},
				createInitialNewTaskRow(
					row.isEditingForbiddenForPeriod,
					row.isMobileView,
					!isEngineer,
					prevDataGridRowsCopy.length
				),
				createInitialAddTaskRow(row.isEditingForbiddenForPeriod)
			);

			return prevDataGridRowsCopy;
		});
	}, [isEngineer]);

	const setIsCellLoading = useCallback(
		(
			rowIdx: number,
			dayKey: string,
			isLoading: {
				fact?: boolean;
				billable?: boolean;
			}
		) => {
			setRows((prevDataGridRows) => {
				if (!prevDataGridRows.length) {
					return prevDataGridRows;
				}

				const rowsCopy = [...prevDataGridRows];

				const row = rowsCopy[rowIdx];

				if (!row) {
					return prevDataGridRows;
				}

				const prevTimeTrackItem = row.timeMap.get(dayKey) || {
					trackId: 0,
					billableTrackedInMinutes: 0,
					factTrackedInMinutes: 0,
					isLoading: {
						fact: false,
						billable: false,
					},
					reportForDate: '',
				};

				row.timeMap.set(dayKey, {
					...prevTimeTrackItem,
					isLoading: {
						...prevTimeTrackItem.isLoading,
						...isLoading,
					},
				});

				rowsCopy[rowIdx] = {
					...row,
				};

				return rowsCopy;
			});
		},
		[]
	);

	return useMemo(
		() => ({
			handleTaskTimeChange,
			handleUpdateTrackItemAfterSuccessRequest,
			handleRowChange,
			handleTaskInfoChange,
			addNewRow,
			setIsCellLoading,
		}),
		[
			handleTaskTimeChange,
			handleUpdateTrackItemAfterSuccessRequest,
			handleRowChange,
			handleTaskInfoChange,
			setIsCellLoading,
			addNewRow,
		]
	);
};
