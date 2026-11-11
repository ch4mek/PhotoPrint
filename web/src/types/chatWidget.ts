import { JSX } from 'react';

export enum ChatRole {
	Assistant = 'Assistant',
	User = 'User',
}

export interface Message {
	id: number | string;
	role: ChatRole;
	message: string | null;
	createdAtUtc: string;
	userId?: number;
	messageContent?: JSX.Element;
}

export interface ChatFormValues {
	question: string;
}
