import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { SendToReviewCell } from '../components/SendToReviewCell';
import {
	defaultDevelopmentPlanFilterValues,
	developmentPlanTabTableColumnsWidths,
	developmentPlanTableKeys,
} from '../constants';
import {
	UserDevelopmentPlanFilterHandlers,
	UserDevelopmentPlanFilters,
	UserDevelopmentPlanRow,
} from '../types';
import { isResetDisabled } from '../utils';

import {
	FreeTextModal,
	getColumnFactory,
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
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { tagCellRenderer } from '@/components/Grid/renderers/cell/tagCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	DEVELOPMENT_PLAN_STATUS_TAG_WIDTH,
	developmentStatusOptions,
} from '@/constants/developmentPlan';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { getDevelopmentStatusTagColor } from '@/helpers/getDevelopmentStatusTagColor';
import { DevelopmentPlanStatus } from '@/types/developmentPlan';

export function useDevelopmentPlanColumns(
	filters: UserDevelopmentPlanFilters,
	setFilters: UserDevelopmentPlanFilterHandlers
) {
	const { t } = useTranslation('profile', { keyPrefix: 'developmentPlanTab' });

	return useMemo(
		() => [
			getColumnFactory<UserDevelopmentPlanRow>({
				key: developmentPlanTableKeys.status,
				name: t('status'),
				width: getColumnWidth(developmentPlanTabTableColumnsWidths.status)
					.width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.status,
						isFilterApplied: filters.status.length > 0,
						setFilter: setFilters.status,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						defaultValue: defaultDevelopmentPlanFilterValues.status,
						renderModal: SelectionModal,
						options: developmentStatusOptions,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					props.row.status !== DevelopmentPlanStatus.None
						? tagCellRenderer(
								props,
								getDevelopmentStatusTagColor,
								undefined,
								DEVELOPMENT_PLAN_STATUS_TAG_WIDTH
						  )
						: undefined,
			}),
			getColumnWithTooltipFactory({
				key: developmentPlanTableKeys.deadlineDate,
				name: t('deadlineDate'),
				width: getColumnWidth(developmentPlanTabTableColumnsWidths.deadlineDate)
					.width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.deadlineDate,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.deadlineDate,
							defaultDevelopmentPlanFilterValues.deadlineDate
						),
						setFilter: setFilters.deadlineDate,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						defaultValue: defaultDevelopmentPlanFilterValues.deadlineDate,
						renderModal: FromToDateModal,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: developmentPlanTableKeys.summary,
				name: t('summary'),
				width: getColumnWidth(developmentPlanTabTableColumnsWidths.summary)
					.width,

				renderCell: DynamicHeightCellRenderer,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.summary,
						isFilterApplied: Boolean(filters.summary),
						setFilter: setFilters.summary,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: defaultDevelopmentPlanFilterValues.summary,
						renderModal: FreeTextModal,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: developmentPlanTableKeys.startDate,
				name: t('startDate'),
				width: getColumnWidth(developmentPlanTabTableColumnsWidths.startDate)
					.width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.startDate,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.startDate,
							defaultDevelopmentPlanFilterValues.startDate
						),
						setFilter: setFilters.startDate,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						defaultValue: defaultDevelopmentPlanFilterValues.startDate,
						renderModal: FromToDateModal,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: developmentPlanTableKeys.endDate,
				name: t('endDate'),
				width: getColumnWidth(developmentPlanTabTableColumnsWidths.endDate)
					.width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.endDate,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.endDate,
							defaultDevelopmentPlanFilterValues.endDate
						),
						setFilter: setFilters.endDate,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						defaultValue: defaultDevelopmentPlanFilterValues.endDate,
						renderModal: FromToDateModal,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				cellClass: EDIT_COLUMN_CELL_CLASS,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				disabled: isResetDisabled(filters),
				clearFilter: setFilters.reset,
				renderCell: SendToReviewCell,
				frozen: true,
			}),
		],
		[filters, t]
	);
}
