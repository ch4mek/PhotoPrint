import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	CONVERSATION_TAB_TABLE_WIDTHS,
	CONVERSATIONS_COLUMNS_KEYS,
	CONVERSATIONS_COLUMNS_NAMES,
	DEFAULT_DATE_FILTER_VALUE,
	typeOptions,
} from '../constants';
import { ConversationActionsCellRenderer } from '../renderers/ConversationActionsCellRenderer';
import { UseConversationColumnProps } from '../types';
import { getConversationResultTagStyles } from '../utils';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { EDIT_COLUMN_WIDTH } from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { attachedDocumentCellRenderer } from '@/components/Grid/renderers/cell/attachedDocumentCellRenderer/attachedDocumentCellRenderer';
import { cellWithLinksRenderer } from '@/components/Grid/renderers/cell/cellWithLinksRenderer';
import { DynamicHeightCellRenderer } from '@/components/Grid/renderers/cell/dynamicHeightCellRenderer';
import { editCellRenderer } from '@/components/Grid/renderers/cell/editCellRenderer';
import { tagCellRenderer } from '@/components/Grid/renderers/cell/tagCellRenderer';
import { FEEDBACK_TYPE_TAG_WIDTH } from '@/constants/feedback';
import { ConversationType } from '@/forms/TeamboardForm/Conversation/types';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { SECTION_TEXT_MAX_ROW_COUNT } from '@/pages/TeamBoardUser/constants';
import { ConversationRow } from '@/pages/TeamBoardUser/types';
import { TeamboardTableMode } from '@/types/teamboard';

