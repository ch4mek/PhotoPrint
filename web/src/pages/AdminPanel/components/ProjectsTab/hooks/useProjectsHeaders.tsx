import { Dispatch, SetStateAction } from 'react';
import { Column } from 'react-data-grid';

import {
	CLIENT_NAME_DEFAULT_VALUE,
	PROJECT_ID_DEFAULT_VALUE,
	PROJECT_COLUMNS_WIDTH,
	PROJECT_NAME_DEFAULT_VALUE,
	ACCESS_USERS_CELL_CLASS,
} from '../constants';
import { ProjectsSetFilters } from '../types';

import {
	FreeTextModal,
	FromHoursToHoursTimeModal,
	FromToProps,
	SelectionModal,
	cellWithTooltipRenderer,
	editCellRenderer,
	getLogicalTextColumnFactory,
	getArrayColumnFactory,
	getColumnFactory,
	getColumnWithColorCircleFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	getLogicColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { createTrackedCellRenderer } from '@/components/Grid/renderers/cell/trackedCellRenderer/trackedCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	activityLogicalOptions,
	statusOptions,
} from '@/constants/activityStatus';
import { logicOptions, reversedLogicOptions } from '@/constants/logicOptions';
import { isHeadOrGroupManager, Roles } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	IS_ACTIVE_DEFAULT_VALUE,
	TRACKED_TIME_DEFAULT_VALUE,
} from '@/pages/constants';
import { ProjectsGridRow } from '@/types/project';

interface ProjectsColumnsFiltersProps {
	projectFilter: string;
	clientFilter: string;
	factTrackedTimeFilter: FromToProps<string>;
	billableTrackedTimeFilter: FromToProps<string>;
	statusFilter: string[];
	publicFilter: string[];
	billableFilter: string[];
	idFilter: string;
	displayTrackedTimeColumns: boolean;
	setFilters: ProjectsSetFilters;
}

const shouldDisableEditColumn = (
	row: ProjectsGridRow | undefined,
	role: Roles
): boolean => {
	if (!role || (isHeadOrGroupManager(role) && !row?.isActive)) {
		return true;
	}

	return false;
};

