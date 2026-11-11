import { ChangeEvent } from 'react';
import { IMaskInputProps } from 'react-imask/esm/mixin';

export type TimeMaskInputProps = {
	hasError?: boolean;
	onChange: (value: string) => void;
	value: string | undefined;
	onBlurEvent?: (
		event: ChangeEvent<HTMLTextAreaElement | HTMLInputElement>
	) => void;
	onFocusEvent?: (
		event: ChangeEvent<HTMLTextAreaElement | HTMLInputElement>
	) => void;
} & Omit<IMaskInputProps, 'onChange' | 'mask'>;
