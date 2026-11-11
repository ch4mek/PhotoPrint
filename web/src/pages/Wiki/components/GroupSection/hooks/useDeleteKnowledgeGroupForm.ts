import { useQueryClient } from '@tanstack/react-query';
import { useNavigate, useParams } from 'react-router-dom';

import {
	getSuccessfullyDeletedMessage,
	NotificationType,
} from '@/constants/notifications';
import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { Routes } from '@/constants/routes';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { UseKnowledgeGroupFormProps } from '@/pages/Wiki/components/GroupSection/types';
import { useDeleteArticle } from '@/queries/wiki/useDeleteArticle';
import { useDeleteKnowledgeGroup } from '@/queries/wiki/useDeleteKnowledgeGroup';
import { SegmentTypes } from '@/types/wiki';

export const useDeleteSegmentForm = ({
	segmentType,
}: UseKnowledgeGroupFormProps) => {
	const toast = useToast();
	const { id: selectArticleId } = useParams();
	const navigate = useNavigate();

	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const { mutateAsync: deleteGroupMutation, isPending: isDeleteGroupLoading } =
		useDeleteKnowledgeGroup({
			onSuccess: () => {
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						getSuccessfullyDeletedMessage(segmentType)
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
		mutateAsync: deleteArticleMutation,
		isPending: isDeleteArticleLoading,
	} = useDeleteArticle({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyDeletedMessage(segmentType)
				)
			);

			if (selectArticleId && +selectArticleId === id) {
				navigate(`${Routes.wiki}`);
			}

			handleClose();

			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
			});
			queryClient.invalidateQueries({
				queryKey: [wikiQueriesKeys.searchArticleData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	return {
		deleteSegmentGroupFormId: id,
		handleDeleteSegmentFormOpen: handleOpen,
		handleDeleteSegmentFormClose: handleClose,
		isDeleteSegmentFormOpen: isOpen,
		isDeleteSegmentLoading:
			segmentType === SegmentTypes.Article
				? isDeleteArticleLoading
				: isDeleteGroupLoading,
		deleteSegmentMutation:
			segmentType === SegmentTypes.Article
				? deleteArticleMutation
				: deleteGroupMutation,
	};
};
