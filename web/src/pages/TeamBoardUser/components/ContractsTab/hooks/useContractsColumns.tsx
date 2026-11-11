import { useMemo } from 'react';

import { CONTRACTS_GRID_COLUMNS } from '../constants';
import { ContractsFilters, SetFiltersType } from '../types';

import {
	editCellRenderer,
	FreeTextModal,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export const useContractsColumns = (
	setFilters: SetFiltersType,
	filters: ContractsFilters,
	{ fireTypes }: { fireTypes: SelectionModalOption[] },
	areFiltersApplied?: boolean,
	canEdit?: boolean
) =>
	useMemo(
		() => [
			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.contractFrom,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.contractFrom || fromToDateObject,
						setFilter: setFilters.contractFrom as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							Boolean(filters.contractFrom?.from) ||
							Boolean(filters.contractFrom?.to),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.contractTo,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.contractTo || fromToDateObject,
						setFilter: setFilters.contractTo as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							Boolean(filters.contractTo?.from) ||
							Boolean(filters.contractTo?.to),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),

			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.fireDate,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.fireDate || fromToDateObject,
						setFilter: setFilters.fireDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							Boolean(filters.fireDate?.from) || Boolean(filters.fireDate?.to),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.fireType,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.fireType,
						setFilter: setFilters.fireType as React.Dispatch<
							React.SetStateAction<string[]>
						>,
						defaultValue: [],
						renderModal: SelectionModal,
						options: fireTypes,
						isFilterApplied: Boolean(filters.fireType.length),
						areValuesEquals: areArraysEqualByPrimitiveItems,
					}),
			}),
			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.fireReason,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.fireReason,
						setFilter: setFilters.fireReason as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: Boolean(filters.fireReason),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...CONTRACTS_GRID_COLUMNS.fireDescription,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.fireDescription,
						setFilter: setFilters.fireDescription as React.Dispatch<
							React.SetStateAction<string>
						>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: Boolean(filters.fireDescription),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),

			getEditColumnFactory({
				key: '',
				name: '',
				disabled: areFiltersApplied !== undefined ? !areFiltersApplied : false,
				sortable: false,
				frozen: true,
				clearFilter: () => setFilters.reset(),
				renderCell: (props) => (canEdit ? editCellRenderer(props) : null),
			}),
		],
		[filters]
	);
