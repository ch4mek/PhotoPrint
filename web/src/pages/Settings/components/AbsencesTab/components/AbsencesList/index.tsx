import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useState } from 'react';

import { useDeleteAgreementForm } from '../../hooks/useDeleteAgreementForm';
import { useMutateAgreementForm } from '../../hooks/useMutateAgreementForm';
import { useUpdateAbsenceForm } from '../../hooks/useUpdateAbsenceForm';
import { AssignsList } from '../AbsenceItem/components/AssignsList';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import { ABSENCE_ENTITY_NAME } from '@/constants/absence';
import { ITEM_WILL_DELETED_POSTFIX } from '@/constants/notifications';
import { AbsenceForm } from '@/forms/Absence/AbsenceForm';
import { AgreementForm } from '@/forms/Absence/AgreementForm';
import { AGREEMENT_ENTITY_NAME } from '@/forms/Absence/AgreementForm/constants';
import { checkIsNameExist } from '@/helpers/checkIsNameExist';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { AbsenceItem } from '@/pages/Settings/components/AbsencesTab/components/AbsenceItem';
import { useDeleteAbsenceForm } from '@/pages/Settings/components/AbsencesTab/hooks/useDeleteAbsenceForm';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

import { AbsencesListWrapper } from './styles';
import { AbsencesListProps, EntityName } from './types';

export function AbsencesList({ data }: AbsencesListProps) {
	const [entityName, setEntityName] = useState<EntityName | null>(null);

	const {
		id,
		isOpen,
		isLoading,
		handleFormSubmit,
		handleClose,
		handleOpen: handleUpdate,
	} = useUpdateAbsenceForm();

	const {
		id: itemToDelete,
		isOpen: isDeleteOpen,
		isLoading: isDeleteLoading,
		handleFormSubmit: deleteSubmit,
		handleClose: handleDeleteClose,
		handleOpen: handleDelete,
		hasAgreements,
	} = useDeleteAbsenceForm();

	const {
		agreementItem,
		isOpen: isAddAgreementOpen,
		isLoading: isAddAgreementLoading,
		handleCreateAgreementFormSubmit: handleAddAgreementSubmit,
		handleUpdateAgreementFormSubmit: handleUpdateAgreementSubmit,
		handleClose: handleAddAgreementClose,
		handleOpen: handleAddAgreement,
	} = useMutateAgreementForm();

	const {
		id: agreementToDelete,
		isOpen: isAgreementDeleteOpen,
		isLoading: isAgreementDeleteLoading,
		handleFormSubmit: deleteAgreementSubmit,
		handleClose: handleAgreementDeleteClose,
		handleOpen: handleAgreementDelete,
	} = useDeleteAgreementForm();

	const {
		id: info,
		isOpen: isAssignsOpen,
		handleOpen: handleAssignsModalOpen,
		handleClose: handleAssignsModalClose,
	} = useShowModal<SimpleConfigurableConfigurationItem<number>>();

	const handleCheckIsNameExist = (name: string, absenceId?: string | number) =>
		checkIsNameExist(data, name, absenceId);
	const handleAssignsOpen =
		(entityTitle: EntityName) =>
		(itemId?: SimpleConfigurableConfigurationItem<number> | undefined) => {
			handleAssignsModalOpen(itemId);
			setEntityName(entityTitle);
		};

	const handleAssignsClose = () => {
		setEntityName(null);
		handleAssignsModalClose();
	};

	return (
		<>
			<AbsencesListWrapper>
				{data.map((item) => (
					<AbsenceItem
						key={item.id}
						{...{
							item,
							handleUpdate,
							handleDelete,
							handleAddAgreement,
							handleAgreementDelete,
							handleAssignsOpen,
						}}
					/>
				))}
			</AbsencesListWrapper>

			{isOpen && (
				<AbsenceForm
					{...{
						handleFormSubmit,
						handleClose,
						handleDetailsFormSubmit: handleFormSubmit,
						isLoading,
						checkIsNameExist: handleCheckIsNameExist,
						id,
					}}
				/>
			)}
			{isDeleteOpen && itemToDelete && (
				<DeleteConfirmationDialog
					title={ABSENCE_ENTITY_NAME}
					handleClose={handleDeleteClose}
					handleSubmit={deleteSubmit}
					isLoading={isDeleteLoading}
					content={
						<Typography variant="m-400">
							{ABSENCE_ENTITY_NAME}{' '}
							<Typography variant="m-600">{itemToDelete.name}</Typography>{' '}
							{hasAgreements && 'and connected agreements '}
							{ITEM_WILL_DELETED_POSTFIX}
						</Typography>
					}
				/>
			)}

			{isAgreementDeleteOpen && agreementToDelete && (
				<DeleteConfirmationDialog
					title={AGREEMENT_ENTITY_NAME}
					handleClose={handleAgreementDeleteClose}
					handleSubmit={deleteAgreementSubmit}
					isLoading={isAgreementDeleteLoading}
					item={agreementToDelete.name}
				/>
			)}

			{isAddAgreementOpen && (
				<AgreementForm
					handleAddAgreementSubmit={handleAddAgreementSubmit}
					handleUpdateAgreementSubmit={handleUpdateAgreementSubmit}
					handleClose={handleAddAgreementClose}
					agreementItem={agreementItem}
					isLoading={isAddAgreementLoading}
				/>
			)}

			{isAssignsOpen && info && entityName && (
				<AssignsList
					{...{ entityName }}
					onClose={handleAssignsClose}
					id={info.id}
				/>
			)}
		</>
	);
}
