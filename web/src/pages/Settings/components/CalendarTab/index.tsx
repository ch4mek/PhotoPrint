import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import {
	LoaderProvider,
	useLoaderContext,
} from '@mms/mms-ui-library/components/ManagedLoader';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useMemo, useState } from 'react';

import { FilterDropDownWithArrows } from '@/components/FilterDropDownWIthArrows';
import {
	GRID_ROW_WITH_ADD_BUTTON_HEIGHT,
	GRID_TIME_CELL_ROW_HEIGHT,
	TOOLTIP_FALLBACK_PLACEMENTS,
} from '@/components/Grid/constants';
import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { Loader } from '@/components/Loader';
import { ResetFilterButton } from '@/components/ResetFilterButton';
import { CURRENT_YEAR } from '@/constants/date';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { SettingsWorkingHoursQueriesKeys } from '@/constants/queriesKeys';
import { SETTINGS_YEAR_FILTER_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { PrevValueKeeperProvider } from '@/context/PrevStringValueKeeper';
import { SettingsCalendarCountriesContext } from '@/context/SettingsCalendarCountriesContext';
import { generateYearsWithFutureYearOptions } from '@/helpers/date';
import { extractKeyFromPrimitive } from '@/helpers/extractKey';
import { useColumnsHover } from '@/hooks/useColumnsHover';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import { useTotalColumnsStuck } from '@/pages/TimeSheet/hooks/useTotalColumnsStuck';
import { useGetWorkingHours } from '@/queries/settings/useGetWorkingHours';
import {
	CalendarCountries,
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { HolidaysGrid } from './components/HolidaysGrid';
import {
	CALENDAR_SKELETON_ROW,
	CALENDAR_SKELETON_ROW_COUNT,
	COUNTRY_NAME_FREEZE_BREAKPOINT,
	TOTAL_FREEZE_BREAKPOINT,
} from './constants';
import { CalendarOperationsContext } from './context/CalendarOperationsContext';
import { useCalendarOperationsContextValues } from './hooks/useCalendarOperationsContext';
import { useGenerateCalendarColumns } from './hooks/useGenerateCalendarColumns';
import {
	CalendarTabControlsWrapper,
	CalendarContentWrapper,
	GridWrapper,
	overrideCalendarGridStyles,
} from './styles';
import {
	convertWorkingHoursResponseToRows,
	getInitialYearFilterValue,
} from './utils';

export function CalendarTabInner() {
	const [gridRows, setGridRows] = useState<Array<WorkingHoursCalendarRow>>([]);
	const {
		configurableValues: { countries },
	} = useConfiguration();
	const { id: userId } = useAuth();

	const { isLoading } = useLoaderContext();

	const calendarCountries = useMemo(
		() =>
			countries.reduce<CalendarCountries>((acc, { id, name }) => {
				acc[id] = name;

				return acc;
			}, {}),
		[countries]
	);

	const queryClient = useQueryClient();
	const [selectedYear, setSelectedYear] = useState(() =>
		getInitialYearFilterValue(userId)
	);

	const { data, isFetching } = useGetWorkingHours(selectedYear);

	const { hoveredColumn, handleMouseOver, handleMouseLeave } =
		useColumnsHover();

	const { tooltipContent, setTooltipContent } = useTooltipContent();

	useEffect(() => {
		if (isFetching) {
			setGridRows(
				getSkeletonRows(CALENDAR_SKELETON_ROW_COUNT, CALENDAR_SKELETON_ROW)
			);

			return;
		}

		if (data) {
			setGridRows(convertWorkingHoursResponseToRows(data, calendarCountries));
		}
	}, [data, calendarCountries, isFetching]);

	const isCountryNameColumnFrozen = useHasScreenWidth(
		COUNTRY_NAME_FREEZE_BREAKPOINT
	);
	const isTotalColumnFrozen = useHasScreenWidth(TOTAL_FREEZE_BREAKPOINT);

	const columns = useGenerateCalendarColumns(
		isFetching,
		isCountryNameColumnFrozen,
		isTotalColumnFrozen
	);

	const overrideDataGridStyles = useMemo(
		() =>
			overrideCalendarGridStyles(
				isCountryNameColumnFrozen,
				isTotalColumnFrozen
			),
		[isCountryNameColumnFrozen, isTotalColumnFrozen]
	);

	const yearOptions = useMemo(() => generateYearsWithFutureYearOptions(), []);

	const getNextYear = (step: 1 | -1) => {
		setSelectedYear((prevYear) => {
			const index = yearOptions.indexOf(prevYear);
			const newIndex = index + step;

			if (index === -1 || newIndex < 0 || newIndex >= yearOptions.length) {
				return prevYear;
			}

			return yearOptions[newIndex];
		});
		queryClient.removeQueries({
			queryKey: [SettingsWorkingHoursQueriesKeys.getWorkingHours, selectedYear],
		});
	};

	const handleLeftClick = () => {
		getNextYear(1);
	};

	const handleRightClick = () => {
		getNextYear(-1);
	};

	const contextValue = useCalendarOperationsContextValues(
		setGridRows,
		selectedYear,
		setTooltipContent
	);

	const handleResetFilter = () => {
		setSelectedYear(CURRENT_YEAR);
	};

	const getRowHeight = useCallback(
		(row: WorkingHoursCalendarRow) =>
			row.type === WorkingHoursCalendarRowType.AddCountryButtonRow
				? GRID_ROW_WITH_ADD_BUTTON_HEIGHT
				: GRID_TIME_CELL_ROW_HEIGHT,
		[]
	);

	useTotalColumnsStuck(isFetching);

	useEffect(
		() => () => {
			sessionStorage.removeItem(SETTINGS_YEAR_FILTER_STORAGE_KEY);
		},
		[]
	);

	return (
		<SettingsCalendarCountriesContext.Provider value={calendarCountries}>
			<Loader isLoading={isLoading || isFetching} loaderType="soft">
				<CalendarContentWrapper>
					<CalendarTabControlsWrapper>
						<FilterDropDownWithArrows<number>
							options={yearOptions}
							value={selectedYear}
							onChange={setSelectedYear}
							onLeftArrowClick={handleLeftClick}
							onRightArrowClick={handleRightClick}
							isLeftArrowDisabled={
								selectedYear === yearOptions[yearOptions.length - 1]
							}
							isRightArrowDisabled={selectedYear === yearOptions[0]}
							getKey={extractKeyFromPrimitive}
							width="small"
						/>
						<ResetFilterButton
							disabled={CURRENT_YEAR === selectedYear}
							onClick={handleResetFilter}
						/>
					</CalendarTabControlsWrapper>
					<PrevValueKeeperProvider>
						<CalendarOperationsContext.Provider value={contextValue}>
							<GridWrapper
								withoutBackground
								defineHeight={false}
								hoveredColumn={isFetching ? 0 : hoveredColumn}
								flexGrow
								onMouseOver={handleMouseOver}
								onMouseLeave={handleMouseLeave}
							>
								<DataGrid
									columns={columns}
									rows={gridRows}
									rowHeight={getRowHeight}
									overrideDataGridStyles={overrideDataGridStyles}
									onRowsChange={setGridRows}
									scrollToTopOnRowsChange={false}
									renderers={noRowsGridFallback}
								/>
							</GridWrapper>
							<HolidaysGrid />
						</CalendarOperationsContext.Provider>
					</PrevValueKeeperProvider>
				</CalendarContentWrapper>
			</Loader>
			{tooltipContent?.text && (
				<Hint
					open
					anchorElement={tooltipContent.anchorElement}
					text={tooltipContent.text}
					position="bottom"
					fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
				/>
			)}
		</SettingsCalendarCountriesContext.Provider>
	);
}

export function CalendarTab() {
	return (
		<LoaderProvider>
			<CalendarTabInner />
		</LoaderProvider>
	);
}
