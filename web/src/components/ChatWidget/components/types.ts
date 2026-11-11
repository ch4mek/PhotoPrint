import { RefAttributes } from 'react';

export interface ChatWidgetProps extends RefAttributes<HTMLButtonElement> {
	isOpen: boolean;
	handleClose: VoidFunction;
}

export interface ChatProps {
	handleClose: VoidFunction;
}
