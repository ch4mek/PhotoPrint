import { useCallback, useContext } from 'react';

import { PersonalDataContext } from '../context/personalDataContext';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import {
	FEEDBACK_SUCCESS_MESSAGE,
	NotificationType,
} from '@/constants/notifications';
import { useToast } from '@/context/index';
import { FeedbackFormValues } from '@/forms/TeamboardForm/Feedback/types';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateFeedback } from '@/queries/teamboard/useCreateFeedback';
import { useUpdateFeedback } from '@/queries/teamboard/useUpdateFeedback';

export const useFeedbackFormState = () => {
	const { handleOpen, isOpen, handleClose, id } = useShowModal<number>();
	const { userId } = useContext(PersonalDataContext);

	const toast = useToast();

	const onMutationSuccess = () => {
		handleClose();
		toast.open(createToast(NotificationType.SUCCESS, FEEDBACK_SUCCESS_MESSAGE));
	};

	const { mutate: createFeedbackMutation } = useCreateFeedback({
		onSuccess: onMutationSuccess,
	});
	const { mutate: updateFeedbackMutation } = useUpdateFeedback({
		onSuccess: onMutationSuccess,
	});

	const createFeedback = (values: FeedbackFormValues) => {
		const { date, actions, conversation, feedback, project, reviewer, type } =
			values;
		createFeedbackMutation({
			externalFeedbackDate: dayjs(date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
			actionsTodo: actions || undefined,
			conversationId: conversation ? conversation.id : undefined,
			externalFeedbackType: type!.name,
			feedback,
			projectId: project!.id,
			reviewer: reviewer || undefined,
			userId: Number(userId),
		});
	};

	const editFeedback = (values: FeedbackFormValues) => {
		const { actions, conversation, reviewer, type } = values;
		updateFeedbackMutation({
			id: id!,
			actionsTodo: actions || undefined,
			conversationId: conversation ? conversation.id : undefined,
			externalFeedbackType: type!.name,
			reviewer: reviewer || undefined,
		});
	};
	const handleFeedbackEditClick = useCallback(
		(projectId?: string | number) => () => {
			handleOpen(projectId as number);
		},
		[]
	);

	const handleFormSubmit = (values: FeedbackFormValues) => {
		const submitFunction = id ? editFeedback : createFeedback;

		submitFunction(values);
	};

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, []);

	return {
		id,
		isOpen,
		handleClose,
		handleFormSubmit,
		handleFeedbackEditClick,
		handleAddClick,
	};
};
