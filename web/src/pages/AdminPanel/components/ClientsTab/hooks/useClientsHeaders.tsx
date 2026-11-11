import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	CLIENT_PROJECTS_ROW_CLASS_NAME,
	CLIENT_ROW_NAME_CLASS_NAME,
	clientColumnKeys,
	clientColumnsClassNames,
	clientColumnsNames,
	CHIEF_ROLES_CLIENT_GRID_WIDTH,
} from '../constants';
import { ClientExpandableNameCellRenderer } from '../renderers/clientExpandableNameCellRenderer';
import { ClientsGridRow } from '../types';
import { isClientProjectsRow } from '../utils';

import {
	FreeTextModal,
	FromHoursToHoursTimeModal,
	FromToProps,
	SelectionModal,
	cellWithTooltipRenderer,
	editCellRenderer,
	getLogicalTextColumnFactory,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
	IGNORE_SELECTION_CLASS_NAME,
} from '@/components/Grid/constants';
import { createTrackedCellRenderer } from '@/components/Grid/renderers/cell/trackedCellRenderer/trackedCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	activityLogicalOptions,
	statusOptions,
} from '@/constants/activityStatus';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	IS_ACTIVE_DEFAULT_VALUE,
	TRACKED_TIME_DEFAULT_VALUE,
} from '@/pages/constants';

interface ClientsColumnsFiltersProps {
	clientNameFilter: string;
	setClientNameFilter: Dispatch<SetStateAction<string>>;
	registeredLocationFilter: string[];
	setRegisteredLocationFilter: Dispatch<SetStateAction<string[]>>;
	factTrackedTimeFilter: FromToProps<string>;
	setFactTrackedTimeFilter: Dispatch<SetStateAction<FromToProps<string>>>;
	billableTrackedTimeFilter: FromToProps<string>;
	setBillableTrackedTimeFilter: Dispatch<SetStateAction<FromToProps<string>>>;
	notesFilter: string;
	setNotesFilter: Dispatch<SetStateAction<string>>;
	idFilter: string;
	setIdFilter: Dispatch<SetStateAction<string>>;
	statusFilter: string[];
	setStatusFilter: Dispatch<SetStateAction<string[]>>;
	columnsWidths: typeof CHIEF_ROLES_CLIENT_GRID_WIDTH;
}

const {
	id,
	billableTime,
	comments,
	factTime,
	name,
	status,
	registeredLocation,
} = clientColumnKeys;

