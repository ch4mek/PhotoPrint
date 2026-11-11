import { useCallback, useContext } from 'react';

import { PersonalDataContext } from '../context/personalDataContext';

import { findById } from '@/helpers/findById';
import { usePerformanceReviewModals } from '@/pages/PerformanceReview/hooks/usePerformanceReviewModalsState';
import { useConversationFormState } from '@/pages/TeamBoardUser/hooks/useConversationFormState';
import { UserPerformanceReviewData } from '@/types/teamboard';

export const usePerformanceReviewFormState = (
	performancesReviewData?: UserPerformanceReviewData[],
	refetch?: () => void
) => {
	const { userId, data } = useContext(PersonalDataContext);

	const {
		handleUserDataUpdate,
		isOpenWarning,
		handleOpenWarning,
		handleCloseWarning,
		isOpenPerformanceAdding,
		handleClosePerformanceAdding,
		handleOpenPerformanceAdding,
		initialPerformanceValue,
		initialConversationValue,
		handlePerformanceDataUpdate,
	} = usePerformanceReviewModals();

	const {
		id,
		handleAddClick: handleAddConversationClick,
		handleConservationEditClick,
		handleFormSubmit,
		handleClose,
		isOpen,
	} = useConversationFormState(refetch, userId, handleOpenWarning);

	const handleUpdateUserData = useCallback(
		() =>
			handleUserDataUpdate({
				id: userId!,
				fullName: data.fullName!,
			}),
		[userId]
	);

	const handleAddClick = useCallback(() => {
		handleUpdateUserData();
		handleOpenPerformanceAdding();
	}, []);

	const handleEditClick = useCallback(
		(itemId?: string | number) => () => {
			const performanceReview = findById(performancesReviewData!, itemId);

			if (performanceReview && performancesReviewData) {
				const { conversationId, performanceReviewDate } = performanceReview;

				if (conversationId) {
					handleConservationEditClick(conversationId)();
				} else {
					handleAddConversationClick();
					handlePerformanceDataUpdate(performanceReviewDate, Number(itemId));
				}
			}
		},
		[performancesReviewData]
	);

	return {
		idConversationForm: id,
		isConversationOpen: isOpen,
		handleCloseConversation: handleClose,
		handleConversationFormSubmit: handleFormSubmit,
		initialConversationValue,
		isOpenWarning,
		handleCloseWarning,
		isOpenPerformanceAdding,
		handleOpenPerformanceAdding,
		handleClosePerformanceAdding,
		initialPerformanceValue,
		handleEditClick,
		handleAddClick,
	};
};
