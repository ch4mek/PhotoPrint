import { useTheme } from '@emotion/react';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import chatAvatar from '@/assets/common/chatWidgetAvatar.jpg';
import { IconSize } from '@/constants/iconSizes';

import { chatHeaderInfoDetails } from './constants';
import {
	ChatAvatarWrapper,
	ChatHeaderInfoDetails,
	ChatHeaderInfoWrapper,
} from './styles';

const { title, subTitle } = chatHeaderInfoDetails;

export function ChatHeaderInfo() {
	const theme = useTheme();

	return (
		<ChatHeaderInfoWrapper>
			<ChatAvatarWrapper>
				<Avatar size={IconSize.XL} src={chatAvatar} />
			</ChatAvatarWrapper>
			<ChatHeaderInfoDetails>
				<Typography variant="small" color={theme.palette['text-black']}>
					{title}
				</Typography>
				<Typography variant="s-400" color={theme.palette['text-gray']}>
					{subTitle}
				</Typography>
			</ChatHeaderInfoDetails>
		</ChatHeaderInfoWrapper>
	);
}
