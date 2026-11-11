import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Hint } from '@mms/mms-ui-library/components/Hint';
import { useCallback, useEffect, useMemo, useReducer } from 'react';

import {
	ASSIGNMENTS_HISTORY_COLUMNS,
	DEFAULT_ASSIGNMENT_HISTORY_FILTERS,
	getAssignmentsHistoryColumnsWidths,
	getAssignmentsHistoryEditColumnIndex,
} from '../../constants';
import { useAssignmentHistoryColumns } from '../../hooks/useAssignmentHistoryColumns';
import { useAssignmentsHistoryRows } from '../../hooks/useAssignmentsHistoryRows';
import { AssignmentsHistoryGridWrapper } from '../../styles';
import { AssignmentHistoryOptions } from '../../types';
import {
	assignmentsHistoryReducer,
	createSetAssignmentsHistoryFilters,
} from '../../utils';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import { useTooltipContent } from '@/hooks/useTooltipContent';
import { getUsersOptions } from '@/pages/Equipment/utils';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetAssignmentsHistory } from '@/queries/equipment/useGetAssignmentsHistory';
import { useGetEquipmentAutoSelectFilters } from '@/queries/equipment/useGetEquipmentAutoSelectFilters';
import { UserClaims } from '@/types/claims';

import { AssignmentsHistoryGridProps } from './types';

export function AssignmentsHistoryGrid({
	handleOpen,
}: AssignmentsHistoryGridProps) {
	const { tooltipContent, setTooltipContent, isTextCut } = useTooltipContent();
	const { role, departmentId, claims } = useAuth();
	const {
		rowsPerPage,
		setRowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: ASSIGNMENTS_HISTORY_COLUMNS.issueDate.key,
	});
	const { page, setPage } = usePageWithSearch();

	const initialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_ASSIGNMENT_HISTORY_FILTERS,
		role,
		departmentId,
		claims,
		[UserClaims.ManageAllEquipment]
	);

	const [filters, dispatch] = useReducer(
		assignmentsHistoryReducer,
		initialFilters
	);

	const setFilters = useMemo(
		() => createSetAssignmentsHistoryFilters(dispatch),
		[]
	);

	const assignmentsHistoryColumnsWidths = useMemo(
		() => getAssignmentsHistoryColumnsWidths(role, claims),
		[role, claims]
	);

	const toast = useToast();

	const handleEdit = (equipmentId?: string | number) => () => {
		handleOpen(equipmentId);
	};

	useEffect(() => setPage(0), [filters, sortColumns, rowsPerPage]);

	const {
		configurableValues: { departmentTypes, equipmentTypes },
	} = useConfiguration();

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const { data, isFetching, isLoading } = useGetAssignmentsHistory(
		page,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		departments,
		equipmentTypes,
		{ onError: createOnErrorHandler(toast) }
	);

	const { data: autoSelectFilters } = useGetEquipmentAutoSelectFilters({
		onError: createOnErrorHandler(toast),
		enabled: !!role,
	});

	const options: AssignmentHistoryOptions = useMemo(
		() => ({
			users: getUsersOptions(autoSelectFilters?.users),
			equipmentTypes: convertToSelectionModalOptions(
				equipmentTypes,
				'fullName'
			),
			departments: convertToSelectionModalOptions(departments, 'fullName'),
		}),
		[equipmentTypes, autoSelectFilters, departments]
	);

	const equipmentColumns = useAssignmentHistoryColumns(
		setFilters,
		filters,
		initialFilters,
		setTooltipContent,
		options,
		assignmentsHistoryColumnsWidths
	);

	const assignmentsHistoryRows = useAssignmentsHistoryRows(
		data!,
		page,
		rowsPerPage,
		handleEdit
	);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const overrideEquipmentGridStyle = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			assignmentsHistoryColumnsWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!data?.items.length,
			getAssignmentsHistoryEditColumnIndex(role, claims),
			gridColumnTemplate
		);
	}, [data?.items, role, claims]);

	const assignmentsHistoryGridPagination = useMemo(
		() =>
			createGridPagination(
				assignmentsHistoryRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[assignmentsHistoryRows.length, page, data?.total]
	);

	return (
		<Loader isLoading={isFetching || isLoading}>
			<AssignmentsHistoryGridWrapper>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						rows={assignmentsHistoryRows}
						{...COMMON_GRID_PROPS}
						columns={equipmentColumns}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideEquipmentGridStyle}
						renderers={noRowsGridFallback}
						scrollToTopOnRowsChange={false}
						paginationProps={assignmentsHistoryGridPagination}
					/>
				</DataGridHoverRowWrapper>
			</AssignmentsHistoryGridWrapper>
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
