import { useQueryClient } from '@tanstack/react-query';
import { useCallback } from 'react';

import { DELETE_SUCCESS_MESSAGE, RESTORE_SUCCESS_MESSAGE } from '../constants';

import { NotificationType } from '@/constants/notifications';
import { projectExitQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useDeleteProjectExits } from '@/queries/projectExits/useDeleteProjectExits';
import { useRestoreProjectExits } from '@/queries/projectExits/useRestoreProjectExits';
import { ProjectExitsGridRow } from '@/types/projectExits';

export const useProjectMenuOptions = () => {
	const toast = useToast();
	const queryClient = useQueryClient();

	const { mutateAsync: deleteProjectMutation, isPending: isProjectDeleting } =
		useDeleteProjectExits();
	const { mutateAsync: restoreProjectMutation, isPending: isProjectRestoring } =
		useRestoreProjectExits();

	const {
		isOpen: isDeleteDialogOpen,
		handleOpen: handleOpenDeleteDialog,
		handleClose: handleCloseDeleteDialog,
		id: rowIdToDelete,
	} = useShowModal<number>();
	const {
		isOpen: isRestoreDialogOpen,
		handleOpen: handleOpenRestoreDialog,
		handleClose: handleCloseRestoreDialog,
		id: rowIdToRestore,
	} = useShowModal<number>();

	const handleConfirmDelete = useCallback(async () => {
		if (rowIdToDelete === undefined) {
			return;
		}

		await deleteProjectMutation(rowIdToDelete, {
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, DELETE_SUCCESS_MESSAGE)
				);
				queryClient.invalidateQueries({
					queryKey: [projectExitQueriesKeys.getProjectExits],
				});
			},
			onError: (error) => {
				createOnErrorHandler(toast)(error);
				handleCloseDeleteDialog();
			},
		});

		handleCloseDeleteDialog();
	}, [
		rowIdToDelete,
		queryClient,
		handleCloseDeleteDialog,
		deleteProjectMutation,
	]);

	const handleDelete = useCallback(
		(row: ProjectExitsGridRow) => {
			handleOpenDeleteDialog(row.id);
		},
		[handleOpenDeleteDialog]
	);

	const handleConfirmRestore = useCallback(async () => {
		if (rowIdToRestore === undefined) {
			return;
		}

		await restoreProjectMutation(rowIdToRestore, {
			onSuccess: () => {
				toast.open(
					createToast(NotificationType.SUCCESS, RESTORE_SUCCESS_MESSAGE)
				);
				queryClient.invalidateQueries({
					queryKey: [projectExitQueriesKeys.getProjectExits],
				});
			},
			onError: (error) => {
				createOnErrorHandler(toast)(error);
				handleCloseDeleteDialog();
			},
		});

		handleCloseRestoreDialog();
	}, [
		rowIdToRestore,
		queryClient,
		handleCloseRestoreDialog,
		restoreProjectMutation,
	]);

	const handleRestore = useCallback(
		(row: ProjectExitsGridRow) => {
			handleOpenRestoreDialog(row.id);
		},
		[handleOpenRestoreDialog]
	);

	return {
		rowIdToDelete,
		isDeleteDialogOpen,
		isProjectDeleting,
		handleDelete,
		handleConfirmDelete,
		handleCloseDeleteDialog,

		rowIdToRestore,
		isRestoreDialogOpen,
		isProjectRestoring,
		handleRestore,
		handleConfirmRestore,
		handleCloseRestoreDialog,
	};
};