export const useClientsColumns = ({
	clientNameFilter,
	setClientNameFilter,
	factTrackedTimeFilter,
	setFactTrackedTimeFilter,
	billableTrackedTimeFilter,
	setBillableTrackedTimeFilter,
	registeredLocationFilter,
	setRegisteredLocationFilter,
	notesFilter,
	setNotesFilter,
	idFilter,
	setIdFilter,
	statusFilter,
	setStatusFilter,
	columnsWidths,
}: ClientsColumnsFiltersProps) => {
	const isResetFiltersDisabled =
		clientNameFilter === '' &&
		areFromToCompatibleObjectsEqual(
			factTrackedTimeFilter,
			TRACKED_TIME_DEFAULT_VALUE
		) &&
		areFromToCompatibleObjectsEqual(
			billableTrackedTimeFilter,
			TRACKED_TIME_DEFAULT_VALUE
		) &&
		notesFilter === '' &&
		idFilter === '' &&
		areArraysEqualByPrimitiveItems(statusFilter, IS_ACTIVE_DEFAULT_VALUE) &&
		areArraysEqualByPrimitiveItems(registeredLocationFilter, []);

	const { role } = useAuth();

	const {
		configurableValues: { countries },
	} = useConfiguration();

	const registeredLocationOptions = countries.map((item) => ({
		...item,
		description: item.name,
		id: item.name,
	}));

	const displayTrackedColumns = useMemo(
		() => !isHeadOrGroupManager(role),
		[role]
	);

	const amountOfColumnsForProjectGrid = displayTrackedColumns ? 3 : 1;

	const clearFilters = () => {
		setClientNameFilter('');
		setNotesFilter('');
		setFactTrackedTimeFilter(TRACKED_TIME_DEFAULT_VALUE);
		setBillableTrackedTimeFilter(TRACKED_TIME_DEFAULT_VALUE);
		setIdFilter('');
		setStatusFilter(IS_ACTIVE_DEFAULT_VALUE);
		setRegisteredLocationFilter([]);
	};

	const clientColumns: Array<Column<ClientsGridRow> | boolean> = [
		getColumnWithTooltipFactory({
			key: id,
			name: clientColumnsNames[id],
			sortable: true,
			cellClass: (row) =>
				isClientProjectsRow(row) ? IGNORE_SELECTION_CLASS_NAME : '',
			width: getColumnWidth(columnsWidths.id).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: idFilter,
					setFilter: setIdFilter,
					defaultValue: '',
					renderModal: FreeTextModal,
					isFilterApplied: idFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
				}),
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					isCopyEnabled: true,
				}),
		}),
		{
			key: name,
			sortable: true,
			name: clientColumnsNames[name],
			cellClass: (row) =>
				isClientProjectsRow(row)
					? `${CLIENT_PROJECTS_ROW_CLASS_NAME} ${IGNORE_SELECTION_CLASS_NAME}`
					: `${CLIENT_ROW_NAME_CLASS_NAME} ${clientColumnsClassNames[name]}`,
			width: getColumnWidth(columnsWidths.client).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ClientsGridRow, string>({
					...props,
					filter: clientNameFilter,
					setFilter: setClientNameFilter,
					isFilterApplied: clientNameFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
					renderModal: FreeTextModal,
					defaultValue: '',
				}),

			renderCell: ClientExpandableNameCellRenderer,
			colSpan(args) {
				return args.type === 'ROW' && args.row.type === 'PROJECT'
					? amountOfColumnsForProjectGrid
					: undefined;
			},
		},
		displayTrackedColumns &&
			getColumnFactory({
				key: factTime,
				name: clientColumnsNames[factTime],
				sortable: true,
				cellClass: (row) =>
					isClientProjectsRow(row)
						? IGNORE_SELECTION_CLASS_NAME
						: clientColumnsClassNames[factTime],
				width: getColumnWidth(columnsWidths.factTime).width,
				renderCell: createTrackedCellRenderer(
					(row) => row.factTrackedTimeInMinutes,
					true
				),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ClientsGridRow, FromToProps<string>>({
						...props,
						filter: factTrackedTimeFilter,
						setFilter: setFactTrackedTimeFilter,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							factTrackedTimeFilter,
							TRACKED_TIME_DEFAULT_VALUE
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromHoursToHoursTimeModal,
						defaultValue: TRACKED_TIME_DEFAULT_VALUE,
					}),
			}),
		displayTrackedColumns &&
			getColumnFactory({
				key: billableTime,
				name: clientColumnsNames[billableTime],
				sortable: true,
				cellClass: (row) =>
					isClientProjectsRow(row)
						? IGNORE_SELECTION_CLASS_NAME
						: clientColumnsClassNames[billableTime],
				width: getColumnWidth(columnsWidths.billableTime).width,
				renderCell: createTrackedCellRenderer(
					(row) => row.billableTrackedTimeInMinutes,
					true
				),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ClientsGridRow, FromToProps<string>>({
						...props,
						filter: billableTrackedTimeFilter,
						setFilter: setBillableTrackedTimeFilter,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							billableTrackedTimeFilter,
							TRACKED_TIME_DEFAULT_VALUE
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromHoursToHoursTimeModal,
						defaultValue: TRACKED_TIME_DEFAULT_VALUE,
					}),
			}),
		getLogicalTextColumnFactory({
			key: status,
			name: clientColumnsNames[status],
			sortable: true,
			cellClass: (row) =>
				isClientProjectsRow(row) ? IGNORE_SELECTION_CLASS_NAME : '',
			width: getColumnWidth(columnsWidths.status).width,
			logicalTextOptions: activityLogicalOptions,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ClientsGridRow, string[]>({
					...props,
					areValuesEquals: areArraysEqualByPrimitiveItems,
					filter: statusFilter,
					setFilter: setStatusFilter,
					renderModal: SelectionModal,
					isFilterApplied: statusFilter.length > 0,
					options: statusOptions,
					defaultValue: IS_ACTIVE_DEFAULT_VALUE,
					disableFilterIconActiveStateAsDefault: true,
				}),
		}),
		displayTrackedColumns &&
			getColumnFactory({
				key: registeredLocation,
				name: clientColumnsNames[registeredLocation],
				sortable: true,
				width: getColumnWidth(columnsWidths.registeredLocation).width,
				cellClass: (row) =>
					isClientProjectsRow(row) ? IGNORE_SELECTION_CLASS_NAME : '',
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ClientsGridRow, string[]>({
						...props,
						filter: registeredLocationFilter,
						setFilter: setRegisteredLocationFilter,
						isFilterApplied: registeredLocationFilter.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						renderModal: SelectionModal,
						options: registeredLocationOptions,
						defaultValue: [],
					}),
			}),
		getColumnFactory({
			key: comments,
			name: clientColumnsNames[comments],
			width: getColumnWidth(columnsWidths.notes).width,
			cellClass: (row) =>
				isClientProjectsRow(row) ? IGNORE_SELECTION_CLASS_NAME : '',
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer<ClientsGridRow, string>({
					...props,
					filter: notesFilter,
					setFilter: setNotesFilter,
					isFilterApplied: notesFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
					renderModal: FreeTextModal,
					defaultValue: '',
				}),
		}),
		getEditColumnFactory({
			key: '',
			name: '',
			width: 48,
			disabled: isResetFiltersDisabled,
			renderCell: (props) =>
				editCellRenderer(props, { isDisabled: isHeadOrGroupManager(role) }),
			clearFilter: clearFilters,
			frozen: true,
			headerCellClass: EDIT_COLUMN_HEADER_CLASS,
			cellClass: (row) =>
				`${EDIT_COLUMN_CELL_CLASS} ${
					isClientProjectsRow(row) ? IGNORE_SELECTION_CLASS_NAME : ''
				}`,
		}),
	];

	return clientColumns.filter(Boolean) as Array<Column<ClientsGridRow>>;
};
