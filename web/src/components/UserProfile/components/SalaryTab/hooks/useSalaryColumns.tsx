import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	defaultUserSalaryFilters,
	userSalaryGridColumnsKeys,
	useSalaryGridColumnWidth,
} from '../constants';
import {
	UserSalaryFilters,
	UserSalaryFiltersHandlers,
	UserSalaryRow,
} from '../types';
import { isResetDisabled } from '../utils';

import {
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { FromToPriceModal } from '@/components/Grid/modals/fromToPriceModalRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { SalaryCell } from '@/components/SalaryCell';
import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';

export function useSalaryColumns(
	filters: UserSalaryFilters,
	setFilters: UserSalaryFiltersHandlers
) {
	const { t } = useTranslation('profile', { keyPrefix: 'salaryTab' });

	return useMemo<Array<Column<UserSalaryRow>>>(
		() => [
			getColumnWithTooltipFactory({
				key: userSalaryGridColumnsKeys.salaryFrom,
				name: t('salaryFrom'),
				width: getColumnWidth(useSalaryGridColumnWidth.salaryFrom).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.salaryFrom,
						setFilter: setFilters.salaryFrom,
						renderModal: FromToDateModal,
						defaultValue: defaultUserSalaryFilters.salaryFrom,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.salaryFrom,
							defaultUserSalaryFilters.salaryFrom
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),

				renderCell: ({ row: { salaryFrom, isCurrent } }) => (
					<SalaryCell {...{ isCurrent }}>{salaryFrom}</SalaryCell>
				),
			}),
			getColumnWithTooltipFactory({
				key: userSalaryGridColumnsKeys.salaryUsd,
				name: t('salary'),
				width: getColumnWidth(useSalaryGridColumnWidth.salaryUsd).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.salaryUsd,
						setFilter: setFilters.salaryUsd,
						renderModal: (modalProps) =>
							FromToPriceModal({
								...modalProps,
								includeDecimalPart: false,
							}),
						defaultValue: defaultUserSalaryFilters.salaryUsd,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.salaryUsd,
							defaultUserSalaryFilters.salaryUsd
						),
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),

				renderCell: ({ row: { salaryUsd, isCurrent } }) => (
					<SalaryCell {...{ isCurrent }}>{salaryUsd}</SalaryCell>
				),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetDisabled(filters),
				clearFilter: setFilters.reset,
				frozen: true,
			}),
		],
		[filters]
	);
}
