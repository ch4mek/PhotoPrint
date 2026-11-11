import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useContext, useMemo } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { useConversationFormState } from '../../hooks/useConversationFormState';
import { FeedbackRow } from '../../types';
import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { displayMinimalGridRowsProp } from '@/components/Grid/constants';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { feedbackAutoselectOptions } from '@/constants/feedback';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { HeightsContext } from '@/context/CellHeightsContext';
import { ConversationForm } from '@/forms/TeamboardForm/Conversation';
import { useUserFeedbacksClients } from '@/queries/teamboard/useUserFeedbackClients';
import { useUserFeedbacksReviewers } from '@/queries/teamboard/useUserFeedbacksReviewers';
import { useUserProjects } from '@/queries/useUserProjects';
import { TeamboardTableMode } from '@/types/teamboard';

import { FEEDBACK_LINE_HEIGHT, FEEDBACK_EDIT_COLUMN_OFFSET } from './constants';
import { useFeedbackTableColumns } from './hooks/useFeedbackColumns';
import { FeedbackTableProps } from './types';
import {
	getFeedbackTableColumnWidth,
	overrideFeedbackGridStyles,
} from './utils';

export function FeedbackTable({
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
}: FeedbackTableProps) {
	const columnWidthsAndOrder = getFeedbackTableColumnWidth(mode);
	const { userId } = useContext(PersonalDataContext);
	const isTabTableMode = mode === TeamboardTableMode.Tab;

	const {
		id: conversationId,
		handleFormSubmit: handleConversationFormSubmit,
		isOpen: isConversationFormOpen,
		handleClose: handleConversationFormClose,
		handleConservationEditClick,
	} = useConversationFormState();

	const handleLinkClick = (row: FeedbackRow) => {
		if (row.conversationId) {
			handleConservationEditClick(row.conversationId)();
		}
	};

	const { data: privateProjectsData } = useUserProjects(userId!, undefined, {
		enabled: Boolean(userId) && isTabTableMode,
	});

	const { data: clientsData } = useUserFeedbacksClients(userId!, {
		enabled: isTabTableMode,
	});

	const { data: reviewersData } = useUserFeedbacksReviewers(userId!, {
		enabled: isTabTableMode,
	});

	const projectsAutocomplete = useMemo(() => {
		if (privateProjectsData) {
			return privateProjectsData.map(({ name }) => name);
		}

		return [];
	}, [privateProjectsData]);

	const feedbackSelectionOptions: Array<SelectionModalOption> = useMemo(
		() =>
			feedbackAutoselectOptions.map(({ id, name }) => ({
				id,
				description: name,
			})),
		[]
	);

	const columns = useFeedbackTableColumns(
		mode,
		columnWidthsAndOrder,
		handleLinkClick,
		feedbackSelectionOptions,
		projectsAutocomplete,
		clientsData || [],
		reviewersData || [],
		filter,
		setFilter,
		areFiltersApplied,
		resetFilter
	);

	const { getHeight, heightsMap, handleUpdateRows } = useDynamicRowsHeight({
		setRows,
		lineHeight: FEEDBACK_LINE_HEIGHT,
	});

	const overrideStyles = useMemo(
		() =>
			overrideFeedbackGridStyles(
				pagination ? !!rows.length : false,
				mode,
				Object.values(columnWidthsAndOrder).length +
					FEEDBACK_EDIT_COLUMN_OFFSET,
				getGridColumnTemplateFromColumnWidths(columnWidthsAndOrder, true)
			),
		[columnWidthsAndOrder, pagination, rows.length, mode]
	);

	const isSection = mode === TeamboardTableMode.Section;

	return (
		<>
			<DataGridHoverRowWrapper defineHeight={false} flexGrow={isTabTableMode}>
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
						paginationProps={pagination}
						renderers={noRowsGridFallback}
						{...(isSection && displayMinimalGridRowsProp)}
						{...(isSection && { maxRowsCount: MAX_VISIBLE_ROWS_COUNT })}
					/>
				</HeightsContext.Provider>
			</DataGridHoverRowWrapper>
			{isConversationFormOpen && conversationId && (
				<ConversationForm
					handleClose={handleConversationFormClose}
					onSubmit={handleConversationFormSubmit}
					id={conversationId}
				/>
			)}
		</>
	);
}
