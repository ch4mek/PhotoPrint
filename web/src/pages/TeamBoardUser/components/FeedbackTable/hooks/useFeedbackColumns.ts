import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_DATE_FILTER_VALUE,
	FEEDBACK_COLUMN_NAMES,
	FEEDBACK_COLUMNS_KEYS,
} from '../constants';
import { linkCellRenderer } from '../renders/linkCellRenderer';
import {
	createColumnsComparator,
	getFeedbackTagStyles,
	shouldDisplayLink,
} from '../utils';

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
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { tagCellRenderer } from '@/components/Grid/renderers/cell/tagCellRenderer';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { FEEDBACK_TYPE_TAG_WIDTH } from '@/constants/feedback';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { SECTION_TEXT_MAX_ROW_COUNT } from '@/pages/TeamBoardUser/constants';
import {
	FeedbackFilter,
	FeedbackFilterSetter,
	FeedbackRow,
} from '@/pages/TeamBoardUser/types';
import { TeamboardTableMode } from '@/types/teamboard';

export const useFeedbackTableColumns = (
	mode: TeamboardTableMode,
	columnWidthsAndOrder: {
		[key in keyof typeof FEEDBACK_COLUMNS_KEYS]?: number | undefined;
	},
	onLinkClick: (row: FeedbackRow) => void,
	feedbackOptions: Array<SelectionModalOption>,
	projectsAutocompleteOptions: string[],
	clientsAutocompleteOptions: string[],
	reviewersAutocompleteOptions: string[],
	filter?: FeedbackFilter,
	filterSetter?: FeedbackFilterSetter,
	areFiltersApplied = false,
	resetFilter?: () => void
): readonly Column<FeedbackRow>[] =>
	useMemo(() => {
		const {
			date,
			type,
			client,
			project,
			feedback,
			actions,
			reviewer,
			conversation,
		} = columnWidthsAndOrder;

		const columns: Array<Column<FeedbackRow> | undefined> = [
			mode === TeamboardTableMode.Tab
				? getColumnWithTooltipFactory({
						key: FEEDBACK_COLUMNS_KEYS.date,
						name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.date],
						width: date,
						sortable: true,
						renderCell: (props) =>
							cellWithTooltipRenderer(props, {
								fieldToDisplay: 'date',
							}),
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<
											FeedbackRow,
											FromToProps<Date>
										>({
											...props,
											filter: filter.date,
											isFilterApplied: !areFromToCompatibleObjectsEqual(
												filter.date,
												DEFAULT_DATE_FILTER_VALUE
											),
											setFilter: filterSetter.date as Dispatch<
												SetStateAction<FromToProps<Date>>
											>,
											areValuesEquals: areFromToCompatibleObjectsEqual,
											defaultValue: DEFAULT_DATE_FILTER_VALUE,
											renderModal: FromToDateModal,
											disableFilterIconActiveStateAsDefault: true,
										})
								: undefined,
				  })
				: undefined,
			getColumnWithTooltipFactory({
				key: FEEDBACK_COLUMNS_KEYS.type,
				name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.type],
				width: type,
				renderCell: (props) =>
					tagCellRenderer(
						props,
						getFeedbackTagStyles,
						undefined,
						FEEDBACK_TYPE_TAG_WIDTH
					),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<FeedbackRow, string[]>({
									...props,
									filter: filter.type,
									isFilterApplied: !areArraysEqualByPrimitiveItems(
										filter.type,
										[]
									),
									setFilter: filterSetter.type as Dispatch<
										SetStateAction<string[]>
									>,
									areValuesEquals: areArraysEqualByPrimitiveItems,
									defaultValue: [],
									renderModal: SelectionModal,
									disableFilterIconActiveStateAsDefault: true,
									options: feedbackOptions,
								})
						: undefined,
			}),
			getColumnWithTooltipFactory({
				key: FEEDBACK_COLUMNS_KEYS.client,
				name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.client],
				width: client,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<FeedbackRow, string>({
									...props,
									filter: filter.client,
									isFilterApplied: !!filter.client,
									setFilter: filterSetter.client as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextAutoComplete,
									disableFilterIconActiveStateAsDefault: true,
									autocompleteOptions: clientsAutocompleteOptions,
								})
						: undefined,
			}),
			mode === TeamboardTableMode.Tab
				? getColumnWithTooltipFactory({
						key: FEEDBACK_COLUMNS_KEYS.project,
						name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.project],
						width: project,
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<FeedbackRow, string>({
											...props,
											filter: filter.project,
											isFilterApplied: !!filter.project,
											setFilter: filterSetter.project as Dispatch<
												SetStateAction<string>
											>,
											areValuesEquals: arePrimitiveValuesEqual,
											defaultValue: '',
											renderModal: FreeTextAutoComplete,
											disableFilterIconActiveStateAsDefault: true,
											autocompleteOptions: projectsAutocompleteOptions,
										})
								: undefined,
				  })
				: undefined,
			getColumnWithTooltipFactory({
				key: FEEDBACK_COLUMNS_KEYS.feedback,
				name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.feedback],
				width: feedback,
				renderCell:
					mode === TeamboardTableMode.Section
						? (props) =>
								DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT)
						: DynamicHeightCellRenderer,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<FeedbackRow, string>({
									...props,
									filter: filter.feedback,
									isFilterApplied: !!filter.feedback,
									setFilter: filterSetter.feedback as Dispatch<
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
				key: FEEDBACK_COLUMNS_KEYS.actions,
				name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.actions],
				width: actions,
				renderCell:
					mode === TeamboardTableMode.Section
						? (props) =>
								DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT)
						: DynamicHeightCellRenderer,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<FeedbackRow, string>({
									...props,
									filter: filter.actions,
									isFilterApplied: !!filter.actions,
									setFilter: filterSetter.actions as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextModal,
									disableFilterIconActiveStateAsDefault: true,
								})
						: undefined,
			}),
			mode === TeamboardTableMode.Tab
				? getColumnWithTooltipFactory({
						key: FEEDBACK_COLUMNS_KEYS.conversation,
						name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.conversation],
						width: conversation,
						renderCell: (props) =>
							linkCellRenderer(props, onLinkClick, shouldDisplayLink),
				  })
				: undefined,
			getColumnWithTooltipFactory({
				key: FEEDBACK_COLUMNS_KEYS.reviewer,
				name: FEEDBACK_COLUMN_NAMES[FEEDBACK_COLUMNS_KEYS.reviewer],
				width: reviewer,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<FeedbackRow, string>({
									...props,
									filter: filter.reviewer,
									isFilterApplied: !!filter.reviewer,
									setFilter: filterSetter.reviewer as Dispatch<
										SetStateAction<string>
									>,
									areValuesEquals: arePrimitiveValuesEqual,
									defaultValue: '',
									renderModal: FreeTextAutoComplete,
									disableFilterIconActiveStateAsDefault: true,
									autocompleteOptions: reviewersAutocompleteOptions,
								})
						: undefined,
			}),
		];

		const columnsOrder = Object.keys(columnWidthsAndOrder);

		const sortedColumns = (
			columns.filter(Boolean) as Array<Column<FeedbackRow>>
		).sort(createColumnsComparator(columnsOrder));

		sortedColumns.push(
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: !areFiltersApplied,
				displayFilterButton: Boolean(filter && filterSetter),
				clearFilter: resetFilter,
				frozen: true,
			})
		);

		return sortedColumns;
	}, [
		mode,
		filter,
		filterSetter,
		resetFilter,
		areFiltersApplied,
		columnWidthsAndOrder,
		feedbackOptions,
		projectsAutocompleteOptions,
		clientsAutocompleteOptions,
		reviewersAutocompleteOptions,
		onLinkClick,
	]);
