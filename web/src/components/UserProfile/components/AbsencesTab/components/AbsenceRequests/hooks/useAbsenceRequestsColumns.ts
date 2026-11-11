import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { absenceRequestColumnWidths } from '../constants';
import { AbsenceRequestFiltersOptions } from '../types';
import { checkIsEmptyFilter } from '../utils/checkIsEmptyFilter';

import {
	cellWithTooltipRenderer,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { ApproveOrRejectRequestCellRenderer } from '@/components/Grid/renderers/cell/approveOrRejectRequestCellRenderer';
import { RequestStatusCellRenderer } from '@/components/Grid/renderers/cell/requestStatusCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	absenceRequestColumns,
	defaultAbsenceRequestFilterValues,
} from '@/constants/absence';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
} from '@/helpers/areValuesEqual';
import {
	AbsenceRequestFilters,
	AbsenceRequestGridRow,
	AbsenceRequestsFilterHandlers,
} from '@/types/absences';

export function useAbsenceRequestsColumns(
	filters: AbsenceRequestFilters,
	setFilters: AbsenceRequestsFilterHandlers,
	filtersOptions: AbsenceRequestFiltersOptions,
	onRequestFormOpen: (row: AbsenceRequestGridRow) => void
) {
	const { t } = useTranslation('profile', { keyPrefix: 'absence' });
	const columns: Column<AbsenceRequestGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...absenceRequestColumns.inquiryTemplateName,
				name: t('inquiryTemplateName'),
				width: getColumnWidth(absenceRequestColumnWidths.inquiryTemplateName)
					.width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.inquiryTemplateName || [],
						isFilterApplied: filters.inquiryTemplateName.length > 0,
						setFilter: setFilters.inquiryTemplateName as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: filtersOptions.absenceTypes,
						defaultValue: defaultAbsenceRequestFilterValues.inquiryTemplateName,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: onRequestFormOpen,
					}),
			}),
			getColumnWithTooltipFactory({
				...absenceRequestColumns.dateFrom,
				name: t('dateFrom'),
				width: getColumnWidth(absenceRequestColumnWidths.dateFrom).width,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.dateFrom || fromToDateObject,
						setFilter: setFilters.dateFrom as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!filters.dateFrom?.from || !!filters.dateFrom?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...absenceRequestColumns.dateTo,
				name: t('dateTo'),
				width: getColumnWidth(absenceRequestColumnWidths.dateTo).width,
				resizable: false,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.dateTo || fromToDateObject,
						setFilter: setFilters.dateTo as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!filters.dateTo?.from || !!filters.dateTo?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...absenceRequestColumns.quantity,
				name: t('quantity'),
				width: getColumnWidth(absenceRequestColumnWidths.quantity).width,
			}),
			getColumnWithTooltipFactory({
				...absenceRequestColumns.status,
				name: t('status'),
				width: getColumnWidth(absenceRequestColumnWidths.status).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.status,
						isFilterApplied: filters.status.length > 0,
						setFilter: setFilters.status,
						renderModal: SelectionModal,
						options: filtersOptions.statuses,
						defaultValue: defaultAbsenceRequestFilterValues.status,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					RequestStatusCellRenderer(props, {
						onCellClick: onRequestFormOpen,
						isClickable: true,
					}),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: checkIsEmptyFilter(filters),
				clearFilter: setFilters.reset,
				frozen: true,
				renderCell: ApproveOrRejectRequestCellRenderer,
			}),
		],
		[filters]
	);

	return columns;
}
