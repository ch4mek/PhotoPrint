import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { contractsGridColumns } from '../constants';
import {
	ContractsFilters,
	SetFiltersType,
	UserContractGridRow,
} from '../types';

import {
	editCellRenderer,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { fromToDateObject } from '@/constants/defaultValues';
import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';

export const useContractsColumns = (
	setFilters: SetFiltersType,
	filters: ContractsFilters,
	areFiltersApplied?: boolean
) => {
	const { t } = useTranslation('profile', { keyPrefix: 'contractTab' });
	const columns: Column<UserContractGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...contractsGridColumns.contractFrom,
				name: t('startDate'),
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.contractFrom || fromToDateObject,
						setFilter: setFilters.contractFrom as Dispatch<
							SetStateAction<FromToProps<Date>>
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
				...contractsGridColumns.contractTo,
				name: t('endDate'),
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.contractTo || fromToDateObject,
						setFilter: setFilters.contractTo as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							Boolean(filters.contractTo?.from) ||
							Boolean(filters.contractTo?.to),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: areFiltersApplied !== undefined ? !areFiltersApplied : false,
				sortable: false,
				frozen: true,
				clearFilter: () => setFilters.reset(),
				renderCell: (props) => editCellRenderer(props),
			}),
		],
		[filters]
	);

	return columns;
};
