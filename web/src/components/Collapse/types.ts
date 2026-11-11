import { HTMLAttributes } from 'react';

export interface CollapseProps extends HTMLAttributes<HTMLDivElement> {
	isOpen?: boolean;
}
