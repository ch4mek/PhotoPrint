import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import {
	MICROFRONTEND_MESSAGES,
	NotificationType,
} from '@/constants/notifications';
import { microfrontendsQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { createToast } from '@/helpers/createToast';
import { useGetMicrofrontends } from '@/queries/microfrontends/useGetMicrofrontends';
import { useUpdateMicrofrontends } from '@/queries/microfrontends/useUpdateMicrofrontends';

import { MicrofrontendDeleteModal } from './components/MicrofrontendDeleteModal';
import { MicrofrontendsForm } from './components/MicrofrontendsForm';
import { MicrofrontendsFormValues } from './components/MicrofrontendsForm/types';
import { removeMicrofrontendFromConfig } from './components/MicrofrontendsForm/utils/microfrontendConfigUtils';
import { MicrofrontendsGrid } from './components/MicrofrontendsGrid';
import { useMicrofrontendsTableState } from './components/MicrofrontendsGrid/hooks/useMicrofrontendsTableState';
import { MicrofrontendsPanelPageWrapper } from './styles';

export function MicrofrontendsPanelPage() {
	const queryClient = useQueryClient();
	const toast = useToast();

	const { data: currentConfig } = useGetMicrofrontends();

	const { mutateAsync: updateMicrofrontends } = useUpdateMicrofrontends();

	const {
		editId,
		deleteId,
		isDeleteOpen,
		isEditOpen,
		columns,
		rows,
		isFetching,
		sortColumns,
		setRows,
		handleSortColumnChange,
		getRowHeight,
		handleEditOpen,
		handleEditClose,
		handleDeleteClose,
	} = useMicrofrontendsTableState();

	const editData = useMemo((): MicrofrontendsFormValues | undefined => {
		if (!editId || !rows.length) {
			return undefined;
		}

		const microfrontend = rows.find((row) => row.name === editId);

		if (!microfrontend) {
			return undefined;
		}

		return {
			name: microfrontend.name,
			scope: microfrontend.scope,
			modules: microfrontend.modules || [],
			routes: microfrontend.routes || [],
			version: microfrontend.version,
			remoteEntries: microfrontend.remoteEntries || '',
			claims: microfrontend.claims || [],
		};
	}, [editId, rows]);

	const deleteData = useMemo(() => {
		if (!deleteId || !rows.length) {
			return undefined;
		}

		return rows.find((row) => row.name === deleteId);
	}, [deleteId, rows]);

	const handleSubmit = useCallback(async () => {
		await queryClient.invalidateQueries({
			queryKey: [microfrontendsQueriesKeys.microfrontends],
		});
	}, [queryClient]);

	const handleDelete = useCallback(
		async (id: string | number) => {
			const existingMicrofrontends = currentConfig?.microfrontends || [];

			const updatedMicrofrontends = removeMicrofrontendFromConfig(
				existingMicrofrontends,
				id as string
			);

			await updateMicrofrontends(
				{ microfrontends: updatedMicrofrontends },
				{
					onSuccess: () => {
						toast.open(
							createToast(
								NotificationType.SUCCESS,
								MICROFRONTEND_MESSAGES.DELETED_SUCCESS
							)
						);
						queryClient.invalidateQueries({
							queryKey: [microfrontendsQueriesKeys.microfrontends],
						});
						handleDeleteClose();
					},
					onError: () => {
						toast.open(
							createToast(
								NotificationType.ERROR,
								MICROFRONTEND_MESSAGES.DELETE_ERROR
							)
						);
					},
				}
			);
		},
		[currentConfig, updateMicrofrontends, queryClient, handleDeleteClose, toast]
	);

	return (
		<MicrofrontendsPanelPageWrapper>
			<MicrofrontendsGrid
				columns={columns}
				rows={rows}
				isFetching={isFetching}
				sortColumns={sortColumns}
				setRows={setRows}
				handleSortColumnChange={handleSortColumnChange}
				getRowHeight={getRowHeight}
				handleOpen={handleEditOpen}
			/>

			{isEditOpen && (
				<MicrofrontendsForm
					id={editId}
					data={editData}
					handleClose={handleEditClose}
					handleSubmit={handleSubmit}
				/>
			)}

			{isDeleteOpen && deleteData && (
				<MicrofrontendDeleteModal
					id={deleteId!}
					name={deleteData.name}
					handleClose={handleDeleteClose}
					handleDelete={handleDelete}
				/>
			)}
		</MicrofrontendsPanelPageWrapper>
	);
}
