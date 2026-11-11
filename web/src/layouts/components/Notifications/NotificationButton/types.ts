import { RefAttributes } from 'react';

export interface NotificationButtonProps
	extends RefAttributes<HTMLButtonElement> {
	onClick: VoidFunction;
	showBadge: boolean;
}