export const useConversationsColumns = ({
	mode,
	filter,
	filterSetter,
	resetFilter,
	supervisorAutocompleteOptions,
	feedbackOptions,
	areFiltersApplied = false,
	shouldHideTypeColumn = false,
	isIntern = false,
}: UseConversationColumnProps): readonly Column<ConversationRow>[] => {
	const isTabMode = mode === TeamboardTableMode.Tab;

	const filteredTypeOptions = useMemo(
		() =>
			isIntern
				? typeOptions.filter(
						({ id }) => id !== ConversationType.PerformanceReview
				  )
				: typeOptions,
		[isIntern]
	);

	return useMemo(() => {
		const {
			date,
			theses,
			employeeTheses,
			agreements,
			conversationType,
			conversationResult,
		} = CONVERSATION_TAB_TABLE_WIDTHS;

		const columns: Array<Column<ConversationRow> | undefined> = [
			getColumnWithTooltipFactory({
				key: CONVERSATIONS_COLUMNS_KEYS.date,
				name: CONVERSATIONS_COLUMNS_NAMES[CONVERSATIONS_COLUMNS_KEYS.date],
				width: date.width,
				sortable: isTabMode,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<
									ConversationRow,
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
			}),
			getColumnWithTooltipFactory({
				key: CONVERSATIONS_COLUMNS_KEYS.conversationResult,
				name: CONVERSATIONS_COLUMNS_NAMES[
					CONVERSATIONS_COLUMNS_KEYS.conversationResult
				],
				sortable: isTabMode,
				width: conversationResult.width,
				renderCell: (props) =>
					props.row.conversationResult
						? tagCellRenderer<ConversationRow>(
								props,
								getConversationResultTagStyles,
								undefined,
								FEEDBACK_TYPE_TAG_WIDTH
						  )
						: null,
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<ConversationRow, string[]>({
									...props,
									filter: filter.conversationResult,
									isFilterApplied: !areArraysEqualByPrimitiveItems(
										filter.conversationResult,
										[]
									),
									setFilter: filterSetter.conversationResult as Dispatch<
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
			!shouldHideTypeColumn && isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.conversationType,
						name: CONVERSATIONS_COLUMNS_NAMES[
							CONVERSATIONS_COLUMNS_KEYS.conversationType
						],
						sortable: true,
						width: conversationType.width,
						renderCell: DynamicHeightCellRenderer,
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<ConversationRow, string[]>({
											...props,
											filter: filter?.conversationType || [],
											isFilterApplied: filter?.conversationType.length > 0,
											setFilter: filterSetter?.conversationType as Dispatch<
												SetStateAction<string[]>
											>,
											renderModal: SelectionModal,
											areValuesEquals: areArraysEqualByPrimitiveItems,
											options: filteredTypeOptions,
											defaultValue: [],
											disableFilterIconActiveStateAsDefault: true,
										})
								: undefined,
				  })
				: undefined,

			isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.theses,
						name: CONVERSATIONS_COLUMNS_NAMES[
							CONVERSATIONS_COLUMNS_KEYS.theses
						],
						width: theses,
						renderCell: (props) =>
							DynamicHeightCellRenderer(
								props,
								SECTION_TEXT_MAX_ROW_COUNT,
								false
							),
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<ConversationRow, string>({
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
				  })
				: undefined,

			isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.employeeTheses,
						name: CONVERSATIONS_COLUMNS_NAMES[
							CONVERSATIONS_COLUMNS_KEYS.employeeTheses
						],
						renderCell: (props) =>
							DynamicHeightCellRenderer(
								props,
								SECTION_TEXT_MAX_ROW_COUNT,
								false
							),
						width: employeeTheses,
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<ConversationRow, string>({
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
				  })
				: undefined,
			getColumnWithTooltipFactory({
				key: CONVERSATIONS_COLUMNS_KEYS.agreements,
				name: CONVERSATIONS_COLUMNS_NAMES[
					CONVERSATIONS_COLUMNS_KEYS.agreements
				],
				width: agreements,
				renderCell: (props) =>
					DynamicHeightCellRenderer(props, SECTION_TEXT_MAX_ROW_COUNT, false),
				renderHeaderCell:
					filter && filterSetter
						? (props) =>
								headerCellWithFilterRenderer<ConversationRow, string>({
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
			isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.supervisor,
						name: CONVERSATIONS_COLUMNS_NAMES[
							CONVERSATIONS_COLUMNS_KEYS.supervisor
						],
						renderHeaderCell:
							filter && filterSetter
								? (props) =>
										headerCellWithFilterRenderer<ConversationRow, string>({
											...props,
											filter: filter.supervisor,
											isFilterApplied: !!filter.supervisor,
											setFilter: filterSetter.supervisor as Dispatch<
												SetStateAction<string>
											>,
											areValuesEquals: arePrimitiveValuesEqual,
											defaultValue: '',
											renderModal: FreeTextAutoComplete,
											disableFilterIconActiveStateAsDefault: true,
											autocompleteOptions: supervisorAutocompleteOptions || [],
										})
								: undefined,
				  })
				: undefined,
			isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.files,
						name: CONVERSATIONS_COLUMNS_NAMES[CONVERSATIONS_COLUMNS_KEYS.files],
						renderCell: attachedDocumentCellRenderer,
				  })
				: undefined,
			isTabMode
				? getColumnWithTooltipFactory({
						key: CONVERSATIONS_COLUMNS_KEYS.documentLinks,
						name: CONVERSATIONS_COLUMNS_NAMES[
							CONVERSATIONS_COLUMNS_KEYS.documentLinks
						],
						renderCell: cellWithLinksRenderer,
				  })
				: undefined,

			getEditColumnFactory({
				key: '',
				name: '',
				width: isTabMode ? EDIT_COLUMN_WIDTH : undefined,
				disabled: !areFiltersApplied,
				displayFilterButton: Boolean(filter && filterSetter),
				clearFilter: resetFilter,
				renderCell: isTabMode
					? ConversationActionsCellRenderer
					: editCellRenderer,
				frozen: true,
			}),
		];

		return columns.filter(Boolean) as Array<Column<ConversationRow>>;
	}, [
		filter,
		filterSetter,
		resetFilter,
		areFiltersApplied,
		supervisorAutocompleteOptions,
		shouldHideTypeColumn,
		feedbackOptions,
		isTabMode,
		filteredTypeOptions,
	]);
};
