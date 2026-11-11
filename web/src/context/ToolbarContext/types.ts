import { MediaType } from '@/types/plateEditor';

export interface PopUpData {
	type: MediaType;
	handleSubmit: (data: EditorToolbarFormValues) => void;
}

interface EditorToolbarFormValues {
	url: string;
}

export interface ToolbarContextValue {
	setPopUpData: (data: PopUpData) => void;
	handleOpen: VoidFunction;
	handleClose: VoidFunction;
	isOpen: boolean;
	popUpData: PopUpData | null;

	handlePopoverOpen: (type: string) => void;
	handlePopoverClose: VoidFunction;
	isToolbarPopoverOpen: ToolbarState;
}

export interface ToolbarState {
	isOpen: boolean;
	type: null | string;
}
