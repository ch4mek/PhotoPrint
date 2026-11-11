import { useMemo, SetStateAction, Dispatch } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';

import { ASSIGNMENTS_HISTORY_COLUMNS } from '../constants';
import {
	AssignmentHistoryFilters,
	AssignmentHistoryOptions,
	SetAssignmentsHistoryFiltersType,
} from '../types';
import { getDepartmentsDisabledOptionProperty } from '../utils';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	SelectionModal,
	cellRenderer,
	editCellRenderer,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	getLogicColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { cellWithClickableContentRenderer } from '@/components/Grid/renderers/cell/cellWithClickableContentRenderer/cellWithClickableContentRenderer';
import { CellWithTooltipAlternativeRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipAlternativeRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { fromToDateObject } from '@/constants/defaultValues';
import { logicOptions } from '@/constants/logicOptions';
import { chiefsAndSA } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserClaims } from '@/types/claims';
import { AssignmentHistoryGridRow } from '@/types/equipment';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { TooltipContent } from '@/types/hooks';

export const useAssignmentHistoryColumns = (
	setFilters: SetAssignmentsHistoryFiltersType,
	filters: AssignmentHistoryFilters,
	initialFilters: AssignmentHistoryFilters,
	setTooltipContent: (value: SetStateAction<TooltipContent>) => void,
	options: AssignmentHistoryOptions,
	assignmentsHistoryColumnsWidths: GridColumnWidthsConfiguration
): readonly Column<AssignmentHistoryGridRow>[] => {
	const { role, departmentId, claims } = useAuth();
	const hasManageAllEquipmentClaim = checkIsHasClaims(
		[UserClaims.ManageAllEquipment],
		claims
	);

	const assignmentHistoryFilterKeys = Object.keys(filters) as Array<
		keyof typeof filters
	>;

	const isEditDisabled = areFiltersEqualByKeys(
		filters,
		initialFilters,
		assignmentHistoryFilterKeys
	);

	const handleClear = () => setFilters.reset(initialFilters);

	const departmentsOptions = useMemo(
		() =>
			options.departments.map((departmentOption) => ({
				...departmentOption,
				disabled: getDepartmentsDisabledOptionProperty(
					role,
					departmentId,
					departmentOption.id,
					hasManageAllEquipmentClaim
				),
			})),
		[options.departments, departmentId, hasManageAllEquipmentClaim, role]
	);

	const headers: Column<AssignmentHistoryGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.userId,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.userId).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userId,
						setFilter: setFilters.userId as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.userId,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) => cellRenderer(props, true),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.equipmentId,
				sortable: true,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.equipmentId)
					.width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentId,
						setFilter: setFilters.equipmentId as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.equipmentId,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.user,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.user).width,
				sortable: true,

				renderCell: (props) => cellRenderer(props, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userFullName || '',
						setFilter: setFilters.userFullName as Dispatch<
							SetStateAction<string>
						>,
						renderModal: FreeTextAutoComplete,
						defaultValue: '',
						autocompleteOptions: options.users,
						isFilterApplied: !!filters.userFullName,
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getLogicColumnFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.isAssigned,
				sortable: true,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.isAssigned).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						filter: filters.isAssigned,
						setFilter: setFilters.availability as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						isFilterApplied: filters.isAssigned.length > 0,
						options: logicOptions,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.department,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.department).width,
				sortable: true,
				resizable: false,

				renderCell: (props) => cellRenderer(props, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.departments,
						setFilter: setFilters.departments as Dispatch<
							SetStateAction<string[]>
						>,
						options: departmentsOptions,
						defaultValue: [],
						renderModal: SelectionModal,
						isFilterApplied: filters.departments.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.equipmentName,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.equipmentName)
					.width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentName,
						setFilter: setFilters.name as Dispatch<SetStateAction<string>>,
						defaultValue: initialFilters.equipmentName,
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.equipmentName,
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.equipmentTypeId,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.equipmentTypeId)
					.width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.type,
						setFilter: setFilters.typeId as Dispatch<SetStateAction<string[]>>,
						renderModal: SelectionModal,
						options: options.equipmentTypes,
						defaultValue: initialFilters.type,
						isFilterApplied: filters.type.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.serialNumber,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.serialNumber)
					.width,
				sortable: true,

				renderCell: (
					props: RenderCellProps<AssignmentHistoryGridRow, unknown>
				) =>
					chiefsAndSA.includes(role) || hasManageAllEquipmentClaim
						? cellWithClickableContentRenderer(props)
						: CellWithTooltipAlternativeRenderer(props, setTooltipContent),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.serialNumber as string,
						setFilter: setFilters.serialNumber as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.serialNumber,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.inventoryNumber,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.inventoryNumber)
					.width,
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.inventoryNumber as string,
						setFilter: setFilters.inventoryNumber as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.inventoryNumber,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.issueDate,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.issueDate).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.issueDate || fromToDateObject,
						setFilter: setFilters.issueDate as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.issueDate?.from || !!filters.issueDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...ASSIGNMENTS_HISTORY_COLUMNS.returnDate,
				width: getColumnWidth(assignmentsHistoryColumnsWidths.returnDate).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.returnDate || fromToDateObject,
						setFilter: setFilters.returnDate as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.returnDate?.from || !!filters.returnDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isEditDisabled,
				renderCell: (props) => editCellRenderer(props, { isDisabled: true }),
				clearFilter: handleClear,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
			}),
		],
		[options, filters, role, hasManageAllEquipmentClaim]
	);

	return useMemo(
		() => headers.filter((header) => header.width !== 0),
		[headers]
	);
};
