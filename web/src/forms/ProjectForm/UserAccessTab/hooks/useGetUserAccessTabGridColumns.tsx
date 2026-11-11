import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { UserAccessFilters, UserAccessSetFilters } from '../../types';
import { billabilityOptions, USER_ACCESS_TAB_GRID_COLUMNS } from '../constants';
import { UserAccessRow } from '../types';

import {
	FreeTextModal,
	FromToProps,
	getColumnFactory,
	getEditColumnFactory,
	getLogicalTextColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { statusOptions } from '@/constants/activityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { Booleanify } from '@/types/helpers';

export const useGetUserAccessTabGridColumns = (
	filters: UserAccessFilters,
	setFilters: UserAccessSetFilters,
	areFiltersAtInitialState: Booleanify<UserAccessFilters>,
	isResetFiltersDisabled: boolean
) => {
	const columns: Array<Column<UserAccessRow>> = useMemo(
		() => [
			getColumnFactory({
				...USER_ACCESS_TAB_GRID_COLUMNS.userFullName,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userName,
						setFilter: setFilters.userName as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						disableFilterIconActiveStateAsDefault: true,
						isFilterApplied: !areFiltersAtInitialState.userName,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getLogicalTextColumnFactory({
				...USER_ACCESS_TAB_GRID_COLUMNS.isActive,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.status,
						setFilter: setFilters.status as Dispatch<SetStateAction<string[]>>,
						renderModal: SelectionModal,
						isFilterApplied: filters.status.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
						options: statusOptions,
						defaultValue: IS_ACTIVE_DEFAULT_VALUE,
					}),
			}),
			getColumnFactory({
				...USER_ACCESS_TAB_GRID_COLUMNS.isTimeOnProjectBillable,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.billability,
						setFilter: setFilters.billability as Dispatch<
							SetStateAction<string[]>
						>,
						defaultValue: [],
						options: billabilityOptions,
						renderModal: SelectionModal,
						disableFilterIconActiveStateAsDefault: true,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						isFilterApplied: !areFiltersAtInitialState.billability,
					}),
			}),

			getColumnFactory({
				...USER_ACCESS_TAB_GRID_COLUMNS.userOnProjectFrom,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.startDate || fromToDateObject,
						isFilterApplied: !areFiltersAtInitialState.startDate,
						setFilter: setFilters?.startDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnFactory({
				...USER_ACCESS_TAB_GRID_COLUMNS.userOnProjectTo,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.endDate || fromToDateObject,
						isFilterApplied: !areFiltersAtInitialState.endDate,
						setFilter: setFilters?.endDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				sortable: false,
				frozen: true,
				disabled: isResetFiltersDisabled,
				clearFilter: setFilters.reset,
			}),
		],
		[filters]
	);

	return { columns };
};
