import React, { useMemo, SetStateAction } from 'react';
import { Column, RenderCellProps } from 'react-data-grid';

import { EQUIPMENT_GRID_COLUMNS } from '../constants';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	SelectionModal,
	cellRenderer,
	editCellRenderer,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { FromToPriceModal } from '@/components/Grid/modals/fromToPriceModalRenderer';
import { cellWithLinkRenderer } from '@/components/Grid/renderers/cell/cellWithLinkRenderer';
import { CellWithTooltipAlternativeRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipAlternativeRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import { isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { isDateRangeApplyDisabled } from '@/helpers/isDateRangeApplyDisabled';
import {
	SetFiltersType,
	EquipmentFilters,
	EquipmentOptions,
} from '@/pages/Equipment/types';
import { UserClaims } from '@/types/claims';
import { EquipmentGridRow } from '@/types/equipment';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { TooltipContent } from '@/types/hooks';

export const useEquipmentsColumns = (
	setFilters: SetFiltersType,
	filters: EquipmentFilters,
	initialFilters: EquipmentFilters,
	setTooltipContent: (value: SetStateAction<TooltipContent>) => void,
	options: EquipmentOptions,
	equipmentColumnWidths: GridColumnWidthsConfiguration
): readonly Column<EquipmentGridRow>[] => {
	const { role, departmentId, claims } = useAuth();
	const hasManageAllEquipmentClaim = checkIsHasClaims(
		[UserClaims.ManageAllEquipment],
		claims
	);

	const equipmentFilterKeys = Object.keys(filters) as Array<
		keyof typeof filters
	>;

	const isEditDisabled = areFiltersEqualByKeys(
		filters,
		initialFilters,
		equipmentFilterKeys
	);

	const handleClear = () => setFilters.reset(initialFilters);

	const departmentsOptions = useMemo(
		() =>
			options.departments.map((departmentOption) => ({
				...departmentOption,
				disabled:
					isHeadOrGroupManager(role) &&
					!hasManageAllEquipmentClaim &&
					departmentId !== departmentOption.id,
			})),
		[options.departments, departmentId, hasManageAllEquipmentClaim]
	);

	const headers: Column<EquipmentGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.id,
				width: getColumnWidth(equipmentColumnWidths.id).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.id,
						setFilter: setFilters.id as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.id,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) => cellRenderer(props, true),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.availability,
				sortable: true,
				width: getColumnWidth(equipmentColumnWidths.availability).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: (filters.availability || []) as string[],
						setFilter: setFilters.avaliability as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: initialFilters.availability,
						renderModal: SelectionModal,
						options: options.availability,
						isFilterApplied: filters.availability.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.invoiceNumber,
				width: getColumnWidth(equipmentColumnWidths.invoiceNumber).width,
				sortable: true,

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.invoiceNumber as string,
						setFilter: setFilters.invoiceNumber as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.invoiceNumber,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.serialNumber,
				width: getColumnWidth(equipmentColumnWidths.serialNumber).width,
				sortable: true,

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.serialNumber as string,
						setFilter: setFilters.serialNumber as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.serialNumber,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.inventoryNumber,
				width: getColumnWidth(equipmentColumnWidths.inventoryNumber).width,

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.inventoryNumber as string,
						setFilter: setFilters.inventoryNumber as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.inventoryNumber,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.name,
				width: getColumnWidth(equipmentColumnWidths.name).width,
				sortable: true,
				resizable: false,

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.name as string,
						setFilter: setFilters.name as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.name,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.equipmentTypeId,
				width: getColumnWidth(equipmentColumnWidths.type).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentTypeId || [],
						setFilter: setFilters.typeId as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: initialFilters.equipmentTypeId,
						renderModal: SelectionModal,
						options: options.equipmentTypes,
						isFilterApplied: filters.equipmentTypeId.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),

				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.purchasePriceUsd,
				width: getColumnWidth(equipmentColumnWidths.priceUSD).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<EquipmentGridRow, FromToProps<string>>({
						...props,
						filter: filters.price,
						setFilter: setFilters.price as React.Dispatch<
							React.SetStateAction<FromToProps<string>>
						>,
						renderModal: FromToPriceModal,
						defaultValue: fromToCurrencyObject,
						isFilterApplied:
							filters.price.from !== null || filters.price.to !== null,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.activeHolderFullName,
				width: getColumnWidth(equipmentColumnWidths.user).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.user || '',
						setFilter: setFilters.user as React.Dispatch<
							React.SetStateAction<string>
						>,
						renderModal: FreeTextAutoComplete,
						defaultValue: '',
						autocompleteOptions: options.users,
						isFilterApplied: Boolean(filters.user),
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.activeHolderDepartmentId,
				width: getColumnWidth(equipmentColumnWidths.department).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.departments || [],
						setFilter: setFilters.departments as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: [],
						renderModal: SelectionModal,
						options: departmentsOptions,
						isFilterApplied: !!filters.departments?.length,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.equipmentCharacteristics,
				width: getColumnWidth(equipmentColumnWidths.equipmentCharacteristics)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentCharacteristics as string,
						setFilter: setFilters.equipmentCharacteristics as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!String(filters.equipmentCharacteristics),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.purchasePlace,
				width: getColumnWidth(equipmentColumnWidths.purchasePlace).width,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.purchasePlace as string,
						setFilter: setFilters.purchasePlace as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!String(filters.purchasePlace),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.approverFullName,
				width: getColumnWidth(equipmentColumnWidths.approver).width,
				sortable: true,
				resizable: false,
				renderCell: cellRenderer,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.approver || '',
						setFilter: setFilters.approver as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.approvers,
						isFilterApplied: !!String(filters.approver),
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.purchaseDate,
				width: getColumnWidth(equipmentColumnWidths.purchaseDate).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<EquipmentGridRow, FromToProps<Date>>({
						...props,
						filter: filters.purchaseDate || fromToDateObject,
						setFilter: setFilters.purchaseDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.purchaseDate?.from || !!filters.purchaseDate?.to,
						isApplyButtonDisabled: isDateRangeApplyDisabled,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.storage,
				width: getColumnWidth(equipmentColumnWidths.storage).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.storage || [],
						setFilter: setFilters.storage as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: initialFilters.storage,
						renderModal: SelectionModal,
						options: options.storage,
						isFilterApplied: filters.storage.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.guaranteeDate,
				width: getColumnWidth(equipmentColumnWidths.guaranteeDate).width,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<EquipmentGridRow, FromToProps<Date>>({
						...props,
						filter: filters.guaranteeDate || fromToDateObject,
						setFilter: setFilters.guaranteeDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.guaranteeDate?.from || !!filters.guaranteeDate?.to,
						isApplyButtonDisabled: isDateRangeApplyDisabled,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.location,
				width: getColumnWidth(equipmentColumnWidths.location).width,
				sortable: true,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.location || [],
						setFilter: setFilters.location as unknown as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: initialFilters.location,
						renderModal: SelectionModal,
						options: options.equipmentLocations,
						isFilterApplied: !!filters.location?.length,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.repairPlace,
				sortable: true,
				resizable: false,
				width: getColumnWidth(equipmentColumnWidths.repairPlace).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.repairPlace,
						setFilter: setFilters.repairPlace as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!String(filters.repairPlace),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.jiraTask,
				width: getColumnWidth(equipmentColumnWidths.jiraTask).width,

				renderCell: (props: RenderCellProps<EquipmentGridRow, unknown>) =>
					cellWithLinkRenderer(props),
			}),
			getColumnWithTooltipFactory({
				...EQUIPMENT_GRID_COLUMNS.comment,
				width: getColumnWidth(equipmentColumnWidths.comment).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.notes as string,
						setFilter: setFilters.notes as React.Dispatch<
							React.SetStateAction<string>
						>,
						renderModal: FreeTextModal,
						isFilterApplied: !!String(filters.notes),
						defaultValue: '',
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isEditDisabled,
				renderCell: (props) =>
					editCellRenderer(props, {
						isDisabled:
							isHeadOrGroupManager(role) && !hasManageAllEquipmentClaim,
					}),
				clearFilter: handleClear,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
			}),
		],
		[options, filters]
	);

	return useMemo(
		() => headers.filter((header) => header.width !== 0),
		[headers]
	);
};
