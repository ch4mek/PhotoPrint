import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	CLAIM_COLUMN_WIDTH,
	departmentClaimsColumnProps,
	firstClaimsColumnSetup,
	getFirstUsersClaimsColumnSetup,
	getUserClaimsColumnProps,
	NAME_COLUMN_KEY,
	userClaimsIdColumnProps,
} from '../constants';

import {
	cellWithColorCircleRenderer,
	cellWithTooltipRenderer,
	editCellRenderer,
	FreeTextAutoComplete,
	FreeTextModal,
	getColumnWithColorCircleFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { getClaimsColumnFactory } from '@/components/Grid/factories/getClaimsColumnFactory';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	ClaimsRow,
	UserClaimsRow,
} from '@/pages/Settings/components/ClaimsTabs/types';
import {
	UserClaimsFilterOptions,
	UserClaimsFilterSetter,
	UserClaimTableFilters,
} from '@/types/settings';

export const useColumns = (
	groupClaims: ClaimsRow[],
	filters: UserClaimTableFilters,
	filterSetter: UserClaimsFilterSetter,
	options: UserClaimsFilterOptions,
	hasMaxColumnWidth: boolean
): readonly Column<UserClaimsRow>[] => {
	const claimsColumns = useMemo<readonly Column<UserClaimsRow>[]>(() => {
		const columns = groupClaims.map<Column<UserClaimsRow>>(({ name }) =>
			getClaimsColumnFactory({
				name,
				width: CLAIM_COLUMN_WIDTH,
				key: name,
			})
		);

		const isResetButtonDisabled = Object.values(filters).every((value) =>
			value ? value.length === 0 : !value
		);

		return [
			getColumnWithTooltipFactory({
				...userClaimsIdColumnProps,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userId,
						setFilter: filterSetter.userId as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.userId,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
						fieldToDisplay: firstClaimsColumnSetup.key,
					}),
			}),
			getColumnWithColorCircleFactory({
				...getUserClaimsColumnProps(hasMaxColumnWidth),
				key: NAME_COLUMN_KEY,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userClaimName,
						isFilterApplied: !!filters.userClaimName,
						setFilter: filterSetter.userClaimName as Dispatch<
							SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.users,
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithColorCircleRenderer(props, {
						fieldToDisplay:
							getFirstUsersClaimsColumnSetup(hasMaxColumnWidth).key,
					}),
			}),
			getColumnWithTooltipFactory({
				...departmentClaimsColumnProps,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.departments || [],
						isFilterApplied: filters.departments.length > 0,
						setFilter: filterSetter.departments as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.departments,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			...columns,
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetButtonDisabled,
				clearFilter: filterSetter.reset,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
				minWidth: 0,
				renderCell: editCellRenderer,
			}),
		];
	}, [groupClaims, filters, options]);

	return claimsColumns;
};
