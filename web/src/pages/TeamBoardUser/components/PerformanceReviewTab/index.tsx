import { useContext, useEffect, useMemo, useState } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { AddingPerformanceReviewForm } from '@/forms/PerformanceReviewForms';
import { ConversationForm } from '@/forms/TeamboardForm/Conversation';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { PerformanceReviewWarning } from '@/pages/PerformanceReview/components/PerformanceReviewWarning';
import { usePerformanceReviewFilters } from '@/pages/TeamBoardUser/components/PerformanceReviewTab/hooks/usePerformanceReviewFilters';
import { PerformanceReviewTable } from '@/pages/TeamBoardUser/components/PerformanceReviewTable';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { usePerformanceReviewFormState } from '@/pages/TeamBoardUser/hooks/usePerformanceReviewFormState';
import { PerformanceReviewRow } from '@/pages/TeamBoardUser/types';
import { convertPerformanceReviewToRows } from '@/pages/TeamBoardUser/utils';
import { useUserPerformanceReview } from '@/queries/teamboard/useUserPerformanceReview';

export function PerformanceReviewTab() {
	const [rows, setRows] = useState<Array<PerformanceReviewRow>>([]);

	const { userId, refetch: refetchPersonalInfo } =
		useContext(PersonalDataContext);

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
	} = usePerformanceReviewFilters();

	const {
		data,
		isFetching,
		isLoading,
		refetch: refetchUserPerformanceReview,
	} = useUserPerformanceReview({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		filters,
		orderBy: `${capitalizeString(orderBy)} ${order}`,
	});

	const isDataLoading = isLoading || isFetching;

	const performanceReviewTablePagination = useMemo(
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

	const {
		idConversationForm,
		isConversationOpen,
		handleCloseConversation,
		handleConversationFormSubmit,
		initialConversationValue,
		isOpenWarning,
		handleCloseWarning,
		isOpenPerformanceAdding,
		handleClosePerformanceAdding,
		initialPerformanceValue,
		handleEditClick,
		handleAddClick,
	} = usePerformanceReviewFormState(data?.items, refetchUserPerformanceReview);

	useEffect(() => {
		if (data?.items && !isDataLoading) {
			setRows(convertPerformanceReviewToRows(data.items, handleEditClick));
		}
	}, [data?.items, isDataLoading]);

	const handleRefetchData = () => {
		refetchPersonalInfo();
		refetchUserPerformanceReview();
	};

	useEffect(() => setPage(0), [filters, sortColumns]);

	return (
		<TabTableWrapper>
			<TabHeader onAddClick={handleAddClick} />
			<TabTableWrapper>
				<Loader isLoading={isDataLoading} size="small">
					<DataGridHoverRowWrapper defineHeight={false}>
						<PerformanceReviewTable
							rows={rows}
							setRows={setRows}
							filter={filters}
							setFilter={setFilters}
							sortColumns={sortColumns}
							onSortColumnsChange={handleSortColumnChange}
							areFiltersApplied={areFiltersApplied}
							resetFilter={resetFilters}
							pagination={performanceReviewTablePagination}
							isLoading={isLoading}
						/>
					</DataGridHoverRowWrapper>
				</Loader>
			</TabTableWrapper>

			{isConversationOpen && (
				<ConversationForm
					handleClose={handleCloseConversation}
					onSubmit={handleConversationFormSubmit}
					initialValues={initialConversationValue}
					id={idConversationForm}
				/>
			)}

			{isOpenWarning && (
				<PerformanceReviewWarning
					handleToggleDialog={handleCloseWarning}
					handleOpenAddingPopup={handleAddClick}
				/>
			)}
			{isOpenPerformanceAdding && (
				<AddingPerformanceReviewForm
					onClose={handleClosePerformanceAdding}
					initialValues={initialPerformanceValue}
					refetch={handleRefetchData}
				/>
			)}
		</TabTableWrapper>
	);
}
