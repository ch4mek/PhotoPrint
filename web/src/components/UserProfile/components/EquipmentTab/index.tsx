import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { AssignStatus } from '@/constants/equipment';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetEquipmentByUserId } from '@/queries/equipment/useGetEquipmentByUserId';

import {
	EDIT_COLUMN_WIDTH,
	equipmentDesktopGridColumnWidths,
	equipmentGridColumns,
	equipmentGridColumnWidths,
	RESET_FILTER_COLUMN_INDEX,
} from './constants';
import { useEquipmentsColumns } from './hooks/useEquipmentColumns';
import { useEquipmentFiltersReducer } from './hooks/useEquipmentFiltersReducer';
import { useEquipmentsRows } from './hooks/useEquipmentRows';
import { UserEquipmentFiltersOptions } from './types';

export function EquipmentTab() {
	const { id } = useAuth();
	const {
		configurableValues: { equipmentTypes },
	} = useConfiguration();
	const { tooltipContent, isTextCut, setTooltipContent } = useTooltipContent();
	const toast = useToast();
	const { t } = useTranslation('profile', { keyPrefix: 'equipmentTab' });
	const [filters, setFilters] = useEquipmentFiltersReducer();
	const {
		page,
		rowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		setPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: equipmentGridColumns.issueDate.key,
		resetPageDeps: [filters],
	});

	const filtersOptions = useMemo<UserEquipmentFiltersOptions>(
		() => ({
			equipmentTypes: convertToSelectionModalOptions(
				equipmentTypes,
				'fullName'
			),
			assignStatuses: [
				{ id: AssignStatus.Assigned, description: t(AssignStatus.Assigned) },
				{ id: AssignStatus.Returned, description: t(AssignStatus.Returned) },
			],
		}),
		[equipmentTypes, t]
	);

	const { data, isLoading, isFetching } = useGetEquipmentByUserId(
		id,
		rowsPerPage,
		page,
		order,
		orderBy,
		filters,
		equipmentTypes,
		toast
	);

	const columns = useEquipmentsColumns(
		filters,
		setFilters,
		filtersOptions,
		setTooltipContent
	);

	const rows = useEquipmentsRows(data?.items);

	const isDesktop = useHasScreenWidth(breakpoints.desktopS);

	const overrideEquipmentGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			isDesktop ? equipmentDesktopGridColumnWidths : equipmentGridColumnWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			RESET_FILTER_COLUMN_INDEX,
			gridColumnsTemplate,
			EDIT_COLUMN_WIDTH
		);
	}, [rows.length, isDesktop]);

	const equipmentGridPagination = useMemo(
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

	const isDataLoading = isLoading || isFetching;

	return (
		<Loader isLoading={isDataLoading} size="small">
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					{...COMMON_GRID_PROPS}
					rows={rows}
					columns={columns}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					renderers={noRowsGridFallback}
					scrollToTopOnRowsChange={false}
					overrideDataGridStyles={overrideEquipmentGridStyles}
					paginationProps={equipmentGridPagination}
					hideVerticalScrollbar={
						rows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT && isDataLoading
					}
				/>
			</DataGridHoverRowWrapper>
			{tooltipContent && isTextCut && (
				<Hint
					open
					anchorElement={tooltipContent.anchorElement}
					text={tooltipContent.text}
				/>
			)}
		</Loader>
	);
}
