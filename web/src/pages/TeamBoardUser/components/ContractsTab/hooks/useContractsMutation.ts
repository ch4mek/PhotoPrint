import { useQueryClient } from '@tanstack/react-query';

import {
	CONTRACT_CREATE_SUCCESS_TOAST_MESSAGE,
	CONTRACT_TIME_PERIOD_TOAST,
} from '../constants';

import { NotificationType } from '@/constants/notifications';
import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { useCreateContract } from '@/queries/teamboard/contracts/useCreateContract';
import { useUpdateContract } from '@/queries/teamboard/contracts/useUpdateContract';
import { ServerResponseError } from '@/queries/types';

export const useContractsMutation = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const showErrorToast = (message: string) => {
		toast.open(createToast(NotificationType.ERROR, message));
	};

	const showContractTimePeriodIssueToast = () => {
		toast.open(createToast(NotificationType.ERROR, CONTRACT_TIME_PERIOD_TOAST));
	};

	const onContractMutationSuccess = () => {
		toast.open(
			createToast(
				NotificationType.SUCCESS,
				CONTRACT_CREATE_SUCCESS_TOAST_MESSAGE
			)
		);
	};

	const onContractMutationError = ({ Message }: ServerResponseError) => {
		if (Message === CONTRACT_TIME_PERIOD_TOAST) {
			showContractTimePeriodIssueToast();

			return;
		}

		showErrorToast(Message);
	};

	const { mutateAsync: createContract } = useCreateContract({
		onSuccess: () => {
			onContractMutationSuccess();

			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getTotalPayloadData],
			});
		},
		onError: ({ response }) => {
			if (response?.data?.Message) {
				onContractMutationError(response.data);
			}
		},
	});

	const { mutateAsync: updateContract } = useUpdateContract({
		onSuccess: () => {
			onContractMutationSuccess();

			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getTotalPayloadData],
			});
		},
		onError: ({ response }) => {
			if (response?.data?.Message) {
				onContractMutationError(response.data);
			}
		},
	});

	return { createContract, updateContract };
};
