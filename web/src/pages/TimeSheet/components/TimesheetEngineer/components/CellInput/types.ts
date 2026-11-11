import { FocusEvent, Ref } from 'react';

export interface CellInputProps {
	ref?: Ref<HTMLInputElement>;
	disabled?: boolean;
	value: string;
	onChange: (value: string) => void;
	onBlur?: (e: FocusEvent<HTMLInputElement>) => void;
	onFocus?: (e: FocusEvent<HTMLInputElement>) => void;
	cellPosition?: 'top' | 'bottom';
	isActive?: boolean;
}

export interface CellInputWrapperProps {
	disabled: boolean;
	cellPosition: CellInputProps['cellPosition'];
}
