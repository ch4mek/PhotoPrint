import { InputProps } from '@mms/mms-ui-library/components/Input';

export type TextFieldProps = {
	label?: string;
	name: string;
	onChange: (field: string, newValue: any) => void;
	onChangeAdditionalCallback?: () => void;
	withTrimOnBlur?: boolean;
} & Partial<Omit<InputProps, 'onChange' | 'ref'>>;
