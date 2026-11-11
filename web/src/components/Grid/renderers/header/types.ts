import { Dispatch, SetStateAction } from 'react';

export interface SelectionModalOption {
	id: string;
	description: string;
	disabled?: boolean;
}

export interface RenderModalProps<T> {
	onApply: () => void;
	onReset: () => void;
	selection: T;
	setSelection: Dispatch<SetStateAction<T>>;
	resetDisabled: boolean;
	applyDisabled: boolean;
	options?: Array<SelectionModalOption>;
	autocompleteOptions?: string[];
	allowEmptySelection?: boolean;
	disabledAllSelection?: boolean;
	getTitleFromOption?: (option: T) => string;
	conteinerMaxHeight?: number;
	isOpen?: boolean;
}
