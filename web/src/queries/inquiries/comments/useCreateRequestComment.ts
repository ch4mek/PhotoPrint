import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { COMMENT_MESSAGES, NotificationType } from '@/constants/notifications';
import { getInquryCommentsEndpoint } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { ServerResponseError } from '@/queries/types';
import { CommentAddData } from '@/types/comments';
import { AppFile } from '@/types/file';

interface CreateCommentParams {
	comment: string;
	inquiryId: number;
	notifyApprovers: boolean;
	files?: Omit<AppFile, 'id'>[];
}

interface CreateCommentResponse extends CommentAddData {
	ValidationFailures?: Array<{ ErrorMessage: string }>;
}

export const useCreateRequestComment = (
	toast: ToastContextValues,
	options?: UseMutationOptions<
		CreateCommentResponse,
		AxiosError<ServerResponseError>,
		CreateCommentParams
	>
) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: ({ comment, inquiryId, notifyApprovers, files = [] }) =>
			axios
				.post<CreateCommentResponse>(getInquryCommentsEndpoint(inquiryId), {
					comment,
					files,
					notifyApprovers,
				})
				.then((res) => res.data),
		onSuccess(data, { inquiryId }) {
			queryClient.invalidateQueries({
				queryKey: [getInquryCommentsEndpoint(inquiryId)],
			});

			toast.open(
				createToast(NotificationType.SUCCESS, COMMENT_MESSAGES.CREATED_SUCCESS)
			);
		},
		onError({ response }) {
			const errorData = response?.data;
			const errorMessage =
				errorData?.Message ||
				errorData?.message ||
				COMMENT_MESSAGES.CREATE_ERROR;

			if (
				errorData?.ValidationFailures &&
				errorData.ValidationFailures.length > 0
			) {
				return;
			}

			toast.open(createToast(NotificationType.ERROR, errorMessage));
		},
		...options,
	});
};
