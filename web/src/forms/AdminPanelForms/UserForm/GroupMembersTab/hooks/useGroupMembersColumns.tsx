import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { DEFAULT_GROUP_MEMBERS_FILTER_VALUES } from '../constants';
import { GROUP_MEMBERS_TABLE_COLUMNS } from '../GroupMembersTable/constants';
import { RevokeGroupMemberRenderer } from '../GroupMembersTable/renderers/revokeGroupMemberRenderer';
import {
	FilterOptions,
	GroupMembersRow,
	GroupMembersTableColumnWidths,
} from '../GroupMembersTable/types';
import { checkForEmptyFilter } from '../utils/checkForEmptyFilter';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { HIDE_BOTTOM_BORDER_CLASS } from '@/pages/TeamBoardUser/components/ProjectsTable/constants';
import {
	GroupMembersTableFilters,
	GroupMembersTableFilterSetters,
} from '@/types/groupMembers';

export const useGroupMembersColumns = (
	displayFilters: boolean,
	columnWidths: GroupMembersTableColumnWidths,
	options: FilterOptions,
	filters?: GroupMembersTableFilters,
	filterSetters?: GroupMembersTableFilterSetters,
	isColumnSortable?: boolean
): Array<Column<GroupMembersRow>> =>
	useMemo(() => {
		const clearFilters = () => {
			filterSetters?.clear();
		};

		const { users: allUserNames, location: allLocations } = options;

		const columns: Array<Column<GroupMembersRow>> = [
			getColumnWithTooltipFactory<GroupMembersRow>({
				...GROUP_MEMBERS_TABLE_COLUMNS.userId,
				cellClass: HIDE_BOTTOM_BORDER_CLASS,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<GroupMembersRow, string>({
									...props,
									filter: filters?.userId || '',
									isFilterApplied: Boolean(filters.userId),
									setFilter: filterSetters?.userId as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: DEFAULT_GROUP_MEMBERS_FILTER_VALUES.userId,
									renderModal: FreeTextModal,
									areValuesEquals: arePrimitiveValuesEqual,
								})
						: undefined,
				width: getColumnWidth(columnWidths.userId).width,
			}),

			getColumnWithTooltipFactory<GroupMembersRow>({
				...GROUP_MEMBERS_TABLE_COLUMNS.userName,
				cellClass: HIDE_BOTTOM_BORDER_CLASS,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<GroupMembersRow, string>({
									...props,
									filter: filters?.userName || '',
									isFilterApplied: filters.userName.length > 0,
									setFilter: filterSetters?.userName as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: DEFAULT_GROUP_MEMBERS_FILTER_VALUES.userName,
									renderModal: FreeTextAutoComplete,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: arePrimitiveValuesEqual,
									autocompleteOptions: allUserNames,
								})
						: undefined,
				width: getColumnWidth(columnWidths.userName).width,
			}),

			getColumnWithTooltipFactory<GroupMembersRow>({
				...GROUP_MEMBERS_TABLE_COLUMNS.email,
				cellClass: HIDE_BOTTOM_BORDER_CLASS,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<GroupMembersRow, string>({
									...props,
									filter: filters?.email || '',
									isFilterApplied: filters.email.length > 0,
									setFilter: filterSetters?.email as Dispatch<
										SetStateAction<string>
									>,
									defaultValue: DEFAULT_GROUP_MEMBERS_FILTER_VALUES.email,
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: arePrimitiveValuesEqual,
								})
						: undefined,
				width: getColumnWidth(columnWidths.email).width,
			}),

			getColumnWithTooltipFactory<GroupMembersRow>({
				...GROUP_MEMBERS_TABLE_COLUMNS.location,
				cellClass: HIDE_BOTTOM_BORDER_CLASS,
				sortable: isColumnSortable,
				renderHeaderCell:
					displayFilters && filters
						? (props) =>
								headerCellWithFilterRenderer<GroupMembersRow, string[]>({
									...props,
									filter: filters.location,
									isFilterApplied: filters.location.length > 0,
									setFilter: filterSetters?.location as Dispatch<
										SetStateAction<string[]>
									>,
									defaultValue: DEFAULT_GROUP_MEMBERS_FILTER_VALUES.location,
									renderModal: SelectionModal,
									disableFilterIconActiveStateAsDefault: true,
									areValuesEquals: areArraysEqualByPrimitiveItems,
									options: allLocations,
								})
						: undefined,
				width: getColumnWidth(columnWidths.location).width,
			}),

			getEditColumnFactory<GroupMembersRow>({
				key: '',
				name: '',
				disabled: filters ? checkForEmptyFilter(filters) : true,
				renderCell: ({ row }) => RevokeGroupMemberRenderer(row),
				clearFilter: clearFilters,
				displayFilterButton: true,
				sortable: false,
				frozen: true,
			}),
		];

		return columns;
	}, [filters, options]);
