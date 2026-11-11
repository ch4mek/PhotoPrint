import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { EditIcon } from '@mms/mms-ui-library/icons/base/EditIcon';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useCallback } from 'react';

import { controlsText } from '@/constants/controlsTexts';
import { AgreementItem } from '@/types/absences';

import { USER_TEXT_FOR_AGREEMENT_LIST } from './constants';
import {
	AgreementInfoSection,
	AgreementItemWrapper,
	AgreementName,
	AmountUsersWithAgreements,
} from './styles';
import { AgreementsListProps } from './types';
import { getDefaultLabelText, getLabelWithText } from './utils';

const { deleteText, editText } = controlsText;

export function AgreementsList({
	agreements,
	absenceId,
	handleAddAgreement,
	handleAgreementDelete,
	handleAgreementAssigns,
}: AgreementsListProps) {
	const menuOptions = useCallback(
		() => [
			{
				addon: <EditIcon />,
				onClick: (agreement: AgreementItem) => {
					handleAddAgreement({ itemId: absenceId, item: agreement });
				},
				value: editText,
			},
			{
				addon: <BasketIcon />,
				onClick: (agreement: AgreementItem) => {
					handleAgreementDelete({ name: agreement.name, id: agreement.id });
				},
				value: deleteText,
			},
		],
		[agreements]
	);

	const handleAmountUsersWithAgreementsClicked =
		(id: number, name: string) => () =>
			handleAgreementAssigns({ id, name });

	return agreements.map(
		({
			id,
			name,
			daysPerContractYear,
			isDefault,
			assignedUsersCount,
			...agreement
		}) => (
			<AgreementItemWrapper key={id}>
				<AgreementInfoSection>
					<Tooltip text={getDefaultLabelText(name, isDefault)} useIfTextCut>
						<AgreementName variant="m-400">
							{getDefaultLabelText(name, isDefault)}
						</AgreementName>
					</Tooltip>
					<AmountUsersWithAgreements
						variant="s-400"
						onClick={
							assignedUsersCount
								? handleAmountUsersWithAgreementsClicked(
										Number(id),
										getDefaultLabelText(name, isDefault)
								  )
								: undefined
						}
					>
						{getLabelWithText(USER_TEXT_FOR_AGREEMENT_LIST, assignedUsersCount)}
					</AmountUsersWithAgreements>
				</AgreementInfoSection>
				<PickerButton
					icon={<OptionDotsIcon />}
					options={menuOptions()}
					item={{
						id,
						name,
						daysPerContractYear,
						isDefault,
						assignedUsersCount,
						...agreement,
					}}
					popoverPosition="left-start"
				/>
			</AgreementItemWrapper>
		)
	);
}
