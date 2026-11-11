import { ReactNode, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import {
	defaultUserPerformanceReviewFilters,
	userPerformanceReviewColumns,
	userPerformanceReviewGridColumnWidths,
} from '../constants';
import {
	UserPerformanceReviewFilters,
	UserPerformanceReviewFiltersHandlers,
	UserPerformanceReviewGridRow,
} from '../types';
import { isResetDisabled } from '../utils';

import {
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { attachedDocumentCellRenderer } from '@/components/Grid/renderers/cell/attachedDocumentCellRenderer/attachedDocumentCellRenderer';
import { cellWithLinksRenderer } from '@/components/Grid/renderers/cell/cellWithLinksRenderer';
import { dateCellRenderer } from '@/components/Grid/renderers/cell/dateCellRenderer';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { PERFORMANCE_REVIEW_DATE_TAG_WIDTH } from '@/constants/performanceReview';
import {
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export function usePerformanceReviewColumns(
	filters: UserPerformanceReviewFilters,
	setFilters: UserPerformanceReviewFiltersHandlers,
	tooltipDateContent?: ReactNode
) {
	const { t } = useTranslation('profile', {
		keyPrefix: 'performanceReviewTab',
	});

	return useMemo(
		() => [
			getColumnWithTooltipFactory<UserPerformanceReviewGridRow>({
				key: userPerformanceReviewColumns.performanceReviewDate.key,
				name: t('performanceReviewDate'),
				width: getColumnWidth(
					userPerformanceReviewGridColumnWidths.performanceReviewDate
				).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.performanceReviewDate,
						isFilterApplied: !areFromToCompatibleObjectsEqual(
							filters.performanceReviewDate,
							defaultUserPerformanceReviewFilters.performanceReviewDate
						),
						setFilter: setFilters.performanceReviewDate,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						defaultValue:
							defaultUserPerformanceReviewFilters.performanceReviewDate,
						renderModal: FromToDateModal,
						tooltipContent: tooltipDateContent,
					}),
				renderCell: (props) =>
					dateCellRenderer(
						props,
						'performanceReviewDate',
						'performanceReviewDateType',
						PERFORMANCE_REVIEW_DATE_TAG_WIDTH
					),
			}),
			getColumnWithTooltipFactory({
				key: userPerformanceReviewColumns.agreements.key,
				name: t('agreements'),
				width: getColumnWidth(userPerformanceReviewGridColumnWidths.agreements)
					.width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.agreements,
						isFilterApplied: Boolean(filters.agreements),
						setFilter: setFilters.agreements,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: defaultUserPerformanceReviewFilters.agreements,
						renderModal: FreeTextModal,
					}),
				renderCell: DynamicHeightCellRenderer,
			}),
			getColumnWithTooltipFactory<UserPerformanceReviewGridRow>({
				key: userPerformanceReviewColumns.files.key,
				name: t('documents'),
				width: getColumnWidth(userPerformanceReviewGridColumnWidths.files)
					.width,
				renderCell: attachedDocumentCellRenderer,
			}),
			getColumnWithTooltipFactory<UserPerformanceReviewGridRow>({
				key: userPerformanceReviewColumns.links.key,
				name: t('docLinks'),
				width: getColumnWidth(userPerformanceReviewGridColumnWidths.links)
					.width,
				renderCell: cellWithLinksRenderer,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetDisabled(filters),
				clearFilter: setFilters.reset,
				frozen: true,
			}),
		],
		[filters, t]
	);
}
