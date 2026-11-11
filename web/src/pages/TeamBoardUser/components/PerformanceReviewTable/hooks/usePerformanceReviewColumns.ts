import { Dispatch, ReactNode, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	PERFORMANCE_REVIEW_COLUMNS_KEYS,
	PERFORMANCE_REVIEW_COLUMNS_NAMES,
	PERFORMANCE_REVIEW_TAB_TABLE_WIDTHS,
} from '../constants';

import {
	editCellRenderer,
	FreeTextModal,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { attachedDocumentCellRenderer } from '@/components/Grid/renderers/cell/attachedDocumentCellRenderer/attachedDocumentCellRenderer';
import { cellWithLinksRenderer } from '@/components/Grid/renderers/cell/cellWithLinksRenderer';
import { dateCellRenderer } from '@/components/Grid/renderers/cell/dateCellRenderer';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { fromToDateObject } from '@/constants/defaultValues';
import { PERFORMANCE_REVIEW_DATE_TAG_WIDTH } from '@/constants/performanceReview';
import {
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { SECTION_TEXT_MAX_ROW_COUNT } from '@/pages/TeamBoardUser/constants';
import {
	PerformanceReviewFilter,
	PerformanceReviewFilterSetter,
	PerformanceReviewRow,
} from '@/pages/TeamBoardUser/types';

export const usePerformanceReviewColumns = (
	filter?: PerformanceReviewFilter,
	filterSetter?: PerformanceReviewFilterSetter,
	areFiltersApplied = false,
	resetFilter?: () => void,
	tooltipDateContent?: ReactNode
): readonly Column<PerformanceReviewRow>[] =>
	useMemo(() => {
		const { performanceReviewDate, theses, employeeTheses, agreements } =
			PERFORMANCE_REVIEW_TAB_TABLE_WIDTHS;

		const columns: Array<Column<PerformanceReviewRow> | undefined> = [
			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.performanceReviewDate,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.performanceReviewDate
				],
				width: performanceReviewDate.width,
				sortable: true,
				renderCell: (props) =>
					dateCellRenderer(
						props,
						'performanceReviewDate',
						'performanceReviewDateType',
						PERFORMANCE_REVIEW_DATE_TAG_WIDTH
					),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<
									PerformanceReviewRow,
									FromToProps<Date>
								>({
									...props,
									filter: filter.performanceReviewDate,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filter.performanceReviewDate,
										fromToDateObject
									),
									setFilter: filterSetter.performanceReviewDate as Dispatch<
										SetStateAction<FromToProps<Date>>
									>,
									areValuesEquals: areFromToCompatibleObjectsEqual,
									defaultValue: fromToDateObject,
									renderModal: FromToDateModal,
									disableFilterIconActiveStateAsDefault: true,
									tooltipContent: tooltipDateContent,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.theses,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.theses
				],
				width: theses,
				renderCell: (props) =>
					DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT, false),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<PerformanceReviewRow, string>({
									...props,
									filter: filter.theses,
									isFilterApplied: !!filter.theses,
									setFilter: filterSetter.theses as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.employeeTheses,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.employeeTheses
				],
				renderCell: (props) =>
					DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT, false),
				width: employeeTheses,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<PerformanceReviewRow, string>({
									...props,
									filter: filter.employeeTheses,
									isFilterApplied: !!filter.employeeTheses,
									setFilter: filterSetter.employeeTheses as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),

			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.agreements,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.agreements
				],
				width: agreements,
				renderCell: (props) =>
					DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT, false),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<PerformanceReviewRow, string>({
									...props,
									filter: filter.agreements,
									isFilterApplied: !!filter.agreements,
									setFilter: filterSetter.agreements as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),

			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.files,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.files
				],
				renderCell: attachedDocumentCellRenderer,
			}),

			getColumnWithTooltipFactory({
				key: PERFORMANCE_REVIEW_COLUMNS_KEYS.documentLinks,
				name: PERFORMANCE_REVIEW_COLUMNS_NAMES[
					PERFORMANCE_REVIEW_COLUMNS_KEYS.documentLinks
				],
				renderCell: cellWithLinksRenderer,
			}),

			getEditColumnFactory({
				key: '',
				name: '',
				disabled: !areFiltersApplied,
				displayFilterButton: Boolean(filter && filterSetter),
				clearFilter: resetFilter,
				frozen: true,
				renderCell: editCellRenderer,
			}),
		];

		return columns.filter(Boolean) as Array<Column<PerformanceReviewRow>>;
	}, [filter, filterSetter, resetFilter, areFiltersApplied]);
