import { MicrofrontendsFormArrayKeys } from '../../types';

export interface ChipArrayFieldProps {
	name: MicrofrontendsFormArrayKeys;
	label: string;
	placeholder?: string;
	isRequired?: boolean;
	maxItems?: number;
	validation?: (value: string) => string | null;
}