export const useProjectsColumns = ({
	clientFilter,
	projectFilter,
	factTrackedTimeFilter,
	billableTrackedTimeFilter,
	statusFilter,
	publicFilter,
	idFilter,
	billableFilter,
	displayTrackedTimeColumns,
	setFilters,
}: ProjectsColumnsFiltersProps): Array<Column<ProjectsGridRow>> => {
	const { role } = useAuth();

	const currentProjectsFilters = {
		projectFilter,
		clientFilter,
		factTrackedTimeFilter,
		billableTrackedTimeFilter,
		statusFilter,
		publicFilter,
		billableFilter,
		idFilter,
	};

	const initialProjectsFilters = {
		projectFilter: PROJECT_NAME_DEFAULT_VALUE,
		clientFilter: CLIENT_NAME_DEFAULT_VALUE,
		factTrackedTimeFilter: TRACKED_TIME_DEFAULT_VALUE,
		billableTrackedTimeFilter: TRACKED_TIME_DEFAULT_VALUE,
		statusFilter: IS_ACTIVE_DEFAULT_VALUE,
		publicFilter: [] as string[],
		billableFilter: [] as string[],
		idFilter: PROJECT_ID_DEFAULT_VALUE,
	};

	const projectsFilterKeys = Object.keys(initialProjectsFilters) as Array<
		keyof typeof initialProjectsFilters
	>;

	const isEditDisabled = areFiltersEqualByKeys(
		currentProjectsFilters,
		initialProjectsFilters,
		projectsFilterKeys
	);

	const handleClear = () => {
		setFilters.reset();
	};

	const projectColumns: Array<Column<ProjectsGridRow> | undefined> = [
		getColumnWithTooltipFactory({
			key: 'id',
			name: 'Id',
			sortable: true,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.id).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: idFilter,
					setFilter: setFilters.idFilter as Dispatch<SetStateAction<string>>,
					defaultValue: '',
					renderModal: FreeTextModal,
					isFilterApplied: idFilter !== PROJECT_ID_DEFAULT_VALUE,
					areValuesEquals: arePrimitiveValuesEqual,
				}),
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					isCopyEnabled: true,
				}),
		}),
		getColumnWithColorCircleFactory({
			key: 'name',
			name: 'Project',
			sortable: true,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.project).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ProjectsGridRow, string>({
					...props,
					filter: projectFilter,
					setFilter: setFilters.projectFilter as Dispatch<
						SetStateAction<string>
					>,
					isFilterApplied: projectFilter !== PROJECT_NAME_DEFAULT_VALUE,
					areValuesEquals: arePrimitiveValuesEqual,
					renderModal: FreeTextModal,
					defaultValue: PROJECT_NAME_DEFAULT_VALUE,
				}),
		}),
		getColumnFactory({
			key: 'clientName',
			name: 'Client',
			sortable: true,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.client).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ProjectsGridRow, string>({
					...props,
					filter: clientFilter,
					setFilter: setFilters.clientFilter as Dispatch<
						SetStateAction<string>
					>,
					isFilterApplied: clientFilter !== CLIENT_NAME_DEFAULT_VALUE,
					areValuesEquals: arePrimitiveValuesEqual,
					renderModal: FreeTextModal,
					defaultValue: CLIENT_NAME_DEFAULT_VALUE,
				}),
		}),
		displayTrackedTimeColumns
			? getColumnFactory({
					key: 'factTrackedTime',
					name: 'Tracked fact',
					sortable: true,
					width: getColumnWidth(PROJECT_COLUMNS_WIDTH.factTracked).width,
					renderCell: createTrackedCellRenderer(
						(row) => row.factTrackedTime,
						true
					),
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectsGridRow, FromToProps<string>>({
							...props,
							filter: factTrackedTimeFilter,
							setFilter: setFilters.factTrackedTimeFilter as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							isFilterApplied:
								factTrackedTimeFilter.from !== null ||
								factTrackedTimeFilter.to !== null,
							areValuesEquals: areFromToCompatibleObjectsEqual,
							renderModal: FromHoursToHoursTimeModal,
							defaultValue: TRACKED_TIME_DEFAULT_VALUE,
						}),
			  })
			: undefined,
		displayTrackedTimeColumns
			? getColumnFactory({
					key: 'billableTrackedTime',
					name: 'Tracked billable',
					sortable: true,
					width: getColumnWidth(PROJECT_COLUMNS_WIDTH.billableTracked).width,
					renderCell: createTrackedCellRenderer(
						(row) => row.billableTrackedTime,
						true
					),
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectsGridRow, FromToProps<string>>({
							...props,
							filter: billableTrackedTimeFilter,
							setFilter: setFilters.billableTrackedTimeFilter as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							isFilterApplied:
								billableTrackedTimeFilter.from !== null ||
								billableTrackedTimeFilter.to !== null,
							areValuesEquals: areFromToCompatibleObjectsEqual,
							renderModal: FromHoursToHoursTimeModal,
							defaultValue: TRACKED_TIME_DEFAULT_VALUE,
						}),
			  })
			: undefined,
		getLogicalTextColumnFactory({
			key: 'isActive',
			name: 'Status',
			sortable: true,
			logicalTextOptions: activityLogicalOptions,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.status).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ProjectsGridRow, string[]>({
					...props,
					areValuesEquals: areArraysEqualByPrimitiveItems,
					filter: statusFilter,
					setFilter: setFilters.statusFilter as Dispatch<
						SetStateAction<string[]>
					>,
					renderModal: SelectionModal,
					isFilterApplied: statusFilter.length > 0,
					options: statusOptions,
					defaultValue: IS_ACTIVE_DEFAULT_VALUE,
					disableFilterIconActiveStateAsDefault: true,
				}),
		}),
		getLogicColumnFactory({
			key: 'isPrivate',
			name: 'Private',
			sortable: true,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.private).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ProjectsGridRow, string[]>({
					...props,
					areValuesEquals: areArraysEqualByPrimitiveItems,
					filter: publicFilter,
					setFilter: setFilters.publicFilter as Dispatch<
						SetStateAction<string[]>
					>,
					renderModal: SelectionModal,
					isFilterApplied: publicFilter.length > 0,
					options: reversedLogicOptions,
					defaultValue: [],
					disableFilterIconActiveStateAsDefault: true,
				}),
		}),
		getLogicColumnFactory({
			key: 'isBillable',
			name: 'Is billable',
			sortable: true,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.billable).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ProjectsGridRow, string[]>({
					...props,
					areValuesEquals: areArraysEqualByPrimitiveItems,
					filter: billableFilter,
					setFilter: setFilters.billableFilter as Dispatch<
						SetStateAction<string[]>
					>,
					renderModal: SelectionModal,
					isFilterApplied: billableFilter.length > 0,
					options: logicOptions,
					defaultValue: [],
					disableFilterIconActiveStateAsDefault: true,
				}),
		}),
		getArrayColumnFactory({
			key: 'projectUsers',
			name: 'Access users',
			sortable: false,
			width: getColumnWidth(PROJECT_COLUMNS_WIDTH.accessUsers).width,
			cellClass: ACCESS_USERS_CELL_CLASS,
		}),
		getEditColumnFactory({
			key: '',
			name: '',
			disabled: isEditDisabled,
			renderCell: (props) =>
				editCellRenderer(props, {
					isDisabled: shouldDisableEditColumn(props.row, role),
				}),
			clearFilter: handleClear,
			frozen: true,
			sortable: false,
			headerCellClass: EDIT_COLUMN_HEADER_CLASS,
			cellClass: EDIT_COLUMN_CELL_CLASS,
		}),
	];

	return projectColumns.filter(
		(projectColumn) => projectColumn !== undefined
	) as Array<Column<ProjectsGridRow>>;
};
