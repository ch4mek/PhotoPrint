import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { DotIcon } from '@mms/mms-ui-library/icons/base/DotIcon';

import { USER_TEXT_FOR_AGREEMENT_LIST } from '../AgreementsList/constants';
import { getLabelWithText } from '../AgreementsList/utils';

import { activitiesType, trackingUnitsType } from '@/constants/absence';
import { dotIconStyles } from '@/constants/commonStyles';

import {
	DescriptionTypography,
	ItemContentWrapper,
	ItemDescription,
} from './styles';
import { ItemContentProps } from './types';
import { getTrackingTypeLabel } from './utils/getTrackingTypeLabel';

export function ItemContent({ item, handleAbsenceAssigns }: ItemContentProps) {
	const theme = useTheme();
	const {
		trackingUnitType,
		name,
		activityType: activityTypeId,
		assignedUsersCount,
		id,
	} = item;

	const contentColor = theme.palette['text-gray'];
	const trackingType = getTrackingTypeLabel(
		trackingUnitsType[trackingUnitType]
	);
	const activityType = activitiesType[activityTypeId] || '';
	const amountOfUsersText = getLabelWithText(
		USER_TEXT_FOR_AGREEMENT_LIST,
		assignedUsersCount
	);

	const handleAmountUsersClick = () =>
		handleAbsenceAssigns?.({ id: Number(id), name });

	return (
		<ItemContentWrapper>
			<Tooltip useIfTextCut text={name}>
				<DescriptionTypography variant="m-700">{name}</DescriptionTypography>
			</Tooltip>
			<ItemDescription>
				<Tooltip useIfTextCut text={trackingType}>
					<DescriptionTypography
						variant="m-400"
						color={contentColor}
						Component="p"
					>
						{trackingType}
					</DescriptionTypography>
				</Tooltip>
				<DotIcon color={theme.palette['graphics-gray']} {...dotIconStyles} />
				<Tooltip useIfTextCut text={activityType}>
					<DescriptionTypography
						variant="m-400"
						color={contentColor}
						Component="p"
					>
						{activityType}
					</DescriptionTypography>
				</Tooltip>
				{!!handleAbsenceAssigns && (
					<>
						<DotIcon
							color={theme.palette['graphics-gray']}
							{...dotIconStyles}
						/>
						<Tooltip useIfTextCut text={amountOfUsersText}>
							<DescriptionTypography
								variant="m-400"
								color={contentColor}
								Component="p"
								onClick={
									assignedUsersCount ? handleAmountUsersClick : undefined
								}
							>
								{amountOfUsersText}
							</DescriptionTypography>
						</Tooltip>
					</>
				)}
			</ItemDescription>
		</ItemContentWrapper>
	);
}
