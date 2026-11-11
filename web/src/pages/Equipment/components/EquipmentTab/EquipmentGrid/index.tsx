import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import { memo, useCallback, useMemo } from 'react';

import { getEquipmentColumns } from '../constants';
import { useEquipmentsColumns } from '../hooks/useEquipmentHeaders';
import { useEquipmentsRows } from '../hooks/useEquipmentsValues';

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
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import { STORAGE_OPTIONS } from '@/pages/Equipment/constants';
import { EquipmentOptions } from '@/pages/Equipment/types';
import { getUsersOptions } from '@/pages/Equipment/utils';
import { useGetEquipmentAutoSelectFilters } from '@/queries/equipment/useGetEquipmentAutoSelectFilters';
import { useGetEquipments } from '@/queries/equipment/useGetEquipments';

import { EquipmentGridInnerProps } from './types';

function EquipmentGridInner({
	filters,
	setFilters,
	rowsPerPage,
	page,
	handleOpen,
	sortColumns,
	onSortColumnsChange,
	setRowsPerPage,
	setPage,
	equipmentTypes,
	equipmentLocationTypes,
	initialFilters,
	equipmentAvailabilityTypes,
	departments,
	order,
	orderBy,
}: EquipmentGridInnerProps) {
	const { role, position, claims } = useAuth();
	const { tooltipContent, isTextCut, setTooltipContent } = useTooltipContent();

	const toast = useToast();

	const handleEdit = (equipmentId?: string | number) => () => {
		handleOpen(equipmentId);
	};

	const { data: autoSelectFilters } = useGetEquipmentAutoSelectFilters({
		onError: createOnErrorHandler(toast),
	});

	const equipmentColumnsWidths = useMemo(
		() => getEquipmentColumns(role, claims),
		[role, claims]
	);

	const { data, dataUpdatedAt, isFetching, isLoading } = useGetEquipments(
		page,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		departments,
		equipmentLocationTypes,
		equipmentTypes,
		equipmentAvailabilityTypes,
		{ enabled: Boolean(position) }
	);

	const options: EquipmentOptions = useMemo(
		() => ({
			users: getUsersOptions(autoSelectFilters?.users),
			equipmentTypes: convertToSelectionModalOptions(
				equipmentTypes,
				'fullName'
			),
			approvers: getUsersOptions(autoSelectFilters?.approvers),
			equipmentLocations: equipmentLocationTypes,
			departments: convertToSelectionModalOptions(departments, 'fullName'),
			availability: convertToSelectionModalOptions(
				equipmentAvailabilityTypes,
				'description'
			),
			storage: STORAGE_OPTIONS,
		}),
		[equipmentTypes, equipmentLocationTypes, autoSelectFilters, departments]
	);

	const equipmentColumns = useEquipmentsColumns(
		setFilters,
		filters,
		initialFilters,
		setTooltipContent,
		options,
		equipmentColumnsWidths
	);

	const equipmentRows = useEquipmentsRows(
		data!,
		dataUpdatedAt,
		page,
		rowsPerPage,
		handleEdit,
		role
	);

	const overrideEquipmentGridStyle = useMemo(() => {
		const columnWidths = getEquipmentColumns(role, claims);
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!equipmentRows.length,
			Object.values(columnWidths).filter(Boolean).length + 2,
			gridColumnTemplate
		);
	}, [equipmentRows.length, role, claims]);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const equipmentGridPagination = useMemo(
		() =>
			createGridPagination(
				equipmentRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[equipmentRows.length, page, rowsPerPage, data?.total]
	);

	return (
		<Loader isLoading={isFetching || isLoading}>
			<DataGridHoverRowWrapper defineHeight={false} flexGrow>
				<DataGrid
					rows={equipmentRows}
					columns={equipmentColumns}
					sortColumns={sortColumns}
					onSortColumnsChange={onSortColumnsChange}
					overrideDataGridStyles={overrideEquipmentGridStyle}
					paginationProps={equipmentGridPagination}
					hideVerticalScrollbar={
						equipmentRows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT &&
						(isLoading || isFetching)
					}
					scrollToTopOnRowsChange={false}
					renderers={noRowsGridFallback}
					{...COMMON_GRID_PROPS}
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

export const EquipmentGrid = memo(EquipmentGridInner);
