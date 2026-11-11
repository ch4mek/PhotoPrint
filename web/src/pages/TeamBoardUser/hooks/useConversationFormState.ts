import { useQueryClient } from '@tanstack/react-query';
import { AxiosError } from 'axios';
import { useCallback, useContext } from 'react';

import { PersonalDataContext } from '../context/personalDataContext';

import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import {
	CONVERSATION_SUCCESS_MESSAGE,
	NotificationType,
	PERFORMANCE_REVIEW_HISTORY_UPDATED,
} from '@/constants/notifications';
import { performanceReviewQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import {
	CONVERSATION_TYPE_OPTIONS,
	PERFORMANCE_RESPONSE_TYPE,
} from '@/forms/TeamboardForm/Conversation/constants';
import {
	DevelopmentPlanPopUp,
	UpdateConversationFormValues,
	UserSalaryPopUp,
} from '@/forms/TeamboardForm/Conversation/types';
import { prepareConversationFilesForRequest } from '@/forms/TeamboardForm/Conversation/utils/prepareConversationFilesForRequest';
import { getDateAsResponseType } from '@/helpers/booking';
import { createToast } from '@/helpers/createToast';
import dayjs from '@/lib/dayjs';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useCreateConverstaion } from '@/queries/teamboard/useCreateConversation';
import { useUpdateConversation } from '@/queries/teamboard/useUpdateConversation';
import { ServerResponseError } from '@/queries/types';
import { TeamboardTableMode } from '@/types/teamboard';

const { PopUp } = TeamboardTableMode;

