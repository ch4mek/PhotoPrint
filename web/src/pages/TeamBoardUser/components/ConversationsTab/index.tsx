import { useCallback, useContext, useEffect, useMemo, useState } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { useConversationFormState } from '../../hooks/useConversationFormState';
import { useConversationViewState } from '../../hooks/useConversationViewState';
import { ConversationRow, TeamboardUserProfileKind } from '../../types';
import {
	convertConversationResponseToRows,
	isLimitedHRAccess,
} from '../../utils';
import { ConversationTable } from '../ConversationTable';
import { ConversationViewPopUp } from '../ConversationTable/components/ConversationViewPopUp';

import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { ConversationForm } from '@/forms/TeamboardForm/Conversation';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useUserConversations } from '@/queries/teamboard/useUserConversations';
import { useUserSupervisors } from '@/queries/teamboard/useUserSupervisors';
import { TeamboardTableMode } from '@/types/teamboard';

import { useConversationFilters } from './hooks/useConversationFilters';

export function ConversationsTab() {
	const {
		sortColumns,
		handleSortColumnChange,
		filters,
		setFilters,
		areFiltersApplied,
		resetFilters,
		page,
		setPage,
		handleOnPerPageChange,
		rowsPerPage,
		order,
		orderBy,
	} = useConversationFilters();

	const { userId, accessLevel, profileKind } = useContext(PersonalDataContext);

	const isIntern = profileKind === TeamboardUserProfileKind.Intern;

	const [rows, setRows] = useState<Array<ConversationRow>>([]);

	const { data: supervisorsData, refetch: refetchSupervisors } =
		useUserSupervisors(userId!);

	const supervisorsAutocompleteOptions = useMemo(
		() => supervisorsData?.map(({ fullName }) => fullName),
		[supervisorsData]
	);

	const { data, isLoading, isFetching } = useUserConversations({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		orderBy: `${capitalizeString(orderBy)} ${order}`,
		filters,
	});

	const isDataLoading = isLoading || isFetching;

	const handleRefetch = useCallback(() => {
		refetchSupervisors();
	}, []);

	const {
		id,
		handleClose,
		handleAddClick,
		handleFormSubmit,
		isOpen,
		handleConservationEditClick,
	} = useConversationFormState(handleRefetch);

	const { viewConversation, isViewOpen, handleViewClose, handleViewClick } =
		useConversationViewState(rows);

	useEffect(() => {
		if (data?.items && !isDataLoading) {
			setRows(
				convertConversationResponseToRows(
					data.items,
					handleConservationEditClick,
					handleViewClick
				)
			);
		}
	}, [data?.items, isDataLoading]);

	const conversationTablePagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	useEffect(() => setPage(0), [filters, sortColumns]);

	const hasHRAccess = isLimitedHRAccess(accessLevel);

	return (
		<TabTableWrapper>
			<TabHeader onAddClick={handleAddClick} />

			<Loader isLoading={isDataLoading} size="small">
				<ConversationTable
					mode={TeamboardTableMode.Tab}
					rows={rows}
					setRows={setRows}
					filter={filters}
					setFilter={setFilters}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					areFiltersApplied={areFiltersApplied}
					resetFilter={resetFilters}
					pagination={conversationTablePagination}
					page={page}
					supervisorAutocompleteOptions={supervisorsAutocompleteOptions}
					isLoading={isLoading}
					shouldHideTypeColumn={hasHRAccess}
					isIntern={isIntern}
				/>
			</Loader>
			{isViewOpen && viewConversation && (
				<ConversationViewPopUp
					conversation={viewConversation}
					handleClose={handleViewClose}
				/>
			)}
			{isOpen && (
				<ConversationForm
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					id={id}
				/>
			)}
		</TabTableWrapper>
	);
}
