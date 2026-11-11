import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { useCallback, useMemo, useState } from 'react';

import {
	SkillName,
	SkillsNames,
	SPECIALIZATION_DELETING_POPUP_TITLE,
} from '../../constants';
import { useInfiniteScroll } from '../../hooks/useInfiniteScroll';
import { useSpecializationsManagement } from '../../hooks/useSpecializationsManagement';
import { SkillListColor } from '../../types';
import { TileList } from '../TileList';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { ITEM_WILL_DELETED_POSTFIX } from '@/constants/notifications';
import { zIndexes } from '@/constants/zIndexes';
import { ListScrollObserverProvider } from '@/context/ListScrollObserver';
import { useToast } from '@/context/Toast';
import { SpecializationForm } from '@/forms/SettingsForm/Skills/Specializations';
import { SpecializationFormValues } from '@/forms/SettingsForm/Skills/Specializations/types';
import { capitalizeString } from '@/helpers/capitalizeString';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { Specialization } from '@/types/settings';

import { SpecializationsListProps } from './types';

export function SpecializationsList({
	selectedDepartmentId,
	selectedSpecialization,
	handleSpecializationItemClick,
}: SpecializationsListProps) {
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
	const [idOfDeleteSpecialization, setIdOfDeleteSpecialization] = useState<
		number | null
	>(null);

	const {
		specializations,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
		createSpecialization,
		updateSpecialization,
		deleteSpecialization,
		checkSpecializationBeforeDelete,
	} = useSpecializationsManagement({
		selectedDepartmentId,
		selectedSpecialization,
		toast,
		handleSpecializationItemClick,
	});

	const scrollContainerRef = useInfiniteScroll({
		hasNextPage,
		isFetchingNextPage,
		fetchNextPage,
		enabled: !isLoading,
	});

	const handleToggleDialog = useCallback(
		() => setIsDeleteDialogOpen((prev) => !prev),
		[]
	);

	const specializationToDelete = useMemo(
		() =>
			specializations.find(
				(specialization) => specialization.id === idOfDeleteSpecialization
			),
		[idOfDeleteSpecialization, specializations]
	);

	const handleCheckSpecialization = useCallback(
		async (specializationId: string | number) => {
			const canDelete = await checkSpecializationBeforeDelete(specializationId);

			if (canDelete) {
				handleToggleDialog();
			}
		},
		[checkSpecializationBeforeDelete, handleToggleDialog]
	);

	const getListItemContextMenuOptions = useCallback(
		(item: Specialization): PickerButtonOption<Specialization>[] =>
			item.canBeUpdated
				? [
						{
							value: 'Edit',
							addon: <EditIcon />,
							onClick: (specialization: Specialization) => {
								handleOpen(specialization.id);
							},
						},
						{
							value: 'Delete',
							addon: <BasketIcon />,
							onClick: (specialization: Specialization) => {
								setIdOfDeleteSpecialization(specialization.id);
								handleCheckSpecialization(specialization.id);
							},
						},
				  ]
				: [],
		[handleOpen, handleCheckSpecialization]
	);

	const handleDeleteSpecialization = async () => {
		if (specializationToDelete) {
			await deleteSpecialization(specializationToDelete);
			handleToggleDialog();
			setIdOfDeleteSpecialization(null);
		}
	};

	const submitFunction = (id === undefined
		? createSpecialization
		: updateSpecialization) as unknown as (
		values: SpecializationFormValues
	) => Promise<unknown>;

	return (
		<>
			<ListScrollObserverProvider>
				<TileList
					beforeColor={SkillListColor.Orange}
					items={specializations}
					title={SkillsNames.specializationsList}
					onAddClick={handleOpen}
					totalItems={total}
					showLoader={isLoading && specializations.length === 0}
					isLoading={isFetching}
					getListItemContextMenuOption={getListItemContextMenuOptions}
					selectedIndex={selectedSpecialization?.id}
					onSelect={handleSpecializationItemClick}
					fieldToDisplay="name"
					isPointerCursor={Boolean(selectedDepartmentId)}
					containerRef={scrollContainerRef}
				/>
			</ListScrollObserverProvider>
			{isOpen && (
				<SpecializationForm
					onSubmit={submitFunction}
					handleClose={handleClose}
					id={id}
				/>
			)}
			{isDeleteDialogOpen && (
				<PopUp
					title={SPECIALIZATION_DELETING_POPUP_TITLE}
					onClose={handleToggleDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleDeleteSpecialization },
						negativeControl: { onClick: handleToggleDialog },
					}}
					zIndex={zIndexes.formPopup}
				>
					<Typography variant="m-400">
						{capitalizeString(SkillName.Specialization)}{' '}
						<Typography variant="m-600">
							{specializationToDelete?.name}
						</Typography>{' '}
						{ITEM_WILL_DELETED_POSTFIX}
					</Typography>
					<ConfirmationTypography variant="m-400">
						{confirmationMessage}
					</ConfirmationTypography>
				</PopUp>
			)}
		</>
	);
}