export const useConversationFormState = (
	refetch?: () => void,
	performanceUserId?: number,
	handleOpenPerformanceWarning?: () => void
) => {
	const { handleOpen, isOpen, handleClose, id } = useShowModal<number>();
	const { userId, refetch: refetchPersonalInfo } =
		useContext(PersonalDataContext);
	const toast = useToast();
	const queryClient = useQueryClient();

	const showToast = (
		message: string,
		type: NotificationType = NotificationType.SUCCESS
	) => toast.open(createToast(type, message));

	const performanceCheckNotifier = (type: string, message: string) => {
		if (type === PERFORMANCE_RESPONSE_TYPE) {
			showToast(message);

			queryClient.invalidateQueries({
				queryKey: [performanceReviewQueriesKeys.getAllPerformanceReviews],
			});
		}
	};

	const handlePerformanceLogic = (type: string, isPassed: boolean | null) => {
		if (type === CONVERSATION_TYPE_OPTIONS[1].id && isPassed) {
			handleOpenPerformanceWarning?.();
		}

		if (type === CONVERSATION_TYPE_OPTIONS[1].id) {
			refetchPersonalInfo();
		}
	};

	const handleSuccessConversation = (
		conversationType: string,
		message: string
	) => {
		refetch?.();
		handleClose();
		showToast(CONVERSATION_SUCCESS_MESSAGE);
		performanceCheckNotifier(conversationType, message);
	};

	const handleErrorConversation = (error: AxiosError<ServerResponseError>) => {
		const errorMessage = error.response?.data.Message;

		if (errorMessage) {
			showToast(errorMessage, NotificationType.ERROR);
		}
	};

	const { mutateAsync: createConversationMutation } = useCreateConverstaion({
		onSuccess: ({ conversationType }) => {
			handleSuccessConversation(
				conversationType ?? '',
				PERFORMANCE_REVIEW_HISTORY_UPDATED
			);
		},
		onError: (error) => handleErrorConversation(error),
	});
	const { mutateAsync: updateConversationMutation } = useUpdateConversation({
		onSuccess: ({ conversationType }) => {
			handleSuccessConversation(
				conversationType ?? '',
				PERFORMANCE_REVIEW_HISTORY_UPDATED
			);
		},
		onError: (error) => handleErrorConversation(error),
	});

	const createUserSalaries = (
		userSalaries: UserSalaryPopUp[],
		conversationId: number | null = null
	) => {
		const salaries = userSalaries.map((item) => ({
			...item,
			conversationId,
			salaryFrom: item.date,
			source: PopUp,
			userId: Number(performanceUserId) || Number(userId),
			id: !conversationId || !item.id ? null : item.id,
		}));

		return salaries;
	};

	const createUserDevelopmentPlans = (
		developmentPlansList: DevelopmentPlanPopUp[],
		conversationId: number | null = null
	) => {
		const developmentPlans = developmentPlansList.map((item) => ({
			...item,
			conversationId,
			source: PopUp,
			userId: Number(performanceUserId) || Number(userId),
			id: !conversationId || !item.id ? null : item.id,
		}));

		return developmentPlans;
	};

	const createConversation = async (
		values: UpdateConversationFormValues,
		isPassedUpdatedToTrue: boolean
	) => {
		const {
			date,
			conversationResult,
			theses,
			agreements,
			employeeTheses,
			userSalaries,
			developmentPlans,
			conversationType,
			documentLinks,
			files,
			passed,
			performanceReviewDate,
			userPerformanceReviewId,
		} = values;

		await createConversationMutation(
			{
				date: date ? dayjs(date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT) : null,
				performanceReviewDate: performanceReviewDate
					? getDateAsResponseType(dayjs(performanceReviewDate))
					: null,
				passed,
				theses,
				employeeTheses,
				agreements,
				userId: Number(performanceUserId) || Number(userId),
				conversationType: conversationType ?? '',
				conversationResult,
				documentLinks,
				files: prepareConversationFilesForRequest(files),
				userPerformanceReviewId,
				salaries: createUserSalaries(userSalaries),
				developmentPlans: createUserDevelopmentPlans(developmentPlans),
			},
			{
				onSuccess: ({ id: conversationId }) => {
					if (conversationId) {
						handlePerformanceLogic(
							conversationType ?? '',
							isPassedUpdatedToTrue
						);
					}
				},
			}
		);
	};

	const updateConversation = async (
		values: UpdateConversationFormValues,
		isPassedUpdatedToTrue: boolean
	) => {
		const {
			date,
			conversationResult,
			theses,
			agreements,
			employeeTheses,
			userSalaries,
			developmentPlans,
			conversationType,
			documentLinks,
			files,
			performanceReviewDate,
			passed,
		} = values;
		await updateConversationMutation(
			{
				id: id!,
				userId: Number(performanceUserId) || Number(userId),
				date: date ? dayjs(date).format(YYYY_MM_DD_DAYJS_DATE_FORMAT) : null,
				performanceReviewDate: performanceReviewDate
					? getDateAsResponseType(dayjs(performanceReviewDate))
					: null,
				conversationResult,
				passed,
				theses,
				employeeTheses,
				agreements,
				conversationType: conversationType ?? '',
				documentLinks,
				files: prepareConversationFilesForRequest(files),
				salaries: createUserSalaries(userSalaries, id),
				developmentPlans: createUserDevelopmentPlans(developmentPlans, id),
			},
			{
				onSuccess: ({ id: conversationId }) => {
					if (conversationId) {
						handlePerformanceLogic(
							conversationType ?? '',
							isPassedUpdatedToTrue
						);
					}
				},
			}
		);
	};

	const handleFormSubmit = async (
		values: UpdateConversationFormValues,
		isPassedUpdatedToTrue: boolean
	) => {
		const submitFunction = id ? updateConversation : createConversation;

		await submitFunction(values, isPassedUpdatedToTrue);
	};

	const handleConservationEditClick = useCallback(
		(conversationId?: string | number) => () => {
			handleOpen(conversationId as number);
		},
		[]
	);

	const handleAddClick = useCallback(() => {
		handleOpen();
	}, []);

	return {
		id,
		isOpen,
		handleClose,
		handleFormSubmit,
		handleConservationEditClick,
		handleAddClick,
	};
};
