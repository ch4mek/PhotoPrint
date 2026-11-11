import { RefAttributes } from 'react';

export interface ChatWidgetRootProps extends RefAttributes<HTMLButtonElement> {
	isOpen: boolean;
	handleCloseWidget: VoidFunction;
}
