import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useTranslation } from 'react-i18next';

import { iconStyles, noIconStyles } from '@/constants/absence';
import { AbsenceType, TrackingUnitType } from '@/types/absences';

import { ABSENCE_USER_ICON_SIZE } from './constants';
import {
	AbsenceItemHeader,
	AbsenceItemHeaderContent,
	AbsenceItemMain,
	AbsenceItemMainInfoSection,
	AbsenceItemWrapper,
	AbsenceTypeTypography,
	AmountDaysTypography,
	AvailabilityTypography,
	IconWrapper,
} from './styles';
import { UserAbsenceCardProps } from './types';

export function UserAbsenceCard({
	absenceItem,
	actions,
	width,
}: UserAbsenceCardProps) {
	const theme = useTheme();
	const { t } = useTranslation('profile', { keyPrefix: 'absence' });
	const { isAgreement, trackingUnitCount, trackingUnitType, type, name } =
		absenceItem;
	const labelKey = (() => {
		const isDays = trackingUnitType === TrackingUnitType.Days;

		if (isAgreement) {
			return isDays ? 'availableDays' : 'availableHours';
		}

		return isDays ? 'balanceDays' : 'balanceHours';
	})();

	const {
		icon: Icon,
		color,
		iconBackground,
		background,
	} = iconStyles(theme)[type as AbsenceType] || noIconStyles(theme);

	return (
		<AbsenceItemWrapper backgroundColor={background[theme.mode]} width={width}>
			<AbsenceItemHeader>
				<AbsenceItemHeaderContent>
					<IconWrapper backgroundColor={iconBackground[theme.mode]}>
						<Icon
							color={color[theme.mode]}
							width={ABSENCE_USER_ICON_SIZE}
							height={ABSENCE_USER_ICON_SIZE}
						/>
					</IconWrapper>

					<Tooltip useIfTextCut text={name}>
						<AbsenceTypeTypography variant="m-700">
							{name}
						</AbsenceTypeTypography>
					</Tooltip>
				</AbsenceItemHeaderContent>

				{actions}
			</AbsenceItemHeader>

			<AbsenceItemMain>
				<AbsenceItemMainInfoSection>
					<AmountDaysTypography variant="m-600">
						{trackingUnitCount}
					</AmountDaysTypography>{' '}
					<AvailabilityTypography variant="xs-400">
						{t(labelKey)}
					</AvailabilityTypography>
				</AbsenceItemMainInfoSection>
			</AbsenceItemMain>
		</AbsenceItemWrapper>
	);
}
