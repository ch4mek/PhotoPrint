import { RefAttributes } from 'react';

export interface DropDownProps extends RefAttributes<HTMLDivElement> {
	anchorElement: HTMLElement | null;
	isOpen: boolean;
	items: string[];
}
