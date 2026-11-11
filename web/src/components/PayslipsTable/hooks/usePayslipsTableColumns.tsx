import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { PayslipLink } from '../components/PayslipLink';
import {
	PAYSLIPS_COLUMN_KEYS,
	PAYSLIPS_COLUMN_SPAN,
	PAYSLIPS_TABLE_COLUMNS_WIDTHS,
} from '../constants';
import {
	PayslipRow,
	PayslipsFiltersSetter,
	PayslipsTableFilters,
} from '../types';

import {
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellRenderer,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { fromToDateObject } from '@/constants/defaultValues';
import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';

export const usePayslipsTableColumns = (
	filters?: PayslipsTableFilters,
	filterSetter?: PayslipsFiltersSetter,
	areFiltersApplied?: boolean,
	resetFilters?: () => void
): Array<Column<PayslipRow>> => {
	const { t } = useTranslation('profile', { keyPrefix: 'salaryTab' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: PAYSLIPS_COLUMN_KEYS.reportDate,
				name: t('reportDate'),
				width: PAYSLIPS_TABLE_COLUMNS_WIDTHS.reportDate,
				renderHeaderCell:
					filterSetter && filters
						? (props) =>
								headerCellWithFilterRenderer<PayslipRow, FromToProps<Date>>({
									...props,
									filter: filters.reportDate || fromToDateObject,
									setFilter: filterSetter.reportDate as React.Dispatch<
										React.SetStateAction<FromToProps<Date>>
									>,
									defaultValue: fromToDateObject,
									renderModal: FromToDateModal,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filters.reportDate,
										fromToDateObject
									),
									areValuesEquals: areFromToCompatibleObjectsEqual,
								})
						: undefined,
				sortable: true,
			}),
			getColumnWithTooltipFactory({
				key: PAYSLIPS_COLUMN_KEYS.fileName,
				name: t('fileName'),
				width: PAYSLIPS_TABLE_COLUMNS_WIDTHS.fileName,
				renderHeaderCell: (props) => headerCellRenderer({ ...props }),
				renderCell: ({ row: { fileName, fileId } }) => (
					<PayslipLink fileName={fileName} fileId={fileId} />
				),
				sortable: false,
				colSpan: ({ type }) => (type === 'ROW' ? PAYSLIPS_COLUMN_SPAN : 1),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: areFiltersApplied !== undefined ? !areFiltersApplied : false,
				clearFilter: resetFilters || undefined,
				renderCell: null,
				sortable: false,
				frozen: true,
			}),
		],
		[areFiltersApplied, filters]
	);
};
