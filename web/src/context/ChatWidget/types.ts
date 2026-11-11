import { RefObject } from 'react';

export interface ChatWidgetContextValues {
	isOpen: boolean;
	anchorElementRef: RefObject<HTMLButtonElement | null>;
	handleOpenWidget: VoidFunction;
	handleCloseWidget: VoidFunction;
	handleToggleWidget: VoidFunction;
}
