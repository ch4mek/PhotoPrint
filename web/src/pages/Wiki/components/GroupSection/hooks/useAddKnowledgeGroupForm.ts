import { useQueryClient } from '@tanstack/react-query';

import {
	getSuccessfullyAddedMessage,
	NotificationType,
} from '@/constants/notifications';
import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { UseKnowledgeGroupFormProps } from '@/pages/Wiki/components/GroupSection/types';
import { useAddArticle } from '@/queries/wiki/useAddArticle';
import { useAddKnowledgeGroup } from '@/queries/wiki/useAddKnowledgeGroup';
import { SegmentTypes } from '@/types/wiki';

export const useAddSegmentForm = ({
	segmentType,
}: UseKnowledgeGroupFormProps) => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const {
		mutateAsync: addKnowledgeGroupMutation,
		isPending: isKnowledgeGroupLoading,
	} = useAddKnowledgeGroup({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(segmentType)
				)
			);

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const { mutateAsync: addArticleMutation, isPending: isArticleLoading } =
		useAddArticle({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyAddedMessage(SegmentTypes.Article)
					)
				);

				handleClose();

				queryClient.invalidateQueries({
					queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
				});
			},
			onError: createOnErrorHandler(toast),
		});

	return {
		addSegmentFormId: id,
		handleAddSegmentFormOpen: handleOpen,
		handleAddSegmentFormClose: handleClose,
		isAddSegmentFormOpen: isOpen,
		addSegmentMutation:
			segmentType === SegmentTypes.Article
				? addArticleMutation
				: addKnowledgeGroupMutation,
		isSegmentLoading:
			segmentType === SegmentTypes.Article
				? isArticleLoading
				: isKnowledgeGroupLoading,
	};
};
