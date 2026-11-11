import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { PART_OF_SUCCESS_ORDER_MESSAGE } from '@/pages/Wiki/components/GroupSection/constants';
import { useUpdateGroupOrder } from '@/queries/wiki/useUpdateGroupOrder';

export const useUpdateOrder = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const {
		mutateAsync: updateGroupOrderMutation,
		isPending: isUpdateGroupOrderLoading,
	} = useUpdateGroupOrder({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(PART_OF_SUCCESS_ORDER_MESSAGE)
				)
			);

			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		updateGroupOrderMutation,
		isUpdateGroupOrderLoading,
	};
};
