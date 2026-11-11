import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { useCallback, useMemo, useState } from 'react';

import {
	SkillName,
	SkillsNames,
	HARD_SKILL_DELETING_POPUP_TITLE,
} from '../../constants';
import { useHardSkillsManagement } from '../../hooks/useHardSkillsManagement';
import { useInfiniteScroll } from '../../hooks/useInfiniteScroll';
import { SkillListColor } from '../../types';
import { TileList } from '../TileList';

import { confirmationMessage } from '@/components/DialogTypography/constants';
import { ConfirmationTypography } from '@/components/DialogTypography/styles';
import {
	HARD_SKILL_DELETING_MESSAGE,
	ITEM_WILL_DELETED_POSTFIX,
} from '@/constants/notifications';
import { zIndexes } from '@/constants/zIndexes';
import { ListScrollObserverProvider } from '@/context/ListScrollObserver';
import { useToast } from '@/context/Toast';
import { HardSkillsForm } from '@/forms/SettingsForm/Skills/HardSkills';
import { capitalizeString } from '@/helpers/capitalizeString';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { HardSkill } from '@/types/settings';

import { HardSkillsProps } from './types';

export function HardSkills({
	selectedDepartmentId,
	selectedSpecialization,
}: HardSkillsProps) {
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const [isDialogOpen, setIsDialogOpen] = useState(false);
	const [idOfDeleteSkill, setIdOfDeleteSkill] = useState<number | string>('');

	const {
		hardSkills,
		total,
		isLoading,
		isFetching,
		fetchNextPage,
		hasNextPage,
		isFetchingNextPage,
		createHardSkill,
		updateHardSkill,
		deleteHardSkill,
	} = useHardSkillsManagement({
		selectedDepartmentId,
		selectedSpecializationId: selectedSpecialization?.id,
		toast,
	});

	const scrollContainerRef = useInfiniteScroll({
		hasNextPage,
		isFetchingNextPage,
		fetchNextPage,
		enabled: !isLoading,
	});

	const handleToggleDialog = useCallback(
		() => setIsDialogOpen((prev) => !prev),
		[]
	);

	const deleteSkillItem = useMemo(
		() => hardSkills.find((skill) => skill.id === idOfDeleteSkill),
		[idOfDeleteSkill, hardSkills]
	);

	const handleDeleteHardSkill = async () => {
		if (deleteSkillItem) {
			await deleteHardSkill(deleteSkillItem);
			handleToggleDialog();
			setIdOfDeleteSkill('');
		}
	};

	const defaultValues = useMemo(
		() => ({
			name: '',
			hardSkillGroupId: null,
		}),
		[isOpen]
	);

	const getListItemContextMenuOption = useCallback(
		(): PickerButtonOption<HardSkill>[] => [
			{
				value: 'Edit',
				addon: <EditIcon />,
				onClick: (hardSkill: HardSkill) => {
					handleOpen(hardSkill.id);
				},
			},
			{
				value: 'Delete',
				addon: <BasketIcon />,
				onClick: (hardSkill: HardSkill) => {
					setIdOfDeleteSkill(hardSkill.id);
					handleToggleDialog();
				},
			},
		],
		[handleOpen, handleToggleDialog]
	);

	return (
		<>
			<ListScrollObserverProvider>
				<TileList
					beforeColor={SkillListColor.Blue}
					items={hardSkills}
					title={SkillsNames.hardSkillsList}
					onAddClick={handleOpen}
					totalItems={total}
					showLoader={isLoading && hardSkills.length === 0}
					isLoading={isFetching}
					getListItemContextMenuOption={getListItemContextMenuOption}
					fieldToDisplay="name"
					containerRef={scrollContainerRef}
				/>
			</ListScrollObserverProvider>
			{isOpen && (
				<HardSkillsForm
					id={id as number}
					onSubmit={id ? updateHardSkill : createHardSkill}
					handleClose={handleClose}
					initialValues={defaultValues}
				/>
			)}
			{isDialogOpen && (
				<PopUp
					title={HARD_SKILL_DELETING_POPUP_TITLE}
					onClose={handleToggleDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleDeleteHardSkill },
						negativeControl: { onClick: handleToggleDialog },
					}}
					zIndex={zIndexes.formPopup}
				>
					<Typography variant="m-400">
						{capitalizeString(SkillName.HardSkill)}{' '}
						<Typography variant="m-600">{deleteSkillItem?.name}</Typography>{' '}
						{ITEM_WILL_DELETED_POSTFIX} {HARD_SKILL_DELETING_MESSAGE}
					</Typography>
					<ConfirmationTypography variant="m-400">
						{confirmationMessage}
					</ConfirmationTypography>
				</PopUp>
			)}
		</>
	);
}
