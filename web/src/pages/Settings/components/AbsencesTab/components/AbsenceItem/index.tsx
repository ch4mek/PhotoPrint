import { useTheme } from '@emotion/react';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { AddIcon } from '@mms/mms-ui-library/icons/base/AddIcon';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { ChevronIcon } from '@mms/mms-ui-library/icons/base/ChevronIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useMemo } from 'react';

import { useToggleContent } from '../../hooks/useToggleContent';

import { ChevronButton } from '@/components/ChevronButton';
import { iconStyles, noIconStyles } from '@/constants/absence';
import { controlsText } from '@/constants/controlsTexts';
import { IconSize } from '@/constants/iconSizes';
import { AbsenceType } from '@/types/absences';

import { AgreementsList } from './components/AgreementsList';
import { ItemContent } from './components/ItemContent';
import {
	AbsenceItemHeader,
	AbsenceItemWrapper,
	Content,
	ContentWrapper,
	ControlsWrapper,
	IconWrapper,
	ItemWrapper,
} from './styles';
import { AbsenceItemProps } from './types';

const { deleteText, editText } = controlsText;

export function AbsenceItem({
	item,
	handleDelete,
	handleUpdate,
	handleAddAgreement,
	handleAgreementDelete,
	handleAssignsOpen,
}: AbsenceItemProps) {
	const theme = useTheme();
	const { isOpened, toggleOpen, contentRef, contentHeight } =
		useToggleContent();

	const { type, id, name, agreements } = item;
	const {
		icon: Icon,
		color,
		iconBackground: background,
	} = iconStyles(theme)[type as AbsenceType] || noIconStyles(theme);

	const isExpandAbsence = agreements.length > 0;

	const menuOptions = useMemo<Array<PickerButtonOption>>(
		() => [
			{
				addon: <EditIcon />,
				onClick: () => handleUpdate(id),
				value: editText,
			},
			{
				addon: <BasketIcon />,
				onClick: () => handleDelete({ id, name }, agreements.length > 0),
				value: deleteText,
			},
		],
		[id, name, agreements.length, handleDelete]
	);

	const handleAdd = () => {
		handleAddAgreement({ itemId: id });
	};

	const isShowAddButton = useMemo(
		() => type === AbsenceType.Vacation || type === AbsenceType.SickDay,
		[type]
	);

	return (
		<AbsenceItemWrapper>
			<AbsenceItemHeader>
				<ItemWrapper onClick={toggleOpen} clickable={isExpandAbsence}>
					<IconWrapper backgroundColor={background[theme.mode]}>
						<Icon
							color={color[theme.mode]}
							width={IconSize.M}
							height={IconSize.M}
						/>
					</IconWrapper>
					<ItemContent
						{...{ item }}
						handleAbsenceAssigns={
							isExpandAbsence ? undefined : handleAssignsOpen('templates')
						}
					/>
				</ItemWrapper>
				<ControlsWrapper>
					{isExpandAbsence && (
						<ChevronButton
							onClick={toggleOpen}
							chevronColor={theme.palette['graphics-primary-2']}
							isActive={isOpened}
							size="medium"
						>
							<ChevronIcon />
						</ChevronButton>
					)}

					{isShowAddButton && (
						<ActionButton onClick={handleAdd}>
							<AddIcon />
						</ActionButton>
					)}

					<PickerButton
						{...{ item }}
						icon={<OptionDotsIcon />}
						options={menuOptions}
						popoverPosition="left-start"
					/>
				</ControlsWrapper>
			</AbsenceItemHeader>
			<ContentWrapper height={contentHeight}>
				{isOpened && (
					<Content ref={contentRef}>
						<AgreementsList
							absenceId={id}
							handleAddAgreement={handleAddAgreement}
							handleAgreementDelete={handleAgreementDelete}
							handleAgreementAssigns={handleAssignsOpen('agreements')}
							agreements={agreements}
						/>
					</Content>
				)}
			</ContentWrapper>
		</AbsenceItemWrapper>
	);
}
