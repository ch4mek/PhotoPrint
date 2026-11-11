import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { useUserRequestNotification } from '../../../hooks/useUserRequestNotification';
import { UserRequestNotificationWrapper } from '../../styles';

import { IconSize } from '@/constants/iconSizes';

import { PreventOverflowTypography } from './styles';
import { UserRequestNotificationProps } from './types';
import { iconStyles } from './utils';

export function UserRequestApprovedOrRejectedNotification({
	item: { id, Message, InquiryId, read, notificationType },
}: UserRequestNotificationProps) {
	const theme = useTheme();
	const handleClick = useUserRequestNotification(id, InquiryId, read);

	const { Icon, color } = iconStyles(theme)[notificationType];

	return (
		<UserRequestNotificationWrapper onClick={handleClick} {...{ color }}>
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
