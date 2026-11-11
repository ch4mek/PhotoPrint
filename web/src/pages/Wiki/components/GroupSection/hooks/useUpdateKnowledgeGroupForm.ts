import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyUpdatedMessage,
	NotificationType,
} from '@/constants/notifications';
import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { UseKnowledgeGroupFormProps } from '@/pages/Wiki/components/GroupSection/types';
import { useChangeArticle } from '@/queries/wiki/useChangeArticle';
import { useUpdateKnowledgeGroupName } from '@/queries/wiki/useUpdateKnowledgeGroupName';
import { SegmentTypes } from '@/types/wiki';

export const useUpdateSegmentForm = ({
	segmentType,
}: UseKnowledgeGroupFormProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: updateKnowledgeGroupMutation,
		isPending: isUpdateKnowledgeGroupLoading,
	} = useUpdateKnowledgeGroupName({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(segmentType)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const {
		mutateAsync: updateArticleMutation,
		isPending: isUpdateArticleLoading,
	} = useChangeArticle({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyUpdatedMessage(segmentType)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
			});
			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getArticleById],
			});
			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.searchArticleData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		updateSegmentFormId: id,
		handleUpdateSegmentFormOpen: handleOpen,
		handleUpdateSegmentFormClose: handleClose,
		isUpdateSegmentFormOpen: isOpen,
		updateSegmentMutation:
			segmentType === SegmentTypes.Article
				? updateArticleMutation
				: updateKnowledgeGroupMutation,
		isUpdateSegmentLoading:
			segmentType === SegmentTypes.Article
				? isUpdateArticleLoading
				: isUpdateKnowledgeGroupLoading,
	};
};
