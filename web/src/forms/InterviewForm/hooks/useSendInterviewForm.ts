import { useQueryClient } from '@tanstack/react-query';

import {
	INTERVIEW_MESSAGES,
	NotificationType,
} from '@/constants/notifications';
import { interviewQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useAddInterview } from '@/queries/interview/useAddInterview';
import { useAddInterviewStage } from '@/queries/interview/useAddInterviewStage';
import { useEditInterview } from '@/queries/interview/useEditInterview';
import {
	UpdateInterview,
	CreateUpdateInterviewTemplate,
} from '@/types/interview';

export const useSendInterviewForm = (
	handleClose: VoidFunction,
	interviewGroupId?: string
) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { mutateAsync: updateMutation, isPending: isLoadingUpdate } =
		useEditInterview({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						INTERVIEW_MESSAGES.UPDATE_SUCCESS
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [interviewQueriesKeys.getInterviewsData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const { mutateAsync: createMutation, isPending: isLoadingCreation } =
		useAddInterview({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						INTERVIEW_MESSAGES.CREATED_SUCCESS
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [interviewQueriesKeys.getInterviewsData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	const {
		mutateAsync: createStageMutation,
		isPending: isLoadingStageCreation,
	} = useAddInterviewStage(interviewGroupId, {
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					INTERVIEW_MESSAGES.CREATED_SUCCESS
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [interviewQueriesKeys.getInterviewsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const handleAddInterview = async (values: CreateUpdateInterviewTemplate) => {
		await createMutation(values);
	};

	const handleEditInterview = async (values: UpdateInterview) => {
		await updateMutation(values);
	};

	const handleAddInterviewStage = async (
		values: CreateUpdateInterviewTemplate
	) => {
		await createStageMutation(values);
	};

	const isLoading =
		isLoadingCreation || isLoadingUpdate || isLoadingStageCreation;

	return {
		handleAddInterview,
		handleEditInterview,
		handleAddInterviewStage,
		isLoading,
	};
};
