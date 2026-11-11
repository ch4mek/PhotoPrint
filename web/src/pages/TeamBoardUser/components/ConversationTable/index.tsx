import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { feedbackAutoselectOptions } from '@/constants/feedback';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { HeightsContext } from '@/context/CellHeightsContext';
import { TeamboardTableMode } from '@/types/teamboard';

import {
	CONVERSATION_LINE_HEIGHT,
	CONVERSATIONS_COLUMNS_KEYS,
} from './constants';
import { useConversationsColumns } from './hooks/useConversationsColumns';
import { ConversationTableProps } from './types';
import {
	getConversationEditColumnIndex,
	getConversationTableColumnWidth,
	overrideConversationGridStyles,
} from './utils';

export function ConversationTable({
	rows,
	setRows,
	mode,
	sortColumns,
	onSortColumnsChange,
	filter,
	setFilter,
	areFiltersApplied,
	resetFilter,
	pagination,
	isLoading,
	supervisorAutocompleteOptions,
	shouldHideTypeColumn,
	isIntern,
}: ConversationTableProps) {
	const feedbackOptions: Array<SelectionModalOption> = useMemo(
		() =>
			feedbackAutoselectOptions.map(({ id, name }) => ({
				id,
				description: name,
			})),
		[]
	);

	const columns = useConversationsColumns({
		mode,
		filter,
		filterSetter: setFilter,
		areFiltersApplied,
		resetFilter,
		supervisorAutocompleteOptions,
		feedbackOptions,
		shouldHideTypeColumn,
		isIntern,
	});

	const { handleUpdateRows, heightsMap, getHeight } = useDynamicRowsHeight({
		setRows,
		defaultRowHeight: DATA_GRID_ROW_HEIGHT_WITH_TAG,
		lineHeight: CONVERSATION_LINE_HEIGHT,
	});

	const overrideStyles = useMemo(() => {
		const columnWidths = getConversationTableColumnWidth(mode);

		const adjustedWidths =
			shouldHideTypeColumn && mode === TeamboardTableMode.Tab
				? Object.fromEntries(
						Object.entries(columnWidths).filter(
							([key]) => key !== CONVERSATIONS_COLUMNS_KEYS.conversationType
						)
				  )
				: columnWidths;

		return overrideConversationGridStyles(
			pagination ? !isLoading && !!rows.length : false,
			mode,
			getConversationEditColumnIndex(mode),
			getGridColumnTemplateFromColumnWidths(adjustedWidths, true)
		);
	}, [mode, pagination, rows.length, isLoading]);

	const isSection = mode === TeamboardTableMode.Section;

	return (
		<DataGridHoverRowWrapper
			defineHeight={false}
			flexGrow={mode === TeamboardTableMode.Tab}
		>
			<HeightsContext.Provider value={heightsMap}>
				<DataGrid
					columns={columns}
					rows={rows}
					sortColumns={sortColumns}
					onSortColumnsChange={onSortColumnsChange}
					overrideDataGridStyles={overrideStyles}
					onRowsChange={handleUpdateRows}
					rowHeight={getHeight}
					scrollToTopOnRowsChange={false}
					paginationProps={!isLoading ? pagination : undefined}
					renderers={noRowsGridFallback}
					enableVirtualization={false}
					{...(isSection && { maxRowsCount: 1 })}
					{...(isSection &&
						rows.length && {
							restrictionRowHeight: getHeight(rows[0]),
						})}
				/>
			</HeightsContext.Provider>
		</DataGridHoverRowWrapper>
	);
}
