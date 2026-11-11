import { ChangeEvent, FocusEvent, Ref } from 'react';
import { IMaskInputProps } from 'react-imask/esm/mixin';

export type TimeMaskInputProps = {
	ref?: Ref<HTMLInputElement>;
	onChange: (value: string) => void;
	onFocus?: (event: FocusEvent<HTMLInputElement>) => void;
	value: string | undefined;
	disabled: boolean;
	onBlurEvent?: (
		event: ChangeEvent<HTMLTextAreaElement | HTMLInputElement>
	) => void;
} & Omit<IMaskInputProps, 'onChange' | 'mask' | 'onFocus' | 'ref'>;
