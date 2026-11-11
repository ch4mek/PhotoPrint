import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { projectTableColumns } from '../constants';

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
import {
	ProjectTableColumnWidths,
	ProjectTableRow,
} from '@/components/UserProfile/types';
import { activityStatusOptions } from '@/constants/activityStatus';
import { billabilityOptions } from '@/constants/billabilityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	ProjectsTableFilters,
	ProjectTableFilterSetter,
} from '@/types/projectsTableFilters';

export const useProjectTableColumns = (
	columnWidths: ProjectTableColumnWidths,
	filters: ProjectsTableFilters,
	isEmployee: boolean,
	filterSetters?: ProjectTableFilterSetter,
	areFiltersApplied?: boolean,
	resetFilters?: () => void
) => {
	const { t } = useTranslation('profile', { keyPrefix: 'projects' });

	return useMemo(() => {
		const columns: Array<Column<ProjectTableRow> | null> = [
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectTableColumns.projectName,
				name: t('projectName'),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ProjectTableRow, string>({
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
					}),
				width: columnWidths.projectName,
			}),
			isEmployee
				? null
				: getColumnWithTooltipFactory<ProjectTableRow>({
						...projectTableColumns.clientName,
						name: t('clientName'),
						renderHeaderCell: (props) =>
							headerCellWithFilterRenderer<ProjectTableRow, string>({
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
							}),

						width: columnWidths.clientName,
				  }),
			getLogicalTextColumnFactory<ProjectTableRow>({
				...projectTableColumns.isActive,
				name: t('status'),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ProjectTableRow, string[]>({
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
					}),

				width: columnWidths.isActive,
			}),
			isEmployee
				? null
				: getLogicalTextColumnFactory<ProjectTableRow>({
						...projectTableColumns.billabillity,
						name: t('billability'),
						renderHeaderCell: (props) =>
							headerCellWithFilterRenderer<ProjectTableRow, string[]>({
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
							}),

						width: columnWidths.billability,
				  }),
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectTableColumns.startDate,
				name: t('startDate'),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ProjectTableRow, FromToProps<Date>>({
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
					}),
				width: columnWidths.startDate,
			}),
			getColumnWithTooltipFactory<ProjectTableRow>({
				...projectTableColumns.endDate,
				name: t('endDate'),
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ProjectTableRow, FromToProps<Date>>({
						...props,
						filter: filters.endDate || fromToDateObject,
						setFilter: filterSetters?.endDate as React.Dispatch<
							React.SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!filters.endDate?.from || !!filters.endDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				width: columnWidths.endDate,
			}),
			getEditColumnFactory<ProjectTableRow>({
				key: '',
				name: '',
				sortable: false,
				frozen: true,
				disabled: !areFiltersApplied,
				clearFilter: resetFilters || undefined,
			}),
		];

		return columns.filter((column) => column !== null);
	}, [filters, areFiltersApplied, isEmployee]);
};
