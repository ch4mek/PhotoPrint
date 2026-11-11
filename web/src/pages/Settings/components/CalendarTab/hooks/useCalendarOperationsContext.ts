import { useQueryClient } from '@tanstack/react-query';
import { Dispatch, useCallback, useMemo, KeyboardEvent } from 'react';

import {
	ADD_COUNTRY_ROW,
	DEFAULT_MONTH_TIME,
	NEW_COUNTRY_ROW,
} from '../constants';
import { CalendarOperationsContextValues } from '../types';

import { EVENT_ENTER_KEY } from '@/constants/eventKeys';
import { CONFIGURATION_QUERY_KEY } from '@/constants/queriesKeys';
import { useStopGridCellPropagation } from '@/hooks/useStopGridCellPropagation';
import { TooltipContent } from '@/types/hooks';
import {
	CalendarMonthTime,
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { useCalendarMutations } from './useCalendarMutations';

export const useCalendarOperationsContextValues = (
	setGridRows: Dispatch<React.SetStateAction<WorkingHoursCalendarRow[]>>,
	year: number,
	setTooltipContent: Dispatch<React.SetStateAction<TooltipContent>>
): CalendarOperationsContextValues => {
	const mutations = useCalendarMutations();
	const queryClient = useQueryClient();

	const refetchConfiguration = useCallback(() => {
		queryClient.refetchQueries({ queryKey: [CONFIGURATION_QUERY_KEY] });
	}, []);

	const onCountryRowChange = useCallback(
		(rowIdx: number, newParts: Partial<WorkingHoursCalendarRow>) => {
			setGridRows((prevRows) => {
				const newRows = [...prevRows];

				const newRow = {
					...newRows[rowIdx],
					...newParts,
				};

				newRows[rowIdx] = newRow;

				const shouldUpdateAddRow = rowIdx + 2 === newRows.length;

				if (shouldUpdateAddRow) {
					newRows[newRows.length - 1] = {
						...newRow,
						type: WorkingHoursCalendarRowType.AddCountryButtonRow,
					};
				}

				return newRows;
			});
		},
		[]
	);

	const onMonthTimeChange = useCallback(
		(rowIdx: number, monthId: number, newTime: number) => {
			setGridRows((prevRows) => {
				const newRows = [...prevRows];

				const prevMonthTime =
					newRows[rowIdx].timeMap.get(monthId) || DEFAULT_MONTH_TIME;

				newRows[rowIdx].timeMap.set(monthId, {
					...prevMonthTime,
					hours: newTime,
				});

				newRows[rowIdx] = {
					...newRows[rowIdx],
				};

				return newRows;
			});
		},
		[]
	);

	const setIsMonthTimeLoading = useCallback(
		(rowIdx: number, monthId: number, isLoading: boolean) => {
			setGridRows((prevRows) => {
				const newRows = [...prevRows];

				const prevMonthTime =
					newRows[rowIdx].timeMap.get(monthId) || DEFAULT_MONTH_TIME;

				newRows[rowIdx].timeMap.set(monthId, {
					...prevMonthTime,
					isLoading,
				});

				newRows[rowIdx] = {
					...newRows[rowIdx],
				};

				return newRows;
			});
		},
		[]
	);

	const setNewMonthTime = useCallback(
		(rowIdx: number, monthId: number, newTime: CalendarMonthTime) => {
			setGridRows((prevRows) => {
				const newRows = [...prevRows];

				newRows[rowIdx].timeMap.set(monthId, newTime);

				newRows[rowIdx] = {
					...newRows[rowIdx],
				};

				return newRows;
			});
		},
		[]
	);

	const setCellHasError = useCallback(
		(rowIdx: number, monthId: number, hasError: boolean) => {
			setGridRows((prevRows) => {
				const newRows = [...prevRows];

				const prevMonthTime =
					newRows[rowIdx].timeMap.get(monthId) || DEFAULT_MONTH_TIME;

				newRows[rowIdx].timeMap.set(monthId, {
					...prevMonthTime,
					hasError,
				});

				newRows[rowIdx] = {
					...newRows[rowIdx],
				};

				return newRows;
			});
		},
		[]
	);

	const onAddNewRow = useCallback(
		(
			rowIdx: number,
			newRow: WorkingHoursCalendarRow,
			onLoadingEnd?: () => void
		) => {
			mutations.createCountry(
				newRow.countryName,
				(newCountry) => {
					setGridRows((prevRows) => {
						const newRows = [...prevRows];

						const readyNewRow = {
							...newRow,
							countryId: newCountry.id,
							countryName: newCountry.name,
							type: WorkingHoursCalendarRowType.CountryRow,
							timeMap: new Map(),
						};

						newRows[rowIdx] = readyNewRow;
						newRows[newRows.length - 1] = NEW_COUNTRY_ROW;
						newRows.push(ADD_COUNTRY_ROW);

						return newRows;
					});

					refetchConfiguration();

					if (onLoadingEnd) {
						onLoadingEnd();
					}
				},
				onLoadingEnd
			);
		},
		[]
	);

	const { handleClick: handleCellClick, handleKeyDown: handleCellKeyDown } =
		useStopGridCellPropagation();

	const handleInputKeyDown = useCallback(
		(e: KeyboardEvent<HTMLInputElement>) => {
			if (e.key === EVENT_ENTER_KEY) {
				e.currentTarget.blur();
			}
		},
		[]
	);

	const contextValue: CalendarOperationsContextValues = useMemo(
		() => ({
			onCountryRowChange,
			onMonthTimeChange,
			setNewMonthTime,
			setCellHasError,
			onAddNewRow,
			setIsMonthTimeLoading,
			year,
			handleCellClick,
			handleCellKeyDown,
			handleInputKeyDown,
			setTooltipContent,
			...mutations,
		}),
		[onCountryRowChange, onMonthTimeChange, year]
	);

	return contextValue;
};
