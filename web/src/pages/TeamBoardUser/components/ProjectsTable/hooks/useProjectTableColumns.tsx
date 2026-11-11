import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { PROJECT_TABLE_COLUMNS } from '../constants';
import { ProjectTableColumnWidths } from '../types';

import {
	FreeTextModal,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	getLogicalTextColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { activityStatusOptions } from '@/constants/activityStatus';
import { billabilityOptions } from '@/constants/billabilityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { ProjectRow } from '@/pages/TeamBoardUser/types';
import {
	ProjectsTableFilters,
	ProjectTableFilterSetter,
} from '@/types/projectsTableFilters';
import { TeamboardTableMode } from '@/types/teamboard';

const STATUS_COLUMN_INDEX = 2;

export const useProjectTableColumns = (
	displayFilters: boolean,
	mode: TeamboardTableMode,
	columnWidths: ProjectTableColumnWidths,
	filters?: ProjectsTableFilters,
	filterSetters?: ProjectTableFilterSetter,
	areFiltersApplied?: boolean,
	resetFilters?: () => void,
	isColumnSortable?: boolean
): Array<Column<ProjectRow>> => {
	const isTab = mode === TeamboardTableMode.Tab;

	return useMemo(() => {
		const columns: Array<Column<ProjectRow>> = [
			getColumnWithTooltipFactory<ProjectRow>({
				...PROJECT_TABLE_COLUMNS.projectName,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<ProjectRow, string>({
									...props,
									filter: filters.projectName,
									isFilterApplied: filters.projectName.length > 0,
									setFilter: filterSetters?.projectName as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: arePrimitiveValuesEqual,
								})
						: undefined,
				width: columnWidths.projectName,
			}),
			getColumnWithTooltipFactory<ProjectRow>({
				...PROJECT_TABLE_COLUMNS.clientName,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<ProjectRow, string>({
									...props,
									filter: filters.clientName,
									isFilterApplied: filters.clientName.length > 0,
									setFilter: filterSetters?.clientName as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: arePrimitiveValuesEqual,
								})
						: undefined,
				width: columnWidths.client,
			}),
			getLogicalTextColumnFactory<ProjectRow>({
				...PROJECT_TABLE_COLUMNS.billabillity,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<ProjectRow, string[]>({
									...props,
									filter: filters.billability,
									isFilterApplied: filters.billability.length > 0,
									setFilter: filterSetters?.billability as Dispatch<
										SetStateAction<string[]>
									>,
									defaultValue: [],
									options: billabilityOptions,
									renderModal: SelectionModal,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: areArraysEqualByPrimitiveItems,
								})
						: undefined,
				width: columnWidths.billability,
			}),
			getColumnWithTooltipFactory<ProjectRow>({
				...PROJECT_TABLE_COLUMNS.startDate,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<ProjectRow, FromToProps<Date>>({
									...props,
									filter: filters.startDate || fromToDateObject,
									setFilter: filterSetters?.startDate as React.Dispatch<
										React.SetStateAction<FromToProps<Date>>
									>,
									defaultValue: fromToDateObject,
									renderModal: FromToDateModal,
									isFilterApplied:
										!!filters.startDate?.from || !!filters.startDate?.to,
									areValuesEquals: areFromToCompatibleObjectsEqual,
								})
						: undefined,
				width: columnWidths.startDate,
			}),
			getColumnWithTooltipFactory<ProjectRow>({
				...PROJECT_TABLE_COLUMNS.endDate,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<ProjectRow, FromToProps<Date>>({
									...props,
									filter: filters.endDate || fromToDateObject,
									setFilter: filterSetters?.endDate as React.Dispatch<
										React.SetStateAction<FromToProps<Date>>
									>,
									defaultValue: fromToDateObject,
									renderModal: FromToDateModal,
									isFilterApplied:
										!!filters.endDate?.from || !!filters.endDate?.to,
									areValuesEquals: areFromToCompatibleObjectsEqual,
								})
						: undefined,
				width: columnWidths.endDate,
			}),
			getEditColumnFactory<ProjectRow>({
				key: '',
				name: '',
				displayFilterButton: displayFilters,
				sortable: false,
				frozen: true,
				disabled: !areFiltersApplied,
				clearFilter: resetFilters || undefined,
			}),
		];

		if (isTab) {
			columns.splice(
				STATUS_COLUMN_INDEX,
				0,
				getLogicalTextColumnFactory<ProjectRow>({
					...PROJECT_TABLE_COLUMNS.status,
					sortable: isColumnSortable,
					renderHeaderCell:
						displayFilters && filters
							? (props) =>
									headerCellWithFilterRenderer<ProjectRow, string[]>({
										...props,
										filter: filters.isActive,
										isFilterApplied: filters.isActive.length > 0,
										setFilter: filterSetters?.isActive as Dispatch<
											SetStateAction<string[]>
										>,
										defaultValue: [],
										options: activityStatusOptions,
										renderModal: SelectionModal,
										disableFilterIconActiveStateAsDefault: true,
										areValuesEquals: areArraysEqualByPrimitiveItems,
									})
							: undefined,
					width: columnWidths.status,
				})
			);
		}

		return columns;
	}, [filters, displayFilters, areFiltersApplied, isTab, isColumnSortable]);
};
