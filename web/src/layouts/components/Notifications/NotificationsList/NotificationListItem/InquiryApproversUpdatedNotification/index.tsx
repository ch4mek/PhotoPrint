import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';

import { useUserRequestNotification } from '../../../hooks/useUserRequestNotification';
import {
	UserRequestNotificationWrapper,
	PreventOverflowTypography,
} from '../../styles';

import { InquiryApproversUpdatedNotification } from '@/types/notifications';

interface InquiryApproversUpdatedNotificationComponentProps {
	item: InquiryApproversUpdatedNotification;
}

export function InquiryApproversUpdatedNotificationComponent({
	item: { id, Message: message, InquiryId: inquiryId, read },
}: InquiryApproversUpdatedNotificationComponentProps) {
	const theme = useTheme();
	const handleClick = useUserRequestNotification(id, inquiryId, read);

	return (
		<UserRequestNotificationWrapper onClick={handleClick}>
			<Tooltip useIfTextCut text={message}>
				<PreventOverflowTypography
					variant="m-400"
					color={theme.palette['text-black']}
				>
					{message}
				</PreventOverflowTypography>
			</Tooltip>
		</UserRequestNotificationWrapper>
	);
}
