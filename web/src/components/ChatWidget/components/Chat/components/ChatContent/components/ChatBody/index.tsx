import { useTheme } from '@emotion/react';
import { DotsLoader } from '@mms/mms-ui-library/components/DotsLoader';
import { lightTheme } from '@mms/mms-ui-library/theme';
import { Fragment, UIEvent, useLayoutEffect, useMemo, useRef } from 'react';

import { ChatDateLabel } from '../ChatDateLabel';
import { ChatMessage } from '../ChatMessage';
import { loadingMessageOptions } from '../ChatMessage/constants';

import { Loader } from '@/components/Loader';
import { isAtScrollTop } from '@/helpers/isAtScrollBottom';
import { ThemeMode } from '@/theme';

import { ChatBodyWrapper } from './styles';
import { ChatBodyProps } from './types';
import { groupMessagesByDate } from './utils/groupMessagesByDate';

export function ChatBody({
	messages,
	isLoading,
	requestNewMessages,
	stickToBottom,
	isMessageRequestLoading,
}: ChatBodyProps) {
	const theme = useTheme();
	const chatBodyRef = useRef<HTMLUListElement>(null);
	const bottomRef = useRef<HTMLDivElement>(null);
	const prevScrollHeightRef = useRef<number | null>(0);

	const { messagesGroups, dateGroups } = useMemo(() => {
		const groupedMessages = groupMessagesByDate(messages);
		const sortedDates = Object.keys(groupedMessages).sort((a, b) =>
			a.localeCompare(b, undefined, {
				numeric: true,
				sensitivity: 'base',
			})
		);

		return {
			messagesGroups: groupedMessages,
			dateGroups: sortedDates,
		};
	}, [messages]);

	const defaultLoadingColor =
		theme.mode === ThemeMode.DARK
			? lightTheme.palette['graphics-divider-1']
			: theme.palette['graphics-primary-2'];

	const handleScroll = (event: UIEvent<HTMLUListElement>) => {
		if (isAtScrollTop(event)) {
			const node = chatBodyRef.current;

			if (node) {
				prevScrollHeightRef.current = node.scrollHeight;
			}

			requestNewMessages();
		}
	};

	useLayoutEffect(() => {
		const node = chatBodyRef.current;

		if (!node) {
			return;
		}

		if (stickToBottom) {
			node.scrollTop = node.scrollHeight;
		} else if (prevScrollHeightRef.current !== null && !stickToBottom) {
			const addedHeight = node.scrollHeight - prevScrollHeightRef.current;
			node.scrollTop += addedHeight;

			prevScrollHeightRef.current = null;
		}
	}, [messages, stickToBottom]);

	return (
		<ChatBodyWrapper
			role="log"
			aria-live="polite"
			aria-relevant="additions"
			ref={chatBodyRef}
			onScroll={handleScroll}
		>
			<Loader isLoading={isLoading} size="small">
				{dateGroups.map((dateKey, index) => (
					<Fragment key={dateKey}>
						<ChatDateLabel date={dateKey} isTheFirstDate={index === 0} />
						{messagesGroups[dateKey].map((message, number) => (
							<ChatMessage key={number} {...message} />
						))}
					</Fragment>
				))}
				{isMessageRequestLoading && (
					<ChatMessage
						{...loadingMessageOptions}
						messageContent={<DotsLoader color={defaultLoadingColor} />}
					/>
				)}
			</Loader>
			<div ref={bottomRef} />
		</ChatBodyWrapper>
	);
}
