import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { useCallback, useMemo, useState } from 'react';

import {
	SkillName,
	SkillsNames,
	SOFT_SKILL_DELETING_POPUP_TITLE,
} from '../../constants';
import { useInfiniteScroll } from '../../hooks/useInfiniteScroll';
import { useSoftSkillsManagement } from '../../hooks/useSoftSkillsManagement';
import { SkillListColor } from '../../types';
import { TileList } from '../TileList';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import { SOFT_SKILL_DELETING_MESSAGE } from '@/constants/notifications';
import { isChiefRole, chiefRoles } from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { ListScrollObserverProvider } from '@/context/ListScrollObserver';
import { useToast } from '@/context/Toast';
import { SoftSkillsForm } from '@/forms/SettingsForm/Skills/SoftSkills';
import { capitalizeString } from '@/helpers/capitalizeString';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { SoftSkill } from '@/types/settings';

export function SoftSkills() {
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const { role } = useAuth();

	const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
	const [idOfDeleteSkill, setIdOfDeleteSkill] = useState<number | null>(null);

	const {
		softSkills,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
		createSoftSkill,
		updateSoftSkill,
		deleteSoftSkill,
	} = useSoftSkillsManagement({
		toast,
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

	const skillToDelete = useMemo(
		() => softSkills.find((skill) => skill.id === idOfDeleteSkill),
		[idOfDeleteSkill, softSkills]
	);

	const handleDeleteSoftSkill = async () => {
		if (skillToDelete) {
			await deleteSoftSkill(skillToDelete);
			handleToggleDialog();
			setIdOfDeleteSkill(null);
		}
	};

	const defaultValues = useMemo(
		() => ({
			name: '',
		}),
		[isOpen]
	);

	const getListItemContextMenuOptions =
		useCallback((): PickerButtonOption<SoftSkill>[] => {
			if (!isChiefRole(role)) {
				return [];
			}

			return [
				{
					value: 'Edit',
					addon: <EditIcon />,
					onClick: (skill: SoftSkill) => {
						handleOpen(skill.id);
					},
				},
				{
					value: 'Delete',
					addon: <BasketIcon />,
					onClick: (skill: SoftSkill) => {
						setIdOfDeleteSkill(skill.id);
						handleToggleDialog();
					},
				},
			];
		}, [role, handleOpen, handleToggleDialog]);

	const handleAddClick = useMemo(
		() => (chiefRoles.includes(role) ? handleOpen : undefined),
		[role, handleOpen]
	);

	return (
		<>
			<ListScrollObserverProvider>
				<TileList
					beforeColor={SkillListColor.Green}
					items={softSkills}
					title={SkillsNames.softSkillsList}
					onAddClick={handleAddClick}
					totalItems={total}
					showLoader={isLoading && softSkills.length === 0}
					isLoading={isFetching}
					getListItemContextMenuOption={getListItemContextMenuOptions}
					fieldToDisplay="name"
					containerRef={scrollContainerRef}
				/>
			</ListScrollObserverProvider>
			{isOpen && (
				<SoftSkillsForm
					id={id as number}
					onSubmit={id ? updateSoftSkill : createSoftSkill}
					handleClose={handleClose}
					initialValues={defaultValues}
				/>
			)}
			{isDeleteDialogOpen && (
				<PopUp
					title={SOFT_SKILL_DELETING_POPUP_TITLE}
					onClose={handleToggleDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleDeleteSoftSkill },
						negativeControl: { onClick: handleToggleDialog },
					}}
					zIndex={zIndexes.formPopup}
				>
					<Typography variant="m-400">
						{capitalizeString(SkillName.SoftSkill)}{' '}
						<Typography variant="m-600">{skillToDelete?.name}</Typography>{' '}
						{SOFT_SKILL_DELETING_MESSAGE}
					</Typography>
					<ConfirmationTypography variant="m-400">
						{confirmationMessage}
					</ConfirmationTypography>
				</PopUp>
			)}
		</>
	);
}
