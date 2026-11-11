import { useLoadingStatus } from '@mms/mms-ui-library/components/ManagedLoader';
import { useContext, useEffect, useState } from 'react';

import { FeedbackTable } from '../../FeedbackTable';
import { FEEDBACK_COLUMNS_KEYS } from '../../FeedbackTable/constants';
import { PersonalDataContext } from '../../../context/personalDataContext';
import { useFeedbackFormState } from '../../../hooks/useFeedbackFormState';
import { FeedbackRow, TeamBoardTabsId } from '../../../types';
import {
	convertFeedbackResponseToRows,
	getFeedbackLoaderRows,
} from '../../../utils';
import { TeamboardSectionContainer } from '../TeamboardSectionContainer';

import { InfoDataTableWrapper } from '@/components/InfoDataContainer/InfoDataTableWrapper';
import { SortDirection } from '@/constants/sort';
import { FeedbackForm } from '@/forms/TeamboardForm/Feedback';
import { useUserFeedbacks } from '@/queries/teamboard/useUserFeedbacks';
import { TeamboardTableMode } from '@/types/teamboard';

const queryParams = {
	skip: 0,
	top: 1,
	orderBy: `${FEEDBACK_COLUMNS_KEYS.date} ${SortDirection.DESC}`,
};

export function FeedbackSection() {
	const { userId } = useContext(PersonalDataContext);
	const { data, isLoading } = useUserFeedbacks({
		...queryParams,
		userId,
	});

	useLoadingStatus(FeedbackSection.name, isLoading);

	const {
		id,
		handleClose,
		handleAddClick,
		handleFormSubmit,
		isOpen,
		handleFeedbackEditClick,
	} = useFeedbackFormState();

	const [rows, setRows] = useState<Array<FeedbackRow>>([]);

	useEffect(() => {
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

	return (
		<>
			<TeamboardSectionContainer
				headerTitle="External feedback"
				tabId={TeamBoardTabsId.EXTERNAL_FEEDBACK}
				handleAddClick={handleAddClick}
			>
				<InfoDataTableWrapper>
					<FeedbackTable
						mode={TeamboardTableMode.Section}
						rows={rows}
						setRows={setRows}
					/>
				</InfoDataTableWrapper>
			</TeamboardSectionContainer>
			{isOpen && userId && (
				<FeedbackForm
					handleClose={handleClose}
					onSubmit={handleFormSubmit}
					id={id}
					userId={userId}
				/>
			)}
		</>
	);
}
