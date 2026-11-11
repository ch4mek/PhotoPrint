import { ButtonHTMLAttributes } from 'react';

export interface PositiveActionButtonProps
	extends ButtonHTMLAttributes<HTMLButtonElement> {
	tooltipText?: string;
	text?: string;
}
