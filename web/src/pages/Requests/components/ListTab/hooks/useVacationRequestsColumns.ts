import { ViewIcon } from '@mms/mms-ui-library/icons/base/ViewIcon';
import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_REQUESTS_FILTERS,
	REQUESTS_COLUMNS,
	RequestsColumnsWidths,
} from '../constants';
import { OptionsProps, RequestFilterType, RequestGridRow } from '../types';
import { checkIsEmptyFilter, generateSetFilters } from '../utils';

import {
	cellWithTooltipRenderer,
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { absenceNameCellRenderer } from '@/components/Grid/renderers/cell/absenceNameCellRenderer';
import { actionCellRenderer } from '@/components/Grid/renderers/cell/actionCellRenderer';
import { RequestStatusCellRenderer } from '@/components/Grid/renderers/cell/requestStatusCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export const useVacationRequestsColumns = (
	filters: RequestFilterType,
	setFilters: ReturnType<typeof generateSetFilters>,
	options: OptionsProps,
	enableCellClick: (row: RequestGridRow) => boolean,
	onCellClick?: (row: RequestGridRow) => void
): Array<Column<RequestGridRow>> => {
	const clearFilter = () => {
		setFilters.reset(DEFAULT_REQUESTS_FILTERS);
	};

	const handleRequestFormOpen = useCallback(
		(row: RequestGridRow) => {
			options.onFormOpen({
				inquiryId: row.id,
			});
		},
		[options]
	);

	const columns: Column<RequestGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.id,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string>({
						...props,
						filter: filters.userId,
						setFilter: setFilters.userId as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.userId,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				width: getColumnWidth(RequestsColumnsWidths.id).width,
			}),
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.user,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string>({
						...props,
						filter: filters.userFullName,
						isFilterApplied: !!filters.userFullName,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(RequestsColumnsWidths.userFullName).width,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row) ? onCellClick : undefined,
						isClickable: props.row.managedByRoleRequest,
					}),
			}),
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.department,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string[]>({
						...props,
						filter: filters.userDepartment || [],
						isFilterApplied: filters.userDepartment.length > 0,
						setFilter: setFilters.departments as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.departments,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(RequestsColumnsWidths.userDepartment).width,
			}),
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.position,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string[]>({
						...props,
						filter: filters.userPosition || [],
						isFilterApplied: filters.userPosition.length > 0,
						setFilter: setFilters.userPosition as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.positions,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
				width: getColumnWidth(RequestsColumnsWidths.userPosition).width,
			}),
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.inquiryTemplateName,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string>({
						...props,
						filter: filters.inquiryTemplateName,
						isFilterApplied: !!filters.inquiryTemplateName,
						setFilter: setFilters.inquiryTemplateName as Dispatch<
							SetStateAction<string>
						>,
						renderModal: FreeTextAutoComplete,
						areValuesEquals: arePrimitiveValuesEqual,
						autocompleteOptions: options.absenceNames,
						defaultValue: '',
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					absenceNameCellRenderer(props, {
						onCellClick: handleRequestFormOpen,
						isClickable: true,
					}),
				width: getColumnWidth(RequestsColumnsWidths.inquiryTemplateName).width,
			}),
			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.quantity,
				width: getColumnWidth(RequestsColumnsWidths.quantity).width,
			}),

			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.status,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string[]>({
						...props,
						filter: filters.status || [],
						isFilterApplied: filters.status.length > 0,
						setFilter: setFilters.status as Dispatch<SetStateAction<string[]>>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.statuses,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					RequestStatusCellRenderer(props, {
						onCellClick: handleRequestFormOpen,
						isClickable: true,
					}),
				width: getColumnWidth(RequestsColumnsWidths.status).width,
			}),
			getColumnWithTooltipFactory<RequestGridRow>({
				...REQUESTS_COLUMNS.dateFrom,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, FromToProps<Date>>({
						...props,
						filter: filters.dateFrom || fromToDateObject,
						setFilter: setFilters?.dateFrom as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!filters.dateFrom?.from || !!filters.dateFrom?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				width: getColumnWidth(RequestsColumnsWidths.dateFrom).width,
			}),

			getColumnWithTooltipFactory<RequestGridRow>({
				...REQUESTS_COLUMNS.dateTo,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, FromToProps<Date>>({
						...props,
						filter: filters.dateTo || fromToDateObject,
						setFilter: setFilters?.dateTo as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!filters.dateTo?.from || !!filters.dateTo?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				width: getColumnWidth(RequestsColumnsWidths.dateTo).width,
			}),

			getColumnWithTooltipFactory<RequestGridRow>({
				...REQUESTS_COLUMNS.created,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, FromToProps<Date>>({
						...props,
						filter: filters.createdAtUtc || fromToDateObject,
						setFilter: setFilters?.createdAtUtc as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.createdAtUtc?.from || !!filters.createdAtUtc?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				width: getColumnWidth(RequestsColumnsWidths.createdAtUtc).width,
			}),

			getColumnWithTooltipFactory({
				...REQUESTS_COLUMNS.notes,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RequestGridRow, string>({
						...props,
						filter: filters.notes,
						setFilter: setFilters.notes as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: !!filters.notes,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				width: getColumnWidth(RequestsColumnsWidths.id).width,
			}),

			getEditColumnFactory({
				key: '',
				name: '',
				renderCell: ({ row }) =>
					actionCellRenderer({
						icon: ViewIcon,
						onClick: () => handleRequestFormOpen(row),
					}),
				disabled: checkIsEmptyFilter(filters),
				clearFilter,
				sortable: false,
				frozen: true,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		],
		[options, filters]
	);

	return useMemo(() => columns.filter(({ width }) => width !== 0), [columns]);
};
