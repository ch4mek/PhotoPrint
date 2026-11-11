import { useMemo, useState } from 'react';

import { useShowModal } from '@/pages/hooks/useShowModal';
import { GetInterviewDataResponse, InterviewData } from '@/types/interview';

import { useDeleteInterviewForm } from './useDeleteInterviewForm';

export const useInterviewModals = (interviews?: GetInterviewDataResponse) => {
	const mainModal = useShowModal();
	const stageModal = useShowModal();
	const addStageModal = useShowModal();

	const deleteForm = useDeleteInterviewForm();

	const [parentInterviewForStage, setParentInterviewForStage] = useState<
		InterviewData | undefined
	>();

	const handleAddStage = (parentId: number) => {
		const parent = interviews?.items.find(
			(int) => int.main.id === parentId
		)?.main;

		setParentInterviewForStage(parent);
		addStageModal.handleOpen();
	};

	const handleCloseAddStageForm = () => {
		setParentInterviewForStage(undefined);
		addStageModal.handleClose();
	};

	const mainInterview = useMemo(
		() =>
			mainModal.id
				? interviews?.items.find((int) => int.main.id === mainModal.id)?.main
				: undefined,
		[mainModal.id, interviews]
	);

	const interviewStage = useMemo(
		() =>
			stageModal.id
				? interviews?.items
						.flatMap((i) => i.stages)
						.find((s) => s.id === stageModal.id)
				: undefined,
		[stageModal.id, interviews]
	);

	return {
		mainInterviewModal: {
			isOpen: mainModal.isOpen,
			open: mainModal.handleOpen,
			close: mainModal.handleClose,
			interview: mainInterview,
		},

		stageInterviewModal: {
			isOpen: stageModal.isOpen,
			open: stageModal.handleOpen,
			close: stageModal.handleClose,
			interview: interviewStage,
		},

		deleteInterviewModal: {
			isOpen: deleteForm.isOpen,
			open: deleteForm.handleOpen,
			close: deleteForm.handleClose,
			submit: deleteForm.handleFormSubmit,
			isLoading: deleteForm.isLoading,
			interview: deleteForm.id,
		},

		addStageModal: {
			isOpen: addStageModal.isOpen,
			open: handleAddStage,
			close: handleCloseAddStageForm,
			parentInterview: parentInterviewForStage,
		},
	};
};
