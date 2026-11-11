import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { defaultDevelopmentPlanFilterValues } from '../../DevelopmentPlanTab/constants';
import { DevelopmentPlanMenuCell } from '../components/DevelopmentPlanMenuCell';
import {
	developmentPlanTableColumnsNames,
	developmentPlanTableColumnsWidths,
} from '../constants';
import { getDevelopmentPlanTableColumnWidth } from '../utils';

import {
	editCellRenderer,
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { tagCellRenderer } from '@/components/Grid/renderers/cell/tagCellRenderer';
import {
	DEVELOPMENT_PLAN_STATUS_CELL_CLASS_NAME,
	DEVELOPMENT_PLAN_STATUS_TAG_WIDTH,
	developmentStatusOptions,
} from '@/constants/developmentPlan';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { getDevelopmentStatusTagColor } from '@/helpers/getDevelopmentStatusTagColor';
import { SECTION_TEXT_MAX_ROW_COUNT } from '@/pages/TeamBoardUser/constants';
import {
	DevelopmentPlanFilterHandlers,
	DevelopmentPlanFilters,
	DevelopmentPlanRow,
	DevelopmentPlanStatus,
} from '@/types/developmentPlan';
import { TeamboardTableMode } from '@/types/teamboard';

export const useDevelopmentPlanColumns = (
	mode: TeamboardTableMode,
	filter?: DevelopmentPlanFilters,
	filterSetter?: DevelopmentPlanFilterHandlers,
	areFiltersApplied = false
): readonly Column<DevelopmentPlanRow>[] =>
	useMemo(() => {
		const isTabMode = TeamboardTableMode.Tab === mode;
		const isPopUpMode = TeamboardTableMode.PopUp === mode;

		const columns: Column<DevelopmentPlanRow>[] = [
			getColumnWithTooltipFactory({
				...developmentPlanTableColumnsNames.status,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].status
				),
				sortable: isTabMode,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.status,
									isFilterApplied: filter.status.length > 0,
									setFilter: filterSetter.status,
									areValuesEquals: areArraysEqualByPrimitiveItems,
									defaultValue: defaultDevelopmentPlanFilterValues.status,
									renderModal: SelectionModal,
									options: developmentStatusOptions,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
				cellClass: DEVELOPMENT_PLAN_STATUS_CELL_CLASS_NAME,
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
				...developmentPlanTableColumnsNames.deadlineDate,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].deadlineDate
				),
				sortable: isTabMode,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.deadlineDate,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filter.deadlineDate,
										defaultDevelopmentPlanFilterValues.deadlineDate
									),
									setFilter: filterSetter.deadlineDate,
									areValuesEquals: areFromToCompatibleObjectsEqual,
									defaultValue: defaultDevelopmentPlanFilterValues.deadlineDate,
									renderModal: FromToDateModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				...developmentPlanTableColumnsNames.summary,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].summary
				),
				renderCell: (props) =>
					isTabMode
						? DynamicHeightCellRenderer(props)
						: DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.summary,
									isFilterApplied: Boolean(filter.summary),
									setFilter: filterSetter.summary,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: defaultDevelopmentPlanFilterValues.summary,
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				...developmentPlanTableColumnsNames.notes,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].notes
				),
				renderCell: DynamicHeightCellRenderer,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.notes,
									isFilterApplied: Boolean(filter.notes),
									setFilter: filterSetter.notes,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: defaultDevelopmentPlanFilterValues.notes,
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				...developmentPlanTableColumnsNames.startDate,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].startDate
				),
				sortable: isTabMode,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.startDate,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filter.startDate,
										defaultDevelopmentPlanFilterValues.startDate
									),
									setFilter: filterSetter.startDate,
									areValuesEquals: areFromToCompatibleObjectsEqual,
									defaultValue: defaultDevelopmentPlanFilterValues.startDate,
									renderModal: FromToDateModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				...developmentPlanTableColumnsNames.endDate,
				width: getDevelopmentPlanTableColumnWidth(
					developmentPlanTableColumnsWidths[mode].endDate
				),
				sortable: true,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer({
									...props,
									filter: filter.endDate,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filter.endDate,
										defaultDevelopmentPlanFilterValues.endDate
									),
									setFilter: filterSetter.endDate,
									areValuesEquals: areFromToCompatibleObjectsEqual,
									defaultValue: defaultDevelopmentPlanFilterValues.endDate,
									renderModal: FromToDateModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: !areFiltersApplied,
				displayFilterButton: Boolean(filter && filterSetter),
				clearFilter: filterSetter?.reset,
				renderCell: isPopUpMode ? editCellRenderer : DevelopmentPlanMenuCell,
				frozen: true,
			}),
		];

		if (isPopUpMode) {
			const startDateColumn = columns.splice(4, 1);
			columns.unshift(...startDateColumn);
		}

		return columns.filter(({ width }) => width !== 0);
	}, [mode, filter, filterSetter, areFiltersApplied]);
