import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TIME_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { ChatRole, Message } from '@/types/chatWidget';

import {
	LoadingMessageWrapper,
	MessageContent,
	MessageItem,
	MessageTime,
	MessageWrapper,
} from './styles';

export function ChatMessage({
	role,
	message,
	createdAtUtc,
	messageContent,
}: Message) {
	const theme = useTheme();
	const formattedTime = dayjs(createdAtUtc).format(TIME_FORMAT);
	const isOwnMessage = role === ChatRole.User;

	return (
		<MessageWrapper {...{ isOwnMessage }}>
			<MessageItem {...{ role }}>
				{!messageContent ? (
					<>
						<MessageContent
							variant="m-400"
							{...{ role }}
							mainColor={theme.palette['text-black']}
						>
							{message}
						</MessageContent>
						<MessageTime {...{ role }} mainColor={theme.palette['text-gray']}>
							<Typography variant="xs-400">{formattedTime}</Typography>
						</MessageTime>
					</>
				) : (
					<LoadingMessageWrapper>{messageContent}</LoadingMessageWrapper>
				)}
			</MessageItem>
		</MessageWrapper>
	);
}
