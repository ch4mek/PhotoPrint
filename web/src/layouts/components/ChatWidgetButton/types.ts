import { RefAttributes } from 'react';

export interface ChatWidgetButtonProps
	extends RefAttributes<HTMLButtonElement> {
	handleClick: VoidFunction;
}
