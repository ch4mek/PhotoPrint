import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PageTitles } from '@/constants/pageTitles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { SortDirection } from '@/constants/sort';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetLabboardInterns } from '@/queries/labboard/useGetLabboardInterns';

import {
	getLabboardColumnsAmount,
	LABBOARD_DEFAULT_ORDER_BY,
} from './constants';
import { useLabboardColumns } from './hooks/useLabboardColumns';
import { useLabboardFilters } from './hooks/useLabboardFilters';
import { useLabboardOptions } from './hooks/useLabboardOptions';
import { getLabboardRowClass } from './utils/getLabboardRowClass';
import { getOverrideLabboardGridStyles } from './utils/getOverrideLabboardGridStyles';
import { getRowKey } from './utils/getRowKey';
import { mapLabboardInternToRow } from './utils/mapLabboardInternToRow';

import 'react-data-grid/lib/styles.css';

export function LabboardPage() {
	const toast = useToast();
	const { filters, setFilters, isResetFiltersDisabled } = useLabboardFilters();
	const { options } = useLabboardOptions();
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const {
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
		order,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState({
		initialOrder: SortDirection.ASC,
		initialOrderBy: LABBOARD_DEFAULT_ORDER_BY,
		resetPageDeps: [filters],
	});

	const handleEdit = useCallback(
		(clickedUserId?: string | number) => {
			if (!clickedUserId || !canOpenProfile(Number(clickedUserId))) {
				return;
			}

			return () => handlePageOpen(clickedUserId.toString());
		},
		[canOpenProfile, handlePageOpen]
	);

	const { columns, labboardColumnsWidths, showGroup } = useLabboardColumns({
		filters,
		setFilters,
		options,
		isResetFiltersDisabled,
		handleUserClick: handleEdit,
	});

	const { isLoading, data, isFetching } = useGetLabboardInterns({
		page: page + 1,
		rowsPerPage: Number(rowsPerPage),
		order,
		orderBy,
		filters,
		englishLevelsOptionsLength: options.englishLevels.length,
		internshipTypesOptionsLength: options.internshipTypes.length,
		finalResultsOptionsLength: options.finalResults.length,
		options: {
			onError: createOnErrorHandler(toast),
		},
	});

	const isDataLoading = isLoading || isFetching;

	const rows = useMemo(
		() =>
			(data?.labBoardInterns.items || []).map((intern) =>
				mapLabboardInternToRow(intern, handleEdit)
			),
		[data?.labBoardInterns.items, handleEdit]
	);

	const overrideLabboardGridStyles = useMemo(
		() =>
			getOverrideLabboardGridStyles(
				labboardColumnsWidths,
				getLabboardColumnsAmount(showGroup),
				rows.length
			),
		[rows.length, labboardColumnsWidths, showGroup]
	);

	const labboardGridPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.labBoardInterns.total
			),
		[
			data?.labBoardInterns.total,
			rows.length,
			page,
			rowsPerPage,
			handleOnPerPageChange,
		]
	);

	return (
		<PageWrapper>
			<PageHeader title={PageTitles.Labboard} />
			<Loader isLoading={isDataLoading}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...COMMON_GRID_PROPS}
						{...{ rows, columns, sortColumns }}
						defaultColumnOptions={{
							sortable: true,
						}}
						rowKeyGetter={getRowKey}
						rowClass={getLabboardRowClass}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideLabboardGridStyles}
						paginationProps={labboardGridPagination}
						renderers={noRowsGridFallback}
						hideVerticalScrollbar={
							rows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT
						}
						scrollToTopOnRowsChange={false}
						enableVirtualization
					/>
				</DataGridHoverRowWrapper>
			</Loader>
		</PageWrapper>
	);
}
