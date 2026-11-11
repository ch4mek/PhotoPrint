import {
	useContext,
	useEffect,
	useLayoutEffect,
	useMemo,
	useState,
} from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';
import { useFeedbackFormState } from '../../hooks/useFeedbackFormState';
import { FeedbackRow } from '../../types';
import {
	convertFeedbackResponseToRows,
	getFeedbackLoaderRows,
} from '../../utils';
import { FeedbackTable } from '../FeedbackTable';

import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { FeedbackForm } from '@/forms/TeamboardForm/Feedback';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useUserFeedbacks } from '@/queries/teamboard/useUserFeedbacks';
import { TeamboardTableMode } from '@/types/teamboard';

import { useFeedbackFilters } from './hooks/useConversationFilters';

export function FeedbackTab() {
	const { userId } = useContext(PersonalDataContext);

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
	} = useFeedbackFilters();

	const [rows, setRows] = useState<Array<FeedbackRow>>([]);

	const { data, isLoading } = useUserFeedbacks({
		skip: page * rowsPerPage,
		top: rowsPerPage,
		orderBy: `${orderBy} ${order}`,
		userId,
		filters,
	});

	const {
		id,
		handleClose,
		handleAddClick,
		handleFormSubmit,
		isOpen,
		handleFeedbackEditClick,
	} = useFeedbackFormState();

	useLayoutEffect(() => {
		if (isLoading) {
			setRows(getFeedbackLoaderRows());

			return;
		}

		if (data?.items) {
			setRows(
				convertFeedbackResponseToRows(data.items, handleFeedbackEditClick)
			);
		}
	}, [data, isLoading]);

	const converstaionTablePagination = useMemo(
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

	return (
		<TabTableWrapper>
			<TabHeader onAddClick={handleAddClick} />
			<Loader isLoading={isLoading} size="small">
				<FeedbackTable
					mode={TeamboardTableMode.Tab}
					rows={rows}
					setRows={setRows}
					filter={filters}
					setFilter={setFilters}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					areFiltersApplied={areFiltersApplied}
					resetFilter={resetFilters}
					pagination={converstaionTablePagination}
				/>
			</Loader>
			{isOpen && userId && (
				<FeedbackForm
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					id={id}
					userId={userId}
				/>
			)}
		</TabTableWrapper>
	);
}
