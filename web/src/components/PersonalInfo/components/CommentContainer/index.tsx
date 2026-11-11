import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { PlusIcon } from '@mms/mms-ui-library/icons/base/PlusIcon';
import { useQueryClient } from '@tanstack/react-query';
import { useMemo } from 'react';

import { COMMENT_UPDATED, NotificationType } from '@/constants/notifications';
import { payloadQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { CommentForm } from '@/forms/TeamboardForm/Comment';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useUpdateComment } from '@/queries/useUpdateComment';

import { PLACEHOLDER_MESSAGE } from './constants';
import { CommentWrapper, Placeholder, FixedTypography } from './styles';

export function CommentContainer({
	refetch,
	comments,
	userId,
}: {
	refetch: VoidFunction;
	comments: string | null;
	userId: string | number | undefined;
}) {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const updateCommentMutation = useUpdateComment({
		onSuccess: () => {
			toast.open(createToast(NotificationType.SUCCESS, COMMENT_UPDATED));

			queryClient.invalidateQueries({
				queryKey: [payloadQueriesKeys.getPayloadData],
			});
			refetch();
		},
		onError: createOnErrorHandler(toast),
	});

	const initialValues = useMemo(
		() => ({
			comments,
		}),
		[comments]
	);

	const handleEditClick = () => {
		handleOpen(userId);
	};

	const handleAddClick = () => {
		handleOpen();
	};

	return (
		<>
			<CommentWrapper>
				{comments && comments.length > 1 ? (
					<>
						<FixedTypography variant="m-400">{comments}</FixedTypography>
						<ActionButton size="small" onClick={handleEditClick}>
							<EditIcon />
						</ActionButton>
					</>
				) : (
					<>
						<Placeholder variant="m-400">{PLACEHOLDER_MESSAGE}</Placeholder>
						<ActionButton size="small" onClick={handleAddClick}>
							<PlusIcon />
						</ActionButton>
					</>
				)}
			</CommentWrapper>
			{isOpen && (
				<CommentForm
					onSubmit={updateCommentMutation.mutateAsync}
					handleClose={handleClose}
					initialValues={initialValues}
					userId={userId}
					id={id}
				/>
			)}
		</>
	);
}
