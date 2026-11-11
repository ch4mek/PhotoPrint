import { useCallback, useState } from 'react';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';

import { MICROFRONTEND_FIELD_TITLE } from './constants';
import { MicrofrontendDeleteModalProps } from './types';

export function MicrofrontendDeleteModal({
	id,
	name,
	handleClose,
	handleDelete,
}: MicrofrontendDeleteModalProps) {
	const [isLoading, setIsLoading] = useState(false);

	const handleConfirmDelete = useCallback(async () => {
		if (!id) {
			return;
		}

		setIsLoading(true);

		await handleDelete(id);
		handleClose();

		setIsLoading(false);
	}, [id, handleDelete, handleClose]);

	return (
		<DeleteConfirmationDialog
			title={MICROFRONTEND_FIELD_TITLE}
			item={name}
			handleClose={handleClose}
			handleSubmit={handleConfirmDelete}
			isLoading={isLoading}
		/>
	);
}
