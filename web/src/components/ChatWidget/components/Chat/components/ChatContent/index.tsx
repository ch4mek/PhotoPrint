import { useCallback, useState } from 'react';

import { ChatBody } from '@/components/ChatWidget/components/Chat/components/ChatContent/components/ChatBody';
import { ChatInput } from '@/components/ChatWidget/components/Chat/components/ChatContent/components/ChatInput';
import {
	DEFAULT_ITEMS_PER_PAGE,
	DEFAULT_ORDER_BY,
} from '@/components/ChatWidget/components/Chat/components/ChatContent/constants';
import { ChatContentWrapper } from '@/components/ChatWidget/components/Chat/components/ChatContent/styles';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useGetChatHistory } from '@/queries/digitalHr/useGetChatHistory';
import { Message } from '@/types/chatWidget';

export function ChatContent() {
	const toast = useToast();
	const [pageNumber, setPageNumber] = useState(0);
	const [messages, setMessages] = useState<Message[]>([]);
	const [shouldStickToBottom, setShouldStickToBottom] = useState(true);
	const [isMessageRequestLoading, setIsMessageRequestLoading] = useState(false);

	const { data, isLoading } = useGetChatHistory(
		DEFAULT_ORDER_BY,
		pageNumber,
		DEFAULT_ITEMS_PER_PAGE,
		{
			onError: createOnErrorHandler(toast),
			onSuccess: (newMessage) => {
				setMessages((prevState) =>
					pageNumber === 0
						? newMessage.items
						: Array.from(
								new Map(
									[...prevState, ...newMessage.items].map((item) => [
										item.id,
										item,
									])
								).values()
						  )
				);
			},
		}
	);

	const requestNewMessages = useCallback(() => {
		if (
			isLoading ||
			!data?.items ||
			data.items.length < DEFAULT_ITEMS_PER_PAGE
		) {
			return;
		}

		if (shouldStickToBottom) {
			setShouldStickToBottom(false);
		}

		setPageNumber(pageNumber + 1);
	}, [shouldStickToBottom, pageNumber, isLoading, data]);

	const addNewMessage = useCallback(
		(newMessage: Message | Message[], isOptimisticAdd: boolean) => {
			setShouldStickToBottom(true);

			const incomingMessages = Array.isArray(newMessage)
				? newMessage
				: [newMessage];

			if (isOptimisticAdd) {
				setMessages((prevState) => [...prevState, ...incomingMessages]);

				return;
			}

			setMessages((prevState) => {
				const withoutLastMessage = prevState.slice(0, -1);

				return [...withoutLastMessage, ...incomingMessages];
			});
		},
		[]
	);

	return (
		<ChatContentWrapper>
			<ChatBody
				stickToBottom={shouldStickToBottom}
				isLoading={isLoading && pageNumber === 0}
				{...{ messages, requestNewMessages, isMessageRequestLoading }}
			/>
			<ChatInput
				{...{ isLoading, addNewMessage, setIsMessageRequestLoading }}
			/>
		</ChatContentWrapper>
	);
}
