import { Message } from '@/types/chatWidget';

export interface ChatBodyProps {
	messages: Message[];
	isLoading: boolean;
	requestNewMessages: VoidFunction;
	stickToBottom: boolean;
	isMessageRequestLoading: boolean;
}
