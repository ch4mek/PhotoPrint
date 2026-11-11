import { TextAreaProps } from '@mms/mms-ui-library/components/TextArea/types';
import { RefAttributes } from 'react';

export type MMSTextAreaProps = {
	name: string;
	onChange?: (field: string, newValue: any) => void;
	// value and onChange are taken from the Filed component of the formik
} & Omit<TextAreaProps, 'value' | 'onChange'> &
	RefAttributes<HTMLDivElement>;
