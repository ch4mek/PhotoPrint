import { useState, useCallback, useMemo } from 'react';

import {
	CONVERSATION_TYPE_OPTIONS,
	PASSED_STATUS_OPTIONS,
} from '@/forms/TeamboardForm/Conversation/constants';
import { ConversationFormField } from '@/forms/TeamboardForm/Conversation/types';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { PerformanceReviewData } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/types';
import { PerformanceReviewUserData } from '@/types/performanceReview';

export function usePerformanceReviewModals() {
	const [performanceUserData, setPerformanceUserData] =
		useState<PerformanceReviewUserData | null>(null);
	const [performanceData, setPerformanceData] =
		useState<PerformanceReviewData | null>(null);
	const [isInitialDataDisabled, setIsInitialDataDisabled] = useState(true);

	const {
		isOpen: isOpenWarning,
		handleOpen: handleOpenWarning,
		handleClose: handleCloseWarning,
	} = useShowModal();

	const {
		isOpen: isOpenPerformanceAdding,
		handleOpen: openPerformanceAdding,
		handleClose: closePerformanceAdding,
	} = useShowModal();

	const initialPerformanceValue = useMemo(
		() => ({
			userData: performanceUserData,
		}),
		[performanceUserData]
	);

	const initialConversationValue = useMemo(() => {
		if (performanceData) {
			const { date, performanceReviewId } = performanceData;

			return {
				[ConversationFormField.PERFORMANCE_DATE]: dayjs(date).startOf('day'),
				[ConversationFormField.TYPE]: CONVERSATION_TYPE_OPTIONS[1],
				[ConversationFormField.PASSED]: PASSED_STATUS_OPTIONS[1],
				performanceReviewId,
			};
		}
	}, [performanceData]);

	const handleUserDataUpdate = useCallback(
		(userData: PerformanceReviewUserData) => {
			setPerformanceUserData(userData);
		},
		[]
	);

	const handlePerformanceDataUpdate = useCallback(
		(date: string, performanceId: number) => {
			setPerformanceData({
				date,
				performanceReviewId: performanceId,
			});
		},
		[]
	);

	const handleOpenPerformanceAdding = useCallback(() => {
		handleCloseWarning();
		openPerformanceAdding();
	}, []);

	const handleClosePerformanceAdding = useCallback(() => {
		setIsInitialDataDisabled(true);
		closePerformanceAdding();
	}, []);

	return {
		isOpenWarning,
		handleOpenWarning,
		handleCloseWarning,
		isOpenPerformanceAdding,
		handleOpenPerformanceAdding,
		handleClosePerformanceAdding,
		handleUserDataUpdate,
		performanceUserData,
		initialPerformanceValue,
		initialConversationValue,
		handlePerformanceDataUpdate,
		setIsInitialDataDisabled,
		isInitialDataDisabled,
	};
}
