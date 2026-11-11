import { JSX } from 'react';

export interface ItemProps {
	nameOfField?: string;
	value?: string | JSX.Element | null;
	secondValue?: string | null;
}
