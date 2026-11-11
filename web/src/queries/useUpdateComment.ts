import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_USER_COMMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { CommentSubmitParams } from '@/forms/TeamboardForm/Comment/types';
import { PersonalDataResponse } from '@/types/personalData';

import { ServerResponseError } from './types';

export const useUpdateComment = (
	options: UseMutationOptions<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		CommentSubmitParams
	>
) =>
	useMutation<
		PersonalDataResponse,
		AxiosError<ServerResponseError>,
		CommentSubmitParams
	>({
		...options,
		mutationFn: ({ userId, comments }) =>
			axios
				.patch(GET_USER_COMMENT_API_ENDPOINT, {
					userId,
					comment: comments,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
