import { Dispatch } from 'react';

import { Message } from '@/types/chatWidget';

export interface ChatInputProps {
	isLoading: boolean;
	addNewMessage: (
		newMessage: Message | Message[],
		isOptimisticAdd: boolean
	) => void;
	setIsMessageRequestLoading: Dispatch<boolean>;
}
