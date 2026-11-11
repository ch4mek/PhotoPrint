import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { useUserRequestNotification } from '../../../hooks/useUserRequestNotification';
import { UserRequestNotificationWrapper } from '../../styles';

import { iconStyles } from '@/constants/absence';
import { IconSize } from '@/constants/iconSizes';

import { PreventOverflowTypography } from './styles';
import { UserRequestNotificationProps } from './types';

export function UserRequestNotification({
	item: { id, Message, InquiryId, InquiryType, read },
}: UserRequestNotificationProps) {
	const theme = useTheme();
	const handleClick = useUserRequestNotification(id, InquiryId, read);

	const { icon: Icon } = iconStyles(theme)[InquiryType];

	return (
		<UserRequestNotificationWrapper
			onClick={handleClick}
			color={theme.palette['graphics-primary-2']}
		>
			<Icon width={IconSize.M} height={IconSize.M} />
			<Tooltip useIfTextCut text={Message}>
				<PreventOverflowTypography
					variant="m-400"
					color={theme.palette['text-black']}
				>
					{Message}
				</PreventOverflowTypography>
			</Tooltip>
		</UserRequestNotificationWrapper>
	);
}
