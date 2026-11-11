import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getNotifyRequestAcceptEndpoint } from '@/constants/queryPaths';

interface NotifyApproversParams {
	inquiryId: number;
	inquiryAcceptId: number;
}

export const useNotifyApprover = (
	options?: UseMutationOptions<
		null,
		AxiosError<{ Message: string }>,
		NotifyApproversParams
	>
) =>
	useMutation({
		mutationFn: ({ inquiryId, inquiryAcceptId }) =>
			axios
				.post(getNotifyRequestAcceptEndpoint(inquiryAcceptId, inquiryId))
				.then((res) => res.data),
		...options,
	});
