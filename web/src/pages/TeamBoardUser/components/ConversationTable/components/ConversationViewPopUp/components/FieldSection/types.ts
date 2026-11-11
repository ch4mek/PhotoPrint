import { ReactNode } from 'react';

export interface FieldSectionProps {
	label: string;
	value?: string;
	children?: ReactNode;
	withDivider?: boolean;
}
