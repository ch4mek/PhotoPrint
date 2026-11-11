import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useCallback, useMemo } from 'react';

import { HolidayForm } from '../HolidayForm';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { InfoDataContainer } from '@/components/InfoDataContainer';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	getSuccessfullyAddedMessage,
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { isChiefRole } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useCalendarExclusions } from '@/queries/calendar/calendarExclusions';
import { GetCalendarExclusionsItem } from '@/queries/calendar/types';
import { useCalendarExclusionsLookup } from '@/queries/calendar/useCalendarExclusionsLookup';
import { useCreateCalendarExclusion } from '@/queries/calendar/useCreateCalendarExclusion';
import { useUpdateCalendarExclusion } from '@/queries/calendar/useUpdateCalendarExclusion';

import {
	dateTypesOptions,
	HOLIDAYS_CONTAINER_TITLE,
	SORT_DATE,
} from './constants';
import { useHolidaysColumns } from './hooks/useHolidaysColumns';
import { useHolidaysGridFilters } from './hooks/useHolidaysGridFilters';
import { useHolidaysRows } from './hooks/useHolidaysRows';
import {
	CalendarGridsContainerWrapper,
	InfoDataContainerWrapper,
} from './styles';
import { HolidaysFiltersOptions } from './types';
import { getCalendarExclusionsTitle } from './utils';
import { createHolidaysConfig } from './utils/createHolidaysConfig';

export function HolidaysGrid() {
	const toast = useToast();
	const { role } = useAuth();
	const {
		configurableValues: { countries },
	} = useConfiguration();

	const selectionCountries = useMemo(
		() =>
			countries.map((country) => ({
				id: country.id.toString(),
				description: country.name,
			})),
		[countries]
	);

	const { filters, setFilters } = useHolidaysGridFilters();

	const {
		id: currentHolidayData,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<GetCalendarExclusionsItem | undefined>();

	const {
		rowsPerPage,
		handleOnPerPageChange,
		page,
		setPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.ASC,
		initialOrderBy: SORT_DATE,
		resetPageDeps: [filters],
	});

	const filtersOptions = useMemo<HolidaysFiltersOptions>(
		() => ({
			countries: selectionCountries,
			dateTypes: dateTypesOptions,
		}),
		[]
	);

	const { data, isLoading, isFetching, refetch } = useCalendarExclusions(
		page,
		rowsPerPage,
		order,
		orderBy,
		filters,
		countries.length,
		filtersOptions.dateTypes.length,
		{
			onError: createOnErrorHandler(toast),
		}
	);

	const isDataLoading = isLoading || isFetching;

	useLoadingStatus(HolidaysGrid.name, isDataLoading);
	const { data: holidayNamesOptionsData, refetch: refetchHolidayOptions } =
		useCalendarExclusionsLookup();

	const updateHolidayMutation = useUpdateCalendarExclusion({
		onSuccess: ({ dateType }) => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(getCalendarExclusionsTitle(dateType))
				)
			);

			refetch();
			handleClose();
			refetchHolidayOptions();
		},
		onError: createOnErrorHandler(toast),
	});
	const createHolidayMutation = useCreateCalendarExclusion({
		onSuccess: ({ dateType }) => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(getCalendarExclusionsTitle(dateType))
				)
			);

			refetch();
			handleClose();
			refetchHolidayOptions();
		},
		onError: createOnErrorHandler(toast),
	});

	const holidayNamesOptions = useMemo(
		() => Array.from(new Set(holidayNamesOptionsData?.map(({ name }) => name))),
		[holidayNamesOptionsData]
	);

	const handleEditClick = useCallback(
		(holiday: GetCalendarExclusionsItem | undefined) => () => {
			handleOpen(holiday);
		},
		[]
	);

	const columns = useHolidaysColumns(
		filters,
		setFilters,
		filtersOptions,
		holidayNamesOptions
	);

	const rows = useHolidaysRows(data?.items, countries, handleEditClick);

	const holidaysTablePagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	const overrideHolidaysGridStyles = useMemo(() => {
		if (!data) {
			return;
		}

		const styleConfig = {
			...createHolidaysConfig(),
		};

		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			styleConfig,
			true
		);

		const editColumnIndex = columns.length + 1;

		return overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			editColumnIndex,
			gridColumnTemplate,
			undefined,
			false
		);
	}, [data, rows.length, columns.length]);

	const handleOpenAddHolidayForm = () => {
		handleOpen();
	};

	return (
		<InfoDataContainerWrapper>
			<InfoDataContainer
				handleAddClick={
					isChiefRole(role) ? handleOpenAddHolidayForm : undefined
				}
				headerTitle={HOLIDAYS_CONTAINER_TITLE}
			>
				<CalendarGridsContainerWrapper>
					<DataGridHoverRowWrapper defineHeight={false} flexGrow>
						<DataGrid
							rows={rows}
							columns={columns}
							sortColumns={sortColumns}
							onSortColumnsChange={handleSortColumnChange}
							overrideDataGridStyles={overrideHolidaysGridStyles}
							renderers={noRowsGridFallback}
							paginationProps={holidaysTablePagination}
							{...COMMON_GRID_PROPS}
						/>
					</DataGridHoverRowWrapper>
				</CalendarGridsContainerWrapper>
			</InfoDataContainer>

			{isOpen && (
				<HolidayForm
					holiday={currentHolidayData}
					handleClose={handleClose}
					countries={countries}
					onSubmit={
						currentHolidayData
							? updateHolidayMutation.mutateAsync
							: createHolidayMutation.mutateAsync
					}
				/>
			)}
		</InfoDataContainerWrapper>
	);
}
